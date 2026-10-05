using System;
using System.Collections.Generic;

namespace Compiler
{
    internal class SyntaxAnalyzer
    {
        private LexicalAnalyzer _lexer;
        private byte _sym;

        public SyntaxAnalyzer(LexicalAnalyzer lexer)
        {
            _lexer = lexer;
        }

        private void NextSum()
        {
            _sym = _lexer.NextSym();
        }

        private void Accept(byte expectedToken, byte errorCode, List<byte> followers)
        {
            if (_sym == expectedToken)
            {
                NextSum();
            }
            else
            {
                InputOutput.Error(errorCode, InputOutput.PositionNow);

                while (_sym != 0 &&
                       _sym != expectedToken &&
                       !followers.Contains(_sym))
                {
                    NextSum();
                }

                if (_sym == expectedToken)
                {
                    NextSum();
                }
            }
        }

        public void Parse()
        {
            NextSum();
            Program();
        }

        private void Program()
        {
            Accept(
                LexicalAnalyzer.programsy,
                1,
                new List<byte>
                {
                    LexicalAnalyzer.ident
                });

            Accept(
                LexicalAnalyzer.ident,
                2,
                new List<byte>
                {
                    LexicalAnalyzer.semicolon
                });

            Accept(
                LexicalAnalyzer.semicolon,
                3,
                new List<byte>
                {
                    LexicalAnalyzer.typesy,
                    LexicalAnalyzer.varsy,
                    LexicalAnalyzer.beginsy
                });

            if (_sym == LexicalAnalyzer.typesy)
            {
                TypeBlock();
            }

            if (_sym == LexicalAnalyzer.varsy)
            {
                VarBlock();
            }

            CompoundStatement();

            Accept(
                LexicalAnalyzer.point,
                4,
                new List<byte>());
        }

        private void TypeBlock()
        {
            Accept(
                LexicalAnalyzer.typesy,
                15,
                new List<byte>
                {
                    LexicalAnalyzer.ident
                });

            while (_sym == LexicalAnalyzer.ident)
            {
                TypeDefinition();
            }
        }

        private void TypeDefinition()
        {
            Accept(
                LexicalAnalyzer.ident,
                2,
                new List<byte>
                {
                    LexicalAnalyzer.equal
                });

            Accept(
                LexicalAnalyzer.equal,
                16,
                new List<byte>
                {
                    LexicalAnalyzer.ident,
                    LexicalAnalyzer.intc,
                    LexicalAnalyzer.leftpar,
                    LexicalAnalyzer.recordsy
                });

            TypeDeclaration();

            Accept(
                LexicalAnalyzer.semicolon,
                3,
                new List<byte>
                {
                    LexicalAnalyzer.ident,
                    LexicalAnalyzer.varsy,
                    LexicalAnalyzer.beginsy
                });
        }

        private void VarBlock()
        {
            Accept(
                LexicalAnalyzer.varsy,
                5,
                new List<byte>
                {
                    LexicalAnalyzer.ident
                });

            while (_sym == LexicalAnalyzer.ident)
            {
                VariableDeclaration();
            }
        }

        private void VariableDeclaration()
        {
            Accept(
                LexicalAnalyzer.ident,
                2,
                new List<byte>
                {
                    LexicalAnalyzer.comma,
                    LexicalAnalyzer.colon
                });

            while (_sym == LexicalAnalyzer.comma)
            {
                NextSum();

                Accept(
                    LexicalAnalyzer.ident,
                    2,
                    new List<byte>
                    {
                        LexicalAnalyzer.comma,
                        LexicalAnalyzer.colon
                    });
            }

            Accept(
                LexicalAnalyzer.colon,
                6,
                new List<byte>
                {
                    LexicalAnalyzer.ident,
                    LexicalAnalyzer.intc,
                    LexicalAnalyzer.leftpar,
                    LexicalAnalyzer.recordsy
                });

            TypeDeclaration();

            Accept(
                LexicalAnalyzer.semicolon,
                3,
                new List<byte>
                {
                    LexicalAnalyzer.ident,
                    LexicalAnalyzer.beginsy,
                    LexicalAnalyzer.colon,
                    LexicalAnalyzer.comma
                });
        }

