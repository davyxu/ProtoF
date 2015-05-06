using System.Text;
using System.IO;
using ProtoF.AST;
using ProtoF.Parser;
using ProtoF.Printer;
using CommandLine;
using CommandLine.Text;
using System;

namespace ProtoF
{
    class CommandOptions
    {
        [Option('m', "method", Required = true, HelpText = "method to operate")]
        public string Method { get; set; }

        [Option('i',"inputfile", Required=true, HelpText="input file name")]
        public string InputFile { get; set; }

        [Option('o', "outputfile", Required = true, HelpText = "output file name")]
        public string OutputFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => {


                  HelpText.DefaultParsingErrorsHandler(this, current);
              });
        }
    }

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

            var parser = new SchemaParser();

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
