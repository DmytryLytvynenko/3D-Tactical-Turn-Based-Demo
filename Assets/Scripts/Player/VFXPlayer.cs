using AYellowpaper.SerializedCollections;
using UnityEngine;

public class VFXPlayer : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<string, Transform> VFXPositions = new SerializedDictionary<string, Transform>();
    [SerializeField] private SerializedDictionary<string, GameObject> VFX = new SerializedDictionary<string, GameObject>();

    public void PlayVFX(string effectName)
    {
        VFXPositions.TryGetValue(effectName, out Transform VFXPos);
        VFX.TryGetValue(effectName, out GameObject vfx);
        Instantiate(vfx, VFXPos.position, Quaternion.identity);
    }
}
