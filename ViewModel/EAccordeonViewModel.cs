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

            MidiControllerBase defaultMidiController;
            mMidiControllers.Add(defaultMidiController = new MidiControllerVirtual(meAccordeon));
            mMidiControllers.Add(new MidiControllerSerial(meAccordeon));
            mSelectedMidiController = defaultMidiController;

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


            SelectedMidiDevice = MidiDevices.FirstOrDefault();


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


        public RegisterInfo[] LeftRegisterList
        {
            get { return meAccordeon.GetLeftRegisters(); }
        }

        public RegisterInfo SelectedLeftRegister
        {
            get { return meAccordeon.SelectedLeftRegister; }
            set
            {
                meAccordeon.SelectedLeftRegister = value;
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
                    meAccordeon.MidiHelper.ChangePatch(1, mSelectedPatchInfo.Id);
            }
        }

        public PatchInfo[] PatchInfoArray
        {
            get;
            set;
        } = PatchInfo.CreatePatchInfoArray();
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