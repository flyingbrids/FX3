using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagerDebugSoftware.PropertiesTools
{
    public class UInt32ValidationAttribute : Attribute
    {
        private uint _max = 65535;
        private uint _min = 0;
        public uint Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }
        public uint Min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
            }
        }
    }
}
