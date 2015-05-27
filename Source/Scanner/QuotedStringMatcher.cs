using System;
using System.Collections.Generic;
using System.Text;
namespace ProtoTool.Scanner
{
    public class QuotedStringMatcher : TokenMatcher
    {
        public override Token Match(Lexer lex)
        {
            if (lex.CurrChar != '"' && lex.CurrChar != '\'')
                return null;

            // 左边的marker
            var marker = lex.CurrChar;
            lex.Consume();

            var sb = new StringBuilder();
            
                       
            while( true )
            {
                var c = lex.CurrChar;
                switch( lex.CurrChar )
                {
                    case '\'':
                        {
                            lex.Consume();

                            switch (lex.CurrChar)
                            {
                                case 'a': c = '\a'; break;
                                case 'b': c = '\b'; break;
                                case 'f': c = '\f'; break;
                                case 'n': c = '\n'; break;
                                case 'r': c = '\r'; break;
                                case 't': c = '\t'; break;
                                case 'v': c = '\v'; break;
                                case '\\':
                                case '\"':
                                case '\'':
                                    {
                                        c = lex.CurrChar;
                                        
                                    }
                                    break;
                                default:
                                    Reporter.Error(ErrorType.Lexer, lex.Loc, "invalid escape sequence");
                                    break;
                            }

                            lex.Consume();
                        }
                        break;
                    case '\r':
                    case '\n':
                        Reporter.Error(ErrorType.Lexer, lex.Loc, "unfinished string");                        
                        break;                        
                }

                if (lex.CurrChar == marker)
                {
                    break;
                }

                sb.Append(lex.CurrChar);
                lex.Consume();

            }

            lex.Consume();


            return new Token(TokenType.QuotedString, sb.ToString() );
        }

    }
}
