using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoF.AST
{
    public class FieldNode : Node
    {
        public string Name;
        public FieldType Type;
        public FieldContainer Container;
        public string TypeName;
        public string DefaultValue;
        public int Number;
        public string TrailingComment;

        // protof特有的
        public int AutoNumber;

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Name, Type, TypeName);
        }

        public string CompleteTypeName
        {
            get
            {
                switch (Container)
                {
                    case FieldContainer.Array:
                        return string.Format("array<{0}>", TypeName);
                }

                return TypeName;
            }
        }

        public override void Print(StringBuilder sb, PrintFormat format, params object[] values )
        {
            var maxNameLength = (int)values[0];
            var maxTypeLength = (int)values[1];

            {
                var space = " ".PadLeft(maxNameLength - Name.Length + 1);

                sb.AppendFormat("   {0}{1}", Name, space);
            }


            {
                var space = " ".PadLeft(maxTypeLength - CompleteTypeName.Length + 1);
                sb.AppendFormat("{0}{1}", CompleteTypeName, space);
            }


            if (!string.IsNullOrEmpty(TrailingComment))
            {
                sb.AppendFormat("//{0}", TrailingComment);
            }

            sb.Append("\n");
        }
    }


    public class MessageNode : Node
    {
        public string Name;
        public string Extends;
        public List<FieldNode> Field = new List<FieldNode>();
        public List<EnumNode> Enum = new List<EnumNode>();

        public void AddEnum(EnumNode n)
        {
            ChildNode.Add(n);
            Enum.Add(n);
        }

        public void AddField(FieldNode n)
        {
            ChildNode.Add(n);
            Field.Add(n);
        }

        public override string ToString()
        {
            return string.Format("{0} fields:{1}", Name, Field.Count);
        }

        public override void Print(StringBuilder sb, PrintFormat format, params object[] values )
        {
            sb.AppendFormat("message {0}\n", Name);
            sb.Append("{\n");

            var maxNameLength = Field.Select(x => x.Name.Length).Max();
            var maxTypeLength = Field.Select(x => x.CompleteTypeName.Length).Max();

            foreach (var n in ChildNode)
            {
                n.Print(sb, format, maxNameLength, maxTypeLength );
            }

            sb.Append("}\n\n");
        }
    }
}
