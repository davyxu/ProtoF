using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoF.Parser;
using System.IO;
using ProtoF.Formater;

namespace ProtoF
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser.SchemaParser();

            var inputFile = "../../../proto/protof.protof";

            var data = File.ReadAllText(inputFile, Encoding.UTF8);

            var file = parser.Parse(data);

            var str = ProtoFFormater.PrintFile(file);
            File.WriteAllText(Path.ChangeExtension(inputFile, ".txt"), str, Encoding.UTF8);
        }
    }
}
