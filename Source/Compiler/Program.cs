using System.Text;
using System.IO;
using ProtoF.AST;
using ProtoF.Parser;
using ProtoF.Printer;

namespace ProtoF
{
    class Program
    {
        static void Main(string[] args)
        {
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
