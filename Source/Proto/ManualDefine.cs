using System.Collections.Generic;

namespace ProtoF.Proto
{
    public enum FieldType
    {
        None = 0,
        Bool = 1,
	    Int32 = 2,
	    UInt32 = 3,
	    Int64 = 4,
	    UInt64 = 5,
	    String = 6,
	    Float = 7,
	    Double = 8,
	    Message = 9,
	    Enum = 10,
	    Bytes = 11,
    }

    public enum FieldContainer
    {
        None = 0,
        Array = 1,
    }

    public class FieldDefine
    {
        public string Name;
        public FieldType Type;
        public FieldContainer Container;
        public string TypeName;
        public string DefaultValue;
        public int Number;
        public int AutoNumber;
        public string TrailingComment;

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Name, Type, TypeName );
        }

        public string CompleteTypeName
        {
            get
            {
                switch( Container )
                {                           
                    case FieldContainer.Array:
                        return string.Format("array<{0}>", TypeName);
                }

                return TypeName;
            }
        }
    }

    public class EnumDefine
    {
        public string Name;
        public List<EnumValueDefine> Value = new List<EnumValueDefine>();

        public override string ToString()
        {
            return string.Format("{0} values:{1}", Name, Value.Count);
        }
    }

    public class EnumValueDefine
    {
        public string Name;
        public int Number;
        public string TrailingComment;

        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Number);
        }
    }

    public class MessageDefine
    {
        public string Name;
        public string Extends;
        public List<FieldDefine> Field = new List<FieldDefine>();
        public List<EnumDefine> Enum = new List<EnumDefine>();

        public override string ToString()
        {
            return string.Format("{0} fields:{1}", Name, Field.Count);
        }
    }

    public class FileDefine
    {
        public string Name;
        public string Package;
        public List<string> Dependency = new List<string>();
        public List<MessageDefine> Message = new List<MessageDefine>();
        public List<EnumDefine> Enum = new List<EnumDefine>();

        public override string ToString()
        {
            return string.Format("{0} msg:{1} enum:{2}", Name, Message.Count, Enum.Count);
        }
    }
}
