using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace eAccordeon.Model
{
    public class MidiControllerVirtual : MidiControllerBase
    {
        Task mVirtualTask;
        bool mSuspend;
        AccordeonState[] mVirtialStateSequence = null;
        public MidiControllerVirtual(EAccordeon eAccordeon)
          : base(eAccordeon)
        {
            mSuspend = true;
            mVirtualTask = Task.Factory.StartNew(Work);
            mSelectedPiese = VirtualPieses.DefaultPiese;
        }
        public override string Caption => "Виртуальный порт";

        private void Work()
        {
            while (true)
            {
                while (mSuspend)
                    Thread.Sleep(100);

                EAccordeon.MidiHelper.ResetDevice();

                if (mVirtialStateSequence == null)
                {
                    if (mSelectedPiese == null)
                        continue;
                    mVirtialStateSequence = mSelectedPiese.Sequience;
                }

                foreach (var s in mVirtialStateSequence)
                {
                    if (mSuspend)
                        break;

                    EAccordeon.HandleKeys(s.PressureCode, s.RightKeysState, s.LeftVoicesState);
                    Thread.Sleep(s.Duration_ms);
                }
            }
        }

        public override void Start()
        {
            mSuspend = false;
        }

        public override void Stop()
        {
            EAccordeon.MidiHelper.ResetDevice();
            mSuspend = true;
        }

        public override bool IsStarted => !mSuspend;


        VirtualPieseInfo mSelectedPiese;
        [DisplayName("Пьесса")]
        [ItemsSource(typeof(VirtualPieseInfoSource))]
        public VirtualPieseInfo SelectedPiese
        {
            get { return mSelectedPiese; }
            set
            {

                if (IsStarted)
                {
                    Stop();
                    mVirtialStateSequence = null;
                    mSelectedPiese = value;
                    Start();
                    return;
                }
                else
                {
                    mVirtialStateSequence = null;
                    mSelectedPiese = value;
                }
            }
        }
    }


    public class VirtualPieseInfoSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            var result = new ItemCollection();
            result.AddRange(VirtualPieses.CreateVirtualPieseInfoArray()
                .Select(o => new Item() { DisplayName = o.Caption, Value = o }));
            return result;
        }
    }



    public class VirtualPieseInfo
    {
        public VirtualPieseInfo(string caption, string beepSource)
        {
            Caption = caption;
            Sequience = VirtualPieses.CreateSequenceFromBeepString(beepSource);
        }

        public VirtualPieseInfo(string caption, AccordeonState[] sequience)
        {
            Caption = caption;
            Sequience = sequience;
        }

        public string Caption { get; set; }
        public AccordeonState[] Sequience { get; set; }
    }

    public static class VirtualPieses
    {

        static VirtualPieses()
        {
            #region KARTOSHKA

            var mVirtialStateSequence = new List<AccordeonState>();

            var pause = new AccordeonState(100, 0, AccordeonRightKeys.None, 0);
            var pauseEnd = new AccordeonState(5000, 0, AccordeonRightKeys.None, 0);

            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(0, 70, AccordeonRightKeys.O4_C, 0));
            for (int i = 0; i < 100; i++)
            {
                mVirtialStateSequence.Add(new AccordeonState(5, (byte)(i * 127 / 100), AccordeonRightKeys.O4_C, 0));
            }
            mVirtialStateSequence.Add(pause);
            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_C, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_D, 0));
            mVirtialStateSequence.Add(pause);
            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_D, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_E, 0));
            mVirtialStateSequence.Add(pause);
            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_E, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(1000, 127, AccordeonRightKeys.O4_F, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_E, 0));
            mVirtialStateSequence.Add(pause);
            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_E, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_D, 0));
            mVirtialStateSequence.Add(pause);
            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_D, 0));
            mVirtialStateSequence.Add(pause);


            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_C, 0));
            mVirtialStateSequence.Add(pause);
            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_C, 0));
            mVirtialStateSequence.Add(pause);


            mVirtialStateSequence.Add(new AccordeonState(1000, 127, AccordeonRightKeys.O4_C, 0));

            mVirtialStateSequence.Add(pause);
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(1000, 127, AccordeonRightKeys.O4_E, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(1000, 127, AccordeonRightKeys.O4_D, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_E, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_E, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(1000, 127, AccordeonRightKeys.O4_F, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_E, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_E, 0));
            mVirtialStateSequence.Add(pause);


            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_D, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_D, 0));
            mVirtialStateSequence.Add(pause);


            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_C, 0));
            mVirtialStateSequence.Add(pause);
            mVirtialStateSequence.Add(new AccordeonState(500, 127, AccordeonRightKeys.O4_C, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(new AccordeonState(1000, 127, AccordeonRightKeys.O4_C, 0));
            mVirtialStateSequence.Add(pause);

            mVirtialStateSequence.Add(pauseEnd);

            DefaultPiese = new VirtualPieseInfo("Картошка", mVirtialStateSequence.ToArray());

            #endregion
        }

        public static VirtualPieseInfo[] CreateVirtualPieseInfoArray()
        {
            var result = new List<VirtualPieseInfo>();

            result.Add(DefaultPiese);

            #region HappyNewYear
            result.Add(new VirtualPieseInfo("HappyNewYear", @"-f261 -l500   \
-n -f349 -l500   \
-n -f349 -l250   \
-n -f392 -l250   \
-n -f349 -l250   \
-n -f329 -l250   \
-n -f293 -l500   \
-n -f293 -l500   \
-n -f293 -l500   \
-n -f392 -l500   \
-n -f392 -l250   \
-n -f440 -l250   \
-n -f392 -l250   \
-n -f349 -l250   \
-n -f329 -l500   \
-n -f329 -l500   \
-n -f329 -l500   \
-n -f440 -l500   \
-n -f440 -l250   \
-n -f493 -l250   \
-n -f440 -l250   \
-n -f392 -l250   \
-n -f349 -l500   \
-n -f293 -l500   \
-n -f261 -l250   \
-n -f261 -l250   \
-n -f293 -l500   \
-n -f392 -l500   \
-n -f329 -l500   \
-n -f349 -l1000  \
-n -f261 -l500   \
-n -f349 -l500   \
-n -f349 -l500   \
-n -f349 -l500   \
-n -f329 -l1000  \
-n -f329 -l500   \
-n -f349 -l500   \
-n -f329 -l500   \
-n -f293 -l500   \
-n -f261 -l1000  \
-n -f392 -l1000  \
-n -f440 -l500   \
-n -f392 -l250   \
-n -f392 -l250   \
-n -f349 -l250   \
-n -f349 -l250   \
-n -f523 -l500   \
-n -f261 -l500   \
-n -f261 -l250   \
-n -f261 -l250   \
-n -f293 -l500   \
-n -f392 -l500   \
-n -f329 -l500   \
-n -f349 -l1000"));

            #endregion

            #region Die Gedanken sind Frei
            result.Add(new VirtualPieseInfo("Die Gedanken sind Frei", @"-f392.0 -l200 -n -f392.0 -l200 -n -f523.2 -l400 -n -f523.2 -l400 -n -f659.2 -l200 -n -f523.2 -l200 -n -f392.0 -l800 -n -f392.0 -l400 -n -f349.2 -l400 -n -f293.7 -l400 -n -f392.0 -l400 -n -f329.6 -l400 -n -f261.6 -l400 -n -f392.0 -l400 -n -f523.2 -l400 -n -f523.2 -l400 -n -f659.2 -l200 -n -f523.2 -l200 -n -f392.0 -l800 -n -f392.0 -l400 -n -f349.2 -l400 -n -f293.7 -l400 -n -f392.0 -l400 -n -f329.6 -l400 -n -f261.6 -l400 -n -f523.2 -l400 -n -f493.9 -l400 -n -f587.3 -l400 -n -f493.9 -l400 -n -f523.2 -l400 -n -f659.2 -l400 -n -f523.2 -l400 -n -f493.9 -l400 -n -f587.3 -l400 -n -f493.9 -l400 -n -f523.2 -l400 -n -f659.2 -l400 -n -f523.2 -l400 -n -f440.0 -l400 -n -f440.0 -l400 -n -f523.2 -l200 -n -f440.0 -l200 -n -f392.0 -l800 -n -f392.0 -l200 -n -f659.2 -l200 -n -f659.2 -l200 -n -f587.3 -l200 -n -f523.2 -l400 -n -f493.9 -l400 -n -f523.2 -l800"));

            #endregion


            #region Imperial Marsch – Star Wars
            result.Add(new VirtualPieseInfo("Imperial Marsch – Star Wars", @"-l350 -f392 -D100 -n -l350 -f392 -D100 -n -l350 -f392 -D100 -n -l250 -f311.1 -D100 -n -l25 -f466.2 -D100 -n -l350 -f392 -D100 -n -l250 -f311.1 -D100 -n -l25 -f466.2 -D100 -n -l700 -f392 -D100 -n -l350 -f587.32 -D100 -n -l350 -f587.32 -D100 -n -l350 -f587.32 -D100 -n -l250 -f622.26 -D100 -n -l25 -f466.2 -D100 -n -l350 -f369.99 -D100 -n -l250 -f311.1 -D100 -n -l25 -f466.2 -D100 -n -l700 -f392 -D100 -n -l350 -f784 -D100 -n -l250 -f392 -D100 -n -l25 -f392 -D100 -n -l350 -f784 -D100 -n -l250 -f739.98 -D100 -n -l25 -f698.46 -D100 -n -l25 -f659.26 -D100 -n -l25 -f622.26 -D100 -n -l50 -f659.26 -D400 -n -l25 -f415.3 -D200 -n -l350 -f554.36 -D100 -n -l250 -f523.25 -D100 -n -l25 -f493.88 -D100 -n -l25 -f466.16 -D100 -n -l25 -f440 -D100 -n -l50 -f466.16 -D400 -n -l25 -f311.13 -D200 -n -l350 -f369.99 -D100 -n -l250 -f311.13 -D100 -n -l25 -f392 -D100 -n -l350 -f466.16 -D100 -n -l250 -f392 -D100 -n -l25 -f466.16 -D100 -n -l700 -f587.32 -D100 -n -l350 -f784 -D100 -n -l250 -f392 -D100 -n -l25 -f392 -D100 -n -l350 -f784 -D100 -n -l250 -f739.98 -D100 -n -l25 -f698.46 -D100 -n -l25 -f659.26 -D100 -n -l25 -f622.26 -D100 -n -l50 -f659.26 -D400 -n -l25 -f415.3 -D200 -n -l350 -f554.36 -D100 -n -l250 -f523.25 -D100 -n -l25 -f493.88 -D100 -n -l25 -f466.16 -D100 -n -l25 -f440 -D100 -n -l50 -f466.16 -D400 -n -l25 -f311.13 -D200 -n -l350 -f392 -D100 -n -l250 -f311.13 -D100 -n -l25 -f466.16 -D100 -n -l300 -f392.00 -D150 -n -l250 -f311.13 -D100 -n -l125 -f466.16 -D100 -n -l700 -f392"));

            #endregion
            return result.ToArray();
        }

        public static VirtualPieseInfo DefaultPiese
        {
            get; private set;
        }

        public static AccordeonState[] CreateSequenceFromBeepString(string src)
        {
            var pause = new AccordeonState(30, 0, AccordeonRightKeys.None, 0);
            var pauseEnd = new AccordeonState(5000, 0, AccordeonRightKeys.None, 0);

            List<AccordeonState> result = new List<AccordeonState>();
            var srcList = src.Split(new char[] { ' ', '\t', '\n', '\r', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            AccordeonState state = new AccordeonState();
            state.PressureCode = 127;
            bool lastNoteWasUserPause = false;

            foreach (var i in srcList)
            {
                if (i.StartsWith("-n"))
                {
                    result.Add(state);

                    if (lastNoteWasUserPause == false)
                        result.Add(pause);

                    lastNoteWasUserPause = false;
                    continue;
                }

                if (i.StartsWith("-d"))
                {
                    var duration = (int)double.Parse(i.Substring(2, i.Length - 2), CultureInfo.InvariantCulture);
                    var p = new AccordeonState(duration, 0, AccordeonRightKeys.None, 0);
                    result.Add(p);
                    lastNoteWasUserPause = true;
                    continue;
                }

                if (i.StartsWith("-D"))
                {
                    var duration = (int)double.Parse(i.Substring(2, i.Length - 2), CultureInfo.InvariantCulture);
                    pause.Duration_ms = duration;
                    result.Add(pause);
                    lastNoteWasUserPause = true;
                    continue;
                }


                if (i.StartsWith("-f"))
                {
                    var freq = double.Parse(i.Substring(2, i.Length - 2), CultureInfo.InvariantCulture);
                    var noteId = MidiHelper.FrequencyToMidiNoteNumber(freq);

                    if (noteId < 53)
                        noteId = 53;

                    if (noteId > 93)
                        noteId = 93;

                    int noteShift = 23 + noteId - 53;

                    state.RightKeysState = (AccordeonRightKeys)(1UL << noteShift);
                    continue;
                }


                if (i.StartsWith("-l"))
                {
                    var duration = double.Parse(i.Substring(2, i.Length - 2), CultureInfo.InvariantCulture);
                    state.Duration_ms = (int)duration;
                    continue;
                }

            }
            result.Add(state);
            result.Add(pauseEnd);
            return result.ToArray();
        }
    }
}
