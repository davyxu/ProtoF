using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProtoF.AST
{
    public class EnumNode : Node
    {
        public string Name;
        public List<EnumValueNode> Value = new List<EnumValueNode>();

        public override string ToString()
        {
            return string.Format("{0} values:{1}", Name, Value.Count);
        }

        public void AddValue(EnumValueNode n)
        {
            ChildNode.Add(n);
            Value.Add(n);
        }

        public override void Print(StringBuilder sb, PrintFormat format, params object[] values)
        {
            sb.AppendFormat("enum {0}\n", Name);
            sb.Append("{\n");

            var maxNameLength = Value.Select(x => x.Name.Length).Max();            

            foreach (var n in ChildNode)
            {
                n.Print(sb, format, maxNameLength );
            }

            sb.Append("}\n\n");
        }
    }

    public class EnumValueNode : Node
    {
        public string Name;
        public int Number;
        public string TrailingComment;

        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Number);
        }

        public override void Print(StringBuilder sb, PrintFormat format, params object[] values)
        {
            var maxNameLength = (int)values[0];

            {
                var space = " ".PadLeft(maxNameLength - Name.Length + 5 );

                sb.AppendFormat("   {0} = {1}{2}", Name, Number, space );
            }

            if (!string.IsNullOrEmpty(TrailingComment))
            {
                sb.AppendFormat("//{0}", TrailingComment);
            }

            sb.Append("\n");
        }
    }
}
