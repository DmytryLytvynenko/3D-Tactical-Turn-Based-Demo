using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIButton : MonoBehaviour
{
    [field:SerializeField] public SkillName SkillName { get; set; }
    [field:SerializeField] public SkillBase Skill { get; set; }
    [field:SerializeField] public SkillAgent SkillAgent { get; set; }
    [field:SerializeField] public Button Button { get; private set; }
    [field:SerializeField] public float ButtonScaleRate { get; private set; }

    [SerializeField] private TextMeshProUGUI buttonName;
    private CancellationTokenSource scaleTaskTokenSource;
    private Vector3 defaultScale;

    public static event Action SkillButtonPressed;
    private void Start()
    {
        defaultScale = transform.localScale;
        buttonName.text = SkillName.ToString();
    }
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    public void SubscibeToEvents()
    {
        SkillAgent.transform.GetComponent<Character>().ActionMade += OnActionMade;
        SkillAgent.transform.GetComponent<Character>().StatsRestored += OnStatsRestored;
        Skill.e_SkillEnded += OnSkillEnded;
        Skill.e_SkillCanceled += OnSkillCanceled;
        Skill.e_InteractableOn += OnInteractableOn;
        Skill.e_InteractableOff += OnInteractableOff;
        Skill.e_SkillSelected += OnSkillSelected;
        Skill.e_SkillDeselected += OnSkillDeselected;
    }
    public void UnsubscribeFromEvents()
    {
        SkillAgent.Character.ActionMade -= OnActionMade;
        SkillAgent.Character.StatsRestored -= OnStatsRestored;
        Skill.e_SkillEnded -= OnSkillEnded;
        Skill.e_SkillCanceled -= OnSkillCanceled;
        Skill.e_InteractableOn -= OnInteractableOn;
        Skill.e_InteractableOff -= OnInteractableOff;
        Skill.e_SkillSelected -= OnSkillSelected;
        Skill.e_SkillDeselected -= OnSkillDeselected;
    }
    public void OnPointerEnter()
    {
        scaleTaskTokenSource?.Cancel();
        scaleTaskTokenSource?.Dispose();
        scaleTaskTokenSource = new CancellationTokenSource();
        Utils.ScaleUpObject(scaleTaskTokenSource.Token, transform, defaultScale * 1.3f, ButtonScaleRate);
    }
    public void OnPointerExit()
    {
        scaleTaskTokenSource?.Cancel();
        scaleTaskTokenSource?.Dispose();
        scaleTaskTokenSource = new CancellationTokenSource();
        Utils.ScaleDownObject(scaleTaskTokenSource.Token, transform, defaultScale, ButtonScaleRate);
    }
    protected virtual void OnSkillCanceled()
    {
        OnSkillDeselected();
        OnInteractableOn();
    }
    public async void UseSkill()
    {
        if (Player.InstancePlayer.Moving)
        {
            Debug.Log("Player is Moving");
            return;
        }
        await SkillAgent.UseSkill(SkillName);
    }
    protected virtual void OnSkillEnded()
    {
        OnSkillDeselected();
    }
    protected virtual void OnSkillSelected()
    {
        SkillButtonPressed?.Invoke();
        Button.image.color = Color.red;
    }
    protected virtual void OnSkillDeselected()
    {
        Button.image.color = Color.white;
    }
    protected virtual void OnInteractableOn()
    {
        Button.interactable = true;
    }
    protected virtual void OnInteractableOff()
    {
        Button.interactable = false;
    }

    protected virtual void OnActionMade()
    {
        CheckSkillCost();
    }
    protected virtual void OnStatsRestored()
    {
        CheckSkillCost();
    }

    private void CheckSkillCost()
    {
        if (SkillAgent.ActionPoints < Skill._SkillData.Cost)
        {
            OnInteractableOff();
        }
        else
        {
            if (Skill.OnCooldown)
            {
                return;
            }
            OnInteractableOn();
        }
    }

}
