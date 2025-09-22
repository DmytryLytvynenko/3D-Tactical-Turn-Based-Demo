using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Utils
{
    public static async Task MoveObject(Transform obj, Transform destination, float lerpRate = 3)
    {
        while ((obj.position - destination.position).magnitude > 0.1f)
        {
            obj.position = Vector3.Lerp(obj.position, destination.position, Time.deltaTime * lerpRate);
            await Task.Yield();
        }
    }
    public static async Task MoveObject(Transform obj, Vector3 destination, float lerpRate = 3)
    {
        while ((obj.position - destination).magnitude > 0.1f)
        {
            obj.position = Vector3.Lerp(obj.position, destination, Time.deltaTime * lerpRate);
            await Task.Yield();
        }
    }
    public static async Task MoveObjectLocalPos(Transform localObj, Vector3 localDestination, float lerpRate = 3)
    {
        while ((localObj.localPosition - localDestination).magnitude > 0.05f)
        {
            localObj.localPosition = Vector3.Lerp(localObj.localPosition, localDestination, Time.deltaTime * lerpRate);
            await Task.Yield();
        }
    }
    public static async Task RotateObject(Transform obj, Quaternion targetRot, float lerpRate = 3)
    {
        while (Quaternion.Angle(obj.rotation, targetRot) > 1)
        {
            obj.rotation = Quaternion.Lerp(obj.rotation, targetRot, Time.deltaTime * lerpRate);
            await Task.Yield();
        }
    }
    public static async Task RotateObjectLocal(Transform obj, Quaternion targetRot, float lerpRate = 3)
    {
        while (Quaternion.Angle(obj.rotation, targetRot) > 0.5f)
        {
            obj.localRotation = Quaternion.Lerp(obj.localRotation, targetRot, Time.deltaTime * lerpRate);
            await Task.Yield();
        }
    }
    public static async Task FadeInText(TextMeshProUGUI text, float lerpRate = 3)
    {
        while (text.color.a < 1f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(text.color.a, 1.1f, Time.deltaTime * lerpRate));
            await Task.Yield();
        }
    }
    public static async Task FadeOutText(TextMeshProUGUI text, float lerpRate = 3)
    {
        while (text.color.a > 0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(text.color.a, -0.1f, Time.deltaTime * lerpRate));
            await Task.Yield();
        }
    }
    public static async Task FadeOutCanvasGroup(CanvasGroup group, float lerpRate = 3)
    {
        while (group.alpha > 0f)
        {
            group.alpha = Mathf.Lerp(group.alpha, -0.1f, Time.deltaTime * lerpRate);
            await Task.Yield();
        }
    }
    public static async Task FadeInImage(Image image, float lerpRate = 3)
    {
        while (image.color.a < 1f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, 1.1f, Time.deltaTime * lerpRate));
            await Task.Yield();
        }
    }
    public static async Task FadeOutImage(Image image, float lerpRate = 3)
    {
        while (image.color.a > 0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, -0.1f, Time.deltaTime * lerpRate));
            await Task.Yield();
        }
    }
    public static async Task ScaleUpObject(CancellationToken ct,Transform obj, Vector3 targetScale, float lerpRate = 3)
    {
        Vector3 realTargetScale = targetScale * 1.1f;
        while (obj.localScale.magnitude < targetScale.magnitude)
        {
            if (ct.IsCancellationRequested)
                return;
            obj.localScale = Vector3.Lerp(obj.localScale, realTargetScale, Time.deltaTime * lerpRate);
            await Task.Yield();
        }
    }
    public static async Task ScaleUpObject(Transform obj, Vector3 targetScale, float lerpRate = 3)
    {
        Vector3 realTargetScale = targetScale * 1.1f;
        while (obj.localScale.magnitude < targetScale.magnitude)
        {
            obj.localScale = Vector3.Lerp(obj.localScale, realTargetScale, Time.deltaTime * lerpRate);
            await Task.Yield();
        }
    }
    public static async Task ScaleDownObject(CancellationToken ct, Transform obj, Vector3 targetScale, float lerpRate = 3)
    {
        Vector3 realTargetScale = targetScale * 0.9f;
        while (obj.localScale.magnitude > targetScale.magnitude)
        {
            if (ct.IsCancellationRequested)
            {
                Debug.Log($"ScaleDownObject stopped, gameObj: {obj.name}");
                return;
            }
            obj.localScale = Vector3.Lerp(obj.localScale, realTargetScale, Time.deltaTime * lerpRate);
            await Task.Yield();
        }
    }
    public static async Task ScaleDownObject(Transform obj, Vector3 targetScale, float lerpRate = 3)
    {
        Vector3 realTargetScale = targetScale * 0.9f;
        while (obj.localScale.magnitude > targetScale.magnitude)
        {
            obj.localScale = Vector3.Lerp(obj.localScale, realTargetScale, Time.deltaTime * lerpRate);
            await Task.Yield();
        }
    }
    public static async Task FadeInDepthOfField(DepthOfField dof, float lerpRate = 3)
    {
        int i = 0;
        dof.active = true;
        while (dof.focusDistance.value > dof.focusDistance.min)
        {
            dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, -.5f, Time.deltaTime * lerpRate);
            i++;
            await Task.Yield();
        }
        Debug.Log("I after FadeInDepthOfField:" + i);
    }
    public static async Task FadeOutDepthOfField(DepthOfField dof, float focus, float lerpRate = 3)
    {
        int i = 0;
        while (dof.focusDistance.value < focus)
        {
            dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, focus + .5f, Time.deltaTime * lerpRate / 6);
            i++;
            await Task.Yield();
        }
        Debug.Log("I after FadeOutDepthOfField:" + i);
        dof.active = false;
    }
}
