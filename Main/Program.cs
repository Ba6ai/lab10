using System;
using System.Collections.Generic;
using System.IO;
using Compiler;

internal class Program
{                                                                                                                                                                   
    private static void Main(string[] args)
    {
        string FilePath = "C:\\Users\\elise\\OneDrive\\УНИК\\2Курс\\2\\ЯП\\kt4\\lab10\\Main\\test.txt";
        string outputFile = "C:\\Users\\elise\\OneDrive\\УНИК\\2Курс\\2\\ЯП\\kt4\\lab10\\Main\\errCode.txt";


        try
        {
            InputOutput.File = new StreamReader(FilePath);
            //StreamWriter FileErrCode = new StreamWriter(outputFile);

            Console.WriteLine("Тест ввода-вывода");

            InputOutput.NextCh();

            while (!InputOutput.IsEoF)
            {
                if (InputOutput.Ch == 'e')
                {
                    InputOutput.Error(42, InputOutput.PositionNow);
                }
                if (InputOutput.Ch == 'h')
                {
                    InputOutput.Error(52, InputOutput.PositionNow);
                }

                InputOutput.NextCh();
            }

            /*
            Console.WriteLine("\nТест лексического анализатора");
            InputOutput.File.Close();
            InputOutput.File = new StreamReader(FilePath);
            InputOutput.Reset();
            InputOutput.NextCh();

            LexicalAnalyzer Analyzer = new LexicalAnalyzer();
            while (true)
            {
                byte tokenCode = Analyzer.NextSym();

                if (tokenCode == 0)
                {
                    break;
                }

                FileErrCode.Write(tokenCode + " ");
                Console.Write(tokenCode + " ");
            }
            */
            InputOutput.File.Close();
            //FileErrCode.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при открытии файла");
            Console.WriteLine(e);
            return;
        }
    }
}