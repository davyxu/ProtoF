using System.Collections.Generic;
using ProtoTool.Scanner;
using ProtoTool.Schema;
using System.Linq;

namespace ProtoTool.Protobuf
{
    public partial class ProtobufParser : Parser
    {
        void ParsePackage(FileNode filenode)
        {
            var node = new PackageNode();

            Consume(TokenType.Package);

            if (!string.IsNullOrEmpty(filenode.Package))
            {
                Reporter.Error( ErrorType.Parse, _lexer.Loc, "package already specified");
            }

            node.Name = FetchToken(TokenType.Identifier, "require package name").Value;

            if (filenode.Package == node.Name)
            {
                Reporter.Error( ErrorType.Parse, _lexer.Loc, "package name duplicated");
            }

            Consume(TokenType.SemiColon);

            Consume(TokenType.EOL);

            filenode.Package = node.Name;
            filenode.Add(node);
        }

        void ParseImport(FileNode filenode)
        {
            

            while (TryConsume(TokenType.Import))
            {
                var node = new ImportNode();

                node.Name = FetchToken(TokenType.QuotedString, "require file name").Value;
                
                if ( filenode.Import.Exists(x => x.Name == node.Name) )
                {
                    Reporter.Error( ErrorType.Parse, _lexer.Loc, "duplicate import filename");
                }

                if (filenode.Name == node.Name)
                {
                    Reporter.Error( ErrorType.Parse, _lexer.Loc, "can not import self");
                }

                var parser = new ProtobufParser(_tool);
                parser.StartParseFile(node.Name);

                filenode.Import.Add(node);
                filenode.Add(node);

                Consume(TokenType.SemiColon);
                Consume(TokenType.EOL);
            }
        }


        public override void ParseFile(FileNode node, string name)
        {                     
            node.Name = name;
            _tool.AddFileNode(node);

            while (CurrToken.Type != TokenType.EOF)
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
                            ParseEnum(node, null);
                        }
                        break;
                    case TokenType.EOF:
                        break;
                    default:
                        {
                            Reporter.Error( ErrorType.Parse, _lexer.Loc, string.Format("unexpect token {0}", _lexer.CurrChar));
                        }
                        break;
                }
            }



            ResolveUnknownNode(node);
        }

    }


}

