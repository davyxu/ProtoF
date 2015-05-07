using System.Collections.Generic;
using ProtoF.Scanner;
using ProtoF.AST;

namespace ProtoF.Parser
{
    public partial class ProtoFParser
    {
        FileNode ParseFile( )
        {
            var node = new FileNode();

            ParseCommentAndEOL(node);

            Consume( TokenType.Package );

            node.Package = FetchToken(TokenType.Identifier, "require package name").Value;

            Consume(TokenType.EOL);

            
            while (CurrToken.Type != TokenType.EOF )
            {
                ParseCommentAndEOL(node);

                switch (CurrToken.Type)
                {
                    case TokenType.Message:
                        {
                            var msg = ParseMessage(node);

                            CheckDuplicate(msg.Loc, node.Package, msg.Name);

                            node.AddMessage(msg);

                            AddSymbol(node.Package, msg.Name, msg);
                        }
                        break;
                    case TokenType.Enum:
                        {
                            var en = ParseEnum();

                            CheckDuplicate(en.Loc, node.Package, en.Name);

                            node.AddEnum(en);

                            AddSymbol(node.Package, en.Name, en);
                        }
                        break;
                    case TokenType.EOF:
                        break;
                    default:
                        {
                            Error(string.Format("unexpect token {0}", _lexer.CurrChar));
                        }
                        break;
                }
            }

            ResolveUnknownNode(node);

            return node;
        }

        void AddSymbol( string packageName, string name, Node n  )
        {
            _symbols.Add(packageName, name, n);
        }


        void CheckDuplicate(Location loc, string packageName, string name)
        {            
            if (_symbols.Get(packageName, name) != null )
            {
                Error(loc, "{0} already defined in {1} package", name, packageName);
            }
        }

        void CheckDuplicate(ContainerNode n, Location loc, string name)
        {
            if (n.Contain( name ) )
            {
                Error(loc, "{0} already defined", name);
            }
        }
    }


}

