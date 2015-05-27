using ProtoTool.Scanner;

namespace ProtoTool.Schema
{
    public partial class Parser
    {
        protected string ReadDefaultValue(FieldNode field)
        {
            switch (field.Type)
            {
                case FieldType.Float:
                case FieldType.Double:
                case FieldType.Int32:
                case FieldType.Int64:
                    {
                        bool sub = TryConsume(TokenType.Sub);

                        if (sub)
                        {
                            return "-" + CurrToken.Value;
                        }
                    }
                    break;
                case FieldType.UInt32:
                case FieldType.UInt64:
                    {
                        bool sub = TryConsume(TokenType.Sub);

                        if (sub)
                        {
                            Reporter.Error(ErrorType.Parse, _lexer.Loc, "invalid unsigned value");
                        }

                    }
                    break;
                case FieldType.Bool:
                    {
                        if (CurrToken.Value != "true" && CurrToken.Value != "false")
                        {
                            Reporter.Error(ErrorType.Parse, _lexer.Loc, "invalid bool value");
                        }
                    }
                    break;
                case FieldType.Message:
                case FieldType.Bytes:
                    {
                        Reporter.Error(ErrorType.Parse, _lexer.Loc, "message and bytes can not have default value");
                    }
                    break;
            }

            return CurrToken.Value;
        }

        protected FieldType GetFieldType()
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
