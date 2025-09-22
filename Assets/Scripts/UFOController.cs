using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class UFOController : MonoBehaviour
{
    [SerializeField] private List<Transform> Nodes = new List<Transform>();
    [SerializeField] private Transform UFO;
    [SerializeField] private Transform Player;
    [SerializeField] private float speedBoost; 
    [SerializeField] private float defaultSpeed = 0.1f;
    [SerializeField] private float maxTiltAngle = 25f;
    [SerializeField] private float tiltLerpRate = 3f;
    public float WayHeight = 3;
    public float Radius = 7;
    public float angleDegStep;
    private float speed = 3;
    private float boostedSpeed;
    private int startIndex = 0; 
    private float changeSpeedDistance;
    private bool changingSpeed = false;

    private Transform currentTargetNode;
    private void Start()
    {
        speed = defaultSpeed;
        boostedSpeed = speed * speedBoost;
        changeSpeedDistance = Mathf.Pow(Radius + 0.5f, 2);
        startIndex = 0;
        currentTargetNode = Nodes[startIndex];
    }
    private void LateUpdate()
    {
        transform.position = Player.position;
        if (changingSpeed)
            return;


        if ((UFO.position - Player.position).sqrMagnitude > changeSpeedDistance)
        {
            SpeedUp();
        }
        else
        {
            SpeedDown();
        }
    }
    private void Update()
    {
        UFO.position = Vector3.MoveTowards(UFO.position,
                                           currentTargetNode.position,              
                                           speed * Time.deltaTime);
        ApplyTilt();
        if ((UFO.position - currentTargetNode.position).sqrMagnitude < 0.01f)
        {
            SwitchToNextNode();
        }
    }
    private void ApplyTilt()
    {
        Vector3 moveDir = (currentTargetNode.position - UFO.position).normalized;

        // Нормализуем текущую скорость между 0 и 1
        float speed01 = Mathf.InverseLerp(0, boostedSpeed/2, speed);

        // Наклон увеличивается с ростом скорости
        float tiltAmount = speed01 * maxTiltAngle;

        float tiltX = moveDir.z * tiltAmount;
        float tiltZ = -moveDir.x * tiltAmount;

        Quaternion targetRotation = Quaternion.Euler(tiltX, 0f, tiltZ);
        UFO.rotation = Quaternion.Lerp(UFO.rotation, targetRotation, Time.deltaTime * tiltLerpRate);
    }
    private void SwitchToNextNode()
    {
        if (startIndex == Nodes.Count - 1)
        {
            startIndex = 0;
            currentTargetNode = Nodes[startIndex];
            return;
        }
        startIndex++;
        currentTargetNode = Nodes[startIndex];
    }
    private async void SpeedUp()
    {
        changingSpeed = true;
        float speedDifference = boostedSpeed - defaultSpeed;
        float expiredTime = 0;
        float progress = 0;
        float t = .75f;
        while (speed < boostedSpeed)
        {
            expiredTime += Time.deltaTime;
            progress = expiredTime / t;
            speed = defaultSpeed + speedDifference * progress;
            await Task.Yield();
        }
        speed = boostedSpeed;
        changingSpeed = false;
    }
    private async void SpeedDown()
    {
        changingSpeed = true;
        float speedDifference = boostedSpeed - defaultSpeed;
        float expiredTime = 0;
        float progress = 0;
        float t = .75f;
        while (speed > defaultSpeed)
        {
            expiredTime += Time.deltaTime;
            progress = expiredTime / t;
            speed = boostedSpeed - speedDifference * progress;
            await Task.Yield();
        }
        speed = defaultSpeed;
        changingSpeed = false;
    }
    public void PlaceDots()
    {
        float angleRadStep = angleDegStep * Mathf.Deg2Rad;
        //float angleRadStep = (2 * Mathf.PI) / (Nodes.Count / 2);
        float yStep = WayHeight/ (Nodes.Count / 2);
        for (int i = 0; i < Nodes.Count / 2; i++)
        {
            float angleRad = angleRadStep * i;
            float x = Mathf.Cos(angleRad) * Radius;
            float z = Mathf.Sin(angleRad) * Radius;
            Nodes[i].transform.localPosition = new Vector3(x,
                                                           yStep * i,
                                                           z);
        }
        for (int i = Nodes.Count / 2, j = 0; i < Nodes.Count; i++, j++) 
        {
            float angleRad = angleRadStep * i;
            float x = Mathf.Cos(angleRad) * Radius;
            float z = Mathf.Sin(angleRad) * Radius;
            Nodes[i].transform.localPosition = new Vector3(x,
                                                           WayHeight - yStep * j,
                                                           z);
        }
    }
    public void NameDots()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].name = $"Node{i + 1}";
        }
    }
    public void EnableUFO()
    {
        transform.position = Player.position;
        UFO.position = Nodes[0].position;
        UFO.gameObject.SetActive(true);
    }
}
