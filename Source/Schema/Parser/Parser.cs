using ProtoTool.Scanner;
using System;
using ProtoTool.Schema;
using System.Text;
using System.IO;

namespace ProtoTool.Schema
{
    public partial class Parser
    {
        protected Lexer _lexer = new Lexer();
        

        protected Tool _tool;

        public Parser( Tool t )
        {
            _tool = t;
        }

        public Token CurrToken
        {
            get { return _lexer.CurrToken; }
        }

        // 给节点标记在文本中的位置
        public void MarkLocation(Node n)
        {
            n.Loc = _lexer.Loc;
        }

        public override string ToString()
        {
            return _lexer.ToString();
        }

        public void Next()
        {
            _lexer.Read();
        }

        static Token _err = new Token(TokenType.Unknown, "err");
        public Token FetchToken(TokenType t, string fmt, params object[] objs)
        {
            if (CurrToken.Type != t)
            {
                Error(_lexer.Loc, fmt, objs);
                return _err;
            }

            var tk = CurrToken;

            Next();

            return tk;
        }

        public void Consume(TokenType t)
        {
            if (CurrToken.Type != t)
            {
                Error(_lexer.Loc, "expect token: {0}", t.ToString());
            }

            Next();
        }

        public bool TryConsume(TokenType t)
        {
            if (CurrToken.Type == t)
            {
                Next();
                return true;
            }

            return false;
        }

        public void Error(string fmt, params object[] objs)
        {
            Error(null, fmt, objs);
        }

        public void Error(Location loc, string fmt, params object[] objs)
        {
            string str;
            if (loc != null)
            {
                str = loc + " " + string.Format(fmt, objs);
            }
            else
            {
                str = string.Format(fmt, objs);
            }

            Console.WriteLine(str);

            throw new Exception(str);
        }


        public FileNode StartParseFile(string filename)
        {
            var n = _tool.GetFileNode(filename);
            if (n != null)
                return n;

            var inputFile = _tool.GetUsableFileName(filename);

            var data = File.ReadAllText(inputFile, Encoding.UTF8);

            return StartParse(data, Path.GetFileName(inputFile));
        }


        public FileNode StartParse(string source, string srcName)
        {
            _unsolvedNode.Clear();
            _lexer.Start(source, srcName);

            Next();

            return ParseFile(srcName);
        }

        public virtual FileNode ParseFile( string srcName )
        {
            throw new NotImplementedException();
        }


        protected void AddSymbol(string packageName, string name, Node n)
        {
            _tool.Symbols.Add(packageName, name, n);
        }


        protected void CheckDuplicate(Location loc, string packageName, string name)
        {
            if (_tool.Symbols.Get(packageName, name) != null)
            {
                Error(loc, "{0} already defined in {1} package", name, packageName);
            }
        }

        protected void CheckDuplicate(ContainerNode n, Location loc, string name)
        {
            if (n.Contain(name))
            {
                Error(loc, "{0} already defined", name);
            }
        }
    }

}
