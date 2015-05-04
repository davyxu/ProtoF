using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoF.Proto;
using ProtoF.Scanner;

namespace ProtoF.Parser
{
    public partial class SchemaParser
    {
        FileDefine ParseFile( )
        {
            var def = new FileDefine();

            Expect( TokenType.Package );

            Check(TokenType.Identifier, "require package name");

            def.Package = CurrToken.Value;
            Next();

            while (CurrToken.Type != TokenType.EOF )
            {
                switch (CurrToken.Type)
                {
                    case TokenType.Message:
                        {
                            def.Message.Add(ParseMessage());
                        }
                        break;
                    case TokenType.Enum:
                        {
                            def.Enum.Add(ParseEnum());
                        }
                        break;
                    default:
                        {
                            Error(string.Format("unexpect token {0}", _lexer.CurrChar));
                        }
                        break;
                }
            }

            return def;
        }

        EnumDefine ParseEnum( )
        {
            var def = new EnumDefine();

            Expect(TokenType.Enum);

            Check(TokenType.Identifier, "require enum type name");
 
            def.Name = CurrToken.Value;
            Next();

            Expect(TokenType.LBrace);


            while (CurrToken.Type != TokenType.RBrace)
            {
                Check(TokenType.Identifier, "require enum name");

                var valueDef = new EnumValueDefine();
                valueDef.Name = CurrToken.Value;

                Next();

                Expect(TokenType.Assign);

                Check(TokenType.Number, "require enum value");
                valueDef.Number = CurrToken.ToInteger();
                
                var comment = ReadComment();
                if ( comment != null )
                {
                    valueDef.TrailingComment = comment;
                }
                

                def.Value.Add(valueDef);
            }

            Expect(TokenType.RBrace);

            return def;
        }

        List<FieldDefine> _unsolvedType = new List<FieldDefine>();

        MessageDefine ParseMessage()
        {
            var def = new MessageDefine();

            Expect(TokenType.Message);

            Check(TokenType.Identifier, "require message type name");

            def.Name = CurrToken.Value;

            Next();

            Expect(TokenType.LBrace);


            while (CurrToken.Type != TokenType.RBrace)
            {
                // 内嵌枚举
                if ( CurrToken.Type == TokenType.Enum )
                {
                    def.Enum.Add(ParseEnum());
                }

                // 字段名
                Check(TokenType.Identifier, "require field name");

                var fieldDef = new FieldDefine();
                fieldDef.Name = CurrToken.Value;

                Next();

                if ( CurrToken.Type == TokenType.Array )
                {
                    fieldDef.Container = FieldContainer.Array;
                    Next();
                    Expect(TokenType.LAngleBracket);
                }
                else
                {
                    fieldDef.Container = FieldContainer.None;
                }
                

                // 字段类型
                fieldDef.Type = GetFieldType();
                fieldDef.TypeName = CurrToken.Value;

                def.Field.Add(fieldDef);

                if ( fieldDef.Type == FieldType.None )
                {
                    _unsolvedType.Add(fieldDef);
                }

                if ( fieldDef.Container == FieldContainer.Array )
                {
                    Next();
                    Check(TokenType.RAngleBracket, string.Format("expect token: {0}", TokenType.RAngleBracket.ToString()));
                }

                var comment = ReadComment();
                if (comment != null)
                {
                    fieldDef.TrailingComment = comment;
                }                

            }

            Expect(TokenType.RBrace);

            return def;
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

