using System;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private bool x; 
    [SerializeField] private bool y; 
    [SerializeField] private bool z; 
    [SerializeField] private bool flip; 
    private Camera mainCamera;
    private Quaternion lookRotation;
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void LateUpdate()
    {
        lookRotation = Quaternion.LookRotation(mainCamera.transform.position - transform.position);

        Quaternion newRotation = Quaternion.Euler(lookRotation.eulerAngles.x * Convert.ToInt32(x),
                                                  lookRotation.eulerAngles.y * Convert.ToInt32(y),
                                                  lookRotation.eulerAngles.z * Convert.ToInt32(z));


        transform.rotation = newRotation;

        transform.Rotate(0, 180 * Convert.ToInt32(flip), 0);
    }
}
