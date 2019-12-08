using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;


// Ссылки:
// Классификация аккордеонов, схемы кнопок: https://www.turkaramamotoru.com/ru/Аккордеон-181719.html


namespace eAccordeon.Model
{
    public class EAccordeon
    {
        MidiHelper mMidiHelper;
        MidiControllerBase mMidiController;
        float mActualPressurePercents;
        float mActualPressurePercents_ExpFilterAlpha;
        AccordeonRightKeys mActualPightKeysState;
        UInt64 mActualLeftVoicesState;
        RegisterInfo[] mRightRegisters;
        RegisterInfo[] mLeftRegisters;
        RegisterInfo mSelectedRightRegister;
        RegisterInfo mSelectedLeftRegister;
        int mChannelIdForRightSide = 1;
        TransformPressureMethod mTransformPressureMethod;
        ushort mPressureSensonUpperLimit;
        int mKeystrokeForce;
        int mRightHandOctaveShift;



        public EAccordeon()
        {
            mMidiHelper = new MidiHelper();
            mMidiController = new MidiControllerSerial(this);

            mRightRegisters = new RegisterInfo[]
            {
                new RegisterInfo("Основная октава", new int[] {0}),
                new RegisterInfo("Основная октава, +1", new int[] {0, 1}),
                new RegisterInfo("Основная октава, -1", new int[] {0, -1}),
                new RegisterInfo("Основная октава, -1, +1", new int[] {0, -1, +1}),
            };
            try { SelectedRightRegister = mRightRegisters[Properties.Settings.Default.SelectedRightRegisterId]; }
            catch { SelectedRightRegister = mRightRegisters[0]; }


            mLeftRegisters = new RegisterInfo[]
            {
                new RegisterInfo("Основная октава", new int[] {0}),
            };
            try { SelectedLeftRegister = mLeftRegisters[Properties.Settings.Default.SelectedLeftRegisterId]; }
            catch { SelectedLeftRegister = mLeftRegisters[0]; }

            mActualPressurePercents_ExpFilterAlpha = Properties.Settings.Default.PressureFilter;
            mChannelIdForRightSide = Properties.Settings.Default.ChannelIdForRightSide;

            mTransformPressureMethod = (TransformPressureMethod)Properties.Settings.Default.TransformPressureMethod;
            mPressureSensonUpperLimit = Properties.Settings.Default.PressureSensonUpperLimit;

            mKeystrokeForce = Properties.Settings.Default.KeystrokeForce;
            mRightHandOctaveShift = Properties.Settings.Default.RightHandOctaveShift;

        }

        public MidiHelper MidiHelper
        {
            get { return mMidiHelper; }
        }
        public MidiControllerBase MidiController
        {
            get { return mMidiController; }
            set { mMidiController = value; }
        }


        public void HandleKeys(AccordeonState state)
        {
            HandleKeys(state.PressureCode, state.RightKeysState, state.LeftVoicesState);
        }


        /// <summary>
        /// Обрабатывает состояние нажатия клавиш и давления в мехах аккордеона. Вызывается при приеме новой команды из конроллера аккордеона.
        /// </summary>
        /// <param name="pressureCode">Давление в мехах аккордеона в кодах от 0 до 127.</param>
        /// <param name="rightKeysState">Состояние нажатия клавиш на левой клавиатуре.</param>
        /// <param name="leftVoicesState">Состояние открытия клапанов в правой части аккордеона.</param>
        public void HandleKeys(byte pressureCode, AccordeonRightKeys rightKeysState, UInt64 leftVoicesState)
        {
            //System.Diagnostics.Debug.Print($"{pressureCode}, {rightKeysState}, {leftVoicesState}");
            //return;

            ProccedPressude(pressureCode);

            if (rightKeysState != mActualPightKeysState)
                ChangeActualRightKeysPressed(rightKeysState);

            if (leftVoicesState != mActualLeftVoicesState)
                ChangeActualLeftVoisesPressed(leftVoicesState);
        }


        public ushort PressureSensonUpperLimit
        {
            get { return mPressureSensonUpperLimit; }
            set
            {
                mPressureSensonUpperLimit = value;
                Properties.Settings.Default.PressureSensonUpperLimit = value;
            }
        }


        public int KeystrokeForce
        {
            get { return mKeystrokeForce; }
            set
            {
                if (value < 1)
                    value = 1;

                if (value > 127)
                    value = 127;

                mKeystrokeForce = value;
                Properties.Settings.Default.KeystrokeForce = value;
            }
        }

        public int RightHandOctaveShift
        {
            get { return mRightHandOctaveShift; }
            set
            {
                mRightHandOctaveShift = value;
                Properties.Settings.Default.RightHandOctaveShift = value;
            }
        }

