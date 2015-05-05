using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoF.AST
{
    public class FileNode : Node
    {
        public string Name;
        public string Package;
        public List<string> Dependency = new List<string>();
        public List<MessageNode> Message = new List<MessageNode>();
        public List<EnumNode> Enum = new List<EnumNode>();

        public void AddMessage(MessageNode n)
        {
            ChildNode.Add(n);
            Message.Add(n);
        }

        public void AddEnum(EnumNode n)
        {
            ChildNode.Add(n);
            Enum.Add(n);
        }

        public override string ToString()
        {
            return string.Format("{0} msg:{1} enum:{2}", Name, Message.Count, Enum.Count);
        }

        public override void Print(StringBuilder sb, PrintFormat format, params object[] values)
        {
            sb.AppendFormat("package {0}\n\n", Package);

            foreach (var n in ChildNode)
            {
                n.Print( sb, format );                
            }

        }

    }
}
