using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UgRS_app
{
    class Filter
    {
        public byte oblici;
        public byte boje;
        public byte masaMin;
        public byte masaMax;

        public Filter(byte oblici, byte boje, byte masaMin, byte masaMax)
        {
            this.oblici = oblici;
            this.boje = boje;
            this.masaMin = masaMin;
            this.masaMax = masaMax;
        }

        public Filter()
        {
  
        }
    }
}
