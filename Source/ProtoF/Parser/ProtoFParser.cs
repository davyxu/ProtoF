using ProtoF.Scanner;
using System;
using ProtoF.AST;

namespace ProtoF.Parser
{
    public partial class ProtoFParser : Parser
    {
        public ProtoFParser()
        {
            _lexer.AddMatcher(new TokenMatcher[]{
                new NumeralMatcher(),
                
                new LineEndMatcher(),
                new WhitespaceMatcher().Ignore(),
                new CommentMatcher(),
                new QuotedStringMatcher(),
                
                new KeywordMatcher(TokenType.Assign, "="),          
                new KeywordMatcher(TokenType.LBracket, "("),
                new KeywordMatcher(TokenType.RBracket, ")"),
                new KeywordMatcher(TokenType.Comma, ":"),
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

 

        public FileNode Parse(string source, string srcName)
        {
            _unsolvedNode.Clear();
            _symbols.Clear();
            _lexer.Start(source, srcName);

            Next();

            return ParseFile();
        }
    }

}
