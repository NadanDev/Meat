using UnityEngine;

public class FiredCamera : MonoBehaviour
{
    [Header("Base Orientation")]
    public Vector3 baseRotationEuler = new Vector3(0f, 90f, 0f);
    private Quaternion baseRotation;

    [Header("Limits")]
    public float maxYawAngle = 10f;
    public float maxPitchAngle = 10f;

    [Header("Sensitivity & Smooth")]
    public float sensitivity = 5f;
    public float returnSmoothTime = 0.5f;
    public float moveSmoothTime = 0.05f;

    private Vector2 currentOffset;
    private Vector2 offsetVelocity;
    private Vector2 recenterVelocity;

    void Start()
    {
        baseRotation = Quaternion.Euler(baseRotationEuler);
        transform.localRotation = baseRotation;
    }

    void Update()
    {
        float dx = Input.GetAxis("Mouse X");
        float dy = -Input.GetAxis("Mouse Y");

        Vector2 targetOffset = currentOffset;

        if (Mathf.Abs(dx) > 0.001f || Mathf.Abs(dy) > 0.001f)
        {
            targetOffset.x = Mathf.Clamp(
                currentOffset.x + dx * sensitivity,
                -maxYawAngle, maxYawAngle
            );
            targetOffset.y = Mathf.Clamp(
                currentOffset.y + dy * sensitivity,
                -maxPitchAngle, maxPitchAngle
            );

            currentOffset = Vector2.SmoothDamp(
                currentOffset, targetOffset,
                ref offsetVelocity, moveSmoothTime
            );
        }
        else
        {
            currentOffset = Vector2.SmoothDamp(
                currentOffset, Vector2.zero,
                ref recenterVelocity, returnSmoothTime
            );
        }

        Quaternion offsetRot = Quaternion.Euler(currentOffset.y, currentOffset.x, 0f);
        transform.localRotation = baseRotation * offsetRot;
    }
}
