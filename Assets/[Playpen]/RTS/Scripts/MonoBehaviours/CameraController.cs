using System;
using Unity.Cinemachine;
using UnityEngine;

namespace RTS
{
    
    /// <summary>
    /// Controls the camera movement, rotation, and zoom in the RTS game.
    /// Uses WASD for movement, QE for rotation, and mouse scroll for zooming.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary> Movement speed of the camera. </summary>
        [Tooltip("Movement speed of the camera.")]
        [SerializeField] private float moveSpeed = 20f;
        
        /// <summary> Rotation speed of the camera. </summary>
        [Tooltip("Rotation speed of the camera.")]
        [SerializeField] private float rotateSpeed = 100f;

        /// <summary> Zoom speed of the camera. </summary>
        [Tooltip("Zoom speed of the camera.")]
        [SerializeField] private float zoomSpeed = 10f;
        
        /// <summary> Minimum zoom level (field of view). </summary>
        [Tooltip("Minimum zoom level (field of view).")]
        [SerializeField] private float minZoom = 20f;
        
        /// <summary> Maximum zoom level (field of view). </summary>
        [Tooltip("Maximum zoom level (field of view).")]
        [SerializeField] private float maxZoom = 60f;
        
        /// <summary> Reference to the Cinemachine virtual camera. </summary>
        [Tooltip("Reference to the Cinemachine virtual camera.")]
        [SerializeField] private CinemachineCamera cinemachineCamera;

        /// <summary> Target field of view for smooth zooming. </summary>
        private float targetFOV;

        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Initializes the target field of view based on the current camera settings.
        /// </summary>
        private void Awake()
        {
            targetFOV = cinemachineCamera.Lens.FieldOfView;
        }

        
        /// <summary>
        /// Update is called once per frame.
        /// Handles camera movement, rotation, and zoom based on user input.
        /// </summary>
        private void Update()
        {
            // Initialize movement direction vector.
            Vector3 moveDir = Vector3.zero;

            // Update movement direction based on WASD input.
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

            // Compute the move direction relative to the camera's orientation.
            Transform cameraTransform = Camera.main.transform;
            moveDir = cameraTransform.forward * moveDir.z + cameraTransform.right * moveDir.x;
            moveDir.y = 0;
            moveDir.Normalize();

            // Move the camera based on the computed direction and speed.
            transform.position += moveDir * moveSpeed * Time.deltaTime;

            // Initialize rotation amount.
            float rotationAmount = 0;
            
            // Update rotation amount based on QE input.
            if (Input.GetKey(KeyCode.Q))
            {
                rotationAmount = 1f;
            }
            if (Input.GetKey(KeyCode.E))
            {
                rotationAmount = -1f;
            }
            
            // Rotate the camera around the Y axis.
            transform.eulerAngles += new Vector3(0f, rotationAmount * rotateSpeed * Time.deltaTime, 0f);

            // Update zoom based on mouse scroll input.
            if (Input.mouseScrollDelta.y > 0)
            {
                targetFOV -= zoomSpeed;
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                targetFOV += zoomSpeed;
            }
            
            // Smoothly interpolate the camera's field of view to the target FOV.
            targetFOV = Mathf.Clamp(targetFOV, minZoom, maxZoom);
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
    }
}

