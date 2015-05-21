using System.Collections.Generic;
using ProtoF.Scanner;
using ProtoF.AST;
using System.Linq;

namespace ProtoF.Parser
{
    public partial class ProtoFParser : Parser
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
                
                if (filenode.Import.Exists(x => x.Name == node.Name))
                {
                    Error(_lexer.Loc, "duplicate import filename");
                }

                if ( filenode.Name == node.Name )
                {
                    Error(_lexer.Loc, "can not import self");
                }

                var parser = new ProtoFParser(_tool);
                parser.StartParseFile(node.Name);

                filenode.Import.Add(node );
                filenode.Add(node);

                Consume(TokenType.EOL);
            }            
        }


        FileNode ParseFile( string name  )
        {
            var node = new FileNode();
            node.Name = name;
            _tool.AddFileNode(node);

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
            _tool.Symbols.Add(packageName, name, n);
        }


        void CheckDuplicate(Location loc, string packageName, string name)
        {
            if (_tool.Symbols.Get(packageName, name) != null)
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

