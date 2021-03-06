﻿using ProtoTool.Schema;
using System.IO;
using System.Linq;
using System.Text;

namespace ProtoTool.Protobuf
{

    [ConvertorAttribute(Name = "pb")]
    partial class ProtobufPrinter : Printer
    {
        public override void Print(PackageNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("package {0};\n", node.Name);
        }

        public override void Print(ImportNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            string fileName;
            
            if ( node.Name.EndsWith(".proto") )
            {
                fileName = node.Name;
            }
            else
            {
                fileName = Path.GetFileNameWithoutExtension(node.Name) + ".proto";
            }


            sb.AppendFormat("import \"{0}\";\n", fileName);
        }

        public override void Print(MessageNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("message {0}\n", node.Name);
            sb.Append("{\n");

            var subopt = new PrintOption(opt);


            int maxNameLength = 0;
            int maxTypeLength = 0;

            if ( node.Field.Count > 0 )
            {
                maxNameLength = node.Field.Select(x => x.Name.Length).Max();
                maxTypeLength = node.Field.Select(x => x.TypeName.Length).Max();
            }

            foreach (var n in node.Child)
            {
                n.PrintVisit(this, sb, subopt,
                        maxNameLength,
                        maxTypeLength);
            }

            sb.Append("}\n");
        }

        public override void Print(EnumValueNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            var maxNameLength = (int)values[0];

            sb.Append(opt.MakeIndentSpace());

            sb.Append(node.Name.PadRight(maxNameLength));

            sb.AppendFormat(" = {0}", node.Number);            

            sb.Append(";");

            var commentSpace = " ".PadLeft(3 - node.Number.ToString().Length);
            sb.Append(commentSpace);

            if (!string.IsNullOrEmpty(node.TrailingComment))
            {
                sb.AppendFormat("//{0}", node.TrailingComment);
            }

            sb.Append("\n");
        }

        public override void Print(FieldNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            var maxNameLength = (int)values[0];
            var maxTypeLength = (int)values[1];

            sb.Append(opt.MakeIndentSpace());

            {                
                sb.AppendFormat("{0} ", node.PBLabel.ToString().ToLower());                
            }

            // 类型
            {
                sb.Append(node.TypeName.PadRight(maxTypeLength + 1));
            }

            // 字段名
            {
                sb.Append(node.Name.PadRight(maxNameLength + 1));                
            }


            // 序号
            {
                sb.AppendFormat("= {0}", node.Number);             
            }

            // Option
            if (node.HasOption)
            {                    

                sb.Append("[");

                if (node.DefaultValue != "")
                {
                    switch( node.Type )
                    {
                        case FieldType.String:
                            sb.AppendFormat("default=\"{0}\"", node.DefaultValue);
                            break;
                        default:
                            sb.AppendFormat("default={0}", node.DefaultValue);
                            break;
                    }
                    
                }

                sb.Append("]");                    
            }

            sb.Append(";");
 

            // 注释
            if (!string.IsNullOrEmpty(node.TrailingComment))
            {
                sb.AppendFormat(" //{0}", node.TrailingComment);
            }

            sb.Append("\n");
        }
    }
}
