using ProtoTool.Schema;
using System.Linq;
using System.Text;

namespace ProtoTool.ProtoF
{
    partial class ProtoFPrinter : Printer
    {
        public override void Print(MessageNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("message {0}\n", node.Name);
            sb.Append("{\n");

            var maxNameLength = node.Field.Select(x => x.Name.Length).Max();
            var maxTypeLength = node.Field.Select(x => x.CompleteTypeName.Length).Max();


            var subopt = new PrintOption(opt);

            foreach (var n in node.Child)
            {
                n.PrintVisit(this, sb, subopt,
                        maxNameLength,
                        maxTypeLength);
            }

            sb.Append("}\n");
        }
    }
}
