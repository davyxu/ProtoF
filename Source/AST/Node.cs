using System.Collections.Generic;
using System.Text;
using ProtoF.Scanner;

namespace ProtoF.AST
{
    public enum FieldType
    {
        None = 0,
        Bool,
        Int32,
        UInt32,
        Int64,
        UInt64,
        String,
        Float,
        Double,
        Message,
        Enum,
        Bytes,
    }

    public enum FieldContainer
    {
        None = 0,
        Array,
    }

    public enum PrintFormat
    {
        ProtoF,
        Protobuf,
    }

    public class Node
    {
        public Location Loc;    // 代码位置
        public List<Node> ChildNode = new List<Node>();
        
        public virtual void Print(StringBuilder sb, PrintFormat format, params object[] values)
        {

        }
    }

    public class CommentNode : Node
    {
        public string Comment;
    }

}
