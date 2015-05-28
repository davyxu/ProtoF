using ProtoTool.Utility;

namespace ProtoTool.Plugin
{
    public interface IPlugin
    {
        void OnLoad(Tool tool);

        void OnExit();
    }
}
