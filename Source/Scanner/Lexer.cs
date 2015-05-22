using System;
using System.Collections.Generic;

namespace ProtoTool.Scanner
{
    public class Location
    {
        public int Line;
        public string FileName;

        public override string ToString()
        {
            return string.Format("{0}:{1}", FileName, Line );
        }
    }

    public class Lexer
    {
        TokenMatcher[] _tokenmatchers;
        Token _token;
        string _srcName;

        public Token CurrToken
        {
            get { return _token; }
        }

        string _source;


        public int Index { get; set; }

        public int Line { get; set; }

        public string Source { get { return _source; } }



        public Location Loc
        {
            get
            {
                Location loc = new Location();
                loc.Line = Line;
                loc.FileName = _srcName;
                return loc;
            }
        }

        public string DebugProgress
        {
            get { return _source.Substring( Index); }
        }

        public char CurrChar
        {
            get
            {
                if (EOF(0))
                    return '\0';

                return _source[Index];
            }
        }

        public int CharLeft
        {
            get { return _source.Length - Index; }
        }

        public void AddMatcher(TokenMatcher[] matcher)
        {
            _tokenmatchers = matcher;
        }

        public void Start( string src, string srcName )
        {
            _source = src;
            Line = 1;
            _srcName = srcName;        
        }

        public override string ToString()
        {
            return string.Format("{0}@line {1}", _token, Line);
        }


        static Token _eof = new Token(TokenType.EOF, null);

        public Token Read()
        {
            return ReadByMatcher(_tokenmatchers);
        }

        public Token ReadByMatcher( TokenMatcher[] matcherlist )
        {
            while (!EOF())
            {

                foreach (var matcher in matcherlist)
                {
                    var token = matcher.Match(this);
                    if (token == null)
                    {
                        continue;
                    }

                    // 跳过已经parse部分, 不返回外部
                    if (matcher.IsIgnored)
                        break;

                    _token = token;
                    return token;
                }

            }

            _token = _eof;
            return _token;
        }

        public char Peek(int offset)
        {
            if (EOF(offset))
                return '\0';

            return _source[Index + offset ];
        }


        public void Consume( int count = 1 )
        {
            Index+= count ;
        }

        public bool EOF(int offset = 0)
        {
            return Index + offset >= _source.Length;
        }

        public void Error(string fmt, params object[] objs)
        {
            string str = Loc + " " + string.Format(fmt, objs);
            Console.WriteLine(str);
            throw new Exception(str);
        }

    }
}
