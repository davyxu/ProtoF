using ProtoTool.Schema;
using ProtoTool.Scanner;

namespace ProtoTool.Protobuf
{
    public partial class ProtobufParser : Parser
    {
        void ParseEnum(FileNode filenode )
        {
            var node = new EnumNode();
            
            ParseCommentAndEOL(node);

            // message的头注释
            Consume(TokenType.Enum);
            MarkLocation(node);

            node.Name = FetchToken(TokenType.Identifier, "require enum type name").Value;

            CheckDuplicate(node.Loc, filenode.Package, node.Name);

            filenode.AddEnum(node);

            AddSymbol(filenode.Package, node.Name, node);

            TryConsume(TokenType.EOL);

            Consume(TokenType.LBrace); TryConsume(TokenType.EOL);


            while (CurrToken.Type != TokenType.RBrace)
            {
                var valueNode = new EnumValueNode();                

                // 字段的头注释
                ParseCommentAndEOL(valueNode);


                MarkLocation(valueNode);
                valueNode.Name = FetchToken(TokenType.Identifier, "require enum name").Value;
               
                CheckDuplicate(node, _lexer.Loc, valueNode.Name);

                Consume(TokenType.Assign);                
                valueNode.Number = FetchToken(TokenType.Number, "require enum value").ToInteger();

                Consume(TokenType.SemiColon);
                
                node.AddValue(valueNode);


                // 尾注释
                ParseTrailingComment(valueNode);

                ParseCommentAndEOL(valueNode);
            }

            Consume(TokenType.RBrace); TryConsume(TokenType.EOL);
        }

    }
}
