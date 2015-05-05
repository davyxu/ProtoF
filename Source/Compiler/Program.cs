using System.Text;
using System.IO;
using ProtoF.AST;
using ProtoF.Parser;

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

            var subopt = new PrintOption();
            subopt.Format = PrintFormat.ProtoF;
            //subopt.ShowAllFieldNumber = true;
            //subopt.ShowAllEnumNumber = true;
            file.Print(sb, subopt );
           
            File.WriteAllText(Path.ChangeExtension(inputFile, ".txt"), sb.ToString(), Encoding.UTF8);
        }
    }
}
