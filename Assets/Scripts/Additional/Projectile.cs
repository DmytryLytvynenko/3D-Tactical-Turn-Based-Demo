using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 Target { get; set; }
    public float FlyTime { get; set; } = 1;
    [field: SerializeField] public float YOffset { get; set; } = 0.3f;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject VFX;
    [SerializeField] private int RayCheckRate = 10;

    private Vector3 start;
    private Vector3 moveVector;
    private float expiredTime;
    private float progress;
    private QuadraticCurve quadraticCurve;
    private bool hit = false;
    private int frameCount = 0;

    public event Action ProjectileArrived;
    private void Start()
    {
        start = transform.position;
        moveVector = Target - start;
        expiredTime = 0f;
        progress = 0f;

        Vector3 control = new Vector3((start.x + Target.x) / 2,
                                       Target.y + YOffset,
                                       (start.z + Target.z) / 2);
        quadraticCurve = new QuadraticCurve(start,Target,control);
    }
    private void Update()
    {
        expiredTime += Time.deltaTime;
        progress = expiredTime / FlyTime;

        Vector3 newPos = quadraticCurve.Evaluate(progress);
        Vector3 newPos2 = quadraticCurve.Evaluate(progress + 0.15f);
        transform.LookAt(newPos2);
        frameCount++;

        if (hit)
        {
            return;
        }
        if (frameCount == RayCheckRate)
        {
            hit = RayCheck(newPos2 - transform.position);
            frameCount = 0;
        }

        transform.position = newPos;
    }
    private void Die()
    {
        Destroy(gameObject);
    }
    private bool RayCheck(Vector3 lookVector)
    {
        Debug.DrawRay(transform.position, lookVector.normalized * 1f * transform.localScale.x, Color.yellow, 0.2f);
        if (Physics.Raycast(transform.position, lookVector,out RaycastHit hitInfo, 1f * transform.localScale.x, hitMask))
        {
            print("Hit" + hitInfo.transform.name);
            ProjectileArrived?.Invoke();
            Instantiate(VFX, hitInfo.point, Quaternion.identity);
            model.SetActive(false);
            Invoke(nameof(Die), 1f);
            return true;
        } 
        return false;

    }
/*    private void OnDrawGizmos()
    {
        if (quadraticCurve == null) return;

        quadraticCurve.DrawPathGizmos();
    }*/
}
