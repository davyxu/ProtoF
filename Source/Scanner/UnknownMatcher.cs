namespace ProtoF.Scanner
{

    public class UnknownMatcher : TokenMatcher
    {
        public override Token Match(Lexer lex)
        {
            int beginIndex = lex.Index;
            lex.Consume();
            return new Token(TokenType.Unknown, lex.Source.Substring( beginIndex, 1) );
        }
    }
}
