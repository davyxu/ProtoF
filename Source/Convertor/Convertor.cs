using ProtoTool.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProtoTool.Convertor
{
    public class ProtoConvertor
    {
        List<System.Type> _parserList = new List<System.Type>();
        List<System.Type> _printerList = new List<System.Type>();

        Tool _tool;
        public ProtoConvertor( Tool tool )
        {
            _tool = tool;
        }

        public void RegisterParser(System.Type type)
        {
            _parserList.Add(type);
        }

        public void RegisterPrinter(System.Type type)
        {
            _printerList.Add(type);
        }

        static Type MatchConvertorAttribute(List<System.Type> list, string name)
        {
            foreach (var t in list)
            {
                var att = Attribute.GetCustomAttribute(t, typeof(ConvertorAttribute)) as ConvertorAttribute;
                if (att == null)
                    continue;

                if (att.Name == name)
                {
                    return t;
                }
            }

            return null;
        }

        Parser CreateParser(string name)
        {
            var type = MatchConvertorAttribute(_parserList, name);
            if (type == null)
                return null;

            var parser = Activator.CreateInstance(type) as Parser;
            parser.Init(_tool);

            return parser;
        }

        Printer CreatePrinter(string name)
        {
            var type = MatchConvertorAttribute(_printerList, name);
            if (type == null)
                return null;

            return Activator.CreateInstance(type) as Printer;
        }


        public void Do(string method, string inputFileName, string outputFileName)
        {
            var sidestr = method.Split('2');
            if (sidestr.Length != 2)
                return;

            Parser parser = CreateParser(sidestr[0]);

            if (parser == null)
            {
                Reporter.Error(ErrorType.Tool, "can not find parser: {0}", sidestr[0]);
                return;
            }

            Printer printer = CreatePrinter(sidestr[1]);

            if (printer == null)
            {
                Reporter.Error(ErrorType.Tool, "can not find printer: {0}", sidestr[1]);
                return;
            }

            var file = parser.StartParseFile(inputFileName);

            var sb = new StringBuilder();

            var opt = new PrintOption();
            opt.Format = PrintFormat.ProtoF;
            //subopt.ShowAllFieldNumber = true;
            //subopt.ShowAllEnumNumber = true;

            file.PrintVisit(printer, sb, opt);

            File.WriteAllText(outputFileName, sb.ToString(), new UTF8Encoding(false));
        }

    }
}
