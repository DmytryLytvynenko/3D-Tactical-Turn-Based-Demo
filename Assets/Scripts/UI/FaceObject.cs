using System;
using UnityEngine;

public class FaceObject : MonoBehaviour
{
    [SerializeField] private Transform lookObj;
    [SerializeField] private float lerpRate = 3f;
    private Quaternion lookRotation;
    private Quaternion defaultRotation;
    private bool look;
    private void Start()
    {
        defaultRotation = transform.rotation;
    }
    private void LateUpdate()
    {
        if (look)
        {
            lookRotation = Quaternion.LookRotation(lookObj.transform.position - transform.position);
        }
        else
        {
            lookRotation = defaultRotation;
        }
        transform.localRotation = Quaternion.Lerp(transform.localRotation, lookRotation, lerpRate * Time.deltaTime);
        Debug.DrawRay(transform.position, transform.forward * 10000f, Color.blue, .1f);
    }

    public void StartFacing()
    {
        look = true;
    }
    public void StopFacing()
    {
        look = false;
    }
}
