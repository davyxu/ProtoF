using ProtoF.AST;
using ProtoF.Printer;
using System.Linq;
using System.Text;

namespace ProtoF.Printer
{
    partial class ProtoFPrinter : IPrinter
    {
        public void Print(MessageNode node, StringBuilder sb, PrintOption opt, params object[] values)
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

        public void Print(FieldNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            var maxNameLength = (int)values[0];
            var maxTypeLength = (int)values[1];

            // 字段名
            {
                var space = " ".PadLeft(maxNameLength - node.Name.Length + 1);
                sb.AppendFormat("{0}{1}{2}", opt.MakeIndentSpace(), node.Name, space);
            }

            // 类型
            {
                var space = " ".PadLeft(maxTypeLength - node.CompleteTypeName.Length + 1);
                sb.AppendFormat("{0}{1}", node.CompleteTypeName, space);
            }

            // 序号
            {
                if (node.Number > 0 && (!node.NumberIsAutoGen || opt.ShowAllFieldNumber))
                {
                    sb.AppendFormat("= {0} ", node.Number);
                }
            }


            // Option
            if (node.HasOption)
            {                    

                sb.Append("[");

                if (node.DefaultValue != "")
                {
                    sb.AppendFormat("default:{0}", node.DefaultValue);
                }

                sb.Append("] ");                    
            }
 


            // 注释
            if (!string.IsNullOrEmpty(node.TrailingComment))
            {
                sb.AppendFormat("//{0}", node.TrailingComment);
            }

            sb.Append("\n");
        }
    }
}
