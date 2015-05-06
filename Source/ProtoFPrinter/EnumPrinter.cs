using ProtoF.AST;
using ProtoF.Printer;
using System.Linq;
using System.Text;

namespace ProtoF.Printer
{
    partial class ProtoFPrinter : IPrinter
    {
        public void Print(EnumValueNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            var maxNameLength = (int)values[0];


            var nameSpace = " ".PadLeft(maxNameLength - node.Name.Length + 1);

            sb.AppendFormat("{0}{1}{2}", opt.MakeIndentSpace(), node.Name, nameSpace);

            if ((!node.NumberIsAutoGen || opt.ShowAllEnumNumber))
            {

                sb.AppendFormat(" = {0}", node.Number);
            }

            var commentSpace = " ".PadLeft(3 - node.Number.ToString().Length);
            sb.Append(commentSpace);

            if (!string.IsNullOrEmpty(node.TrailingComment))
            {
                sb.AppendFormat("//{0}", node.TrailingComment);
            }

            sb.Append("\n");
        }

        public void Print(EnumNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("enum {0}\n", node.Name);
            sb.Append("{\n");

            var maxNameLength = node.Value.Select(x => x.Name.Length).Max();

            var subopt = new PrintOption(opt);

            foreach (var n in node.Child)
            {
                n.PrintVisit(this, sb, subopt, maxNameLength);
            }

            sb.Append("}\n");
        }
    }
}
