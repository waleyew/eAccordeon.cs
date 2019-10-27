using eAccordeon.ViewModel;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eAccordeon.Model
{
    public class MidiHelper
    {
        MidiOut midiOut;


        public void SetVolume(int channel, int volume)
        {
            if (volume < 0)
                volume = 0;
            if (volume > 127)
                volume = 127;

            midiOut.Send(MidiMessage.ChangeControl(7, volume, channel).RawData);
        }

        public void ChangePatch(int channel, int patchId)
        {
            midiOut.Send(MidiMessage.ChangePatch(2, 1).RawData);
        }


        /// <summary>
        /// Возвращает массив информации о доступных в системе устройств воспроизведения звука MIDI.
        /// </summary>
        /// <returns></returns>
        public MidiDeviceInfo[] GetDevices()
        {
            List<MidiDeviceInfo> result = new List<MidiDeviceInfo>();
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
                result.Add(new MidiDeviceInfo(i, MidiOut.DeviceInfo(i)));

            return result.ToArray();
        }

        public void SetDevice(MidiDeviceInfo deviceInfo)
        {
            if (midiOut != null)
            {
                midiOut.Close();
                midiOut.Dispose();
            }

            midiOut = new MidiOut(deviceInfo.DeviceId);
        }

        internal void ResetDevice()
        {
            midiOut?.Reset();
        }

        internal void Note(int channel, int note, NoteState state, int volume = 127)
        {
            MidiMessage msg;
            switch (state)
            {
                case NoteState.On:
                    msg = MidiMessage.StartNote(note, volume, channel);
                    break;

                default:
                    msg = MidiMessage.StopNote(note, volume, channel);
                    break;
            }
            midiOut?.Send(msg.RawData);
        }



        public static int FrequencyToMidiNoteNumber(double frequency)
        {
            return (int)Math.Round(69 + 12 * Math.Log(frequency / 440.0, 2.0));
        }

    }




    public enum NoteState
    {
        Off,
        On,
    }
}
