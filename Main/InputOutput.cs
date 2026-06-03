using System;
using System.Collections.Generic;
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
        public static char Ch
        {
            get;
            set;
        }

        public static TextPosition positionNow = new TextPosition();
        private static string line = "";
        private static byte lastInLine = 0;

        public static List<Err> err = new List<Err>();

        public static StreamReader File
        {
            get;
            set;
        }

        private static uint errCount = 0;

        public static bool IsEoF
        {
            get;
            private set;
        } = false;

        public static void NextCh()
        {
            if (positionNow.CharNumber >= lastInLine)
            {
                ListThisLine();
                if (err.Count > 0)
                {
                    ListErrors();
                }
                ReadNextLine();

                if (line == null)
                {
                    return;
                }
 
                positionNow.LineNumber = positionNow.LineNumber + 1;
                positionNow.CharNumber = 0;
            }
            else
            {
                ++positionNow.CharNumber;
            }

            Ch = line[positionNow.CharNumber];
        }

        private static void ListThisLine()
        {
            Console.WriteLine(line);
        }

        private static void ReadNextLine()
        {
            if (!File.EndOfStream)
            {
                line = File.ReadLine() + " ";
                lastInLine = (byte)(line.Length - 1);
            }
            else
            {
                line = null;
                IsEoF = true;
                End();
            }
        }

        private static void End()
        {
            Console.WriteLine($"Компиляция завершена: ошибок - {errCount}!");
        }

        private static void ListErrors()
        {
            int pos = $"{positionNow.LineNumber} ".Length;
            string s;
            foreach (Err item in err)
            {
                ++errCount;
                s = "";

                while (s.Length + 2 < pos + item.ErrorPosition.CharNumber)
                {
                    s += " ";
                }

                s += $"^ **{errCount}** ошибка код {item.ErrorCode}";
                Console.WriteLine(s);
            }
            err.Clear();
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            Err e;
            if (err.Count <= ERRMAX)
            {
                e = new Err(position, errorCode);
                err.Add(e);
            }
        }

        public static void Reset()
        {
            IsEoF = false;
            line = "";
            lastInLine = 0;
            positionNow = new TextPosition(1, 0);
        }
    }
}
