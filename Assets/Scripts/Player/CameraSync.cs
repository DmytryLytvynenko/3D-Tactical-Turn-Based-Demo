using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSync : MonoBehaviour
{
    [SerializeField] private Camera childCam;
    Camera mainCam;
    private void Start()
    {
        mainCam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        childCam.fieldOfView = mainCam.fieldOfView;
    }
}
