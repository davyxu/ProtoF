

namespace ProtoTool.Scanner
{    
    public class WhitespaceMatcher : TokenMatcher
    {
        public static Token WhiteToken = new Token(TokenType.Whitespace, null);

        bool IsWhiteSpace( char c )
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }

        public override Token Match(Lexer lex)
        {
            
            int count = 0;
            for (; ; count++)
            {
                var c = lex.Peek(count);

                if (c == ' ' || c == '\t' )
                {
                    continue;
                }         
                
                break;                
            }

            if (count == 0)
                return null;

            int beginIndex = lex.Index;

            lex.Consume( count );

            return WhiteToken;
        }
    }
}
