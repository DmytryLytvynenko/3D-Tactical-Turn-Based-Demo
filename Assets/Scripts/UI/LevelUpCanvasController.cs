using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LevelUpCanvasController : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private AnimationCurve curveAppear;
    [SerializeField] private AnimationCurve curveDisappear;
    [SerializeField] private float buttonAppearenceTime;
    [SerializeField] private float timeBetweenButtonAppearence;
    [SerializeField] private float dofLerpTime = 6;
    [SerializeField] private GameObject blockPanel;
    [SerializeField] private GameObject blockPanelMainCamera;
    [SerializeField] private List<LevelUpButtonUI> LevelUpButtons = new();
    [SerializeField] private LevelUPCardCollection LevelUpCards;

    private List<Task> buttonAnimTasks = new();
    private float defaultButtonscale;
    private float defaultdepthOfField;
    private DepthOfField depthOfField;
    private void Start()
    {
        volume.profile.TryGet(out DepthOfField _depthOfField);
        depthOfField = _depthOfField;
        defaultButtonscale = LevelUpButtons.First().transform.localScale.x;
        defaultdepthOfField = depthOfField.focusDistance.value;
        //AnimateButtonsAppear();
    }
    private void OnEnable()
    {
        LevelCounter.LevelIncreased += OnLevelIncreased;
        LevelUpButtonUI.UpgradeChosen += OnUpgradeChosen;
    }
    private void OnDisable()
    {
        LevelCounter.LevelIncreased -= OnLevelIncreased;
        LevelUpButtonUI.UpgradeChosen -= OnUpgradeChosen;
        LevelUpCards.DisableBehavior();
    }
    private void OnUpgradeChosen(LevelUpCard card)
    {
        AnimateButtonsDisappear();
        card.ApplyCardEffect();
        LevelUpCards.DeleteCard(card);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            AnimateButtonsAppear();
        }
    }
    public async void AnimateButtonsAppear()
    {
        int i = 0;
        blockPanel.SetActive(true);
        blockPanelMainCamera.SetActive(true);

        List<LevelUpCard> cards = LevelUpCards.GetThreeRandom();


        await Utils.FadeInDepthOfField(depthOfField, dofLerpTime);
        foreach (var button in LevelUpButtons)
        {
            button.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            button.gameObject.SetActive(true);
            button.enabled = true;
            button.UpdateButtonData(cards[i]);
            button.EnableObjectFacing();
            buttonAnimTasks.Add(AnimateButtonAppear(button.transform));
            i++;
            await Task.Delay((int)(timeBetweenButtonAppearence * 1000));
        }
        await Task.WhenAll(buttonAnimTasks.ToArray());
        blockPanel.SetActive(false);
    }
    public async void AnimateButtonsDisappear()
    {
        blockPanel.SetActive(true);
        await Task.Delay(300);
        foreach (var button in LevelUpButtons)
        {
            buttonAnimTasks.Add(AnimateButtonDisappear(button.transform));
            await Task.Delay((int)(timeBetweenButtonAppearence * 1000));
        }
        await Task.WhenAll(buttonAnimTasks.ToArray());
        foreach (var button in LevelUpButtons)
        {
            button.enabled = false;
            button.DisableObjectFacing();
            button.gameObject.SetActive(false);
        }
        Utils.FadeOutDepthOfField(depthOfField, defaultdepthOfField, dofLerpTime);
        blockPanel.SetActive(false);
        blockPanelMainCamera.SetActive(false);
    }
    private async Task AnimateButtonAppear(Transform button)
    {
        float progress = 0;
        float expiredTime = 0;
        while (progress < 1f)
        {
            expiredTime += Time.deltaTime;
            progress = expiredTime / buttonAppearenceTime;
            float newScale = curveAppear.Evaluate(progress) * defaultButtonscale;
            button.localScale = new Vector3(newScale, newScale, newScale);
            await Task.Yield();
        }
    }
    private async Task AnimateButtonDisappear(Transform button)
    {
        float progress = 0;
        float expiredTime = 0;
        while (progress < 1f)
        {
            expiredTime += Time.deltaTime;
            progress = expiredTime / buttonAppearenceTime;
            float newScale = curveDisappear.Evaluate(progress) * defaultButtonscale;
            button.localScale = new Vector3(newScale, newScale, newScale);
            await Task.Yield();
        }
    }
    private void OnLevelIncreased()
    {
        AnimateButtonsAppear();
    }
}
