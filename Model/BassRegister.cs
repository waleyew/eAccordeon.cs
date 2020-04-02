using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAccordeon.Model
{
    public class BassRegister
    {
        BassValveInfo[] bassValveInfoArray;

        private BassRegister(string caption, BassValveInfo[] bassValveInfoArray)
        {
            Caption = caption;
            this.bassValveInfoArray = bassValveInfoArray;
        }

        public string Caption
        {
            get;
            private set;
        }

        public int[] GetNotes(int bit)
        {
            if (bit >= 0 && bit < bassValveInfoArray.Length)
            {
                var result = bassValveInfoArray[bit].MidiNumbers;
                //for (int i = 0; i < result.Length; i++)
                //{
                //    System.Diagnostics.Debug.Write(result[i]);
                //    System.Diagnostics.Debug.Write(" ");
                //}
                //System.Diagnostics.Debug.WriteLine("");
                return result;
            }
            else
                return null;
        }

        internal static BassRegister[] GetRegisters()
        {
            List<BassRegister> result = new List<Model.BassRegister>();

            var reg1 = new BassRegister("Полный регистр",
                new BassValveInfo[]
                {
                    new BassValveInfo(66, 54),  // 1
                    new BassValveInfo(71, 59),  // 2
                    new BassValveInfo(64, 52),  // 3
                    new BassValveInfo(69, 57),  // 4
                    new BassValveInfo(74, 62),  // 5
                    new BassValveInfo(67, 55),  // 6
                    new BassValveInfo(72, 60),  // 7
                    new BassValveInfo(65, 53),  // 8
                    new BassValveInfo(70, 58),  // 9
                    new BassValveInfo(75, 63),  // 10
                    new BassValveInfo(68, 56),  // 11
                    new BassValveInfo(73, 61),  // 12
                    new BassValveInfo(54, 42),  // 13
                    new BassValveInfo(47, 35),  // 14
                    new BassValveInfo(52, 40),  // 15
                    new BassValveInfo(45, 33),  // 16
                    new BassValveInfo(50, 38),  // 17
                    new BassValveInfo(43, 31),  // 18
                    new BassValveInfo(48, 36),  // 19
                    new BassValveInfo(53, 41),  // 20
                    new BassValveInfo(46, 34),  // 21
                    new BassValveInfo(51, 39),  // 22
                    new BassValveInfo(44, 32),  // 23
                    new BassValveInfo(49, 37),  // 24
                });
            result.Add(reg1);


            var reg2 = new BassRegister("Выкл. верхн. регистр",
               new BassValveInfo[]
               {
                    new BassValveInfo(54),  // 1
                    new BassValveInfo(59),  // 2
                    new BassValveInfo(52),  // 3
                    new BassValveInfo(57),  // 4
                    new BassValveInfo(62),  // 5
                    new BassValveInfo(55),  // 6
                    new BassValveInfo(60),  // 7
                    new BassValveInfo(53),  // 8
                    new BassValveInfo(58),  // 9
                    new BassValveInfo(63),  // 10
                    new BassValveInfo(56),  // 11
                    new BassValveInfo(61),  // 12
                    new BassValveInfo(54, 42),  // 13
                    new BassValveInfo(47, 35),  // 14
                    new BassValveInfo(52, 40),  // 15
                    new BassValveInfo(45, 33),  // 16
                    new BassValveInfo(50, 38),  // 17
                    new BassValveInfo(43, 31),  // 18
                    new BassValveInfo(48, 36),  // 19
                    new BassValveInfo(53, 41),  // 20
                    new BassValveInfo(46, 34),  // 21
                    new BassValveInfo(51, 39),  // 22
                    new BassValveInfo(44, 32),  // 23
                    new BassValveInfo(49, 37),  // 24
               });
            result.Add(reg2);

            var reg3 = new BassRegister("Выкл. верхн. и нижн. регистр",
               new BassValveInfo[]
               {
                     new BassValveInfo(54),  // 1
                    new BassValveInfo(59),  // 2
                    new BassValveInfo(52),  // 3
                    new BassValveInfo(57),  // 4
                    new BassValveInfo(62),  // 5
                    new BassValveInfo(55),  // 6
                    new BassValveInfo(60),  // 7
                    new BassValveInfo(53),  // 8
                    new BassValveInfo(58),  // 9
                    new BassValveInfo(63),  // 10
                    new BassValveInfo(56),  // 11
                    new BassValveInfo(61),  // 12
                    new BassValveInfo(54),  // 13
                    new BassValveInfo(47),  // 14
                    new BassValveInfo(52),  // 15
                    new BassValveInfo(45),  // 16
                    new BassValveInfo(50),  // 17
                    new BassValveInfo(43),  // 18
                    new BassValveInfo(48),  // 19
                    new BassValveInfo(53),  // 20
                    new BassValveInfo(46),  // 21
                    new BassValveInfo(51),  // 22
                    new BassValveInfo(44),  // 23
                    new BassValveInfo(49),  // 24
               });
            result.Add(reg3);

            return result.ToArray();
        }
    }


    public struct BassValveInfo
    {
        public BassValveInfo(params int[] midiNumbers)
        {
            MidiNumbers = midiNumbers;
        }

        public int[] MidiNumbers
        {
            get;
            private set;
        }
    }
}
