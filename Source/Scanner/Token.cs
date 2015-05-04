
namespace ProtoF.Scanner
{
    public enum TokenType
    {
        None,
        Unknown,
        Whitespace,
        EOL,
        EOF,

        Comment,
        Identifier,
        Number,
        QuotedString,
        
        Assign,         // =        
        Comma,          // ,
        Dot,            // .
        SemiColon,      // ;
        LBracket,       // (
        RBracket,       // )
        LSqualBracket,  // [
        RSqualBracket,  // ]
        LAngleBracket,  // <
        RAngleBracket,  // >
        LBrace,         // {
        RBrace,         // }

        // protof
        Package,
        Import,
        Enum,
        Message,

        // type
        Bool,
        Int32,
        UInt32, 
        Int64,
        UInt64,
        String,
        Double,
        Float,        
        Bytes,
        Array,
    }
    public class Token
    {
        string _value;
        TokenType _type;

        public Token( TokenType type, string value )
        {
            _type = type;
            _value = value;
        }

        public TokenType Type
        { 
            get { return _type;  } 
        }

        public string Value
        {
            get { return _value;  }
        }

        public float ToNumber()
        {
            return float.Parse(_value);
        }

        public int ToInteger()
        {
            return int.Parse(_value);
        }

        public override string ToString()
        {            
            return _type.ToString() + " " + Value;
        }
    }

    public class TokenPos
    {
        public int Pos;

        public TokenPos(int line, int col)
        {

        }
    }
}
