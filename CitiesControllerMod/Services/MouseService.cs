using CitiesControllerMod.Helpers;
using UnityEngine;

namespace CitiesControllerMod.Services
{
    public enum ClickStageEnum
    {
        Started = 0,
        Down = 1,
        Up = 2,
        Ready = 3
    }

    public class ClickEventController
    {
        public ClickStageEnum Stage { get; set; } = ClickStageEnum.Ready;
        public bool ClickRequestActive { get; set; } = false;
        public bool ReadyForClickRequest { get; set; } = false;
    }

    public class MouseService
    {
        public ClickEventController LeftClick { get; set; } = new ClickEventController();
        public ClickEventController RightClick { get; set; } = new ClickEventController();

        public void UpdateClickSimulationState()
        {
            switch (LeftClick.Stage)
            {
                case ClickStageEnum.Started:
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    LeftClick.Stage = ClickStageEnum.Down;
                    break;
                case ClickStageEnum.Down:
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                    LeftClick.Stage = ClickStageEnum.Up;
                    break;
                case ClickStageEnum.Up:
                    LeftClick.Stage = ClickStageEnum.Ready;
                    MoveMouseToScreenCenter();
                    break;
                default:
                    break;
            }

            switch (RightClick.Stage) // maybe redundant
            {
                case ClickStageEnum.Started:
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                    RightClick.Stage = ClickStageEnum.Down;
                    break;
                case ClickStageEnum.Down:
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                    RightClick.Stage = ClickStageEnum.Up;
                    break;
                case ClickStageEnum.Up:
                    RightClick.Stage = ClickStageEnum.Ready;
                    MoveMouseToScreenCenter();
                    break;
                default:
                    break;
            }

            if (!IsClickDownUpProcessRunning())
            {
                if (LeftClick.ClickRequestActive)
                {
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    LeftClick.ClickRequestActive = false;
                }
                else
                {
                    if (!LeftClick.ReadyForClickRequest)
                    {
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                        LeftClick.ReadyForClickRequest = true;
                    }

                }

                if (RightClick.ClickRequestActive)
                {
                    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                    RightClick.ClickRequestActive = false;
                }
                else
                {
                    if (!RightClick.ReadyForClickRequest)
                    {
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                        RightClick.ReadyForClickRequest = true;
                    }
                }
            }
        }

        public bool IsClickOrderGiven()
        {
            return LeftClick.ClickRequestActive || RightClick.ClickRequestActive;
        }

        public bool IsClickDownUpProcessRunning()
        {
            return LeftClick.Stage != ClickStageEnum.Ready || RightClick.Stage != ClickStageEnum.Ready;
        }

        public void BeginLeftClickDownUp()
        {
            if (!IsClickDownUpProcessRunning())
            {
                LeftClick.Stage = ClickStageEnum.Started;
            }
        }

        public void SetLeftClickStageToDown(bool centerMouseWhenDone)
        {
            if (!IsClickDownUpProcessRunning())
            {
                LeftClick.ClickRequestActive = true;
                LeftClick.ReadyForClickRequest = false;
                if (centerMouseWhenDone)
                    MoveMouseToScreenCenter();
            }
        }
        public void SetRightClickStageToDown(bool centerMouseWhenDone)
        {
            if (!IsClickDownUpProcessRunning())
            {
                RightClick.ClickRequestActive = true;
                RightClick.ReadyForClickRequest = false;
                if (centerMouseWhenDone)
                    MoveMouseToScreenCenter();
            }
        }

        public void BeginRightClickDownUp()
        {
            if (!IsClickDownUpProcessRunning())
            {
                RightClick.Stage = ClickStageEnum.Started;
            }
        }

        public void MoveMouseToScreenCenter()
        {
            if (!IsClickDownUpProcessRunning())
                MouseOperations.SetCursorPosition(Screen.width / 2, Screen.height / 2);
        }

        public void MoveMouseToScreenCenterIfZooming()
        {
            if (Input.mouseScrollDelta.x + Input.mouseScrollDelta.y != 0)
            {
                MoveMouseToScreenCenter();
            }
        }

        public void SetCursorPosition(int x, int y)
        {
            MouseOperations.SetCursorPosition(x, y);
        }
    }
}
