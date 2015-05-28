using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoTool.Plugin;
using ProtoTool.Utility;

namespace ProtoTool.Protobuf
{
    class Plugin : IPlugin
    {
        public void OnLoad( Tool tool)
        {
            tool.Convertor.RegisterParser(typeof(ProtobufParser));
            tool.Convertor.RegisterPrinter(typeof(ProtobufPrinter));
        }

        public void OnExit()
        {

        }
    }
}
