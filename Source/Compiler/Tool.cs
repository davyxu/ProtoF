using ProtoTool.Scanner;
using ProtoTool.Schema;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProtoTool
{
    public class Tool
    {
        public string SearchPath { get; set; }

        Dictionary<string, FileNode> _fileNode = new Dictionary<string, FileNode>();

        public FileNode GetFileNode( string name )
        {
            FileNode n;
            if (_fileNode.TryGetValue(name, out n))
                return n;

            return null;
        }

        public void AddFileNode( FileNode n )
        {
            _fileNode.Add(n.Name, n);
        }

        public string GetUsableFileName(string filename)
        {
            if (string.IsNullOrEmpty(SearchPath))
                return filename;

            return Path.Combine(SearchPath, filename);
        }


        SymbolTable _symbols = new SymbolTable();

        public SymbolTable Symbols
        {
            get { return _symbols; }
        }

        public static void Convertor(string inputFileName, string outputFileName, Parser parser, Printer printer)
        {
            var file = parser.StartParseFile(inputFileName);

            var sb = new StringBuilder();

            var opt = new PrintOption();
            opt.Format = PrintFormat.ProtoF;
            //subopt.ShowAllFieldNumber = true;
            //subopt.ShowAllEnumNumber = true;

            file.PrintVisit(printer, sb, opt);

            File.WriteAllText(outputFileName, sb.ToString(), new UTF8Encoding(false));
        }

    }
}
