using System;
using System.Collections.Generic;
using System.IO;
using Compiler;

internal class Program
{                                                                                                                                                                   
    private static void Main(string[] args)
    {
        string FilePath = "C:\\Users\\elise\\OneDrive\\УНИК\\2Курс\\2\\ЯП\\kt4\\lab10\\Main\\test.txt";
        string Stokens = "C:\\Users\\elise\\OneDrive\\УНИК\\2Курс\\2\\ЯП\\kt4\\lab10\\Main\\Tokens.txt";

        try
        {
            InputOutput.File = new StreamReader(FilePath);
            StreamWriter FileTokens = new StreamWriter(Stokens);

            FileTokens.AutoFlush = true;

            Console.WriteLine("ТЕСТ ВВОДА-ВЫВОДА");
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
            InputOutput.File.Close();


            Console.WriteLine("\nТЕСТ ЛЕКСИЧЕСКОГО АНАЛИЗАТОРА");
            InputOutput.Reset();
            InputOutput.File = new StreamReader (FilePath);
            InputOutput.NextCh();
            LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();
            while (!InputOutput.IsEoF)
            {
                byte tokenCode = lexicalAnalyzer.NextSym();
                

                FileTokens.Write(tokenCode + " ");
                Console.Write(tokenCode + " ");
            }


            FileTokens.Close();
            InputOutput.File.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при открытии файла");
            Console.WriteLine(e);
            return;
        }
    }
}