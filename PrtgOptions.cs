using System;
using System.Collections.Generic;
using System.Text;

namespace prtg_exporter_core
{

    public class PrtgOptions
    {
        public const string Key = "PRTG";


        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }



}
