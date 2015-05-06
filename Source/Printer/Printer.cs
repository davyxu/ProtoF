using ProtoF.AST;
using System.Text;

namespace ProtoF.Printer
{
    public enum PrintFormat
    {
        ProtoF,
        Protobuf,
    }

    public struct PrintOption
    {
        public PrintFormat Format;
        public int Indent;
        public bool ShowAllFieldNumber; // 显示所有字段序号
        public bool ShowAllEnumNumber; // 显示所有枚举序号

        public PrintOption(PrintOption parent)
        {
            this = (PrintOption)parent.MemberwiseClone();

            Indent = parent.Indent + 1;
        }

        // 按缩进生成tab
        public string MakeIndentSpace()
        {
            if (Indent == 0)
                return string.Empty;

            return "\t".PadLeft(Indent);
        }
    }


    public interface IPrinter
    {
        void Print(FileNode node, StringBuilder sb, PrintOption opt, params object[] values);
        void Print(MessageNode node, StringBuilder sb, PrintOption opt, params object[] values);

        void Print(FieldNode node, StringBuilder sb, PrintOption opt, params object[] values);

        void Print(EnumValueNode node, StringBuilder sb, PrintOption opt, params object[] values);
        void Print(EnumNode node, StringBuilder sb, PrintOption opt, params object[] values);

        void Print(CommentNode node, StringBuilder sb, PrintOption opt, params object[] values);

        void Print(EOLNode node, StringBuilder sb, PrintOption opt, params object[] values);
    }
}
