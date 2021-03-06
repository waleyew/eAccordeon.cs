﻿using eAccordeon.Model;
using eAccordeon.Tools;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace eAccordeon.ViewModel
{
    public class EAccordeonViewModel : ViewModelBase
    {
        EAccordeon meAccordeon;
        MidiDeviceInfo mSelectedMidiDevice;
        DispatcherTimer mTimer;
        string mDebugText;

        MidiControllerBase mSelectedMidiController;
        ObservableCollection<MidiControllerBase> mMidiControllers = new ObservableCollection<MidiControllerBase>();




        public EAccordeonViewModel()
        {
            meAccordeon = new EAccordeon();

            try
            {
                var devId = Properties.Settings.Default.SelectedMidiDeviceId;
                SelectedMidiDevice = MidiDevices[devId];
            }
            catch { };

            try
            {
                var patchId = Properties.Settings.Default.SelectedPatchId;
                mSelectedPatchInfo = PatchInfoArray[patchId];
            }
            catch { };

            try
            {
                var patchId = Properties.Settings.Default.SelectedPatchBassId;
                mSelectedPatchBassInfo = PatchBassInfoArray[patchId];
            }
            catch { };

            MidiControllerBase defaultMidiController;
            mMidiControllers.Add(defaultMidiController = new MidiControllerVirtual(meAccordeon));
            mMidiControllers.Add(new MidiControllerSerial(meAccordeon));

            try
            {
                var cId = Properties.Settings.Default.SelectedMidiControllerId;
                mSelectedMidiController = mMidiControllers[cId];
            }
            catch { mSelectedMidiController = defaultMidiController; }


            ShowMIDIDeviceInfoCommand = new RelayCommand(o =>
            {
                MessageBox.Show(SelectedMidiDevice.GetDescription());
            },
            o => { return SelectedMidiDevice != null; })
            { HadleExceptions = true };


            MidiControllerStartCommand = new RelayCommand(o =>
            {
                SelectedMidiController.Start();
            },
            o =>
            {
                return !SelectedMidiController.IsStarted;
            })
            { HadleExceptions = true };

            MidiControllerStopCommand = new RelayCommand(o =>
            {
                SelectedMidiController.Stop();
            },
            o =>
            {
                return SelectedMidiController.IsStarted;
            })
            { HadleExceptions = true };


            mTimer = new DispatcherTimer(DispatcherPriority.Background);
            mTimer.Interval = TimeSpan.FromMilliseconds(100);
            mTimer.Tick += MTimer_Tick;
            mTimer.Start();
        }

        private void MTimer_Tick(object sender, EventArgs e)
        {
            DebugText = SelectedMidiController.DebugText;
        }

        public ObservableCollection<MidiControllerBase> MidiControllers
        {
            get { return mMidiControllers; }
        }
        public MidiControllerBase SelectedMidiController
        {
            get { return mSelectedMidiController; }
            set
            {
                if (mSelectedMidiController != null)
                    mSelectedMidiController.Stop();

                mSelectedMidiController = value;
                OnPropertyChanged();

                Properties.Settings.Default.SelectedMidiControllerId = MidiControllers.IndexOf(mSelectedMidiController);
            }
        }

        public bool MidiControllerIsReadonly
        {
            get
            {
                if (mSelectedMidiController == null)
                    return false;

                return mSelectedMidiController.IsStarted;
            }
        }

        public string DebugText
        {
            get { return mDebugText; }
            private set
            {
                if (mDebugText == value)
                    return;

                mDebugText = value;
                OnPropertyChanged();
            }
        }

        public MidiDeviceInfo[] MidiDevices
        {
            get { return meAccordeon.MidiHelper.GetDevices(); }
        }

        public RelayCommand ShowMIDIDeviceInfoCommand
        {
            get;
            set;
        }

        public MidiDeviceInfo SelectedMidiDevice
        {
            get { return mSelectedMidiDevice; }
            set
            {
                meAccordeon.MidiHelper.SetDevice(value);
                mSelectedMidiDevice = value;

                try { Properties.Settings.Default.SelectedMidiDeviceId = value.DeviceId; }
                catch { Properties.Settings.Default.SelectedMidiDeviceId = 0; }

                OnPropertyChanged();
            }
        }



        public RelayCommand MidiControllerStartCommand
        {
            get;
            set;
        }

        public RelayCommand MidiControllerStopCommand
        {
            get;
            set;
        }



        public RegisterInfo[] RightRegisterList
        {
            get { return meAccordeon.GetRightRegisters(); }
        }

        public RegisterInfo SelectedRightRegister
        {
            get { return meAccordeon.SelectedRightRegister; }
            set
            {
                meAccordeon.SelectedRightRegister = value;
                OnPropertyChanged();
            }
        }


        public BassRegister[] LeftRegisterList
        {
            get { return meAccordeon.BassRegisters; }
        }

        public BassRegister SelectedLeftRegister
        {
            get { return meAccordeon.SelectedBassRegister; }
            set
            {
                meAccordeon.SelectedBassRegister = value;
                OnPropertyChanged();
            }
        }


        PatchInfo mSelectedPatchInfo;
        public PatchInfo SelectedPatchInfo
        {
            get { return mSelectedPatchInfo; }
            set
            {
                mSelectedPatchInfo = value;
                if (mSelectedPatchInfo != null)
                {
                    meAccordeon.MidiHelper.ChangePatch(meAccordeon.ChannelIdForRightSide, mSelectedPatchInfo.Id);
                    Properties.Settings.Default.SelectedPatchId = Array.IndexOf(PatchInfoArray, mSelectedPatchInfo);
                }
                else
                    Properties.Settings.Default.SelectedPatchId = 0;
            }
        }

        public PatchInfo[] PatchInfoArray
        {
            get;
            set;
        } = PatchInfo.CreatePatchInfoArray();



        PatchInfo mSelectedPatchBassInfo;
        public PatchInfo SelectedPatchBassInfo
        {
            get { return mSelectedPatchBassInfo; }
            set
            {
                mSelectedPatchBassInfo = value;
                if (mSelectedPatchBassInfo != null)
                {
                    meAccordeon.MidiHelper.ChangePatch(meAccordeon.ChannelIdForLeftSide, mSelectedPatchBassInfo.Id);
                    Properties.Settings.Default.SelectedPatchBassId = Array.IndexOf(PatchBassInfoArray, mSelectedPatchBassInfo);
                }
                else
                    Properties.Settings.Default.SelectedPatchBassId = 0;
            }
        }

        public PatchInfo[] PatchBassInfoArray
        {
            get;
            set;
        } = PatchInfo.CreatePatchInfoArray();


        public float PressudeFilterP
        {
            get { return (1f - meAccordeon.PressudeFilter) * 100f; }
            set
            {
                meAccordeon.PressudeFilter = 1f - value / 100f;
                OnPropertyChanged();
            }
        }

        public Array TransformPressureMethods
        {
            get { return Enum.GetValues(typeof(TransformPressureMethod)); }
        }

        public TransformPressureMethod TransformPressureMethod
        {
            get { return meAccordeon.TransformPressureMethod; }
            set
            {
                meAccordeon.TransformPressureMethod = value;
                OnPropertyChanged();
            }
        }

        public ushort PressureSensonUpperLimit
        {
            get { return meAccordeon.PressureSensonUpperLimit; }
            set
            {
                meAccordeon.PressureSensonUpperLimit = value;
                OnPropertyChanged();
            }
        }

        public int KeystrokeForce
        {
            get { return meAccordeon.KeystrokeForce; }
            set
            {
                meAccordeon.KeystrokeForce = value;
                OnPropertyChanged();
            }
        }

        public int RightHandOctaveShift
        {
            get { return meAccordeon.RightHandOctaveShift; }
            set
            {
                meAccordeon.RightHandOctaveShift = value;
                OnPropertyChanged();
            }
        }






        public int ChannelIdForRightSide
        {
            get { return meAccordeon.ChannelIdForRightSide; }
            set { meAccordeon.ChannelIdForRightSide = value; }
        }

        public int ChannelIdForLeftSide
        {
            get { return meAccordeon.ChannelIdForLeftSide; }
            set { meAccordeon.ChannelIdForLeftSide = value; }
        }
    }

    public class PatchInfo
    {
        PatchInfo(int id)
        {
            Id = id;
            Caption = PatchChangeEvent.GetPatchName(id);
        }

        public static PatchInfo[] CreatePatchInfoArray()
        {
            var result = new PatchInfo[128];
            for (int i = 0; i < 128; i++)
                result[i] = new PatchInfo(i);
            return result;
        }
        public string Caption { get; private set; }
        public int Id { get; private set; }
    }
}
