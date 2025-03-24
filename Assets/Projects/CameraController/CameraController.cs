using System;
using UnityEngine;

namespace neuroears.allen.utils
{
    public class CameraController : MonoBehaviour
    {
        public float moveSpeed = 1f;         // 이동 속도 (기본값 1)
        public float zoomSpeed = 5f;         // 줌 속도
        public float rotateSpeed = 1f;       // 회전 속도 (기본값 1)
        public float minZoom = 1f;           // 최소 줌
        public float maxZoom = 50f;          // 최대 줌

        private Vector3 initialPosition;     // 초기 카메라 위치
        private Quaternion initialRotation;  // 초기 카메라 회전
        private Vector3 lastMousePosition;   // 마지막 마우스 위치

        void Start()
        {
            // 초기 위치와 회전을 저장
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }

        void Update()
        {
            HandleMouseInput();
            ResetSubscribe();
        }

        private void ResetSubscribe()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Reset();
        }

        private void HandleMouseInput()
        {
            // 마우스 우클릭 상태에서 회전
            if (Input.GetMouseButton(1)) // 우클릭
            {
                Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
                float rotationX = deltaMouse.y * rotateSpeed * Time.deltaTime;
                float rotationY = -deltaMouse.x * rotateSpeed * Time.deltaTime;

                // 카메라 회전
                transform.eulerAngles += new Vector3(rotationX, rotationY, 0);
            }

            // 마우스 클릭 상태에서 이동
            if (Input.GetMouseButton(2)) // 휠 클릭
            {
                Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
                Vector3 move = new Vector3(-deltaMouse.x, -deltaMouse.y, 0) * moveSpeed * Time.deltaTime;
                transform.Translate(move, Space.Self);
            }

            // 마우스 스크롤 줌 (z축 이동)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                Vector3 zoom = transform.forward * scroll * zoomSpeed;
                transform.position += zoom;
            }

            // 현재 마우스 위치 저장
            lastMousePosition = Input.mousePosition;
        }

        // 초기 상태로 돌아가는 함수
        public void Reset()
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }
    }

}
