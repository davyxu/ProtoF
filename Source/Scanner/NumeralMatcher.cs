using System;

namespace ProtoTool.Scanner
{
    public class NumeralMatcher : TokenMatcher
    {
        public override Token Match(Lexer lex)
        {

            if (!Char.IsDigit(lex.CurrChar))
                return null;

            int beginIndex = lex.Index;


            do
            {
                lex.Consume();

            } while (char.IsDigit(lex.CurrChar) || lex.CurrChar == '.');


            return new Token(TokenType.Number, lex.Source.Substring( beginIndex, lex.Index - beginIndex) );
        }

    }
}
