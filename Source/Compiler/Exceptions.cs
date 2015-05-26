using ProtoTool.Scanner;
using System;

namespace ProtoTool
{
    public enum ErrorType
    {
        Lexer,
        Parse,

    }


    public class ProtoExceptioin : Exception
    {
        ErrorType _type;

        public ProtoExceptioin(ErrorType type, string msg)
            : base(msg)
        {
            _type = type;
        }
    }

    public static class Reporter
    {
        public static void Error(ErrorType type,string fmt, params object[] objs)
        {
            Error(type, null, fmt, objs);
        }

        public static void Error(ErrorType type, Location loc, string fmt, params object[] objs)
        {
            string str;
            if (loc != null)
            {
                str = loc + " " + string.Format(fmt, objs);
            }
            else
            {
                str = string.Format(fmt, objs);
            }            

            throw new ProtoTool.ProtoExceptioin(type, str);
        }
    }
}
