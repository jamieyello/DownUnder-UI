using System;
using System.Collections.Generic;
using static PS4Mono.NativeMethods;

namespace PS4Mono
{
    internal static class RawInputDeviceManager
    {
        private static HashSet<IntPtr> _ignore;

        internal static void Initialize(int pollTime)
        {
            //Ignore devices already seen/added.
            //Each device has a unique handle, hence the HashSet usage.
            _ignore = new HashSet<IntPtr>();

            //Register all connected devices.
            PollDevices();

            //If you wanted to look for new devices more often, change the interval here to your desired time.
            var poll = new System.Timers.Timer();
            poll.Elapsed += (s, e) => PollDevices();
            poll.Interval = pollTime;
            poll.AutoReset = true;
            poll.Enabled = true;
        }

        /// <summary>
        /// Detect any newly added devices.
        /// </summary>
        private static void PollDevices()
        {
            var devices = GetRawInputDeviceList();
            foreach(var device in devices)
            {
                if (_ignore.Contains(device.DeviceHandle))
                    continue;
                if(device.DeviceType == InputDeviceType.HID)
                {
                    TryRegisterPS4Controller(device.DeviceHandle, out var controller);
                }
                _ignore.Add(device.DeviceHandle);
            }
        }
    }
}
