using ProtoTool.Scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoTool
{
    class UnitTest
    {
        static void Do()
        {
            var lex = new Lexer();

            lex.AddMatcher(new TokenMatcher[]{
                new NumeralMatcher(),
                
                new LineEndMatcher(),
                new WhitespaceMatcher().Ignore(),
                new CommentMatcher(),                
                new UnknownMatcher(),
            });

            lex.Start("0x837f", "A");
            lex.Read();
        }
    }
}
