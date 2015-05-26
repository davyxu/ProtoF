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
        protected bool ResolveFieldType(string packageName, FieldNode fieldNode)
        {
            var symbols = _fileNode.ScopeSymbols.Get(packageName, fieldNode.TypeName);
            if (symbols == null)
            {
                return false;
            }

            return MakeFieldValid(symbols, fieldNode);
        }

        bool MakeFieldValid( Node symbols, FieldNode fieldNode )
        {
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
                var symbols = _tool.LookUpSymbols(fn.Package, fieldNode.TypeName);
                if (symbols == null)
                {
                    Reporter.Error(ErrorType.Parse, fieldNode.Loc, "'{0}' undefined!", fieldNode.TypeName);
                    continue;
                }

                if (!MakeFieldValid(symbols, fieldNode))
                {
                    Reporter.Error(ErrorType.Parse, fieldNode.Loc, "'{0}' resolve type error!", fieldNode.TypeName);                    
                }

            }
        }
    }
}
