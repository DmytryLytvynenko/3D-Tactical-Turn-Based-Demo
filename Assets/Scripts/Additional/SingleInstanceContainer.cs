using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SingleInstanceContainer
{
    public static Character Player { get; set; } = null;
    public static CameraControl Camera { get; set; } = null;
}
