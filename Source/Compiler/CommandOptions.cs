using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoF
{
    class CommandOptions
    {
        [Option('m', "method", Required = true, HelpText = "method to operate")]
        public string Method { get; set; }

        [Option('i', "inputfile", Required = true, HelpText = "input file name")]
        public string InputFile { get; set; }

        [Option('o', "outputfile", Required = true, HelpText = "output file name")]
        public string OutputFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) =>
              {


                  HelpText.DefaultParsingErrorsHandler(this, current);
              });
        }
    }

}
