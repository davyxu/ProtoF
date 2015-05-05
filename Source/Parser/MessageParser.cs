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

            Expect(TokenType.Message);
            MarkLocation(node);

            Check(TokenType.Identifier, "require message type name");

            node.Name = CurrToken.Value;

            Next();

            Consume(TokenType.EOL);

            Expect(TokenType.LBrace); Consume(TokenType.EOL);


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

            Expect(TokenType.RBrace); Consume(TokenType.EOL);

            return node;
        }

        void ParseField( FileNode fn, MessageNode node )
        {
            var fieldNode = new FieldNode();

            // 字段的头注释
            ParseCommentAndEOL(node);

            // 字段名
            Check(TokenType.Identifier, "require field name");
            MarkLocation(fieldNode);

            fieldNode.Name = CurrToken.Value;
            CheckDuplicate(node, _lexer.Loc, fieldNode.Name);


            Next();

            if (CurrToken.Type == TokenType.Array)
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

            if (fieldNode.Type == FieldType.None && !ResolveFieldType(fn, fieldNode))
            {
                AddUnsolveNode(fieldNode);
            }

            node.AddField(fieldNode);


            Next();

            if (fieldNode.Container == FieldContainer.Array)
            {
                Expect(TokenType.RAngleBracket);
            }

            // 尾注释
            ParseTrailingComment(fieldNode);
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


        List<FieldNode> _unsolvedNode = new List<FieldNode>();

        void AddUnsolveNode(FieldNode n)
        {
            _unsolvedNode.Add(n);
        }

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
                return true;                
            }

            var msg = symbols as MessageNode;
            if (msg != null)
            {
                fieldNode.Type = FieldType.Message;
                return true;                      
            }

            return false;
        }

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
