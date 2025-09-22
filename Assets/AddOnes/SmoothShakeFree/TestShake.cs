using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmoothShakeFree
{
    public class TestShake : MonoBehaviour
    {
        [SerializeField] private SmoothShake shake;
        [SerializeField] private List<SmoothShakeFreePreset> presets = new List<SmoothShakeFreePreset>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                shake.StartShake(presets[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                shake.StartShake(presets[1]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                shake.StartShake(presets[2]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                shake.StartShake(presets[3]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                shake.StartShake(presets[4]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                shake.StartShake(presets[5]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                shake.StartShake(presets[6]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                shake.StartShake(presets[7]);
            }
        }
    }
}
