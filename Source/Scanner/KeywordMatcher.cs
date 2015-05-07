

namespace ProtoF.Scanner
{
    public class KeywordMatcher : TokenMatcher
    {
        string _word;
        TokenType _type;

        public KeywordMatcher( TokenType t, string word)
        {
            _word = word;
            _type = t;
        }

        public override Token Match(Lexer lex)
        {
            if (lex.CharLeft < _word.Length)
                return null;

            int index = 0;

            foreach( var c in _word )
            {
                if (lex.Peek(index) != c)
                    return null;

                index++;
            }


            lex.Consume(_word.Length);


            return new Token(_type, _word );
        }

    }
}
