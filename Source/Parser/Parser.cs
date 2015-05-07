using ProtoF.Scanner;
using System;
using ProtoF.AST;

namespace ProtoF.Parser
{
    public class Parser
    {
        protected Lexer _lexer = new Lexer();
        protected SymbolTable _symbols = new SymbolTable();

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
    }

}
