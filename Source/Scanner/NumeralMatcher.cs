using System;
using System.Text;

namespace ProtoTool.Scanner
{
    public class NumeralMatcher : TokenMatcher
    {
        int DigiValue( char c )
        {
            if (c >= '0' && c <= '9')
            {
                return (int)( c - '0');
            }

            if (c >= 'a' && c <= 'f')
            {
                return (int)(c - 'a' + 10);
            }

            if (c >= 'A' && c <= 'F')
            {
                return (int)(c - 'A' + 10);
            }

            return 16;
        }

        // 尾数
        void ScanMantissa( Lexer lex, StringBuilder sb, int baseNum )
        {
            while( DigiValue( lex.CurrChar) < baseNum )
            {
                sb.Append(lex.CurrChar);
                lex.Consume();
            }
        }

        public override Token Match( Lexer lex)
        {            
            if (!Char.IsDigit(lex.CurrChar))
                return null;

            var sb = new StringBuilder();

            bool needfraction = true;

            if (lex.CurrChar == '0')
            {
                sb.Append(lex.CurrChar);
                lex.Consume();

                if ( lex.CurrChar == 'x' || lex.CurrChar == 'X')
                {
                    sb.Append(lex.CurrChar);
                    lex.Consume();

                    ScanMantissa(lex, sb, 16 );
                    needfraction = false;
                }
                else
                {
                    // 可以处理8进制
                    ScanMantissa(lex, sb, 10);
                }
            }
            else
            {
                ScanMantissa(lex, sb, 10);                
            }

            // 小数部分
            if (needfraction && lex.CurrChar == '.')
            {
                sb.Append(lex.CurrChar);
                lex.Consume();
                ScanMantissa(lex, sb, 10);
            }
            

            return new Token(TokenType.Number, sb.ToString() );
        }

    }
}
