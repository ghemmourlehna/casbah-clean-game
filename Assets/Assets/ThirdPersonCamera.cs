using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float mouseSensitivity = 2f;

    void LateUpdate() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        Vector3 angles = transform.eulerAngles;
        angles.x -= mouseY;
        angles.y += mouseX;
        angles.x = Mathf.Clamp(angles.x, -30, 60);

        transform.eulerAngles = angles;

        Vector3 pos = target.position - transform.forward * distance + Vector3.up * height;
        transform.position = pos;
    }
}