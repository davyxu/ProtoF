using System;

namespace ProtoTool.Scanner
{
    public class IdentifierMatcher : TokenMatcher
    {
        public override Token Match(Lexer lex)
        {

            if (!( Char.IsLetter(lex.CurrChar) || lex.CurrChar == '_' ))
                return null;

            int beginIndex = lex.Index;


            do
            {
                // TODO 中间遇到EOF情况
                lex.Consume();

            } while (char.IsLetterOrDigit(lex.CurrChar) || lex.CurrChar == '_');


            return new Token( TokenType.Identifier, lex.Source.Substring( beginIndex, lex.Index - beginIndex) );
        }

    }
}
