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

            _tool.CheckDuplicate(node.Loc, filenode.Package, node.Name);

            filenode.AddMessage(node);

            _fileNode.AddSymbol(filenode.Package, node.Name, node);


            TryConsume(TokenType.EOL);
            ParseCommentAndEOL(node);
            Consume(TokenType.LBrace);
            TryConsume(TokenType.EOL);


            while (CurrToken.Type != TokenType.RBrace)
            {
                ParseCommentAndEOL(node);

                // 内嵌枚举
                while (CurrToken.Type == TokenType.Enum)
                {
                    _fileNode.EnterScope();
                    ParseEnum(filenode, node);
                    _fileNode.LeaveScope();

                    ParseCommentAndEOL(node);
                }

                if (CurrToken.Type == TokenType.RBrace)
                    break;

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
                case TokenType.Message:
                    {
                        Reporter.Error( ErrorType.Parse, _lexer.Loc, "DO NOT SUPPORT nested message type");
                    }
                    break;
                default:
                    {
                        Reporter.Error( ErrorType.Parse, _lexer.Loc, "Unknown label");
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
                
                if ( key.Value == "default" )
                {
                    fieldNode.DefaultValue = ReadDefaultValue(fieldNode);
                    Next();
                }
                else
                {
                    Reporter.Error( ErrorType.Parse, loc, "unknown field option '{0}'", key.Value);
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
            string packageName = fn.Package;
            // 字段类型
            fieldNode.Type = GetFieldType();
            fieldNode.TypeName = CurrToken.Value;

            var maybePackageName = CurrToken.Value;


            Next();

            if ( CurrToken.Type == TokenType.Dot )
            {
                Next();
                fieldNode.TypeName = CurrToken.Value;
                packageName = maybePackageName;

                Next();
            }

            if (fieldNode.Type == FieldType.None && !ResolveFieldType(packageName, fieldNode))
            {
                AddUnsolveNode(fieldNode);
            }

            node.AddField(fieldNode);
        }



      
    }
}
