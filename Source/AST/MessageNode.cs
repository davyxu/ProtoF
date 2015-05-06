using ProtoF.Printer;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoF.AST
{
    public class FieldNode : TrailingCommentNode
    {
        // Type
        public FieldType Type;
        public FieldContainer Container;
        public string TypeName;
        public Node TypeRef;

        // Option
        public bool HasOption;
        public string DefaultValue;

        // Number
        public int Number;
        public bool NumberIsAutoGen;

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

        public override void PrintVisit(IPrinter printer, StringBuilder sb, PrintOption opt, params object[] values)
        {
            printer.Print(this, sb, opt, values);
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

        public override void PrintVisit(IPrinter printer, StringBuilder sb, PrintOption opt, params object[] values)
        {
            printer.Print(this, sb, opt, values);
        }
    }
}
