using System;
using System.Collections.Generic;
using System.IO;
using Compiler;

internal class Program
{
    private static void Main(string[] args)
    {
        string FilePath = "C:\\Users\\elise\\OneDrive\\УНИК\\2Курс\\2\\ЯП\\kt4\\lab10\\Main";
        try
        {
            InputOutput.File = new StreamReader(FilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return;
        }

        Console.WriteLine("Тест ввода-вывода");

        string firstLine = InputOutput.File.ReadLine();


    }
}