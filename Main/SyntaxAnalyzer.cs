using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Compiler
{
    internal class SyntaxAnalyzer
    {
        private LexicalAnalyzer _lexer;
        private byte _sym; // Текущий токен полученный от лексёра

        public SyntaxAnalyzer(LexicalAnalyzer lexer)
        {
            _lexer = lexer;
        }
        // Шаг вперёд
        private void NextSum()
        {
            _sym = _lexer.NextSym();
        }
        // Проверка то ли слово идёт?
        private void Accept(byte expectedToken, byte errorCode, List<byte> followers)
        {
            if (_sym == expectedToken)
            {
                NextSum();
            }
            else
            {
                // Регистрация ошибки
                InputOutput.Error(errorCode, InputOutput.PositionNow);
                // Пропуска пока не конец файла или не ожидаемый токен или до разрешённого следующего
                while (_sym != 0 && _sym != expectedToken && followers.Contains(_sym))
                {
                    NextSum();
                }

                if (_sym == expectedToken)
                {
                    NextSum();
                }
            }
        }

        // Главная точка старта парсера
        public void Parse()
        {
            NextSum();
            Program();
        }

        private void Program()
        {
            // Проверка слова program
            Accept(LexicalAnalyzer.programsy, 1, new List<byte> { LexicalAnalyzer.ident });
            // Проверка имя программы
            Accept(LexicalAnalyzer.ident, 2, new List<byte> { LexicalAnalyzer.semicolon });
            // Проверка ;
            Accept(LexicalAnalyzer.semicolon, 3, new List<byte> { LexicalAnalyzer.varsy, LexicalAnalyzer.beginsy});

            // Если дальше идёт var - запуск разбора программы
            if (_sym == LexicalAnalyzer.varsy)
            {
                VarBlock();
            }
            // После переменных должен идти begin...end
            CompoundStatement();
            // Программа завершается точкой
            Accept(LexicalAnalyzer.point, 4, new List<byte>());
        }

        private void VarBlock()
        {
            Accept(LexicalAnalyzer.varsy, 5, new List<byte> {LexicalAnalyzer.ident });
            // Пока идентификаторы - читаем их строки
            while (_sym == LexicalAnalyzer.ident)
            {
                VariableDeclaration();
            }
        }
        // Описание одной строки переменных
        private void VariableDeclaration()
        {
            // Первый идентификатор
            Accept(LexicalAnalyzer.ident, 2, new List<byte>{ LexicalAnalyzer.comma, LexicalAnalyzer.colon});

            // Если переменные через запятую
            while (_sym == LexicalAnalyzer.comma)
            {
                NextSum();
                Accept(LexicalAnalyzer.ident, 2, new List<byte> { LexicalAnalyzer.comma, LexicalAnalyzer.colon });
            }

            // После имён должно быть :
            Accept(LexicalAnalyzer.colon, 6, new List<byte> { LexicalAnalyzer.ident, LexicalAnalyzer.recordsy });

            // Определение типа данных
            TypeDeclaration();

            // На конце строк должна быть ;
            Accept(LexicalAnalyzer.semicolon, 3, new List<byte> { LexicalAnalyzer.ident, LexicalAnalyzer.beginsy });
        }

        // Определение типа данных
        private void TypeDeclaration()
        {
            if (_sym == LexicalAnalyzer.recordsy)
            {
                RecordType();
            }
            else if (_sym == LexicalAnalyzer.ident)
            {
                // Если простые типы
                NextSum();
            }
            else
            {
                InputOutput.Error(7, InputOutput.PositionNow);
            }
        }

        // Описание внутренней функции record
        private void RecordType()
        {
            Accept(LexicalAnalyzer.recordsy, 8, new List<byte> { LexicalAnalyzer.ident });
            // Внутри идут обычные поля
            while (_sym == LexicalAnalyzer.ident)
            {
                Accept(LexicalAnalyzer.ident, 2, new List<byte> { LexicalAnalyzer.colon });
                Accept(LexicalAnalyzer.colon, 6, new List<byte> { LexicalAnalyzer.ident });
                Accept(LexicalAnalyzer.ident, 7, new List<byte> { LexicalAnalyzer.semicolon });
                Accept(LexicalAnalyzer.semicolon, 3, new List<byte> { LexicalAnalyzer.ident, LexicalAnalyzer.endsy });
            }

            // ЗАпись завершится ключевыйм словом end
            Accept(LexicalAnalyzer.endsy, 9, new List<byte> {LexicalAnalyzer.semicolon });
        }

        // Составной оператор begin
        private void CompoundStatement()
        {
            // Проверка слова begin
            Accept(LexicalAnalyzer.beginsy, 10, new List<byte> {LexicalAnalyzer.ident, LexicalAnalyzer.withsy, LexicalAnalyzer.endsy});
            // Цикл пока не end
            while (_sym != LexicalAnalyzer.endsy && _sym != 0)
            {
                // Разбор оператора
                Statement();

                // Операторы разделяются ;
                if (_sym == LexicalAnalyzer.semicolon)
                {
                    NextSum();
                }
                else if (_sym == LexicalAnalyzer.endsy)
                {
                    InputOutput.Error(3, InputOutput.PositionNow);
                }
            }
            // Проверка на end
            Accept(LexicalAnalyzer.endsy, 9, new List<byte> { LexicalAnalyzer.point, LexicalAnalyzer.semicolon });
            
        }

        // РАспознаватель типа оператора
        private void Statement()
        {
            switch (_sym)
            {
                case LexicalAnalyzer.beginsy:
                    CompoundStatement();
                    break;
                case LexicalAnalyzer.withsy:
                    WithStatement();
                    break;
                case LexicalAnalyzer.ident:
                    AssigmentStatement();
                    break;
                default:
                    InputOutput.Error(42, InputOutput.PositionNow);
                    NextSum();
                    break;
            }
        }

        // Оператор присваивания <переменная> := <выражение>
        private void AssigmentStatement()
        {
            // Читает имя переменной или путь к полю записи
            Dessignator();
            // Проверка знака присваивания
            Accept(LexicalAnalyzer.assign, 11, new List<byte> { LexicalAnalyzer.ident, LexicalAnalyzer.intc });
            // Проверяет то что лежит справа от :=
            Expression();
        }

        private void Dessignator()
        {
            // Первая часть - имя переменной
            Accept(LexicalAnalyzer.ident, 2, new List<byte> { LexicalAnalyzer.point, LexicalAnalyzer.assign });
            // Если точка, то заход во внутрь
            while (_sym == LexicalAnalyzer.point)
            {
                NextSum();
                Accept(LexicalAnalyzer.ident, 2, new List<byte> { LexicalAnalyzer.point, LexicalAnalyzer.assign });
            }
        }

        private void WithStatement()
        {
            Accept(LexicalAnalyzer.withsy, 12, new List<byte> { LexicalAnalyzer.ident });
            Accept(LexicalAnalyzer.ident, 2, new List<byte> { LexicalAnalyzer.dosy });
            Accept(LexicalAnalyzer.dosy, 13, new List<byte> { LexicalAnalyzer.beginsy, LexicalAnalyzer.ident });

            Statement();
        }

        // Для синтаксиса достаточно проверить одну переменную или число
        private void Expression()
        {
            if (_sym == LexicalAnalyzer.ident || _sym == LexicalAnalyzer.intc)
            {
                NextSum();
            }
            else
            {
                InputOutput.Error(14, InputOutput.PositionNow);
            }
        }
    }
}
