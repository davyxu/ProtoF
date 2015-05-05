using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoF.AST
{
    public class FieldNode : TrailingCommentNode
    {
        public FieldType Type;
        public FieldContainer Container;
        public string TypeName;
        public Node TypeRef;

        public string DefaultValue;
        public int Number;
        

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

        public override void Print(StringBuilder sb, PrintOption opt, params object[] values )
        {
            var maxNameLength = (int)values[0];
            var maxTypeLength = (int)values[1];

            {
                var space = " ".PadLeft(maxNameLength - Name.Length + 1);                
                sb.AppendFormat("{0}{1}{2}", opt.MakeIndentSpace(), Name, space);
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


    public class MessageNode : ContainerNode
    {
        public string Extends;
        public List<FieldNode> Field = new List<FieldNode>();
        public List<EnumNode> Enum = new List<EnumNode>();        

        public void AddEnum(EnumNode n)
        {
            AddSymbol(n.Name);
            Child.Add(n);
            Enum.Add(n);
        }

        public void AddField(FieldNode n)
        {
            AddSymbol(n.Name);
            Child.Add(n);
            Field.Add(n);
        }        

        public override string ToString()
        {
            return string.Format("{0} fields:{1}", Name, Field.Count);
        }

        public override void Print(StringBuilder sb, PrintOption opt, params object[] values )
        {
            sb.AppendFormat("message {0}\n", Name);
            sb.Append("{\n");

            var maxNameLength = Field.Select(x => x.Name.Length).Max();
            var maxTypeLength = Field.Select(x => x.CompleteTypeName.Length).Max();

            var subopt = new PrintOption(opt);

            foreach (var n in Child)
            {
                n.Print(sb, subopt, maxNameLength, maxTypeLength );
            }

            sb.Append("}\n");
        }
    }
}
