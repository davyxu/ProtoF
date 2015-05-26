using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoTool.Schema
{
    public class FileNode : Node
    {
        public string Package;
        public List<ImportNode> Import = new List<ImportNode>();
        public List<MessageNode> Message = new List<MessageNode>();
        public List<EnumNode> Enum = new List<EnumNode>();

        

        public void AddMessage(MessageNode n)
        {
            Child.Add(n);
            Message.Add(n);
        }        


        public MessageNode GetMessageByName( string name )
        {
            return Message.Find(x=> x.Name == name );
        }

        public EnumNode GetEnumByName(string name)
        {
            return Enum.Find(x => x.Name == name);
        }

        public void AddEnum(EnumNode n)
        {
            Child.Add(n);
            Enum.Add(n);
        }

        public override string ToString()
        {
            return string.Format("{0} msg:{1} enum:{2}", Name, Message.Count, Enum.Count);
        }

        public override void PrintVisit(Printer printer, StringBuilder sb, PrintOption opt, params object[] values)
        {
            printer.Print(this, sb, opt, values);
        }
        

        Stack<SymbolTable> _symbolStack = new Stack<SymbolTable>();

        public void AddSymbol(string packageName, string name, Node n)
        {
            _symbolStack.Peek().Add(packageName, name, n);
        }

        public SymbolTable ScopeSymbols
        {
            get { return _symbolStack.Peek(); }
        }

        SymbolTable _staticSymbols;

        public SymbolTable StaticSymbols
        {
            get { 

                if ( _symbolStack.Count > 0 )
                {
                    return ScopeSymbols;
                }

                return _staticSymbols; 
            }
        }

        public bool IsTopScope()
        {
            return _symbolStack.Count == 1;
        }

        public void EnterScope()
        {
            _symbolStack.Push(new SymbolTable());
        }

        public void LeaveScope()
        {
            if ( IsTopScope() )
            {
                _staticSymbols = ScopeSymbols;
            }

            _symbolStack.Pop();
        }
    }

    public class PackageNode : Node
    {        
        public override string ToString()
        {
            return base.ToString() + " " + Name;
        }

        public override void PrintVisit(Printer printer, StringBuilder sb, PrintOption opt, params object[] values)
        {
            printer.Print(this, sb, opt, values);
        }
    }

    public class ImportNode : Node
    {     

        public override string ToString()
        {
            return base.ToString() + " " + Name;
        }

        public override void PrintVisit(Printer printer, StringBuilder sb, PrintOption opt, params object[] values)
        {
            printer.Print(this, sb, opt, values);
        }
    }
}
