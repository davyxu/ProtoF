using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProtoF.AST
{
    public class FileNode : Node
    {
        public string Package;
        public List<string> Dependency = new List<string>();
        public List<MessageNode> Message = new List<MessageNode>();
        public List<EnumNode> Enum = new List<EnumNode>();

        public void AddMessage(MessageNode n)
        {
            Child.Add(n);
            Message.Add(n);
        }


        public MessageNode GetMessageByName( string name )
        {
            return Message.Find(x=> x.Name == name );
        }

        public EnumNode GetEnumByName(string name)
        {
            return Enum.Find(x => x.Name == name);
        }

        public void AddEnum(EnumNode n)
        {
            Child.Add(n);
            Enum.Add(n);
        }

        public override string ToString()
        {
            return string.Format("{0} msg:{1} enum:{2}", Name, Message.Count, Enum.Count);
        }

        public override void Print(StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("package {0}\n", Package);


            foreach (var n in Child)
            {
                n.Print(sb, opt);                
            }

        }

    }
}
