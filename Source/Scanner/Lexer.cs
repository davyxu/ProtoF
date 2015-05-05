using System.Collections.Generic;

namespace ProtoF.Scanner
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
        Tokenizer _tokenizer;
        Token _token;
        string _srcName;

        public void AddMatcher(TokenMatcher[] matcher)
        {
            _tokenmatchers = matcher;
        }

        public void Start( string src, string srcName )
        {
            _srcName = srcName;
            _tokenizer = new Tokenizer(src);            
        }

        public override string ToString()
        {
            return string.Format("{0}@line {1}", _token, _tokenizer.Line);
        }

        public Token CurrToken
        {
            get { return _token; }
        }

        public char CurrChar
        {
            get { return _tokenizer.Current; }
        }

        public Location Loc
        {
            get
            {
                Location loc = new Location();
                loc.Line = _tokenizer.Line;
                loc.FileName = _srcName;
                return loc;
            }
        }

        public string DebugProgress
        {
            get { return _tokenizer.Source.Substring(_tokenizer.Index); }
        }


        static Token _eof = new Token(TokenType.EOF, null);

        public Token Read()
        {
            return ReadByMatcher(_tokenmatchers);
        }

        public Token ReadByMatcher( TokenMatcher[] matcherlist )
        {
            while (!_tokenizer.EOF())
            {

                foreach (var matcher in matcherlist)
                {
                    var token = matcher.Match(_tokenizer);
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
    }
}
