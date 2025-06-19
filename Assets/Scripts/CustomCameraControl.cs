using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraControl : MonoBehaviour
{
    public Transform target;           // �� �߽���
    public float distance = 10f;       // ī�޶� �Ÿ�
    public float rotateSpeed = 3f;     // ȸ�� �ӵ�
    public float zoomSpeed = 2f;       // �� �ӵ�
    public float moveSpeed = 5f;       // �̵� �ӵ�

    private float rotationX = 0;
    private float rotationY = 0;

    void Start()
    {
        // �ʱ� ���� ����
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.x;
        rotationY = angles.y;
    }

    void Update()
    {
        HandleRotation();
        HandleZoom();
        HandleMovement();
        UpdateCameraPosition();
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(0)) // ��Ŭ�� �巡�׷� ȸ��
        {
            rotationY += Input.GetAxis("Mouse X") * rotateSpeed;
            rotationX -= Input.GetAxis("Mouse Y") * rotateSpeed;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, 2f, 50f);
    }

    void HandleMovement()
    {
        if (Input.GetMouseButton(1)) // ��Ŭ�� �巡�׷� ��
        {
            Vector3 move = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0);
            target.Translate(move * moveSpeed * Time.deltaTime, Space.Self);
        }

        // Ű����ε� �̵� (�ɼ�)
        Vector3 keyMove = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) keyMove += transform.forward;
        if (Input.GetKey(KeyCode.S)) keyMove -= transform.forward;
        if (Input.GetKey(KeyCode.A)) keyMove -= transform.right;
        if (Input.GetKey(KeyCode.D)) keyMove += transform.right;

        if (keyMove != Vector3.zero)
        {
            target.Translate(keyMove * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        transform.position = position;
        transform.rotation = rotation;
    }
}
