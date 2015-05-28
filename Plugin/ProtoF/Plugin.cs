using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoTool.Plugin;
using ProtoTool.Utility;

namespace ProtoTool.ProtoF
{
    class Plugin : IPlugin
    {
        public void OnLoad(Tool tool)
        {
            tool.Convertor.RegisterParser(typeof(ProtoFParser));
            tool.Convertor.RegisterPrinter(typeof(ProtoFPrinter));
        }

        public void OnExit()
        {

        }
    }
}
