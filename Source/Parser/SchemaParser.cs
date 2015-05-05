using ProtoF.Scanner;
using System;
using ProtoF.AST;

namespace ProtoF.Parser
{
    public partial class SchemaParser
    {
        Lexer _lexer = new Lexer();
        SymbolTable _symbols = new SymbolTable();

        public SchemaParser()
        {
            _lexer.AddMatcher(new TokenMatcher[]{
                new NumeralMatcher(),
                
                new LineEndMatcher(),
                new WhitespaceMatcher().Ignore(),
                new CommentMatcher(),
                
                new KeywordMatcher(TokenType.Assign, "="),          
                new KeywordMatcher(TokenType.LBracket, "("),
                new KeywordMatcher(TokenType.RBracket, ")"),
                new KeywordMatcher(TokenType.Comma, ","),
                new KeywordMatcher(TokenType.Dot, "."),
                new KeywordMatcher(TokenType.SemiColon, ";"),
                new KeywordMatcher(TokenType.LSqualBracket, "["),
                new KeywordMatcher(TokenType.RSqualBracket, "]"),
                new KeywordMatcher(TokenType.LBrace, "{"),
                new KeywordMatcher(TokenType.RBrace, "}"),
                
                new KeywordMatcher(TokenType.LAngleBracket, "<"),
                new KeywordMatcher(TokenType.RAngleBracket, ">"),
                
                new KeywordMatcher(TokenType.Package, "package"),
                new KeywordMatcher(TokenType.Import, "import"),
                new KeywordMatcher(TokenType.Enum, "enum"),
                new KeywordMatcher(TokenType.Message, "message"),
                
                new KeywordMatcher(TokenType.Bool, "bool"),
                new KeywordMatcher(TokenType.Int32, "int32"),
                new KeywordMatcher(TokenType.UInt32, "uint32"),
                new KeywordMatcher(TokenType.Int64, "int64"),
                new KeywordMatcher(TokenType.UInt64, "uint64"),
                new KeywordMatcher(TokenType.String, "string"),
                new KeywordMatcher(TokenType.Float, "float"),
                new KeywordMatcher(TokenType.Double, "float64"),
                new KeywordMatcher(TokenType.Bytes, "bytes"),
                new KeywordMatcher(TokenType.Array, "array"),

                new IdentifierMatcher(),
                new UnknownMatcher(),
            });
        }

        public Token CurrToken
        {
            get { return _lexer.CurrToken;  }
        }

        public FileNode Parse(string source, string srcName)
        {
            _unsolvedNode.Clear();
            _symbols.Clear();
            _lexer.Start(source, srcName);

            Next();

            return ParseFile();
        }

        // 给节点标记在文本中的位置
        void MarkLocation( Node n )
        {
            n.Loc = _lexer.Loc;
        }

        public override string ToString()
        {
            return _lexer.ToString();
        }

        void Next()
        {
            _lexer.Read();
        }


        void Check(TokenType t, string fmt, params object[] objs)
        {
            if (CurrToken.Type != t)
            {
                Error(fmt, objs);
            }            
        }

        void Expect(TokenType t)
        {
            if (CurrToken.Type != t)
            {
                Error("expect token: {0}", t.ToString());
            }

            Next();
        }

        void Consume(TokenType t )
        {
            if (CurrToken.Type == t)
            {
                Next();
            }
        }

        void Error(string fmt, params object[] objs)
        {            
            Error(null, fmt, objs);
        }

        void Error( Location loc, string fmt, params object[] objs )
        {
            string str;
            if ( loc != null )
            {
                str = loc + " " + string.Format(fmt, objs);
            }
            else
            {
                str = string.Format(fmt, objs);
            }

            Console.WriteLine(str);            
        }
    }

}