        public TransformPressureMethod TransformPressureMethod
        {
            get { return mTransformPressureMethod; }
            set
            {
                mTransformPressureMethod = value;
                Properties.Settings.Default.TransformPressureMethod = (int)value;
            }
        }

        /// <summary>
        /// Преобразует измеренное значение давления из 16-битного числа в код Midi
        /// </summary>
        /// <param name="pressure"></param>
        /// <returns></returns>
        public byte TransformPressure(ushort pressure)
        {
            var result = pressure;

            switch (mTransformPressureMethod)
            {
                case TransformPressureMethod.DisableSensor:
                    return 127;

                case TransformPressureMethod.Linear:
                    result = (ushort)(result * 127 / mPressureSensonUpperLimit);
                    break;

                case TransformPressureMethod.Logariphmic:
                    if (result == 0)
                        return 0;
                    result = (ushort)(Math.Exp(4.844 * result / mPressureSensonUpperLimit));
                    break;

            }

            if (result > 127)
                result = 127;

            return (byte)result;
        }

        /// <summary>
        /// Применяет уровень звука (от давленя мехов)
        /// </summary>
        /// <param name="pressureCode"></param>
        private void ProccedPressude(byte pressureCode)
        {
            var alpha = mActualPressurePercents_ExpFilterAlpha;
            mActualPressurePercents = pressureCode * alpha + mActualPressurePercents * (1f - alpha);
            ChangePressure((byte)mActualPressurePercents);
        }

        byte mAppliedPressureCode;
        private void ChangePressure(byte pressure)
        {
            if (mAppliedPressureCode == pressure)
                return;

            mAppliedPressureCode = pressure;
            mMidiHelper.SetVolume(mChannelIdForRightSide, pressure);
        }

        private void ChangeActualLeftVoisesPressed(ulong leftVoicesState)
        {
            //throw new NotImplementedException();
        }

        private void ChangeActualRightKeysPressed(AccordeonRightKeys rightKeysState)
        {
            for (int i = 16; i < 64; i++)
            {
                UInt64 msk = 1UL << i;
                bool oldState = ((UInt64)mActualPightKeysState & msk) != 0;
                bool newState = ((UInt64)rightKeysState & msk) != 0;

                if (newState != oldState)
                {
                    AccordeonRightKeys note = (AccordeonRightKeys)msk;
                    OnRightKeyStateChanged(note, newState ? NoteState.On : NoteState.Off);
                }
            }
            mActualPightKeysState = rightKeysState;
        }


        private void OnRightKeyStateChanged(AccordeonRightKeys noteKey, NoteState state)
        {
            var pureNote = AccordeonRightKeyToNote(mRightHandOctaveShift, noteKey);
            var realNotes = SelectedRightRegister.GetNotes(pureNote);

            foreach (var n in realNotes)
                mMidiHelper.Note(mChannelIdForRightSide, n, state, mKeystrokeForce);
        }


        /// <summary>
        /// Возвращает актуальное давление в мехах акордеона в процентах.
        /// </summary>
        public float ActualPressurePercents
        {
            get { return mActualPressurePercents; }
        }

        public float PressudeFilter
        {
            get { return mActualPressurePercents_ExpFilterAlpha; }
            set
            {
                if (value < 0.01f)
                    value = 0.01f;

                if (value > 1f)
                    value = 1f;
                mActualPressurePercents_ExpFilterAlpha = value;
                Properties.Settings.Default.PressureFilter = mActualPressurePercents_ExpFilterAlpha;
            }
        }


        /// <summary>
        /// Возвращает состояния зажатия клавиш на правой клавиатуре аккордеона.
        /// </summary>
        public AccordeonRightKeys ActualPightKeysState
        {
            get { return mActualPightKeysState; }
        }

        /// <summary>
        /// Возвращает состояние голосов в левой части аккореона.
        /// </summary>
        public UInt64 ActualLeftVoicesState
        {
            get { return mActualLeftVoicesState; }
        }



        /// <summary>
        /// Возвращает код ноты MIDI из кода левой клавиши на аккордеоне.
        /// </summary>
        /// <param name="octaveShift">Смещение октавы.</param>
        /// <param name="key">Клавиша.</param>
        /// <returns></returns>
        public int AccordeonRightKeyToNote(int octaveShift, AccordeonRightKeys key)
        {
            int baseOctave = 3 + octaveShift; // Малая октава имеет номер 3
            int tone = -1;

            UInt64 tmp = (UInt64)key;

            //0xFFF0000000000000; // Октава 3
            //0x000FFF0000000000; // Октава 2
            //0x000000FFF0000000; // Октава 1
            //0x000000000FFF0000; // Октава малая

            tmp >>= 18; // Сдвигаем к малой октаве

            for (int o = 0; o <= 3; o++)
            {
                if (tone >= 0)
                    break;

                var tmpOctave = tmp & 0xFFF;
                if (tmpOctave != 0)
                {
                    for (var t = 0; t < 12; t++)
                    {
                        if ((tmpOctave & (1UL << t)) > 0)
                        {
                            tone = t;
                            break;
                        }

                    }
                }

                ++baseOctave;
                tmp >>= 12;
            }

            if (tone < 0)  // Какая клавиша нажата не определено
                return -1;

            return baseOctave * 12 + tone;
        }



