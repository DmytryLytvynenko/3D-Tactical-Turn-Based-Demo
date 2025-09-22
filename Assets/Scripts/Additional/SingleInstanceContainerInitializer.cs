using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleInstanceContainerInitializer : MonoBehaviour
{
    private void Awake()
    {
        SingleInstanceContainer.Player = FindFirstObjectByType<Player>();
        SingleInstanceContainer.Camera = FindFirstObjectByType<CameraControl>();
    }
}
