using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;

namespace eAccordeon.Model
{
    public class MidiDeviceInfo
    {
        int mDeviceId;
        MidiOutCapabilities mMidiOutCapabilities;

        public MidiDeviceInfo(int deviceId, MidiOutCapabilities midiOutCapabilities)
        {
            mDeviceId = deviceId;
            mMidiOutCapabilities = midiOutCapabilities;
        }
        public int DeviceId
        {
            get { return mDeviceId; }
        }

        public MidiOutCapabilities MidiOutCapabilities
        {
            get { return mMidiOutCapabilities; }
        }

        public string Caption
        {
            get { return mMidiOutCapabilities.ProductName; }
        }

        internal string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Производитель:\t{MidiOutCapabilities.Manufacturer}");
            sb.AppendLine($"Количество поддерживаемых голосов:\t{MidiOutCapabilities.Voices}");
            sb.AppendLine($"Идентификатор производителя:\t{MidiOutCapabilities.ProductId}");
            sb.AppendLine($"Полифония (нот):\t{MidiOutCapabilities.Notes}");
            sb.AppendLine($"Поддержка всех каналов:\t{MidiOutCapabilities.SupportsAllChannels}");
            sb.AppendLine($"Поточное воспроизведение:\t{MidiOutCapabilities.SupportsMidiStreamOut}");
            sb.AppendLine($"Поддержка кеширования патчей:\t{MidiOutCapabilities.SupportsPatchCaching}");
            sb.AppendLine($"Поддержка регулирования громкости:\t{MidiOutCapabilities.SupportsVolumeControl}");
            sb.AppendLine($"Технология:\t{MidiOutCapabilities.Technology}");
            sb.AppendLine($"Количество поддерживаемых голосов:\t{MidiOutCapabilities.Voices}");

            return sb.ToString();
        }
    }

}
