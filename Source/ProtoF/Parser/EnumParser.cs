using ProtoF.AST;
using ProtoF.Scanner;

namespace ProtoF.Parser
{
    public partial class ProtoFParser
    {
        EnumNode ParseEnum()
        {
            var node = new EnumNode();
            
            ParseCommentAndEOL(node);

            // message的头注释
            Consume(TokenType.Enum);
            MarkLocation(node);

            node.Name = FetchToken(TokenType.Identifier, "require enum type name").Value;

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

                if ( TryConsume(TokenType.Assign) )
                {
                    valueNode.Number = FetchToken(TokenType.Number, "require enum value").ToInteger();
                }
                else
                {
                    valueNode.NumberIsAutoGen = true;
                }

                
                node.AddValue(valueNode);


                // 尾注释
                ParseTrailingComment(valueNode);                
            }

            Consume(TokenType.RBrace); TryConsume(TokenType.EOL);

            FillEnumNumber(node);

            return node;
        }

        void FillEnumNumber(EnumNode node)
        {
            int autoNumber = 1;

            foreach (var en in node.Value)
            {
                if (en.NumberIsAutoGen)
                {
                    en.Number = autoNumber;
                }
                else
                {
                    if (en.Number < autoNumber)
                    {
                        Error(en.Loc, "enum number < auto gen number {0}", autoNumber);
                        continue;
                    }

                    autoNumber = en.Number;
                }

                autoNumber++;
            }
        }

    }
}
