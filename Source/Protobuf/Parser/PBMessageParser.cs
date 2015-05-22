using ProtoTool.Schema;
using ProtoTool.Scanner;
using System.Collections.Generic;

namespace ProtoTool.Protobuf
{
    public partial class ProtobufParser : Parser
    {
        void ParseMessage( FileNode filenode )
        {
            var node = new MessageNode();

            // message的头注释
            ParseCommentAndEOL(node);

            Consume(TokenType.Message);
            MarkLocation(node);

            node.Name = FetchToken(TokenType.Identifier, "require message type name").Value;

            CheckDuplicate(node.Loc, filenode.Package, node.Name);

            filenode.AddMessage(node);

            AddSymbol(filenode.Package, node.Name, node);


            TryConsume(TokenType.EOL);

            Consume(TokenType.LBrace); TryConsume(TokenType.EOL);


            while (CurrToken.Type != TokenType.RBrace)
            {
                // 内嵌枚举
                if (CurrToken.Type == TokenType.Enum)
                {
                    ParseEnum(filenode);                   
                }

                ParseField(filenode, node);
            }

            Consume(TokenType.RBrace); TryConsume(TokenType.EOL);
        }


        void ParseLabel( FileNode fn, FieldNode fieldNode )
        {            
            switch (CurrToken.Type)
            {
                case TokenType.Repeated:
                    fieldNode.PBLabel = PBFieldLabel.Repeated;
                    fieldNode.Container = FieldContainer.Array;
                    break;
                case TokenType.Required:
                    fieldNode.PBLabel = PBFieldLabel.Required;
                    break;
                case TokenType.Optional:
                    fieldNode.PBLabel = PBFieldLabel.Optional;
                    break;
                default:
                    {
                        Error(_lexer.Loc, "Unknown label");
                        return;
                    }
            }

            Next();
        }


        void ParseField( FileNode fn, MessageNode node )
        {
            var fieldNode = new FieldNode();

            // 字段的头注释
            ParseCommentAndEOL(node);

            ParseLabel(fn, fieldNode);

            ParseFieldType(fn, node, fieldNode);

            ParseFieldName(node, fieldNode);

            ParseNumber(node, fieldNode);

            if ( CurrToken.Type == TokenType.LSqualBracket)
            {                
               ParseOption(node, fieldNode);
            }

            Consume(TokenType.SemiColon);


            // 尾注释
            ParseTrailingComment(fieldNode);

            ParseCommentAndEOL(node);
        }

        void ParseNumber(MessageNode node, FieldNode fieldNode)
        {
            Consume(TokenType.Assign);
            fieldNode.Number = FetchToken(TokenType.Number, "require field number").ToInteger();
        }

        void ParseOption(MessageNode node, FieldNode fieldNode)
        {
            // [
            Consume(TokenType.LSqualBracket);

            fieldNode.HasOption = true;

            while( CurrToken.Type != TokenType.RSqualBracket )
            {
                Location loc = _lexer.Loc;

                var key = FetchToken(TokenType.Identifier, "require option identify");
                Consume(TokenType.Assign);
                var value = CurrToken;
                Next();

                if ( key.Value == "default" )
                {
                    fieldNode.DefaultValue = value.Value;
                }
                else
                {
                    Error(loc, "unknown field option '{0}'", key.Value);
                }
            }

            // ]
            Consume(TokenType.RSqualBracket);
        }

        void ParseFieldName(MessageNode node, FieldNode fieldNode)
        {
            MarkLocation(fieldNode);
            // 字段名
            fieldNode.Name = FetchToken(TokenType.Identifier, "require field name").Value;
            
            CheckDuplicate(node, _lexer.Loc, fieldNode.Name);
        }


        private void ParseFieldType(FileNode fn, MessageNode node, FieldNode fieldNode)
        {
            // 字段类型
            fieldNode.Type = GetFieldType();
            fieldNode.TypeName = CurrToken.Value;

            if (fieldNode.Type == FieldType.None && !ResolveFieldType(fn, fieldNode))
            {
                AddUnsolveNode(fieldNode);
            }

            node.AddField(fieldNode);


            Next();

        }

        FieldType GetFieldType()
        {
            FieldType ret = FieldType.None;

            switch (CurrToken.Type)
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
                default:
                    return FieldType.None;
                    
            }


            return ret;
        }


      
    }
}
