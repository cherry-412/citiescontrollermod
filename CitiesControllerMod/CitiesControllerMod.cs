using ICities;
using UnityEngine;
using CitiesControllerMod.Services;
using XInputDotNetPure;
using System;

namespace CitiesControllerMod
{
    public class CitiesControllerMod : IUserMod
    {
        public string Name
        {
            get { return "Cherry's Controller Mod"; }
        }

        public string Description
        {
            get { return "(Alpha "+ GetVersion()+") - Designed for the xbox one controller and the steam deck"; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            //for testing only atm. currently these only last as long as the current session
            UIHelperBase group = helper.AddGroup("Camera sensitivity settings");
            group.AddSlider("Position (inverse)", 1, 50, 0.01f, 25f, (value) => CameraService.PosSensitivity = value);
            group.AddSlider("Orbit", 1, 5, 0.01f, 4f, (value) => CameraService.OrbitSensitivity = value);
            group.AddSlider("Zoom (inverse)", 1, 30, 0.01f, 15f, (value) => CameraService.ZoomSensitivity = value);
        }

        public string GetVersion()
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version}";
        }
    }

    public class JoystickIntegrationProcess : ThreadingExtensionBase
    {
        private UINavigationService UINavigationService = new UINavigationService();
        private CameraService CameraService = new CameraService();
        private MouseService MouseService = new MouseService();
        private InputService InputService = new InputService();

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            UINavigationService.EnsureUIElementAvailability();
            UINavigationService.UpdateToolstripStates();
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (UINavigationService.ToolstripTabIsOpen)
            {
                if (InputService.GetButtonClickInstant(JoystickInputs.LB, state.Buttons.LeftShoulder))
                    UINavigationService.ToolstripTabGroupMoveSelection(false);

                if (InputService.GetButtonClickInstant(JoystickInputs.RB, state.Buttons.RightShoulder))
                    UINavigationService.ToolstripTabGroupMoveSelection(true);

                if (InputService.GetButtonClickInstant(JoystickInputs.DpadLeft, state.DPad.Left))
                    UINavigationService.ToolstripTabMoveSelection(false);

                if (InputService.GetButtonClickInstant(JoystickInputs.DpadRight, state.DPad.Right))
                    UINavigationService.ToolstripTabMoveSelection(true);
            }
            else
            {
                if (InputService.GetButtonClickInstant(JoystickInputs.DpadLeft, state.DPad.Left))
                    UINavigationService.ToolstripMoveSelection(false);

                if (InputService.GetButtonClickInstant(JoystickInputs.DpadRight, state.DPad.Right))
                    UINavigationService.ToolstripMoveSelection(true);
            }

            if (InputService.GetButtonClickInstant(JoystickInputs.LeftStickPress, state.Buttons.LeftStick))
                UINavigationService.PressPlayButton();

            //if (InputService.GetButtonClickHoldable(JoystickInputs.LeftStickPress, state.Buttons.LeftStick))
              //  UINavigationService.ToolstripMoveSelection();
              // this won't work yet, need to find a way to choose between instant vs. holdable based on hold time

            MouseService.UpdateClickSimulationState();

            if (!MouseService.IsClickDownUpProcessRunning())
            {
                if (InputService.GetButtonClickInstant(JoystickInputs.Back, state.Buttons.Back))
                {
                    UINavigationService.PressFreecameraButton(); // should hide cursor and disable other button presses like pause
                }
                if (InputService.GetButtonClickInstant(JoystickInputs.Y, state.Buttons.Y))
                {
                    UINavigationService.PressBulldozerButton(); // also need a shortcut for ug mode
                }
                if (InputService.GetButtonClickInstant(JoystickInputs.Start, state.Buttons.Start))
                {
                    UINavigationService.PressEscButton(); // will need to use this button to unpause as well
                }

                if (UINavigationService.ToolstripTabIsOpen)
                {
                    var controlPointCount = UINavigationService.NetToolControlPointCount();
                    switch (UINavigationService.ToolstripSelectionName())
                    {
                        case "Zoning":
                        case "District":
                        case "Landscaping":
                            if (InputService.GetButtonClickHoldable(JoystickInputs.A, state.Buttons.A))
                            {
                                MouseService.SetLeftClickStageToDown(true);
                            }
                            if (controlPointCount > 0)
                            {
                                if (InputService.GetButtonClickHoldable(JoystickInputs.B, state.Buttons.B))
                                {
                                    MouseService.SetRightClickStageToDown(true);
                                }
                            }
                            else
                            {
                                if (InputService.GetButtonClickInstant(JoystickInputs.B, state.Buttons.B))
                                    UINavigationService.PressTSCloseButton();
                            }
                            break;
                        default:
                            if (InputService.GetButtonClickInstant(JoystickInputs.A, state.Buttons.A))
                            {
                                MouseService.SetLeftClickStageToDown(false);
                            }

                            if (InputService.GetButtonClickInstant(JoystickInputs.B, state.Buttons.B))
                            {
                                if (controlPointCount > 0)
                                {
                                    MouseService.SetRightClickStageToDown(true);
                                }
                                else
                                {
                                    UINavigationService.PressTSCloseButton();
                                }
                            }
                            break;
                    }
                }
                else if (UINavigationService.ToolstripIsInInspectMode)
                {
                    if (InputService.GetButtonClickInstant(JoystickInputs.A, state.Buttons.A))
                    {
                        MouseService.SetLeftClickStageToDown(true);
                    }
                    if (InputService.GetButtonClickInstant(JoystickInputs.B, state.Buttons.B))
                    {
                        //UIView.Find<UIComponent>("Close").SimulateClick(); open panel closing will go here
                    }
                }
                else
                {
                    if (InputService.GetButtonClickInstant(JoystickInputs.A, state.Buttons.A))
                        UINavigationService.ToolstripClickSelection();
                    if (InputService.GetButtonClickInstant(JoystickInputs.B, state.Buttons.B))
                        UINavigationService.GoToInspectMode();
                }
            }
            CameraService.UpdateCameraPosition(new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y));
            CameraService.UpdateCameraOrbit(new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y));
            CameraService.UpdateCameraZoom(new Vector2(state.Triggers.Left, state.Triggers.Right));
            UINavigationService.RenderToolStripTabHover();
        }
    }
}
