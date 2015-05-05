using ProtoF.AST;
using ProtoF.Scanner;

namespace ProtoF.Parser
{
    public partial class SchemaParser
    {
        EnumNode ParseEnum()
        {
            var node = new EnumNode();
            
            ParseCommentAndEOL(node);

            // message的头注释
            Expect(TokenType.Enum);
            MarkLocation(node);

            Check(TokenType.Identifier, "require enum type name");

            node.Name = CurrToken.Value;
            Next();

            Consume(TokenType.EOL);

            Expect(TokenType.LBrace); Consume(TokenType.EOL);


            while (CurrToken.Type != TokenType.RBrace)
            {
                var valueNode = new EnumValueNode();                

                // 字段的头注释
                ParseCommentAndEOL(valueNode);

                Check(TokenType.Identifier, "require enum name");
                MarkLocation(valueNode);

                
               
                valueNode.Name = CurrToken.Value;

                CheckDuplicate(node, _lexer.Loc, valueNode.Name);

                Next();

                Expect(TokenType.Assign);

                Check(TokenType.Number, "require enum value");
                valueNode.Number = CurrToken.ToInteger();
                node.AddValue(valueNode);
                Next();

                // 尾注释
                ParseTrailingComment(valueNode);                
            }

            Expect(TokenType.RBrace); Consume(TokenType.EOL);

            return node;
        }
    }
}
