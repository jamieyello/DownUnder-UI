using DownUnder.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace DownUnder.Utility
{
    public class Camera
    {
        public Rectangle screen_dimensions;
        private Vector2 position = new Vector2();
        private float zoom = 1f;
        private const float base_camera_control_speed = 8f;

        public Vector2 Position()
        {
            Debug.WriteLine("Screen dimensions: " + screen_dimensions.Size + " Zoom() = " + Zoom());
            return position * Zoom() - screen_dimensions.Size.ToVector2() / 2;
        }

        public float Zoom()
        {
            return (float)screen_dimensions.Size.Y / 1080 * zoom;
        }

        public float InverseZoom()
        {
            return 1f / Zoom();
        }

        public Camera(Rectangle screen_dimensions_, float update_rate_)
        {
            screen_dimensions = screen_dimensions_;
        }

        public Camera(Rectangle screen_dimensions_, float update_rate_, Vector2 position_, float zoom_)
        {
            screen_dimensions = screen_dimensions_;
            position = position_;
            zoom = zoom_;
        }

        public void Control(ref InputSystem input_system, int player_index, float update_rate)
        {
            float boost = (input_system.GetAction(player_index, ActionType.menu_select_speed_modifier).analog + 1) * 2;
            float modifier = InverseZoom() * base_camera_control_speed * update_rate * boost;

            position.X += input_system.GetAction(player_index, ActionType.menu_select_right).analog * modifier;
            position.X -= input_system.GetAction(player_index, ActionType.menu_select_left).analog * modifier;
            position.Y -= input_system.GetAction(player_index, ActionType.menu_select_up).analog * modifier;
            position.Y += input_system.GetAction(player_index, ActionType.menu_select_down).analog * modifier;

            zoom -= input_system.GetAction(player_index, ActionType.camera_zoom_out).analog * 0.007f;
            zoom += input_system.GetAction(player_index, ActionType.camera_zoom_in).analog * 0.007f;
            if (zoom < 0.08f) zoom = 0.08f;
        }
    }
}