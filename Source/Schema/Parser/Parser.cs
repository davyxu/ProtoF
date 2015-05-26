using ProtoTool.Scanner;
using System;
using ProtoTool.Schema;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace ProtoTool.Schema
{
    public partial class Parser
    {
        protected Lexer _lexer = new Lexer();

        protected FileNode _fileNode;

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
                Reporter.Error( ErrorType.Parse,_lexer.Loc, fmt, objs);
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
                Reporter.Error( ErrorType.Parse, _lexer.Loc, "expect token: {0}", t.ToString());
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

      


        public FileNode StartParseFile(string filename)
        {
            var n = _tool.GetFileNode(filename);
            if (n != null)
                return n;

            var inputFile = _tool.GetUsableFileName(filename);

            try
            {
                var data = File.ReadAllText(inputFile, Encoding.UTF8);

                return StartParse(data, Path.GetFileName(inputFile));
            }
            catch( FileNotFoundException e )
            {
                throw new ProtoExceptioin(ErrorType.Parse, e.Message);
            }
        }


        public FileNode StartParse(string source, string srcName)
        {
            _unsolvedNode.Clear();
            _lexer.Start(source, srcName);

            Next();

            _fileNode = new FileNode();

            _fileNode.EnterScope();

            ParseFile(_fileNode, srcName);

            _fileNode.LeaveScope();

            return _fileNode;
        }

        public virtual void ParseFile(FileNode node, string srcName)
        {
            throw new NotImplementedException();
        }



        



        protected void CheckDuplicate(ContainerNode n, Location loc, string name)
        {
            if (n.Contain(name))
            {
                Reporter.Error( ErrorType.Parse, loc, "{0} already defined", name);
            }
        }
    }

}
