

namespace ProtoF.Scanner
{
    public class LineEndMatcher : TokenMatcher
    {
        public static Token LineEndToken = new Token(TokenType.EOL, null);

        public override Token Match(Tokenizer tz)
        {
            switch( tz.Current )
            {
                case '\r':
                    if ( tz.Peek(1) == '\n' )
                    {
                        // Windows
                        tz.Consume(2);
                    }
                    else
                    {
                        // Mac
                        tz.Consume(1);
                    }

                    tz.Line++;
                    return LineEndToken;
                case '\n': // Linux
                    tz.Consume(1);
                    tz.Line++;
                    return LineEndToken;
            }

            return null;
        }
    }
}
