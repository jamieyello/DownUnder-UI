using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32.SafeHandles;
using static Microsoft.Xna.Framework.MathHelper;
using Microsoft.Xna.Framework.Input;

namespace PS4Mono
{
    internal class Ps4Controller
    {
        #region Constants

        /// <summary>
        /// Used to normalize an axis value.
        /// </summary>
        private const float AXISNORMAl = .00787401574f;

        /// <summary>
        /// Used to normalize a trigger value.
        /// </summary>
        private const float TRIGGERNORMAL = .00392156862f;

        #endregion

        #region Fields

        private byte[] _readData;
        private HidPCaps _caps;
        private FileStream _fsRead;
        private SafeFileHandle _handleRead;

        private static List<int> _openSlots = new List<int>();
        private static int _count = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Index of the controller as far as this library is concerned.
        /// </summary>
        internal int Index { get; }

        /// <summary>
        /// Gets whether or not this controller is connected. Most likely always true.
        /// </summary>
        internal bool IsConnected { get; private set; } = true;

        /// <summary>
        /// Gets the current state of the pressed buttons during this frame.
        /// </summary>
        internal Buttons CurrentFrameState { get; private set; }

        /// <summary>
        /// Gets the state of the pressed buttons during the previous frame.
        /// </summary>
        internal Buttons PreviousFrameState { get; private set; }

        /// <summary>
        /// Gets the state of the pressed buttons as of the last controller update.
        /// </summary>
        internal Buttons AsyncState { get; private set; }

        /// <summary>
        /// Left stick X axis value.
        /// </summary>
        internal float LX { get; private set; } = 0;

        /// <summary>
        /// Left stick Y axis value.
        /// </summary>
        internal float LY { get; private set; } = 0;

        /// <summary>
        /// Right stick X axis value.
        /// </summary>
        internal float RX { get; private set; } = 0;

        /// <summary>
        /// Right stick Y axis value.
        /// </summary>
        internal float RY { get; private set; } = 0;

        /// <summary>
        /// Raw left trigger value.
        /// </summary>
        internal float LeftTrigger { get; private set; } = 0;

        /// <summary>
        /// Raw right trigger value.
        /// </summary>
        internal float RightTrigger { get; private set; } = 0;

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new PS4 controller.
        /// </summary>
        /// <param name="read">The "file" that the controller sends info to.</param>
        /// <param name="info">The capabilities of the controller.</param>
        internal Ps4Controller(SafeFileHandle read, HidPCaps info)
        {
            _handleRead = read;
            _caps = info;

            _fsRead = new FileStream(read, FileAccess.ReadWrite, _caps.InputReportByteLength, false);

            // Typically you don't want to explicitly new an enum as it can cause undefined results.
            // However Monogame doesn't have a default value for Buttons, so we're forced to.
            // This will make it so the variable is equal to 0, therefore having no flags set.
            // We could also just set the variable to zero, but this way is clearer to me.

            CurrentFrameState = new Buttons();
            PreviousFrameState = new Buttons();
            AsyncState = new Buttons();

            //Start reading the file.
            ReadAsync();

            //Determine the controller index.
            if (_openSlots.Count == 0)
            {
                Index = _count++;
            }
            else
            {
                Index = _openSlots.Min();
                _openSlots.Remove(Index);
            }

            Ps4Input.OnControllerConnected(this);
        }

        /// <summary>
        /// Dispose of all dynamic resources. Trigger Disconnect event.
        /// </summary>
        private void Close()
        {
            if (IsConnected)
            {
                if (_fsRead != null)
                    _fsRead.Close();
                if (_handleRead != null && !_handleRead.IsInvalid)
                    _handleRead.Close();

                _openSlots.Add(Index);
                --_count;

                Ps4Input.OnControllerDisconnected(Index);
                IsConnected = false;
            }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"PS4 Controller: {Index}";
        }

        #endregion

        #region Internal/Private Methods

        internal void Update()
        {
            PreviousFrameState = CurrentFrameState;
            CurrentFrameState = AsyncState;
            AsyncState = new Buttons();
        }

