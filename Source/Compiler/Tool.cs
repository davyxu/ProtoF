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

        Dictionary<string, FileNode> _fileMap = new Dictionary<string, FileNode>();

        public FileNode GetFileNode( string name )
        {
            FileNode n;
            if (_fileMap.TryGetValue(name, out n))
                return n;

            return null;
        }

        public void AddFileNode( FileNode n )
        {
            _fileMap.Add(n.Name, n);
        }




        public void CheckDuplicate(Location loc, string packageName, string name)
        {
            foreach (var v in _fileMap)
            {
                if (v.Value.StaticSymbols.Get(packageName, name) != null)
                {
                    Reporter.Error( ErrorType.Parse,loc, "{0} already defined in {1} package", name, packageName);
                }
            }
        }

        public Node LookUpSymbols( string packageName, string name )
        {
            foreach (var v in _fileMap)
            {
                var symbols = v.Value.StaticSymbols.Get(packageName, name);
                if ( symbols != null)
                {
                    return symbols;
                }
            }

            return null;
        }



        public string GetUsableFileName(string filename)
        {
            if (string.IsNullOrEmpty(SearchPath))
                return filename;

            return Path.Combine(SearchPath, filename);
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
