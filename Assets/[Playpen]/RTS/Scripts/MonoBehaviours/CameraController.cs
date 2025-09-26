using UnityEngine;

namespace RTS
{

    
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 20f;
        
        [SerializeField]
        private float rotateSpeed = 100f;
        
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
        }
    }

    
}

