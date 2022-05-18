using UnityEngine;

namespace CitiesControllerMod.Services
{
    public class CameraService
    {
        public static float OrbitSensitivity = 4f;
        public static float PosSensitivity = 25f;
        public static float ZoomSensitivity = 15f;

        public void UpdateCameraPosition(Vector2 leftAnalogStickPosition)
        {

            GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (gameObject != null)
            {
                CameraController cameraController = gameObject.GetComponent<CameraController>();
                Vector3 currentPos = cameraController.m_currentPosition;
                Vector3 targetPos = currentPos;
                float cameraAngle = cameraController.m_currentAngle.x * Mathf.PI / 180f;

                float speedModifier = cameraController.m_targetSize / PosSensitivity; // CAM POS SPEED MODIFIER
                Vector2 scrollVector = new Vector3(-leftAnalogStickPosition.x * speedModifier, leftAnalogStickPosition.y * speedModifier);
                Vector2 scrollVectorWithAngle;

                scrollVectorWithAngle.x = scrollVector.x * Mathf.Cos(cameraAngle) - scrollVector.y * Mathf.Sin(cameraAngle);
                scrollVectorWithAngle.y = scrollVector.x * Mathf.Sin(cameraAngle) + scrollVector.y * Mathf.Cos(cameraAngle);

                targetPos.x -= scrollVectorWithAngle.x;
                targetPos.z += scrollVectorWithAngle.y;

                cameraController.m_targetPosition = targetPos;
            }
        }

        public void UpdateCameraOrbit(Vector2 rightAnalogStickPosition)
        {
            GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (gameObject != null)
            {
                CameraController cameraController = gameObject.GetComponent<CameraController>();
                cameraController.m_targetAngle.x +=rightAnalogStickPosition.x * OrbitSensitivity; // ORBIT SPEED MODIFIER
                cameraController.m_targetAngle.y -= rightAnalogStickPosition.y * OrbitSensitivity;

                //clamp
                if (cameraController.m_targetAngle.y < 0)
                {
                    cameraController.m_targetAngle.y = 0;
                }

                if (cameraController.m_targetAngle.y > 90)
                {
                    cameraController.m_targetAngle.y = 90;
                }
            }
        }

        public void UpdateCameraZoom (Vector2 triggerPositions)
        {
            GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (gameObject != null)
            {
                CameraController cameraController = gameObject.GetComponent<CameraController>();
                cameraController.m_targetSize += triggerPositions.x * (cameraController.m_targetSize / ZoomSensitivity); // ZOOM SPEED MODIFIER
                cameraController.m_targetSize -= triggerPositions.y * (cameraController.m_targetSize / ZoomSensitivity);
            }
        }
    }
}
