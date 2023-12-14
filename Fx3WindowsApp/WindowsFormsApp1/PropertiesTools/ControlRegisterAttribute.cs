using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsbApi;

namespace ImagerDebugSoftware.PropertiesTools
{
    public class ControlRegisterAttribute : Attribute
    {
        private REGISTER _offset;
        private int _methodType = 0;
        private int _shiftBits = 0;
        private int _valueRange = 32;

        //The register offset address in APEX board
        public REGISTER Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
            }
        }

        //Call which method to process the data: 
        //0: WriteToRegister(UInt32 data, REGISTER offsetAddress) or WriteToRegister(byte[] buf, REGISTER offsetAddress)
        //1: (call ResetRegisterBits(1<<ShiftBits, Offset) when value=0, call SetRegisterBits(1<<ShiftBits, Offset) when value=1)
        public int MethodType
        {
            get
            {
                return _methodType;
            }
            set
            {
                _methodType = value;
            }
        }

        //Shift operation, left shift operation(ShiftBits>0) or right shift operation(ShiftBits<0), for instance: original number is 1111, and it will execute 1111<<ShiftBits when ShiftBits >= 0, and it will execute 1111>>ShiftBits when ShiftBits <0
        public int ShiftBits
        {
            get
            {
                return _shiftBits;
            }
            set
            {
                _shiftBits = value;
            }
        }

        //Value Range: define the range of bits affected by register write
        public int ValueRange
        {
            get
            {
                return _valueRange;
            }
            set
            {
                _valueRange = value;
            }
        }
    }
}