        private void TypeDeclaration()
        {
            if (_sym == LexicalAnalyzer.recordsy)
            {
                RecordType();
            }
            else if (_sym == LexicalAnalyzer.ident)
            {
                NextSum();

                if (_sym == LexicalAnalyzer.twopoints)
                {
                    NextSum();

                    if (_sym == LexicalAnalyzer.ident ||
                        _sym == LexicalAnalyzer.intc)
                    {
                        NextSum();
                    }
                    else
                    {
                        InputOutput.Error(
                            7,
                            InputOutput.PositionNow);
                    }
                }
            }
            else if (_sym == LexicalAnalyzer.intc)
            {
                NextSum();

                Accept(
                    LexicalAnalyzer.twopoints,
                    17,
                    new List<byte>
                    {
                        LexicalAnalyzer.ident,
                        LexicalAnalyzer.intc
                    });

                if (_sym == LexicalAnalyzer.ident ||
                    _sym == LexicalAnalyzer.intc)
                {
                    NextSum();
                }
                else
                {
                    InputOutput.Error(
                        7,
                        InputOutput.PositionNow);
                }
            }
            else if (_sym == LexicalAnalyzer.leftpar)
            {
                NextSum();

                Accept(
                    LexicalAnalyzer.ident,
                    7,
                    new List<byte>
                    {
                        LexicalAnalyzer.comma,
                        LexicalAnalyzer.rightpar
                    });

                while (_sym == LexicalAnalyzer.comma)
                {
                    NextSum();

                    Accept(
                        LexicalAnalyzer.ident,
                        2,
                        new List<byte>
                        {
                            LexicalAnalyzer.comma,
                            LexicalAnalyzer.rightpar
                        });
                }

                Accept(
                    LexicalAnalyzer.rightpar,
                    18,
                    new List<byte>
                    {
                        LexicalAnalyzer.semicolon
                    });
            }
            else
            {
                InputOutput.Error(
                    7,
                    InputOutput.PositionNow);
            }
        }

        private void RecordType()
        {
            Accept(
                LexicalAnalyzer.recordsy,
                8,
                new List<byte>
                {
                    LexicalAnalyzer.ident
                });

            while (_sym == LexicalAnalyzer.ident)
            {
                Accept(
                    LexicalAnalyzer.ident,
                    2,
                    new List<byte>
                    {
                        LexicalAnalyzer.colon
                    });

                Accept(
                    LexicalAnalyzer.colon,
                    6,
                    new List<byte>
                    {
                        LexicalAnalyzer.ident,
                        LexicalAnalyzer.recordsy
                    });

                TypeDeclaration();

                Accept(
                    LexicalAnalyzer.semicolon,
                    3,
                    new List<byte>
                    {
                        LexicalAnalyzer.ident,
                        LexicalAnalyzer.endsy
                    });
            }

            Accept(
                LexicalAnalyzer.endsy,
                9,
                new List<byte>
                {
                    LexicalAnalyzer.semicolon
                });
        }

        private void CompoundStatement()
        {
            Accept(
                LexicalAnalyzer.beginsy,
                10,
                new List<byte>
                {
                    LexicalAnalyzer.ident,
                    LexicalAnalyzer.withsy,
                    LexicalAnalyzer.beginsy,
                    LexicalAnalyzer.endsy
                });

            while (_sym != LexicalAnalyzer.endsy &&
                   _sym != 0)
            {
                Statement();

                if (_sym == LexicalAnalyzer.semicolon)
                {
                    NextSum();
                }
                else if (_sym != LexicalAnalyzer.endsy)
                {
                    InputOutput.Error(
                        3,
                        InputOutput.PositionNow);
                }
            }

            Accept(
                LexicalAnalyzer.endsy,
                9,
                new List<byte>
                {
                    LexicalAnalyzer.point,
                    LexicalAnalyzer.semicolon
                });
        }

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
                    InputOutput.Error(
                        42,
                        InputOutput.PositionNow);

                    NextSum();
                    break;
            }
        }

        private void AssigmentStatement()
        {
            Dessignator();

            Accept(
                LexicalAnalyzer.assign,
                11,
                new List<byte>
                {
                    LexicalAnalyzer.ident,
                    LexicalAnalyzer.intc,
                    LexicalAnalyzer.semicolon,
                    LexicalAnalyzer.endsy
                });

            Expression();
        }

        private void Dessignator()
        {
            Accept(
                LexicalAnalyzer.ident,
                2,
                new List<byte>
                {
                    LexicalAnalyzer.point,
                    LexicalAnalyzer.assign
                });

            while (_sym == LexicalAnalyzer.point)
            {
                NextSum();

                Accept(
                    LexicalAnalyzer.ident,
                    2,
                    new List<byte>
                    {
                        LexicalAnalyzer.point,
                        LexicalAnalyzer.assign
                    });
            }
        }

        private void WithStatement()
        {
            Accept(
                LexicalAnalyzer.withsy,
                12,
                new List<byte>
                {
                    LexicalAnalyzer.ident
                });

            Accept(
                LexicalAnalyzer.ident,
                2,
                new List<byte>
                {
                    LexicalAnalyzer.dosy
                });

            Accept(
                LexicalAnalyzer.dosy,
                13,
                new List<byte>
                {
                    LexicalAnalyzer.beginsy,
                    LexicalAnalyzer.ident
                });

            Statement();
        }

        private void Expression()
        {
            if (_sym == LexicalAnalyzer.ident ||
                _sym == LexicalAnalyzer.intc)
            {
                NextSum();
            }
            else
            {
                InputOutput.Error(
                    14,
                    InputOutput.PositionNow);
            }
        }
    }
}