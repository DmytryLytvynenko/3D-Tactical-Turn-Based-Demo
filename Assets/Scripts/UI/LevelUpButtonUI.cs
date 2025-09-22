using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FaceObject))]
public class LevelUpButtonUI : MonoBehaviour
{
    [field: SerializeField] public float ButtonScaleRate { get; private set; }
    private LevelUpCard LevelUpCard;
    [SerializeField] public TextMeshProUGUI buttonText;
    [SerializeField] public Image buttonImage;
    [SerializeField] public Image iconImage;
    private FaceObject faceObject;
    private CancellationTokenSource scaleTaskTokenSource;
    private Vector3 defaultScale;
    public static event Action<LevelUpCard> UpgradeChosen;

    private void Awake()
    {
        faceObject = GetComponent<FaceObject>();
        defaultScale = transform.localScale;
        gameObject.SetActive(false);
    }
    public void OnPointerEnter()
    {
        faceObject.StartFacing();
        scaleTaskTokenSource?.Cancel();
        scaleTaskTokenSource?.Dispose();
        scaleTaskTokenSource = new CancellationTokenSource();
        Utils.ScaleUpObject(scaleTaskTokenSource.Token, transform, defaultScale * 1.3f, ButtonScaleRate);
    }
    public void OnPointerExit()
    {
        faceObject.StopFacing();
        scaleTaskTokenSource?.Cancel();
        scaleTaskTokenSource?.Dispose();
        scaleTaskTokenSource = new CancellationTokenSource();
        Utils.ScaleDownObject(scaleTaskTokenSource.Token, transform, defaultScale, ButtonScaleRate);
    }
    public void ChooseUpgrade()
    {
        Debug.Log("UpgradeChosen");
        UpgradeChosen?.Invoke(LevelUpCard);
    }
    public void EnableObjectFacing()
    {
        faceObject.enabled = true;
    }
    public void DisableObjectFacing()
    {
        faceObject.enabled = false;
    }
    private void SetText(string text)
    {
        buttonText.text = text;
    }
    private void SetImage(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }
    private void SetColor(Color color)
    {
        buttonImage.color = color;
    }
    public void UpdateButtonData(LevelUpCard card)
    {
        LevelUpCard = card;
        SetImage(card.Icon);
        SetText(card.Description);
        SetColor(card.CardColor);
    }
}
