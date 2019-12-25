using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PS4Mono
{
    public static class Ps4Input
    {
        #region Events

        /// <summary>
        /// Triggered when a new PS4 controller is connected.
        /// </summary>
        public static event EventHandler<int> Ps4Connected;

        /// <summary>
        /// Triggered when a new PS4 controller is disconnected.
        /// </summary>
        public static event EventHandler<int> Ps4Disconnected;

        #endregion

        #region Fields

        private static List<Ps4Controller> _controllers = new List<Ps4Controller>();
        private static float _deadZone = .15f;
        private static bool _isInitialized = false;

        #endregion

        #region Properties
        
        internal static List<Ps4Controller> Controllers
        {
            get { return _controllers; }
        }

        /// <summary>
        /// Returns the amount of currently connected devices
        /// </summary>
        public static int Ps4Count { get; private set; } = 0;

        /// <summary>
        /// Get or set the Ps4 controller deadzone.
        /// </summary>
        public static float Ps4AxisDeadZone
        {
            get { return _deadZone; }
            set { _deadZone = MathHelper.Clamp(value, 0, 1); }
        }

        #endregion

        #region Controller

        /// <summary>
        /// Gets the indices of all connected ps4 controllers.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<int> ConnectedPs4()
        {
            for (var i = 0; i < _controllers.Count; i++)
            {
                if (_controllers[i] != null)
                    yield return i;
            }
        }

        /// <summary>
        /// Determine if the specified controller is plugged in.
        /// </summary>
        /// <param name="index">The index to check</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool Ps4IsConnected(int index)
        {
            if (index >= _controllers.Count || index < 0)
                return false;
            return _controllers[index] != null;
        }

        /// <summary>
        /// Get the raw axis value of the specified controller.
        /// </summary>
        /// <param name="index">Controller index to check.</param>
        /// <param name="axis">Axis to check.</param>
        /// <returns></returns>
        public static float Ps4RawAxis(int index, Axis axis)
        {
            if (!Ps4IsConnected(index))
                return 0;

            switch(axis)
            {
                case Axis.LeftX:
                    return _controllers[index].LX;
                case Axis.LeftY:
                    return _controllers[index].LY;
                case Axis.RightX:
                    return _controllers[index].RX;
                case Axis.RightY:
                    return _controllers[index].RY;
                default:
                    throw new ArgumentException("Invalid axis input", "axis");
            }
        }

        /// <summary>
        /// Get the raw axis values of a stick from the specified controller.
        /// </summary>
        /// <param name="index">Ps4 controller index.</param>
        /// <param name="stick">Stick to retrieve axis values from.</param>
        /// <returns></returns>
        public static Vector2 Ps4RawAxis(int index, Buttons stick)
        {
            if (!Ps4IsConnected(index))
                return Vector2.Zero;

            switch(stick)
            {
                case Buttons.LeftStick:
                    return new Vector2(_controllers[index].LX, _controllers[index].LY);
                case Buttons.RightStick:
                    return new Vector2(_controllers[index].RX, _controllers[index].RY);
                default:
                    throw new ArgumentException($"Not a valid stick button: {stick}", "stick");
            }
        }

        /// <summary>
        /// Gets the raw trigger value of the specified controller.
        /// </summary>
        /// <param name="index">Controller index to check.</param>
        /// <param name="trigger">Trigger to check.</param>
        /// <returns></returns>
        public static float Ps4RawTrigger(int index, Buttons trigger)
        {
            if (!Ps4IsConnected(index))
                return 0;

            switch(trigger)
            {
                case Buttons.LeftTrigger:
                    return _controllers[index].LeftTrigger;
                case Buttons.RightTrigger:
                    return _controllers[index].RightTrigger;
                default:
                    throw new ArgumentException($"Not a valid axis button: {trigger}", "trigger");
            }
        }

        /// <summary>
        /// Check if the specified button is being pressed.
        /// </summary>
        /// <param name="index">Controller index to check.</param>
        /// <param name="button">The <see cref="Buttons"/> to check for.</param>
        public static bool Ps4Check(int index, Buttons button)
        {
            if (!Ps4IsConnected(index))
                return false;

            if (_controllers[index].CurrentFrameState.HasFlag(button))
                return true;

            return false;
        }

        /// <summary>
        /// Check if the specified button is being pressed at the time of the function call.
        /// </summary>
        /// <param name="index">Controller index to check.</param>
        /// <param name="button">The <see cref="Buttons"/> to check for.</param>
        public static bool Ps4CheckAsnyc(int index, Buttons button)
        {
            if (!Ps4IsConnected(index))
                return false;

            return _controllers[index].AsyncState.HasFlag(button);
        }

        /// <summary>
        /// Check if the specified button has just been pressed.
        /// </summary>
        /// <param name="index">Controller index to check.</param>
        /// <param name="button">The <see cref="Buttons"/> to check for.</param>
        public static bool Ps4CheckPressed(int index, Buttons button)
        {
            if (!Ps4IsConnected(index))
                return false;

            if (_controllers[index].CurrentFrameState.HasFlag(button) && !_controllers[index].CurrentFrameState.HasFlag(button))
                return true;

            return false;
        }

        /// <summary>
        /// Check if the specified button has just been released.
        /// </summary>
        /// <param name="index">Controller index to check.</param>
        /// <param name="button">The <see cref="Buttons"/> to check for.</param>
        public static bool Ps4CheckReleased(int index, Buttons button)
        {
            if (!Ps4IsConnected(index))
                return false;

            if (!_controllers[index].CurrentFrameState.HasFlag(button) && _controllers[index].CurrentFrameState.HasFlag(button))
                return true;

            return false;
        }

        internal static void OnControllerConnected(Ps4Controller controller)
        {
            if (controller.Index == _controllers.Count)
                _controllers.Add(controller);
            else
                _controllers[controller.Index] = controller;

            ++Ps4Count;

            Ps4Connected?.Invoke(null, controller.Index);
        }

        internal static void OnControllerDisconnected(int index)
        {
            if(Ps4IsConnected(index))
            {
                --Ps4Count;
                _controllers[index] = null;
                Ps4Disconnected?.Invoke(null, index);
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes connected controllers.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="pollInterval">How often it checks for incoming devices in milliseconds.</param>
        public static void Initialize(Game game, int pollInterval = 2000)
        {
            if (!_isInitialized)
                RawInputDeviceManager.Initialize(pollInterval);
            else
                throw new InvalidOperationException("Already initialized Ps4Input");
        }

        /// <summary>
        /// Updates connected controllers
        /// </summary>
        public static void Update()
        {
            foreach (var controller in _controllers.Where(c => c != null))
                controller.Update();
        }

        #endregion
    }
}
