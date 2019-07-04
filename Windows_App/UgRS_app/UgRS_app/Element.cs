using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UgRS_app
{
    class Element
    {
        public byte oblik;
        public byte boja;
        public byte masa;

        public Element(byte oblik, byte boja, byte masa)
        {
            this.oblik = oblik;
            this.boja = boja;
            this.masa = masa;
        }

        public Element()
        {

        }
    }
}
