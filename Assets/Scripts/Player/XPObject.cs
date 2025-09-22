using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class XPObject : MonoBehaviour
{
    [field: SerializeField] public int XPPoints { get; private set; }
    [SerializeField] private float launchForce = 1f;
    [SerializeField] private Vector2 startAngularForce;
    [SerializeField] private GameObject VFX;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 100f;
        rb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);
        rb.angularVelocity = new Vector3(Random.Range(startAngularForce.x, startAngularForce.y),
                                         Random.Range(startAngularForce.x, startAngularForce.y),
                                         Random.Range(startAngularForce.x, startAngularForce.y));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LevelCounter level))
        {
            level.PickupXpObject(this);
        }
        Instantiate(VFX, transform.position, Quaternion.identity); //VFX
        Destroy(gameObject);
    }
}
