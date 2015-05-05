using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProtoF.AST
{
   

    public class EnumValueNode : TrailingCommentNode
    {
        public int Number;
        public bool NumberIsAutoGen;

        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Number);
        }

        public override void Print(StringBuilder sb, PrintOption opt, params object[] values)
        {
            var maxNameLength = (int)values[0];


            var nameSpace = " ".PadLeft(maxNameLength - Name.Length + 1);
            
            sb.AppendFormat("{0}{1}{2}", opt.MakeIndentSpace(), Name, nameSpace);

            if ((!NumberIsAutoGen || opt.ShowAllEnumNumber ))
            {

                sb.AppendFormat(" = {0}", Number);
            }

            var commentSpace = " ".PadLeft(3 - Number.ToString().Length);
            sb.Append(commentSpace);

            if (!string.IsNullOrEmpty(TrailingComment))
            {
                sb.AppendFormat("//{0}", TrailingComment);
            }

            sb.Append("\n");
        }
    }

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
                n.Print(sb, subopt, maxNameLength);
            }

            sb.Append("}\n");
        }
    }
}
