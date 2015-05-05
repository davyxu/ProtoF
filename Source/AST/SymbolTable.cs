using System.Collections.Generic;

namespace ProtoF.AST
{
    public class SymbolTable
    {
        Dictionary<string, Node> _symbols = new Dictionary<string, Node>();

        public void Add(string packageName, string name, Node n )
        {            
            _symbols.Add(MakeKey(packageName, name ), n );
        }

        public Node Get( string packageName, string name )
        {
            Node n;
            if ( _symbols.TryGetValue(MakeKey(packageName, name), out n ) )
            {
                return n;
            }

            return null;
        }

        public void Clear()
        {
            _symbols.Clear();
        }

        string MakeKey( string packageName, string name )
        {
            return packageName + "." + name;
        }
    }
}
