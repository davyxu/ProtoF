using System.Collections.Generic;
using System.Text;
using ProtoF.Scanner;
using ProtoF.Parser;

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

    public struct PrintOption
    {
        public PrintFormat Format;
        public int Indent;

        public PrintOption( PrintOption parent )
        {
            Format = parent.Format;
            Indent = parent.Indent + 1;
        }

        // 按缩进生成tab
        public string MakeIndentSpace( )
        {
            if (Indent == 0)
                return string.Empty;

            return "\t".PadLeft(Indent);
        }
    }

    public class Node
    {
        public string Name;
        public Location Loc;    // 代码位置

        List<Node> _childNode = new List<Node>();

        public List<Node> Child
        {
            get { return _childNode; }
        }

        public void Add( Node n )
        {
            _childNode.Add(n);
        }        

        
        public virtual void Print(StringBuilder sb, PrintOption opt, params object[] values)
        {

        }
    }

    public class ContainerNode : Node
    {
        protected HashSet<string> _symbols = new HashSet<string>();

        protected void AddSymbol( string name )
        {
            _symbols.Add(name);
        }

        public bool Contain( string name )
        {
            return _symbols.Contains(name);
        }
    }

    // 带尾注释的节点
    public class TrailingCommentNode : Node
    {
        public string TrailingComment;
    }


    // 纯注释节点
    public class CommentNode : Node
    {
        public string Comment;        

        public override void Print(StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("{0}//{1}\n", opt.MakeIndentSpace(), Comment);
        }

        public override string ToString()
        {
            return base.ToString() + " " + Comment;
        }
    }

    // 空行
    public class EOLNode : Node
    {
        public override void Print(StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.Append("\n");            
        }
    }

}
