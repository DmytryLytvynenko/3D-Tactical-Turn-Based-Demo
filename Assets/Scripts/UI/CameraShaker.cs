using SmoothShakeFree;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }
    [SerializeField] private SmoothShake shaker;
    [SerializeField] private SerializedDictionary<ShakeType, SmoothShakeFreePreset> presets = new();

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static void Shake(ShakeType type)
    {
        Instance.presets.TryGetValue(type, out SmoothShakeFreePreset preset);
        Instance.shaker.StartShake(preset);
    }
}
