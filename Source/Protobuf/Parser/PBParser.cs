using ProtoTool.Scanner;
using ProtoTool.Schema;
using System.IO;
using System.Text;

namespace ProtoTool.Protobuf
{
    public partial class ProtobufParser : Parser
    {
        public ProtobufParser(Tool t)
            : base(t)
        {
            _lexer.AddMatcher(new TokenMatcher[]{
                new NumeralMatcher(),
                
                new LineEndMatcher(),
                new WhitespaceMatcher().Ignore(),
                new CommentMatcher(),
                new QuotedStringMatcher(),
                
                new KeywordMatcher(TokenType.Assign, "="),                          
                new KeywordMatcher(TokenType.Comma, ":"),
                new KeywordMatcher(TokenType.Dot, "."),
                new KeywordMatcher(TokenType.Sub, "-"),
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

                new KeywordMatcher(TokenType.Optional, "optional"),
                new KeywordMatcher(TokenType.Required, "required"),
                new KeywordMatcher(TokenType.Repeated, "repeated"),
                
                new KeywordMatcher(TokenType.Bool, "bool"),
                new KeywordMatcher(TokenType.Int32, "int32"),
                new KeywordMatcher(TokenType.UInt32, "uint32"),
                new KeywordMatcher(TokenType.Int64, "int64"),
                new KeywordMatcher(TokenType.UInt64, "uint64"),
                new KeywordMatcher(TokenType.String, "string"),
                new KeywordMatcher(TokenType.Float, "float"),
                new KeywordMatcher(TokenType.Double, "float64"),
                new KeywordMatcher(TokenType.Bytes, "bytes"),                

                new IdentifierMatcher(),
                new UnknownMatcher(),
            });
        }
    }

}
