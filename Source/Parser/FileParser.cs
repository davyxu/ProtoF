using System.Collections.Generic;
using ProtoF.Scanner;
using ProtoF.AST;

namespace ProtoF.Parser
{
    public partial class SchemaParser
    {
        FileNode ParseFile( )
        {
            var node = new FileNode();

            Expect( TokenType.Package );

            Check(TokenType.Identifier, "require package name");

            node.Package = CurrToken.Value;
            Next();

            while (CurrToken.Type != TokenType.EOF )
            {
                switch (CurrToken.Type)
                {
                    case TokenType.Message:
                        {
                            node.AddMessage(ParseMessage());                            
                        }
                        break;
                    case TokenType.Enum:
                        {
                            node.AddEnum(ParseEnum());                            
                        }
                        break;
                    default:
                        {
                            Error(string.Format("unexpect token {0}", _lexer.CurrChar));
                        }
                        break;
                }
            }

            return node;
        }

        EnumNode ParseEnum( )
        {
            var node = new EnumNode();

            Expect(TokenType.Enum);

            Check(TokenType.Identifier, "require enum type name");
 
            node.Name = CurrToken.Value;
            Next();

            Expect(TokenType.LBrace);


            while (CurrToken.Type != TokenType.RBrace)
            {
                Check(TokenType.Identifier, "require enum name");

                var valueNode = new EnumValueNode();
                valueNode.Name = CurrToken.Value;

                Next();

                Expect(TokenType.Assign);

                Check(TokenType.Number, "require enum value");
                valueNode.Number = CurrToken.ToInteger();
                
                var comment = ReadComment();
                if ( comment != null )
                {
                    valueNode.TrailingComment = comment;
                }

                node.AddValue(valueNode);                
            }

            Expect(TokenType.RBrace);

            return node;
        }

        List<FieldNode> _unsolvedNode = new List<FieldNode>();

        MessageNode ParseMessage()
        {
            var node = new MessageNode();

            Expect(TokenType.Message);

            Check(TokenType.Identifier, "require message type name");

            node.Name = CurrToken.Value;

            Next();

            Expect(TokenType.LBrace);


            while (CurrToken.Type != TokenType.RBrace)
            {
                // 内嵌枚举
                if ( CurrToken.Type == TokenType.Enum )
                {
                    node.AddEnum(ParseEnum());                    
                }

                // 字段名
                Check(TokenType.Identifier, "require field name");

                var fieldNode = new FieldNode();
                fieldNode.Name = CurrToken.Value;

                Next();

                if ( CurrToken.Type == TokenType.Array )
                {
                    fieldNode.Container = FieldContainer.Array;
                    Next();
                    Expect(TokenType.LAngleBracket);
                }
                else
                {
                    fieldNode.Container = FieldContainer.None;
                }
                

                // 字段类型
                fieldNode.Type = GetFieldType();
                fieldNode.TypeName = CurrToken.Value;
                
                node.AddField(fieldNode);

                if ( fieldNode.Type == FieldType.None )
                {
                    _unsolvedNode.Add(fieldNode);
                }

                if ( fieldNode.Container == FieldContainer.Array )
                {
                    Next();
                    Check(TokenType.RAngleBracket, string.Format("expect token: {0}", TokenType.RAngleBracket.ToString()));
                }

                var comment = ReadComment();
                if (comment != null)
                {
                    fieldNode.TrailingComment = comment;
                }                

            }

            Expect(TokenType.RBrace);

            return node;
        }

        FieldType GetFieldType( )
        {
            FieldType ret = FieldType.None;

            switch( CurrToken.Type )
            {
                case TokenType.Bool:
                    ret = FieldType.Bool;
                    break;
                case TokenType.Int32:
                    ret = FieldType.Int32;
                    break;
                case TokenType.UInt32:
                    ret = FieldType.UInt32;
                    break;
                case TokenType.UInt64:
                    ret = FieldType.UInt64;
                    break;
                case TokenType.Int64:
                    ret = FieldType.Int64;
                    break;
                case TokenType.String:
                    ret = FieldType.String;
                    break;
                case TokenType.Float:
                    ret = FieldType.Float;
                    break;
                case TokenType.Double:
                    ret = FieldType.Double;
                    break;
                case TokenType.Bytes:
                    ret = FieldType.Bytes;
                    break;
            }


            return ret;
        }
    }
}

