using System;
using UnityEngine;

namespace CitiesControllerMod.Services
{
    public class CameraService
    {
        public static float OrbitSensitivity = 4f;
        public static float PosSensitivity = 25f;
        public static float ZoomSensitivity = 15f;
        public static CameraController MainCameraController;

        public void EnsureMainCameraControllerExists()
        {
            if (MainCameraController == null)
            {
                GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
                if (gameObject != null)
                {
                    MainCameraController = gameObject.GetComponent<CameraController>();
                }
            }
        }

        public void UpdateCameraPosition(Vector2 leftAnalogStickPosition)
        {
            if (MainCameraController != null)
            {
                Vector3 currentPos = MainCameraController.m_currentPosition;
                Vector3 targetPos = currentPos;
                float cameraAngle = MainCameraController.m_currentAngle.x * Mathf.PI / 180f;

                float speedModifier = MainCameraController.m_targetSize / PosSensitivity; // CAM POS SPEED MODIFIER
                Vector2 scrollVector = new Vector3(-leftAnalogStickPosition.x * speedModifier, leftAnalogStickPosition.y * speedModifier);
                Vector2 scrollVectorWithAngle;

                scrollVectorWithAngle.x = scrollVector.x * Mathf.Cos(cameraAngle) - scrollVector.y * Mathf.Sin(cameraAngle);
                scrollVectorWithAngle.y = scrollVector.x * Mathf.Sin(cameraAngle) + scrollVector.y * Mathf.Cos(cameraAngle);

                targetPos.x -= scrollVectorWithAngle.x;
                targetPos.z += scrollVectorWithAngle.y;

                MainCameraController.m_targetPosition = targetPos;
            }
        }

        public void UpdateCameraOrbit(Vector2 rightAnalogStickPosition)
        {
            if (MainCameraController != null)
            {
                MainCameraController.m_targetAngle.x += rightAnalogStickPosition.x * OrbitSensitivity; // ORBIT SPEED MODIFIER
                MainCameraController.m_targetAngle.y -= rightAnalogStickPosition.y * OrbitSensitivity;

                //clamp
                if (MainCameraController.m_targetAngle.y < 0)
                {
                    MainCameraController.m_targetAngle.y = 0;
                }

                if (MainCameraController.m_targetAngle.y > 90)
                {
                    MainCameraController.m_targetAngle.y = 90;
                }
            }
        }

        public void UpdateCameraZoom(Vector2 triggerPositions)
        {
            if (MainCameraController != null)
            {
                MainCameraController.m_targetSize += triggerPositions.x * (MainCameraController.m_targetSize / ZoomSensitivity); // ZOOM SPEED MODIFIER
                MainCameraController.m_targetSize -= triggerPositions.y * (MainCameraController.m_targetSize / ZoomSensitivity);
            }
        }

        public void ClearFreecamCursorLock()
        {
            if (MainCameraController)
            {
                if (!MainCameraController.m_freeCamera)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }
}
