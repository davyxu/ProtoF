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
                Error(_lexer.Loc, "package already specified");
            }

            node.Name = FetchToken(TokenType.Identifier, "require package name").Value;

            if (filenode.Package == node.Name)
            {
                Error(_lexer.Loc, "package name duplicated");
            }

            Consume(TokenType.SemiColon);

            Consume(TokenType.EOL);

            filenode.Package = node.Name;
            filenode.Add(node);
        }

        void ParseImport(FileNode filenode)
        {
            var node = new ImportNode();

            while (TryConsume(TokenType.Import))
            {
                node.Name = FetchToken(TokenType.QuotedString, "require file name").Value;

                if (filenode.Import.Exists(x => x.Name == node.Name))
                {
                    Error(_lexer.Loc, "duplicate import filename");
                }

                if (filenode.Name == node.Name)
                {
                    Error(_lexer.Loc, "can not import self");
                }

                var parser = new ProtobufParser(_tool);
                parser.StartParseFile(node.Name);

                filenode.Import.Add(node);
                filenode.Add(node);

                Consume(TokenType.SemiColon);
                Consume(TokenType.EOL);
            }
        }


        public override FileNode ParseFile(string name)
        {
            var node = new FileNode();
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

    }


}

