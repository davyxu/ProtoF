using System.Text;
using System.IO;
using ProtoF.Parser;
using ProtoF.Printer;
using CommandLine.Text;
using System;

namespace ProtoF
{

    class Program
    {
        private static readonly HeadingInfo HeadingInfo = new HeadingInfo("ProtoF", "1.0");

        static void Main(string[] args)
        {
            var options = new CommandOptions();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Environment.Exit(-1);
                return;
            }

            var tool = new Tool();
            tool.SearchPath = options.SearchPath;

            var file = new ProtoFParser(tool).StartParseFile(options.InputFile);

            var sb = new StringBuilder();

            var opt = new PrintOption();
            opt.Format = PrintFormat.ProtoF;
            //subopt.ShowAllFieldNumber = true;
            //subopt.ShowAllEnumNumber = true;

            file.PrintVisit(new ProtoFPrinter(), sb, opt);


            File.WriteAllText(options.OutputFile, sb.ToString(), Encoding.UTF8);
        }
    }
}
