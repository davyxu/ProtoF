using System.Collections.Generic;
using System.Text;
using ProtoF.Printer;

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

        public override void PrintVisit(IPrinter printer, StringBuilder sb, PrintOption opt, params object[] values)
        {
            printer.Print(this, sb, opt, values);
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

        public override void PrintVisit(IPrinter printer, StringBuilder sb, PrintOption opt, params object[] values)
        {
            printer.Print(this, sb, opt, values);
        }

    }
}
