using System;
using Unity.Cinemachine;
using UnityEngine;

namespace RTS
{

    
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 20f;
        
        [SerializeField]
        private float rotateSpeed = 100f;

        [SerializeField] private float zoomSpeed = 10f;
        
        [SerializeField] private float minZoom = 20f;
        [SerializeField] private float maxZoom = 60f;
        
        [SerializeField] private CinemachineCamera cinemachineCamera;

        private float targetFOV;

        private void Awake()
        {
            targetFOV = cinemachineCamera.Lens.FieldOfView;
        }

        private void Update()
        {
            Vector3 moveDir = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                moveDir.z = 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDir.z = -1f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                moveDir.x = -1f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                moveDir.x = 1f;
            }

            Transform cameraTransform = Camera.main.transform;
            moveDir = cameraTransform.forward * moveDir.z + cameraTransform.right * moveDir.x;
            moveDir.y = 0;
            moveDir.Normalize();
            
            transform.position += moveDir * moveSpeed * Time.deltaTime;

            float rotationAmount = 0;
            if (Input.GetKey(KeyCode.Q))
            {
                rotationAmount = 1f;
            }

            if (Input.GetKey(KeyCode.E))
            {
                rotationAmount = -1f;
            }
            
            transform.eulerAngles += new Vector3(0f, rotationAmount * rotateSpeed * Time.deltaTime, 0f);

            if (Input.mouseScrollDelta.y > 0)
            {
                targetFOV -= zoomSpeed;
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                targetFOV += zoomSpeed;
            }
            targetFOV = Mathf.Clamp(targetFOV, minZoom, maxZoom);
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
    }
}

