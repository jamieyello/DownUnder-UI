using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace PS4Mono
{
    internal static class NativeMethods
    {
        private const string User32 = "User32.dll";
        private const string Hid = "Hid.dll";
        private const long InvalidHandleValue = -1;

        #region Win32

        internal static Exception GetExceptionForLastWin32Error()
        {
            int errorCode = Marshal.GetLastWin32Error();
            Exception ex = Marshal.GetExceptionForHR(errorCode);
            if (ex == null)
                ex = new Win32Exception(errorCode);
            return ex;
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport(User32, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        private static extern int GetRawInputDeviceList(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), Optional] RawInputDeviceDescriptor[] rawInputDeviceList,
            [In, Out] ref int deviceCount,
            [In] int size
        );

        [SuppressUnmanagedCodeSecurity]
        [DllImport(User32, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        private static extern int GetRawInputDeviceInfoW(
            [In, Optional] IntPtr devHandle,
            [In] GetInfoCommand command,
            [In, Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 3), Optional] string data,
            [In, Out] ref int dataSize
        );

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        internal static extern SafeFileHandle CreateFileW(
            [In] string fileName,
            [In] FileAccess desiredAccess,
            [In] FileShare shareMode,
            [In, Optional] IntPtr securityAttribs,
            [In] FileMode creationDisposition,
            [In] FileAttributes flagsAndAttribs,
            [In, Optional] IntPtr templateFile
        );

        [DllImport(Hid, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool HidD_GetPreparsedData(
            [In] SafeFileHandle HidDevice,
            [In, Out] ref IntPtr preparsedData
        );

        [DllImport(Hid, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool HidD_FreePreparsedData(
            [In] ref IntPtr preparsedData
        );

        [DllImport(Hid, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        internal static extern NTStatus HidP_GetCaps(
            [In] IntPtr preparsedData,
            [In, Out] ref HidPCaps caps
        );

        [DllImport(Hid, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal unsafe static extern bool HidD_GetProductString(
            [In] SafeFileHandle hidDevice,
            [In, Out] char* buffer,
            [In] int bufferLength
        );

        #endregion

        #region Wrappers

        internal static RawInputDeviceDescriptor[] GetRawInputDeviceList()
        {
            var size = Marshal.SizeOf(typeof(RawInputDeviceDescriptor));
            var deviceCount = 0;

            var result = GetRawInputDeviceList(null, ref deviceCount, size);
            if (result == -1)
                throw new Win32Exception("Failed to retrieve raw input device count", GetExceptionForLastWin32Error());

            if (deviceCount < 0)
                throw new InvalidDataException("Invalid raw input device count: " + deviceCount.ToString(System.Globalization.CultureInfo.InvariantCulture));

            var list = new RawInputDeviceDescriptor[deviceCount];
            result = GetRawInputDeviceList(list, ref deviceCount, size);

            if (result == -1)
                throw new Win32Exception("Failed to retrieve raw input device count", GetExceptionForLastWin32Error());

            if (result == list.Length)
                return list;

            throw new InvalidDataException("Failed to retrieve raw input device count: device count mismatch.");
        }

        internal static string GetRawInputDeviceName(IntPtr devHandle)
        {
            var charCount = 0;

            var result = GetRawInputDeviceInfoW(devHandle, GetInfoCommand.DeviceName, null, ref charCount);
            if (result != 0)
            {
                if (result > 0)
                    throw new NotSupportedException("Failed to retrieve raw input device name buffer size: bad implementation.");
                throw new Win32Exception("Failed to retrieve raw input device name buffer size.", GetExceptionForLastWin32Error());
            }

            if (charCount <= 0)
                throw new NotSupportedException("Invalid raw input device name character count.");

            var buffer = new string('\0', charCount);
            result = GetRawInputDeviceInfoW(devHandle, GetInfoCommand.DeviceName, buffer, ref charCount);
            if (result == charCount)
                return buffer;

            if (result == 0)
                throw new NotSupportedException("Failed to retrieve raw input device name: bad implementation.");

            if (result == -1)
                throw new Win32Exception("Failed to retrieve raw input device name.", GetExceptionForLastWin32Error());

            throw new NotSupportedException("Failed to retrieve raw input device name: string length mismatch (bad implementation).");
        }

        internal unsafe static bool TryRegisterPS4Controller(IntPtr handle, out Ps4Controller controller)
        {
            controller = null;
            var devName = GetRawInputDeviceName(handle);
            char[] arr = new char[126];
            string product = "";
            var devHandle = CreateFileW(devName, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            fixed (char* p = arr)
            {
                if (HidD_GetProductString(devHandle, p, 126))
                {
                    product = new string(p);
                    var index = product.IndexOf('\0');
                    if (index != -1)
                        product.Remove(index);
                }
            }

            if (product == "Wireless Controller")
            {
                var preparsedData = IntPtr.Zero;
                if (!HidD_GetPreparsedData(devHandle, ref preparsedData))
                {
                    devHandle.Close();
                    return false;
                }

                HidPCaps caps = new HidPCaps();
                if (HidP_GetCaps(preparsedData, ref caps) != NTStatus.Success)
                {
                    HidD_FreePreparsedData(ref preparsedData);
                    devHandle.Close();
                    return false;
                }

                if (caps.InputReportByteLength == 64)
                {
                    HidD_FreePreparsedData(ref preparsedData);
                    controller = new Ps4Controller(devHandle, caps);
                    return true;
                }
                else
                {
                    devHandle.Close();
                    return false;
                }
            }
            devHandle.Close();
            return false;

        }

        #endregion
    }
}
