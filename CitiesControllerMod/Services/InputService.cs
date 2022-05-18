using System.Collections.Generic;

namespace CitiesControllerMod.Services
{
    public class InputService
    {
        Dictionary<JoystickInputs, JoystickInputState> inputs = new Dictionary<JoystickInputs, JoystickInputState>() { };

        public InputService(){}

        public bool GetButtonClickInstant(JoystickInputs button, XInputDotNetPure.ButtonState pressed)
        {
            return ButtonClickHandler(button, pressed, false) == JoystickInputState.Pressed;
        }

        public bool GetButtonClickHoldable(JoystickInputs button, XInputDotNetPure.ButtonState pressed)
        {
            return ButtonClickHandler(button, pressed, true) == JoystickInputState.Held;
        }
        private JoystickInputState ButtonClickHandler(JoystickInputs button, XInputDotNetPure.ButtonState pressed, bool isHoldable)
        {
            JoystickInputState state;
            inputs.TryGetValue(button, out state);
            if (pressed == XInputDotNetPure.ButtonState.Pressed && state != JoystickInputState.Released)
            {
                if (state == JoystickInputState.Ready)
                {
                    if (isHoldable)
                        state = JoystickInputState.Held;
                    else
                        state = JoystickInputState.Pressed;
                }
                else if (state == JoystickInputState.Pressed)
                {
                    state = JoystickInputState.Released;
                }
            }
            else if (pressed == XInputDotNetPure.ButtonState.Released)
            {
                state = JoystickInputState.Ready;
            }
            inputs[button] = state;
            return state;
        }
    }

    public enum JoystickInputState
    {
        Ready,
        Pressed,
        Held,
        Released
    }

    public enum JoystickInputs
    {
        A,
        B,
        X,
        Y,
        LB,
        RB,
        Back,
        Start,
        LeftStickPress,
        RightStickPress,
        DpadLeft,
        DpadUp,
        DpadRight,
        DpadDown
    }
}
