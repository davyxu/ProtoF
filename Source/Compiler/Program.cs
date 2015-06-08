using System;
using ProtoTool.Schema;
using ProtoTool.Scanner;
using ProtoTool.Plugin;
using System.Text;
using System.IO;

namespace ProtoTool
{

    partial class Program
    {
        static void PrintHelp( )
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage: ProtoTool [OPTION]");
            sb.AppendLine("-s   proto file search path");
            sb.AppendLine("-i   input file name");
            sb.AppendLine("-o   output file name");
            sb.AppendLine("--pluginpath  set plugin search path");
            sb.AppendLine("--autogenheader  add auto gen header at front");
            sb.AppendLine("--conv   convert proto to proto format, like pb2pf, pb2pb, etc...");
        }

        static void Main(string[] args)
        {
            var cmdline = new Utility.CommandLineParser(args);

            if ( cmdline.Count == 0 )
            {
                PrintHelp();
                return;
            }

            var pluginPath = cmdline.GetContent("--pluginpath");

            var pluginMgr = new PluginManager();

            if ( cmdline.Exists("--pluginpath"))
            {
                pluginPath = cmdline.GetContent("--pluginpath");
            }


            pluginMgr.Init(pluginPath);


            var tool = new Tool();
            tool.SearchPath = cmdline.GetContent("-s");            


            pluginMgr.Iterate((plugin) => { 
                plugin.OnLoad(tool); 
            });

#if !DEBUG
            try
            {
#endif
                if (cmdline.Exists("--conv"))
                {
                    var convMethod = cmdline.GetContent("--conv");

                    tool.Convertor.Do(convMethod, cmdline.GetContent("-i"), cmdline.GetContent("-o"), cmdline.Exists("--autogenheader") );
                }

                pluginMgr.Iterate((plugin) =>
                {
                    plugin.OnExit();
                });


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
