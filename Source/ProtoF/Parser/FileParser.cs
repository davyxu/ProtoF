using System.Collections.Generic;
using ProtoF.Scanner;
using ProtoF.AST;
using System.Linq;

namespace ProtoF.Parser
{
    public partial class ProtoFParser
    {
        void ParsePackage( FileNode filenode )
        {
            var node = new PackageNode();

            if (TryConsume(TokenType.Package))
            {

                if ( !string.IsNullOrEmpty( filenode.Package ))
                {
                    Error(_lexer.Loc, "package already specified");
                }

                node.Name = FetchToken(TokenType.Identifier, "require package name").Value;

                if ( filenode.Package == node.Name )
                {
                    Error(_lexer.Loc, "package name duplicated");
                }

                Consume(TokenType.EOL);

                filenode.Package = node.Name;
                filenode.Add(node);
            }
        }

        void ParseImport( FileNode filenode )
        {
            var node = new ImportNode();

            while (TryConsume(TokenType.Import))
            {
                node.Name = FetchToken(TokenType.QuotedString, "require file name").Value;

                if ( filenode.Import.Contains(node.Name) )
                {
                    Error(_lexer.Loc, "duplicate import filename");
                }

                filenode.Import.Add(node.Name);
                filenode.Add(node);

                Consume(TokenType.EOL);
            }            
        }


        FileNode ParseFile( )
        {
            var node = new FileNode();

            while (CurrToken.Type != TokenType.EOF )
            {
                ParseCommentAndEOL(node);

                switch (CurrToken.Type)
                {
                    case TokenType.Package:
                        {
                            ParsePackage(node);
                        }
                        break;
                    case TokenType.Import:
                        {
                            ParseImport(node);
                        }
                        break;
                    case TokenType.Message:
                        {
                            ParseMessage(node);
                        }
                        break;
                    case TokenType.Enum:
                        {
                            ParseEnum(node);
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

