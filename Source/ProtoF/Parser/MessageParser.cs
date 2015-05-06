using ProtoF.AST;
using ProtoF.Scanner;
using System.Collections.Generic;

namespace ProtoF.Parser
{
    public partial class SchemaParser
    {
        

        MessageNode ParseMessage( FileNode fn )
        {
            var node = new MessageNode();

            // message的头注释
            ParseCommentAndEOL(node);

            Consume(TokenType.Message);
            MarkLocation(node);

            node.Name = FetchToken(TokenType.Identifier, "require message type name").Value;

            TryConsume(TokenType.EOL);

            Consume(TokenType.LBrace); TryConsume(TokenType.EOL);


            while (CurrToken.Type != TokenType.RBrace)
            {
                // 内嵌枚举
                if (CurrToken.Type == TokenType.Enum)
                {
                    var en = ParseEnum();

                    CheckDuplicate(node, en.Loc, en.Name);

                    node.AddEnum(en);
                }

                ParseField(fn, node);
            }

            Consume(TokenType.RBrace); TryConsume(TokenType.EOL);

            FillFieldNumber(node);

            return node;
        }



        void ParseField( FileNode fn, MessageNode node )
        {
            var fieldNode = new FieldNode();

            // 字段的头注释
            ParseCommentAndEOL(node);

            ParseFieldName(node, fieldNode);

            ParseFieldType(fn, node, fieldNode);

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
            // 类型
            if (CurrToken.Type == TokenType.Array)
            {
                fieldNode.Container = FieldContainer.Array;
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

            if (fieldNode.Type == FieldType.None && !ResolveFieldType(fn, fieldNode))
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

        void FillFieldNumber( MessageNode node )
        {
            int autoNumber = 1;

            foreach( var field in node.Field )
            {
                if ( field.Number == 0 )
                {
                    field.Number = autoNumber;
                    field.NumberIsAutoGen = true;
                }
                else
                {
                    if (field.Number < autoNumber )
                    {
                        Error(field.Loc, "field number < auto gen number {0}", autoNumber);
                        continue;
                    }

                    autoNumber = field.Number;
                }

                autoNumber++;
            }
        }


        List<FieldNode> _unsolvedNode = new List<FieldNode>();

        void AddUnsolveNode(FieldNode n)
        {
            _unsolvedNode.Add(n);
        }

        // 就这节点类型
        bool ResolveFieldType( FileNode fn, FieldNode fieldNode )
        {
            var symbols = _symbols.Get(fn.Package, fieldNode.TypeName);
            if (symbols == null)
            {   
                return false;
            }

            var en = symbols as EnumNode;
            if (en != null)
            {
                fieldNode.Type = FieldType.Enum;
                fieldNode.TypeRef = en;
                return true;                
            }

            var msg = symbols as MessageNode;
            if (msg != null)
            {
                fieldNode.Type = FieldType.Message;
                fieldNode.TypeRef = en;
                return true;                      
            }

            return false;
        }

        // 解决前向引用的未知节点
        void ResolveUnknownNode( FileNode fn )
        {
            foreach (FieldNode fieldNode in _unsolvedNode)
            {
                if ( !ResolveFieldType( fn, fieldNode ) )
                {
                    Error(fieldNode.Loc, "{0} undefined!", fieldNode.TypeName);
                }
                
            }
        }
    }
}
