using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MouseFollower : MonoBehaviour
{
    [SerializeField] private RectTransform lookTagrget;
    [SerializeField] private float lookTagrgetZDistance;
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        lookTagrget.localPosition = new Vector3(lookTagrget.localPosition.x, lookTagrget.localPosition.y, lookTagrgetZDistance);
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out pos
        );
        rectTransform.anchoredPosition = pos;
    }
}
