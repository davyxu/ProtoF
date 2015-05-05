using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProtoF.AST
{
    public class EnumNode : ContainerNode
    {
        
        public List<EnumValueNode> Value = new List<EnumValueNode>();

        public override string ToString()
        {
            return string.Format("{0} values:{1}", Name, Value.Count);
        }

        public void AddValue(EnumValueNode n)
        {
            Child.Add(n);
            Value.Add(n);
        }

        public override void Print(StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("enum {0}\n", Name);
            sb.Append("{\n");

            var maxNameLength = Value.Select(x => x.Name.Length).Max();

            var subopt = new PrintOption(opt);

            foreach (var n in Child)
            {
                n.Print(sb, subopt, maxNameLength );
            }

            sb.Append("}\n");
        }
    }

    public class EnumValueNode : TrailingCommentNode
    {
        public int Number;

        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Number);
        }

        public override void Print(StringBuilder sb, PrintOption opt, params object[] values)
        {
            var maxNameLength = (int)values[0];

            {
                var space = " ".PadLeft(maxNameLength - Name.Length + 5 );
                sb.AppendFormat("{0}{1} = {2}{3}", opt.MakeIndentSpace(), Name, Number, space);
            }

            if (!string.IsNullOrEmpty(TrailingComment))
            {
                sb.AppendFormat("//{0}", TrailingComment);
            }

            sb.Append("\n");
        }
    }
}
