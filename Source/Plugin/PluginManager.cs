using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ProtoTool.Plugin
{
    public class PluginManager
    {
        const string PluginEntryClassName = ".Plugin";

        Dictionary<string, IPlugin> _pluginMap = new Dictionary<string, IPlugin>();

        public void Init(string pluginDir)
        {                       
            foreach (string filename in Directory.GetFiles(pluginDir, "*.dll"))
            {

                var finalfilename = Path.GetFullPath(filename);
                var asm = Assembly.LoadFile(finalfilename);
                if (asm == null)
                    continue;

                var name = Path.GetFileNameWithoutExtension(filename);

                var type = asm.GetType(name + PluginEntryClassName );

                if (type == null)
                    continue;

                var plugin = Activator.CreateInstance(type) as IPlugin;

                if (plugin == null)
                    continue;

                _pluginMap.Add(name, plugin);
            }
        }

        public IPlugin Get( string name )
        {
            IPlugin p;
            if (_pluginMap.TryGetValue(name, out p ))
            {
                return p;
            }

            return null;
        }

        public void Iterate( Action<IPlugin> callback )
        {
            foreach( var v in _pluginMap )
            {
                callback(v.Value);                
            }
        }

    }
}