        /// <summary>
        /// Set values based on info read from the input buffer.
        /// </summary>
        /// <param name="message">input buffer</param>
        /// <remarks>
        /// Information on what the buffer contains can be found here:
        /// http://www.psdevwiki.com/ps4/DS4-USB#Report_Structure
        /// </remarks>
        private void OnDataReceived(byte[] message)
        {
            LX = Clamp((message[1] - 128) * AXISNORMAl, -1, 1);
            LY = Clamp((message[2] - 128) * AXISNORMAl, -1, 1);
            RX = Clamp((message[3] - 128) * AXISNORMAl, -1, 1);
            RY = Clamp((message[4] - 128) * AXISNORMAl, -1, 1);

            LeftTrigger = Clamp(message[8] * TRIGGERNORMAL, 0, 1);
            RightTrigger = Clamp(message[9] * TRIGGERNORMAL, 0, 1);

            byte dpad = (byte)(message[5] & 15);
            AsyncState = new Buttons();
            AsyncState = AsyncState | ((dpad == 0 || dpad == 7 || dpad == 1) ? Buttons.DPadUp : 0);
            AsyncState = AsyncState | ((dpad == 7 || dpad == 6 || dpad == 5) ? Buttons.DPadLeft : 0);
            AsyncState = AsyncState | ((dpad == 5 || dpad == 4 || dpad == 3) ? Buttons.DPadDown : 0);
            AsyncState = AsyncState | ((dpad == 3 || dpad == 2 || dpad == 1) ? Buttons.DPadRight : 0);

            AsyncState = AsyncState | (((message[5] & 128) == 128) ? Buttons.Y : 0);
            AsyncState = AsyncState | (((message[5] & 64) == 64) ? Buttons.B : 0);
            AsyncState = AsyncState | (((message[5] & 32) == 32) ? Buttons.A : 0);
            AsyncState = AsyncState | (((message[5] & 16) == 16) ? Buttons.X : 0);

            AsyncState = AsyncState | (((message[6] & 1) == 1) ? Buttons.LeftShoulder : 0);
            AsyncState = AsyncState | (((message[6] & 2) == 2) ? Buttons.RightShoulder : 0);
            AsyncState = AsyncState | (((message[6] & 4) == 4) ? Buttons.LeftTrigger : 0);
            AsyncState = AsyncState | (((message[6] & 8) == 8) ? Buttons.RightTrigger : 0);

            AsyncState = AsyncState | (((message[6] & 16) == 16) ? Buttons.Back : 0);
            AsyncState = AsyncState | (((message[6] & 32) == 32) ? Buttons.Start : 0);
            AsyncState = AsyncState | (((message[6] & 64) == 64) ? Buttons.LeftStick : 0);
            AsyncState = AsyncState | (((message[6] & 128) == 128) ? Buttons.RightStick : 0);

            AsyncState = AsyncState | (LY <= -Ps4Input.Ps4AxisDeadZone ? Buttons.LeftThumbstickDown : 0);
            AsyncState = AsyncState | (LX <= -Ps4Input.Ps4AxisDeadZone ? Buttons.LeftThumbstickLeft : 0);
            AsyncState = AsyncState | (LY >= Ps4Input.Ps4AxisDeadZone ? Buttons.LeftThumbstickRight : 0);
            AsyncState = AsyncState | (LX >= Ps4Input.Ps4AxisDeadZone ? Buttons.LeftThumbstickUp : 0);

            AsyncState = AsyncState | (RY <= -Ps4Input.Ps4AxisDeadZone ? Buttons.RightThumbstickDown : 0);
            AsyncState = AsyncState | (RX <= -Ps4Input.Ps4AxisDeadZone ? Buttons.RightThumbstickLeft  : 0);
            AsyncState = AsyncState | (RY >= Ps4Input.Ps4AxisDeadZone ? Buttons.RightThumbstickRight : 0);
            AsyncState = AsyncState | (RX >= Ps4Input.Ps4AxisDeadZone ? Buttons.RightThumbstickUp : 0);

            AsyncState = AsyncState | (((message[7] & 1) == 1) ? Buttons.BigButton : 0);

            // There is more data present, but none that is shared with the Monogame button set.

            CurrentFrameState = CurrentFrameState | AsyncState;
        }

        private void ReadAsync()
        {
            _readData = new byte[_caps.InputReportByteLength];
            if (_fsRead.CanRead)
                _fsRead.BeginRead(_readData, 0, _readData.Length, new AsyncCallback(GetInputReportData), _readData);
            else
                throw new IOException("PS4 controller cannot read.");
        }

        private void GetInputReportData(IAsyncResult ar)
        {
            try
            {
                _fsRead.EndRead(ar);
            }
            catch (IOException)
            {
                //Controller has been disconnected.
                Close();
            }

            if (_fsRead.CanRead)
                _fsRead.BeginRead(_readData, 0, _readData.Length, new AsyncCallback(GetInputReportData), _readData);
            else if (IsConnected)
                throw new IOException("PS4 Controller cannot read when it should be able to.");

            OnDataReceived(_readData);

        }

        #endregion
    }
}