using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float positionLerpRate;
    [SerializeField] private float rotationLerpRate;
    [SerializeField] private Vector2 minMaxFieldOfView;
    [SerializeField] private Transform anchor;
    private Transform defaultAnchor;
    [SerializeField] Camera cam;

    private float prevMagnitude = 0;
    private int touchCount = 0;
    private InputActions inputActions;
    private Quaternion targetRotation;
    private Quaternion targetCameraRotation;
    private Vector3 targetCameraPosition;
    private float targetCameraFOV;
    private float rotateDelta;

    private void Awake()
    {
        targetRotation = transform.rotation;
        inputActions = new InputActions();

        defaultAnchor = anchor;
    }

    private void OnEnable()
    {
        BindInputs();
    }
    private void OnDisable()
    {
        inputActions.Player.touch0contact.Disable();
        inputActions.Player.touch1contact.Disable();
        inputActions.Player.touch0pos.Disable();
        inputActions.Player.touch1pos.Disable();
        inputActions.Player.Drag.Disable();
        inputActions.Player.MMBHold.Disable();
        inputActions.Player.RotateCamera.Disable();
    }
    private void Start()
    {
        targetCameraFOV = 60;
        CameraZoom(0);
    }
    private void LateUpdate()
    {
        CameraRotateDesktop(rotateDelta);
        UpdateAnchorPosition();
        UpdateAnchorRotation();

        UpdateCameraPosition();
        UpdateCameraRotation();

        UpdateCameraFOV();
    }
    public void Teleport(Vector3 teleportPosition) 
    {
        cam.transform.position = teleportPosition;
    }
    public void SetAnchor(Transform newAnchor)
    {
        anchor = newAnchor;
    }
    public void SetDefalutAnchor()
    {
        anchor = defaultAnchor;
    }
    private void UpdateAnchorPosition()
    {
        if (anchor == null)
        {
            return;
        }

        Vector3 targetPosition = new Vector3(
            anchor.position.x,
            anchor.position.y,
            anchor.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionLerpRate);
    }
    private void UpdateCameraPosition()
    {
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, targetCameraPosition, Time.deltaTime * positionLerpRate);
    }
    private void UpdateAnchorRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpRate);
    }
    private void UpdateCameraRotation()
    {
        float t = Mathf.Clamp(Time.deltaTime * rotationLerpRate, 0f, 0.99f);
        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, targetCameraRotation, t);
        //cam.transform.localRotation = Quaternion.Euler(cam.transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
    }
    private void UpdateCameraFOV()
    {
        float t = Mathf.Clamp(Time.deltaTime * positionLerpRate, 0f, 0.99f);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetCameraFOV, t);
    }
    private void BindInputs()
    {
        if (SystemInfo.deviceType.ToString() == "Desktop")
        {
            inputActions.Player.Drag.Enable();
            inputActions.Player.MMBHold.Enable();
            inputActions.Player.ZoomWheel.Enable();
            inputActions.Player.RotateCamera.Enable();
            /*  inputActions.Player.MMBHold.started += _ =>
            {
                rotButtonHold = true;
            };
            inputActions.Player.MMBHold.canceled += _ =>
            {
                rotButtonHold = false;
            };
            inputActions.Player.Drag.performed += _ =>
            {
                if (rotButtonHold)
                    CameraRotateDesktop(inputActions.Player.Drag.ReadValue<Vector2>());
            };*/
            inputActions.Player.RotateCamera.performed += _ =>
            {
                rotateDelta = inputActions.Player.RotateCamera.ReadValue<float>();
            };
            inputActions.Player.RotateCamera.started += _ =>
            {
                rotateDelta = inputActions.Player.RotateCamera.ReadValue<float>();
            };
            inputActions.Player.RotateCamera.canceled += _ =>
            {
                rotateDelta = 0;
            };
            inputActions.Player.ZoomWheel.performed += _ =>
            {
                float delta = inputActions.Player.ZoomWheel.ReadValue<float>();
                CameraZoom(-delta * zoomSpeed);
            };
        }
        else
        {
            inputActions.Player.Drag.Enable();
            inputActions.Player.touch0contact.Enable();
            inputActions.Player.touch1contact.Enable();

            inputActions.Player.touch0pos.Enable();
            inputActions.Player.touch1pos.Enable();

            inputActions.Player.touch0contact.performed += _ => touchCount++;
            inputActions.Player.touch1contact.performed += _ => touchCount++;
            inputActions.Player.touch0contact.canceled += _ =>
            {
                touchCount--;
                prevMagnitude = 0;
            };
            inputActions.Player.touch1contact.canceled += _ =>
            {
                touchCount--;
                prevMagnitude = 0;
            };

            inputActions.Player.touch1pos.performed += _ =>
            {
                if (touchCount < 2)
                    return;
                var magnitude = (inputActions.Player.touch0pos.ReadValue<Vector2>() - inputActions.Player.touch1pos.ReadValue<Vector2>()).magnitude;
                if (prevMagnitude == 0)
                    prevMagnitude = magnitude;
                var difference = magnitude - prevMagnitude;
                prevMagnitude = magnitude;
                CameraZoom(-difference * zoomSpeed);
            };

            inputActions.Player.Drag.performed += _ => CameraRotateMobile(inputActions.Player.Drag.ReadValue<Vector2>());
        }
    }
    private void CameraRotateMobile(Vector2 delta)
    {
        delta.x = Mathf.Clamp(delta.x, -100, 100);
        if (touchCount > 1) return;

        //Debug.Log(touchCount);
        if (Mathf.Abs(delta.x) < 25)
            return;
        targetRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + delta.x * rotationSpeed, 0);
    }
    private void CameraRotateDesktop(Vector2 delta)
    {
        targetRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + delta.x * rotationSpeed, 0);
    }
    private void CameraRotateDesktop(float delta)
    {
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y + delta * rotationSpeed, targetRotation.eulerAngles.z);
    }
    private void CameraZoom(float increment)
    {
        targetCameraFOV = Mathf.Clamp(targetCameraFOV + increment, minMaxFieldOfView.x, minMaxFieldOfView.y);
        float yPos = 17f / 700f * targetCameraFOV - 1f/14f;
        float zPos = 1f / 140f * targetCameraFOV - 31f/14f;
        float xRot = 3f / 7f * targetCameraFOV + 15f/7f;

        targetCameraPosition = new Vector3(cam.transform.localPosition.x, yPos, zPos);
        //cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, yPos, zPos);
        targetCameraRotation = Quaternion.Euler(xRot, cam.transform.localRotation.y, cam.transform.localRotation.z);
        //cam.transform.localRotation = Quaternion.Euler(xRot, cam.transform.localRotation.y, cam.transform.localRotation.z);
    }
}