
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidLibrary;
using System.Threading;

namespace ScaleReader {
    class USBScale
    {
        public bool IsConnected
        {
            get
            {
                return scale == null ? false : scale.IsConnected;
            }
        }
        public decimal ScaleStatus
        {
            get
            {
                return inData.Data[1];
            }
        }
        public decimal ScaleWeightUnits
        {
            get
            {
                return inData.Data[2];
            }
        }
        private HidDevice scale;
        private HidDeviceData inData;

        public HidDevice[] GetDevices() 
        { 
           return HidDevices.Enumerate(0x0eb8, 0xf000).Cast<HidDevice>().ToArray(); 
        } 

        public bool Connect()
        {
            // Find a Scale
            HidDevice[] deviceList = GetDevices();

            if (deviceList.Length > 0)

                return Connect(deviceList[0]);

            else

                return false;
        }
        public bool Connect(HidDevice device)
        {
            scale = device;
            int waitTries = 0;
            scale.OpenDevice();

            // sometimes the scale is not ready immediatly after
            // Open() so wait till its ready
            while (!scale.IsConnected && waitTries < 10)
            {
                Thread.Sleep(50);
                waitTries++;
            }
            return scale.IsConnected;
        }
        public void Disconnect()
        {
            if (scale.IsConnected)
            {
                scale.CloseDevice();
                scale.Dispose();
            }
        }
        public void DebugScaleData()
        {
            for (int i = 0; i < inData.Data.Length; ++i)
            {
                Console.WriteLine("Byte {0}: {1}", i, inData.Data[i]);
            }
        }
        public void GetWeight(out decimal? weight, out bool? isStable)
        {
            weight = null;
            isStable = false;

            if (scale.IsConnected)
            {
                inData = scale.Read(250);
                // Byte 0 == Report ID?
                // Byte 1 == Scale Status (1 == Fault, 2 == Stable @ 0, 3 == In Motion, 4 == Stable, 5 == Under 0, 6 == Over Weight, 7 == Requires Calibration, 8 == Requires Re-Zeroing)
                // Byte 2 == Weight Unit
                // Byte 3 == Data Scaling (decimal placement)
                // Byte 4 == Weight LSB
                // Byte 5 == Weight MSB

                // FIXME: dividing by 100 probably wont work with
                // every scale, need to figure out what to do with
                // Byte 3
                weight = (Convert.ToDecimal(inData.Data[4]) + Convert.ToDecimal(inData.Data[5]) * 256) / 100;
/*
                switch (Convert.ToInt16(inData.Data[2]))
                {
                    case 3:  // Kilos
                        //weight = weight * (decimal?)0.453592;
                        Console.WriteLine("case3");
                        break;
                    case 11: // Ounces
                        //weight = weight * (decimal?)16;
                        Console.WriteLine("case11");
                        break;
                    case 12: // Pounds
                        // already in pounds, do nothing
                        Console.WriteLine("case12");
                        break;
                }
 */ 
                isStable = inData.Data[1] == 0x4;
            }
        }
    }
}

