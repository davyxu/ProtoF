

namespace ProtoF.Scanner
{    
    public class CommentMatcher : TokenMatcher
    {
        public override Token Match(Lexer lex)
        {
            if (lex.CurrChar != '/' || lex.Peek(1) != '/')
                return null;

            lex.Consume(2);

            int beginIndex = lex.Index;

            do
            {
                lex.Consume();

            } while (lex.CurrChar != '\r' && lex.CurrChar != '\n' && lex.CurrChar != '\0');



            return new Token(TokenType.Comment, lex.Source.Substring(beginIndex, lex.Index - beginIndex));
        }
    }
}
