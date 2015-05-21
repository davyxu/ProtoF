using ProtoF.AST;
using ProtoF.Scanner;
using System.Collections.Generic;
using System.IO;

namespace ProtoF
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

    }
}
