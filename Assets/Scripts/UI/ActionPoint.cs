using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionPoint : MonoBehaviour
{
    [SerializeField] private Image Filling;

    [SerializeField] private float ImageScaleRate = 3;
    private CancellationTokenSource scaleTaskTokenSource;
    [SerializeField] private Vector3 defaultMaxScale = new Vector3(1.83f, 1.83f, 1.83f);
    [SerializeField] private Vector3 defaultMinScale = new Vector3(0.01f, 0.01f, 0.01f);

    public void Activate()
    {
        if (gameObject.activeSelf)
        {
            return;
        }
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        gameObject.SetActive(false);
    }
    public async Task AnimateAppearence()
    {
        Filling.enabled = true;
        Debug.Log($"Filling.enabled = true; obj{Filling.name}");
        scaleTaskTokenSource?.Cancel();
        scaleTaskTokenSource?.Dispose();
        scaleTaskTokenSource = new CancellationTokenSource();
        await Utils.ScaleUpObject(scaleTaskTokenSource.Token, Filling.transform, defaultMaxScale, ImageScaleRate);
    }
    public async Task AnimateDisappearence()
    {
        scaleTaskTokenSource?.Cancel();
        scaleTaskTokenSource?.Dispose();
        scaleTaskTokenSource = new CancellationTokenSource();
        await Utils.ScaleDownObject(scaleTaskTokenSource.Token, Filling.transform, defaultMinScale, ImageScaleRate);
        if (scaleTaskTokenSource.IsCancellationRequested)
        {
            return;
        }
        Filling.enabled = false;
    }
    public void StopCurrentScaleTask()
    {
        scaleTaskTokenSource?.Cancel();
    } 
}
