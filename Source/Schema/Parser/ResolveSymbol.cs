using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoTool.Schema
{
    public partial class Parser
    {
        protected List<FieldNode> _unsolvedNode = new List<FieldNode>();

        protected void AddUnsolveNode(FieldNode n)
        {
            _unsolvedNode.Add(n);
        }

        // 就这节点类型
        protected bool ResolveFieldType(FileNode fn, FieldNode fieldNode)
        {
            var symbols = _tool.Symbols.Get(fn.Package, fieldNode.TypeName);
            if (symbols == null)
            {
                return false;
            }

            var en = symbols as EnumNode;
            if (en != null)
            {
                fieldNode.Type = FieldType.Enum;
                fieldNode.TypeRef = en;
                return true;
            }

            var msg = symbols as MessageNode;
            if (msg != null)
            {
                fieldNode.Type = FieldType.Message;
                fieldNode.TypeRef = en;
                return true;
            }

            return false;
        }

        // 解决前向引用的未知节点
        protected void ResolveUnknownNode(FileNode fn)
        {
            foreach (FieldNode fieldNode in _unsolvedNode)
            {
                if (!ResolveFieldType(fn, fieldNode))
                {
                    Error(fieldNode.Loc, "'{0}' undefined!", fieldNode.TypeName);
                }

            }
        }
    }
}
