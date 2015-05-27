using CommandLine.Text;
using System;
using ProtoTool.Schema;
using ProtoTool.ProtoF;
using ProtoTool.Protobuf;
using ProtoTool.Scanner;

namespace ProtoTool
{

    class Program
    {
        private static readonly HeadingInfo HeadingInfo = new HeadingInfo("ProtoF", "1.0");

        static void UnitTest( )
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

        static void Main(string[] args)
        {
            var options = new CommandOptions();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Environment.Exit(-1);
                return;
            }

            var tool = new Tool();
            tool.SearchPath = options.SearchPath;


            Parser parser;
            Printer printer;

            if ( options.Method == "pf2pf" )
            {
                parser = new ProtoFParser(tool);
                printer = new ProtoFPrinter();
            }
            else if (options.Method == "pb2pf")
            {
                parser = new ProtobufParser(tool);
                printer = new ProtoFPrinter();
            }
            else if (options.Method == "pf2pb")
            {
                parser = new ProtoFParser(tool);
                printer = new ProtobufPrinter();
            }
            else if (options.Method == "pb2pb")
            {
                parser = new ProtobufParser(tool);
                printer = new ProtobufPrinter();
            }
            else
            {
                throw new Exception("Unknown method");
            }

#if !DEBUG
            try
            {
#endif
                Tool.Convertor(options.InputFile, options.OutputFile, parser, printer);
                return;

#if !DEBUG
            }
            catch (ProtoExceptioin e)
            {
                Console.WriteLine(e.Message.ToString());
            }
            catch( Exception e )
            {
                Console.WriteLine(e.ToString());
            }

            Console.Read();

            Environment.Exit(-1);     
#endif

        }
    }
}
