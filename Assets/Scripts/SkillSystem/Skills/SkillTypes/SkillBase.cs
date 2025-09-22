using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum SkillName
{
    BladeSlashPlayer,
    AxeSlashBarberian,
    Kick,
    ArcherBowShot,
    PlayerJump,
    SheepTransform,
    None
}

[Serializable]
public class SkillBase : ScriptableObject
{
    [field: SerializeField] public SkillName skillName { get; protected set; }
    [field: SerializeField] public SkillData _SkillData { get; protected set; }
    [field: SerializeField] public SkillAgent skillAgent { protected get;  set; }
    public bool SkillEnded { get; protected set; }
    public bool OnCooldown { get { return turnCounter != 0; } }
    protected int turnCounter = 0;

    public event Action e_SkillStarted;
    public event Action e_SkillCanceled;
    public event Action e_SkillEnded;

    public event Action e_SkillSelected;
    public event Action e_SkillDeselected;

    public event Action e_InteractableOn;
    public event Action e_InteractableOff;

    public SkillBase Clone()
    {

        SkillBase clonedSkill = CreateInstance<SkillBase>();

        clonedSkill.skillName = this.skillName;
        clonedSkill._SkillData = this._SkillData;

        return clonedSkill;
    }
    public virtual void OnStart()
    {
        TurnSwitcher.TurnSwitched += OnTurnSwitched;
        turnCounter = 0;
    }
    public virtual void OnEnd()
    {
        TurnSwitcher.TurnSwitched -= OnTurnSwitched;
    }
    public virtual Task UseSkill(CancellationToken ct, SkillParameters skillParameters = null)
    {
        return Task.CompletedTask;
    }
    protected virtual void OnTurnSwitched()
    {
        turnCounter--;
        if (turnCounter <= 0)
        {
            turnCounter = 0;
            OnInteractableOn();
        }
    }
    public void DeactivateSkill()
    {
        OnSkillDeselected();
        OnSkillCanceled();
        OnInteractableOn();
        ClearAllHightlight();
    }
    public virtual void ClearAllHightlight() { }

    //event methods
    protected virtual void OnSkillStarted()
    {
        e_SkillStarted?.Invoke();
    }
    protected virtual void OnSkillCanceled()
    {
        e_SkillCanceled?.Invoke();
    }
    protected virtual void OnSkillEnded()
    {
        skillAgent.Character.characterStats.UseActionPoints(_SkillData.Cost);
        //skillAgent.Character.characterStats.ActionPoints -= _SkillData.Cost;
        skillAgent.Character.InvokeActionMade();
        e_SkillEnded?.Invoke();
    }
    protected virtual void OnSkillSelected()
    {
        e_SkillSelected?.Invoke();
    }
    protected virtual void OnSkillDeselected()
    {
        e_SkillDeselected?.Invoke();
    }
    protected virtual void OnInteractableOn()
    {
        e_InteractableOn?.Invoke();
    }
    protected virtual void OnInteractableOff()
    {
        e_InteractableOff?.Invoke();
    }

}
