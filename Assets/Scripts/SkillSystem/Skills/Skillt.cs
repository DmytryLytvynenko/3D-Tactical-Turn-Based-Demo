using UnityEngine;
using UnityEngine.UI;

public class Skillt : MonoBehaviour
{
    [field: SerializeField] public SkillName skillName { get; private set; }
    [field: SerializeField] public SkillData skillData { get; private set; }
    public bool OnCooldown { get { return turnCounter != 0; } }

    [SerializeField] private Button button;
    public  int turnCounter { private get; set; } = 0;

    private Coroutine useSkillRoutine;

    private void OnEnable()
    {
        TurnSwitcher.TurnSwitched += OnTurnSwitched;
    }
    private void OnDisable()
    {
        TurnSwitcher.TurnSwitched -= OnTurnSwitched;
    }

    public void UseSkill()
    {
        if (Player.UsingSkill)
        {
            DeactivateSkill();
            turnCounter = 0;
            if (useSkillRoutine != null) StopCoroutine(useSkillRoutine);
            useSkillRoutine = null;
            Player.UsingSkill = false;
            return;
        }
        if (turnCounter != 0) return;

        turnCounter = skillData.Cooldown;
        if (useSkillRoutine != null) StopCoroutine(useSkillRoutine);
        useSkillRoutine = null;
    }
    private void OnTurnSwitched()
    {
        turnCounter--;
        if (turnCounter <= 0)
        {
            turnCounter = 0;
            InteractableOn();
        }
    }
    public void InteractableOn()
    {
        button.interactable = true;
    }
    public void InteractableOff()
    {
        button.interactable = false;
    }
    public void ActivateSkill()
    {
        button.gameObject.GetComponent<Image>().color = Color.red;
    }
    public void DeactivateSkill()
    {
        button.gameObject.GetComponent<Image>().color = Color.white;
    }
}
