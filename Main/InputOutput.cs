using System;
using System.Collections.Generic;
using System.Drawing;
using System.Formats.Asn1;
using System.IO;
using System.Runtime.CompilerServices;

namespace Compiler
{
    struct TextPosition
    {
        private uint _lineNumber;
        private byte _charNumber;

        public TextPosition(uint ln = 0, byte c = 0)
        {
            _lineNumber = ln;
            _charNumber = c;
        }
        
        public uint LineNumber
        {
            get
            {
                return this._lineNumber;
            }
            set
            {
                _lineNumber = value;
            }
        }
        public byte CharNumber
        {
            get
            {
                return this._charNumber;
            }
            set
            {
                _charNumber = value;
            }
        }
    }

    struct Err
    {
        private TextPosition _errorPosition;
        private byte _errorCode;

        public Err(TextPosition errorPosition, byte errorCode)
        {
            this._errorPosition = errorPosition;
            this._errorCode = errorCode;
        }

        public TextPosition ErrorPosition
        {
            get
            {
                return this._errorPosition;
            }
            set
            {
                _errorPosition = value;
            }
        }
        public byte ErrorCode
        {
            get
            {
                return this._errorCode;
            }
            set
            {
                _errorCode = value;
            }
        }
    }

    internal class InputOutput
    {
        const byte ERRMAX = 9;

        private static TextPosition _positionNow;
        private static List<Err> _err;
        private static string _line;
        private static byte _lastInLine;
        private static uint _errCount;
        private static bool _IsEof;
        static InputOutput()
        {
            _positionNow = new TextPosition(0, 0);
            _err = new List<Err>();
            _line = "";
            _lastInLine = 0;
            _errCount = 0;
            _IsEof = false;
        }

        public static StreamReader File
        {
            get;
            set;
        }
        public static char Ch
        {
            get;
            set;
        }
        public static bool IsEoF
        {
            get;
            set;
        }
        public static TextPosition PositionNow
        {
            get
            {
                return _positionNow;
            }
            set
            {
                _positionNow = value;
            }
        }
        public static string Line
        {
            get
            {
                return _line;
            }
            set
            {
                _line = value;
            }
        }
        public static byte LastInLine
        {
            get
            {
                return _lastInLine;
            }
            set
            {
                _lastInLine = value;
            }
        }
        public static uint ErrCount
        {
            get
            {
                return _errCount;
            }
            set
            {
                _errCount = value;
            }
        }

        public static void NextCh()
        {
            if (IsEoF)
            {
                Ch = '\0';
                return;
            }
            if (_positionNow.CharNumber >= _lastInLine)
            {
                ListThisLine();
                if (_err.Count > 0)
                {
                    ListErrors();
                }
                ReadNextLine();

                if (_line == null)
                {
                    Ch = '\0';
                    return;
                }

                _positionNow.LineNumber ++;
                _positionNow.CharNumber = 0;
            }
            else
            {
                ++_positionNow.CharNumber;
            }

            Ch = _line[_positionNow.CharNumber];
        }

        private static void ListThisLine()
        {
            Console.WriteLine(_line);
        }

        private static void ReadNextLine()
        {
            if (!File.EndOfStream)
            {
                _line = File.ReadLine() + " ";
                _lastInLine = (byte)(_line.Length - 1);
            }
            else
            {
                _line = null;
                IsEoF = true;
                End();
            }
        }

        private static void End()
        {
            Console.WriteLine($"Компиляция завершена: ошибок - {_errCount}!");
        }

        private static void ListErrors()
        {
            int pos = $"{_positionNow.LineNumber} ".Length;
            string s;
            foreach (Err item in _err)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ++_errCount;
                s = "";

                while (s.Length + 2 < pos + item.ErrorPosition.CharNumber)
                {
                    s += " ";
                }

                s += $"^ **{_errCount}** ошибка код {item.ErrorCode}";
                Console.WriteLine(s);
            }
            _err.Clear();
            Console.ResetColor();
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            Err e;
            if (_err.Count <= ERRMAX)
            {
                e = new Err(position, errorCode);
                _err.Add(e);
            }
        }

        public static void Reset()
        {
            Ch = ' ';
            IsEoF = false;
            _line = "";
            _lastInLine = 0;
            _errCount = 0;
            _err.Clear();
            PositionNow = new TextPosition(1, 0);
        }
    }
}
