using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1 
{
    public class Pair 
    {
        public Pair() 
        {

        }

        public Pair(String a, String p) {
            addr = a;
            port = p;
        }

        public IPEndPoint endpoint { get; set; }
        public String addr{get;set;}

        public String port{get;set;}

    }
}