        public RegisterInfo SelectedRightRegister
        {
            get { return mSelectedRightRegister; }
            set
            {
                mSelectedRightRegister = value;
                mMidiHelper.ResetDevice();

                Properties.Settings.Default.SelectedRightRegisterId = Array.IndexOf(mRightRegisters, value);
            }
        }

        public RegisterInfo SelectedLeftRegister
        {
            get { return mSelectedLeftRegister; }
            set
            {
                mSelectedLeftRegister = value;
                mMidiHelper.ResetDevice();

                Properties.Settings.Default.SelectedLeftRegisterId = Array.IndexOf(mLeftRegisters, value);
            }
        }

        public int ChannelIdForRightSide
        {
            get { return mChannelIdForRightSide; }
            set
            {
                if (value < 1)
                    value = 1;
                if (value > 15)
                    value = 15;
                mChannelIdForRightSide = value;
                Properties.Settings.Default.ChannelIdForRightSide = mChannelIdForRightSide;
            }
        }

        public RegisterInfo[] GetRightRegisters()
        {
            return mRightRegisters;
        }

        public RegisterInfo[] GetLeftRegisters()
        {
            return mLeftRegisters;
        }
    }


    /// <summary>
    /// Правая клавиатура аккордеона: 41 клавиша, от Фа малой октавы до Ля третьей октавы
    /// </summary>    
    [Flags]
    public enum AccordeonRightKeys : UInt64
    {
        None = 0,

        O3_F = 1UL << 23,       // ФА малой октавы (Первая нота полного аккордеона и 7/8)
        O3_F_DIEZ = 1UL << 24,  // ФА ДИЕЗ малой октавы
        O3_G = 1UL << 25,       // Соль малой октавы (первая нота у аккордеона 3/4)
        O3_G_DIEZ = 1UL << 26,
        O3_A = 1UL << 27,
        O3_A_DIEZ = 1UL << 28,
        O3_B = 1UL << 29,

        O4_C = 1UL << 30,
        O4_C_DIEZ = 1UL << 31,
        O4_D = 1UL << 32,
        O4_D_DIEZ = 1UL << 33,
        O4_E = 1UL << 34,
        O4_F = 1UL << 35,
        O4_F_DIEZ = 1UL << 36,
        O4_G = 1UL << 37,
        O4_G_DIEZ = 1UL << 38,
        O4_A1 = 1UL << 39,
        O4_A_DIEZ1 = 1UL << 40,
        O4_B1 = 1UL << 41,

        O5_C1 = 1UL << 42,
        O5_C_DIEZ1 = 1UL << 43,
        O5_D1 = 1UL << 44,
        O5_D_DIEZ1 = 1UL << 45,
        O5_E1 = 1UL << 46,
        O5_F1 = 1UL << 47,
        O5_F_DIEZ1 = 1UL << 48,
        O5_G = 1UL << 49,
        O5_G_DIEZ = 1UL << 50,
        O5_A = 1UL << 51,
        O5_A_DIEZ = 1UL << 52,
        O5_B = 1UL << 53,

        O6_C = 1UL << 54,
        O6_C_DIEZ = 1UL << 55,
        O6_D = 1UL << 56,
        O6_D_DIEZ = 1UL << 57,
        O6_E = 1UL << 58,           // Ми третьей октавы (последняя нота аккордеона 3/4)
        O6_F = 1UL << 59,
        O6_F_DIEZ = 1UL << 60,
        O6_G = 1UL << 61,
        O6_G_DIEZ = 1UL << 62,
        O6_A = 1UL << 63,           // Ля третьей октавы (последняя нота полного аккордеона)
    }





    public struct AccordeonState
    {
        public AccordeonState(int duration_ms, byte pressure, AccordeonRightKeys rightKeysState, UInt64 leftVoicesState)
        {
            PressureCode = pressure;
            RightKeysState = rightKeysState;
            LeftVoicesState = leftVoicesState;
            Duration_ms = duration_ms;
        }

        public byte PressureCode;
        public AccordeonRightKeys RightKeysState;
        public UInt64 LeftVoicesState;
        public int Duration_ms;
    }


    public enum TransformPressureMethod
    {
        DisableSensor = 0,
        Linear = 1,
        Logariphmic = 2,
    }

}
