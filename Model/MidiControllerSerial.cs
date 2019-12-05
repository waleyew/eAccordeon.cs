using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace eAccordeon.Model
{
    public class MidiControllerSerial : MidiControllerBase
    {
        SerialPort mPort;
        Task mPortListenerTask;
        public MidiControllerSerial(EAccordeon eAccordeon)
            : base(eAccordeon)
        {
            mPort = new SerialPort();

            mPort.ReadTimeout = 1000;

            try { mPort.BaudRate = Properties.Settings.Default.SerialportController_BaudRate; }
            catch { }

            try { mPort.PortName = Properties.Settings.Default.SerialportController_PortName; }
            catch { }

            mPortListenerTask = Task.Factory.StartNew(ListenSerial);
        }

        public override bool IsStarted => (mPort != null) && mPort.IsOpen;

        private void ListenSerial()
        {
            const int packetSize = 1 + 1 + 8 + 8 + 1 + 1;
            byte[] packetData = new byte[packetSize];

            byte pressure = 0;
            AccordeonRightKeys rightKeysState = AccordeonRightKeys.None;
            UInt64 leftVoicesState = 0;


            while (true)
            {
                // Если порт закрыт, то ждем когда откроется
                while (!mPort.IsOpen)
                {
                    Thread.Sleep(100);
                    DebugText = "Offline";
                }

                // Блокировка нужна, чтобы не возникало казусов при открытии и закрытии порта
                lock (mPort)
                {
                    try
                    {
                        var readed = 0;
                        packetData[0] = 0;
                        while (packetData[0] != 0x5B)
                            readed = mPort.Read(packetData, 0, 1);

                        while (readed < packetSize)
                            readed += mPort.Read(packetData, readed, packetSize - readed);
                    }
                    catch (TimeoutException)
                    {
                        mPort.DiscardInBuffer();
                        mPort.DiscardOutBuffer();
                        DebugText = "timeout";
                        continue;
                    }
                    catch
                    {
                        DebugText = "IO Error";
                        continue;
                    }

                    var startByte = packetData[0];
                    var endByte = packetData[packetData.Length - 1];

                    // Проверка контрольных сумм
                    if (startByte != 0x5B || endByte != 0x5E)
                    {
                        EAccordeon.MidiHelper.ResetDevice();
                        mPort.DiscardInBuffer();
                        mPort.DiscardOutBuffer();

                        DebugText = "Checksum error";
                        continue;
                    }

                    int offset = 1;
                    pressure = packetData[offset];
                    offset += sizeof(byte);

                    rightKeysState = (AccordeonRightKeys)BitConverter.ToUInt64(packetData, offset);
                    offset += sizeof(ulong);

                    leftVoicesState = BitConverter.ToUInt64(packetData, offset);
                    offset += sizeof(ulong);

                    var pressedKey = System.Text.Encoding.ASCII.GetChars(packetData, offset, 1)[0];
                    var pressedKeyCode = (int)pressedKey;

                    EAccordeon.HandleKeys(pressure, rightKeysState, leftVoicesState);

                    DebugText = $"pressure={pressure}; rightKeysState={rightKeysState}; leftVoicesState={leftVoicesState}; pressedKey={pressedKey} ({pressedKeyCode})";
                }
            }
        }

        public SerialPort Port
        {
            get { return mPort; }
        }

        public override void Start()
        {
            lock (mPort)
            {
                if (mPort.IsOpen)
                    return;

                mPort.Open();
                mPort.DiscardInBuffer();
                mPort.DiscardOutBuffer();
            }
        }

        public override void Stop()
        {
            EAccordeon.MidiHelper.ResetDevice();
            lock (mPort)
            {
                mPort.Close();
            }
        }



        public override string Caption => "COM-порт";
        public string SerialPortName
        {
            get { return mPort.PortName; }
            set
            {
                mPort.PortName = value;
                Properties.Settings.Default.SerialportController_PortName = mPort.PortName;
            }
        }

        public int SerialPortBaudRate
        {
            get { return mPort.BaudRate; }
            set
            {
                mPort.BaudRate = value;
                Properties.Settings.Default.SerialportController_BaudRate = mPort.BaudRate;
            }
        }

    }
}
