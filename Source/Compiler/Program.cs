using System.Text;
using System.IO;
using ProtoF.AST;

namespace ProtoF
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser.SchemaParser();

            var inputFile = "../../../proto/protof.protof";

            var data = File.ReadAllText(inputFile, Encoding.UTF8);

            var file = parser.Parse(data, Path.GetFileName(inputFile) );

            var sb = new StringBuilder();
            file.Print(sb, PrintFormat.ProtoF);
           
            File.WriteAllText(Path.ChangeExtension(inputFile, ".txt"), sb.ToString(), Encoding.UTF8);
        }
    }
}
