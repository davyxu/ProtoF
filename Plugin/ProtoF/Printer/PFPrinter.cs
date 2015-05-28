using ProtoTool.Schema;
using System.Linq;
using System.Text;

namespace ProtoTool.ProtoF
{
    [ConvertorAttribute(Name = "pf")]
    partial class ProtoFPrinter : Printer
    {
        public override void Print(MessageNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("message {0}\n", node.Name);
            sb.Append("{\n");

            var subopt = new PrintOption(opt);

            int maxNameLength = 0;
            int maxTypeLength = 0;

            if (node.Field.Count > 0)
            {
                maxNameLength = node.Field.Select(x => x.Name.Length).Max();
                maxTypeLength = node.Field.Select(x => x.CompleteTypeName.Length).Max();
            }

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
