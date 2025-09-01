using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;

    private void Update()
    {
        transform.position = cameraTarget.position;
    }
}
