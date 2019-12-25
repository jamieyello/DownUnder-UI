using System;
using System.Runtime.InteropServices;

namespace PS4Mono
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 32)]
    internal struct DeviceInfo
    {
        #region Nested Class

        [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 24)]
        private struct Info
        {
            [FieldOffset(0)]
            internal HumanInterfaceDeviceInfo HID;

            internal static readonly Info Empty;
        }

        #endregion

        #region Fields

        private int structSize;
        private InputDeviceType type;
        private Info info;

        #endregion

        #region Initialize

        private DeviceInfo(int structureSize)
        {
            structSize = structureSize;
            type = InputDeviceType.Mouse;
            info = Info.Empty;
        }

        internal int StructureSize
        {
            get { return structSize; }
        }

        public InputDeviceType DeviceType
        {
            get { return type; }
        }

        public HumanInterfaceDeviceInfo? HidInfo
        {
            get
            {
                if (type == InputDeviceType.HID)
                    return info.HID;
                return null;
            }
        }

        #endregion

        public static readonly DeviceInfo Default = new DeviceInfo(Marshal.SizeOf(typeof(DeviceInfo)));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 16)]
    internal struct HumanInterfaceDeviceInfo
    {

        /// <summary>The vendor identifier for the HID.</summary>
        internal int VendorId;
        /// <summary>The product identifier for the HID.</summary>
        internal int ProductId;
        /// <summary>The version number for the HID.</summary>
        internal int VersionNumber;
        /// <summary>The top-level collection (TLC usage page and usage) for the device.</summary>
        internal TopLevelCollectionUsage TopLevelCollection;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct RawInput
    {
        private RawInputHeader header; // 16/24 bytes (x86/x64)

        /// <summary>Gets the type of the raw input device.</summary>
        public InputDeviceType DeviceType { get { return header.DeviceType; } }


        /// <summary>Gets a handle to the device generating the raw input data.</summary>
        public IntPtr DeviceHandle { get { return header.DeviceHandle; } }


        /// <summary>The value passed in the WParam parameter of the WM_INPUT message.</summary>
        public IntPtr WParam { get { return header.WParameter; } }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct RawInputDevice
    {
        internal TopLevelCollectionUsage tlc;
        internal RawInputDeviceRegistrationOptions options;
        internal IntPtr targetWindowHandle;

        internal RawInputDevice(TopLevelCollectionUsage tlc, RawInputDeviceRegistrationOptions options, IntPtr targetWindowHandle)
        {
            this.tlc = tlc;
            this.options = options;
            this.targetWindowHandle = targetWindowHandle;
        }

        public TopLevelCollectionUsage TLC
        {
            get { return tlc; }
        }

        public RawInputDeviceRegistrationOptions RegistrationOptions
        {
            get { return options; }
        }

        public IntPtr TargetWindowHandle
        {
            get { return targetWindowHandle; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDeviceDescriptor
    {

        private IntPtr deviceHandle;
        private InputDeviceType deviceType;


        /// <summary>Gets a handle to the raw input device.</summary>
        internal IntPtr DeviceHandle
        {
            get { return deviceHandle; }
        }


        /// <summary>Gets a value indicating the device type.</summary>
        internal InputDeviceType DeviceType
        {
            get { return deviceType; }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct RawInputHeader
    {

        /// <summary>The type of raw input.</summary>
        internal InputDeviceType DeviceType;

        /// <summary>The size, in bytes, of the entire input packet of data.
        /// <para>This includes RAWINPUT plus possible extra input reports in the RAWHID variable length array.</para>
        /// </summary>
        internal int Size;

        /// <summary>A handle to the device generating the raw input data.</summary>
        internal IntPtr DeviceHandle;

        /// <summary>The value passed in the WParam parameter of the WM_INPUT message.</summary>
        internal IntPtr WParameter;



        internal RawInputHeader(InputDeviceType deviceType, int structSize, IntPtr device, IntPtr param)
        {
            DeviceType = deviceType;
            Size = structSize;
            DeviceHandle = device;
            WParameter = param;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct HidPCaps
    {
        internal HidUsage Usage;
        internal HidUsagePage UsagePage;
        internal ushort InputReportByteLength;
        internal ushort OutputReportByteLength;
        internal ushort FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        internal ushort[] Reserved;
        internal ushort NumberLinkCollectionNodes;
        internal ushort NumberInputButtonCaps;
        internal ushort NumberInputValueCaps;
        internal ushort NumberInputDataIndices;
        internal ushort NumberOutputButtonCaps;
        internal ushort NumberOutputValueCaps;
        internal ushort NumberOutputDataIndices;
        internal ushort NumberFeatureButtonCaps;
        internal ushort NumberFeatureValueCaps;
        internal ushort NumberFeatureDataIndices;
    }
}