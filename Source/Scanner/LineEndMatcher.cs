

namespace ProtoTool.Scanner
{
    public class LineEndMatcher : TokenMatcher
    {
        public static Token LineEndToken = new Token(TokenType.EOL, null);

        public override Token Match(Lexer lex)
        {
            switch( lex.CurrChar )
            {
                case '\r':
                    if ( lex.Peek(1) == '\n' )
                    {
                        // Windows
                        lex.Consume(2);
                    }
                    else
                    {
                        // Mac
                        lex.Consume(1);
                    }

                    lex.Line++;
                    return LineEndToken;
                case '\n': // Linux
                    lex.Consume(1);
                    lex.Line++;
                    return LineEndToken;
            }

            return null;
        }
    }
}
