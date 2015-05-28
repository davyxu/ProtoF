
namespace ProtoTool.Utility
{
    public class CommandLineParser
    {
        string[] _args;
        public CommandLineParser( string[] args )
        {
            _args = args;
        }
        

        public string[] RawArgs
        {
            get { return _args; }
        }

        public string Get( int index )
        {
            return _args[index];
        }

        public bool Has( int index )
        {
            return index < _args.Length;
        }

        public int Count
        {
            get { return _args.Length; }
        }

        public bool Exists( string name )
        {
            return GetIndex(name) != -1;
        }

        public int GetIndex( string name )
        {
            for( int i = 0;i< _args.Length;i++)
            {
                if (_args[i] == name)
                    return i;
            }

            return -1;
        }

        public string GetContent( string name )
        {
            var index = GetIndex(name);
            if (index == -1)
                return string.Empty;

            return Get(index + 1);
        }

    }
}
