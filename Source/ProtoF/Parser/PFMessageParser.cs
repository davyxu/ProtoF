using ProtoTool.Schema;
using ProtoTool.Scanner;
using System.Collections.Generic;

namespace ProtoTool.ProtoF
{
    public partial class ProtoFParser : Parser
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

            Consume(TokenType.LBrace); TryConsume(TokenType.EOL);


            while (CurrToken.Type != TokenType.RBrace)
            {
                ParseCommentAndEOL(node);

                // 内嵌枚举
                while (CurrToken.Type == TokenType.Enum)
                {
                    _fileNode.EnterScope();
                    ParseEnum(filenode, node );
                    _fileNode.LeaveScope();

                    ParseCommentAndEOL(node);
                }

                if (CurrToken.Type == TokenType.RBrace)
                    break;

                ParseField(filenode, node);
            }

            Consume(TokenType.RBrace); TryConsume(TokenType.EOL);

            FillFieldNumber(node);
        }



        void ParseField( FileNode fn, MessageNode node )
        {
            var fieldNode = new FieldNode();

            // 字段的头注释
            ParseCommentAndEOL(node);

            ParseFieldType(fn, node, fieldNode);

            ParseFieldName(node, fieldNode);

            switch( CurrToken.Type )
            {
                case TokenType.Assign:
                    ParseNumber(node, fieldNode);

                    if ( CurrToken.Type == TokenType.LSqualBracket )
                    {
                        ParseOption(node, fieldNode);
                    }

                    break;
                case TokenType.LSqualBracket:
                    ParseOption(node, fieldNode);
                    break;
            }


            // 尾注释
            ParseTrailingComment(fieldNode);

            // 字段的头注释
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
                Consume(TokenType.Comma);

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
            // 类型
            if (CurrToken.Type == TokenType.Array)
            {
                fieldNode.Container = FieldContainer.Array;
                fieldNode.PBLabel = PBFieldLabel.Repeated;
                Next();
                Consume(TokenType.LAngleBracket);
            }
            else
            {
                fieldNode.Container = FieldContainer.None;
            }


            // 字段类型
            fieldNode.Type = GetFieldType();
            fieldNode.TypeName = CurrToken.Value;

            if (fieldNode.Type == FieldType.None && !ResolveFieldType(fn.Package, fieldNode))
            {
                AddUnsolveNode(fieldNode);
            }

            node.AddField(fieldNode);


            Next();

            if (fieldNode.Container == FieldContainer.Array)
            {
                Consume(TokenType.RAngleBracket);
            }
        }


        void FillFieldNumber( MessageNode node )
        {
            int autoNumber = 1;

            foreach( var field in node.Field )
            {
                if (field.Number == 0)
                {
                    field.Number = autoNumber;
                    field.NumberIsAutoGen = true;
                }
                else
                {
                    if (field.Number < autoNumber )
                    {
                        Reporter.Error(ErrorType.Parse, field.Loc, "field number < auto gen number {0}", autoNumber);
                        continue;
                    }

                    autoNumber = field.Number;
                }

                autoNumber++;
            }
        }
    }
}
