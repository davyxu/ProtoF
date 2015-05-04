
using ProtoF.Proto;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProtoF.Formater
{
    class ProtoFFormater
    {
        public static string PrintFile( FileDefine def )
        {
            var sb = new StringBuilder();

            sb.AppendFormat("package {0}\n\n", def.Package);

            foreach( var msg  in def.Message )
            {
                PrintMessage(sb, msg);
            }

            return sb.ToString();
        }

        static void PrintMessage( StringBuilder sb, MessageDefine def )
        {
            sb.AppendFormat("message {0}\n", def.Name);
            sb.Append("{\n");          
  
            var maxNameLength = def.Field.Select( x => x.Name.Length ).Max();
            var maxTypeLength = def.Field.Select(x => x.CompleteTypeName.Length).Max();

            foreach( var fd in def.Field )
            {
                {
                    var space = " ".PadLeft(maxNameLength - fd.Name.Length + 1);

                    sb.AppendFormat("   {0}{1}", fd.Name, space);
                }


                {
                    var space = " ".PadLeft(maxTypeLength - fd.CompleteTypeName.Length + 1);
                    sb.AppendFormat("{0}{1}", fd.CompleteTypeName, space);
                }
                                

                if ( !string.IsNullOrEmpty( fd.TrailingComment ) )
                {
                    sb.AppendFormat("//{0}", fd.TrailingComment);
                }

                sb.Append("\n");
            }

            sb.Append("}\n\n");
        }
    }
}
