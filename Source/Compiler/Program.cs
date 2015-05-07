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

            var parser = new ProtoFParser();

            var inputFile = "../../../proto/protof.protof";

            var data = File.ReadAllText(inputFile, Encoding.UTF8);

            var file = parser.Parse(data, Path.GetFileName(inputFile) );

            var sb = new StringBuilder();

            var opt = new PrintOption();
            opt.Format = PrintFormat.ProtoF;
            //subopt.ShowAllFieldNumber = true;
            //subopt.ShowAllEnumNumber = true;

            var printer = new ProtoFPrinter();
            file.PrintVisit(printer, sb, opt);

           
            File.WriteAllText(Path.ChangeExtension(inputFile, ".txt"), sb.ToString(), Encoding.UTF8);
        }
    }
}
