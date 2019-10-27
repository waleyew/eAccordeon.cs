using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAccordeon.Model
{
    public class RegisterInfo
    {
        public RegisterInfo(string caption, int[] octaveOffsets)
        {
            Caption = caption;
            OctaveOffsets = octaveOffsets;
            if (octaveOffsets == null)
                throw new ArgumentNullException("octaveOffsets");

            if (octaveOffsets.Length == 0)
                throw new ArgumentException("octaveOffsets");
        }
        public string Caption { get; internal set; }
        public int[] OctaveOffsets { get; internal set; }

        public int[] GetNotes(int pureNote)
        {
            int voicesCount = OctaveOffsets.Length;
            int[] result = new int[voicesCount];
            for (int i = 0; i < voicesCount; i++)
            {
                result[i] = pureNote + OctaveOffsets[i] * 12;
            }
            return result;
        }
    }
}
