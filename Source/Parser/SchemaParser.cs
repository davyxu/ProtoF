using ProtoF.Scanner;
using System;
using ProtoF.AST;

namespace ProtoF.Parser
{
    public partial class SchemaParser
    {
        Lexer _lexer = new Lexer();

        public SchemaParser()
        {
            _lexer.AddMatcher(new TokenMatcher[]{
                new NumeralMatcher(),
                
                new LineEndMatcher().Ignore(),
                new WhitespaceMatcher().Ignore(),
                new CommentMatcher().Ignore(),
                
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
            _lexer.Start(source, srcName);

            Next();

            return ParseFile();
        }

        public override string ToString()
        {
            return _lexer.ToString();
        }

        void Next()
        {
            _lexer.Read();
        }


        static TokenMatcher[] _commentMatcher = new TokenMatcher[]{
            new CommentMatcher(),
            new WhitespaceMatcher().Ignore(),
            new LineEndMatcher(),
            new UnknownMatcher(),
        };
        
        string ReadComment( )
        {
            var token = _lexer.ReadByMatcher(_commentMatcher);
            if ( token.Type != TokenType.Comment )
            {
                Next();
                return null;
            }
            Next();

            return token.Value;
        }

        void Check(TokenType t, string error )
        {
            if (CurrToken.Type != t)
            {
                throw new Exception(error);
            }            
        }

        void Expect(TokenType t)
        {
            if (CurrToken.Type != t)
            {
                throw new Exception(string.Format("expect token: {0}", t.ToString()));
            }

            Next();
        }

        void Error(string str)
        {
            throw new Exception(str);
        }
    }

}
