using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SkillAgent : MonoBehaviour
{
    public Character Character { get; set; }
    public int ActionPoints { get { return Character.characterStats.ActionPoints; } }
    [SerializeField] private List<SkillBase> skills = new List<SkillBase>();

    public event Action<SkillBase> SkillAdded;
    public event Action<SkillBase> SkillRemoved;
    public SkillBase testSkill;
    public SkillBase UsingSkill = null;
    public bool IsBusy { get { return UsingSkill != null; } }
    private CancellationTokenSource SkillTokenSource;
    private void Awake()
    {
        Character = GetComponent<Character>();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Minus))
        {
            RemoveSkill(skills.Last());
        }
        if (Input.GetKeyUp(KeyCode.Equals))
        {
            AddSkill(testSkill);
        }
    }
    public async Task UseSkill(SkillName skillName, SkillParameters parameters = null)
    {
        SkillBase skill = skills.First(x => x.skillName == skillName);
        if (!skills.Any(x => x.skillName == skillName))
        {
            Debug.LogWarning($"No such skill in the skill list, agent: {gameObject.name}");
            return;
        }

        if (UsingSkill && UsingSkill == skill)
        {
            UsingSkill.DeactivateSkill();
            SkillTokenSource.Cancel();
            Player.UsingSkill = false;
            UsingSkill = null;
        }
        else
        {
            if (Player.UsingSkill)
            {
                Debug.Log("Player is busy");
                return;
            }
            UsingSkill = skill;
            skill.skillAgent = this;

            SkillTokenSource = new CancellationTokenSource();
            try
            {
                await skill.UseSkill(SkillTokenSource.Token, parameters);
                //Character.InvokeActionMade();
            }
            catch (OperationCanceledException ex)
            {
                print("SkillTask canceled");
                print(ex);
            }
            finally
            {
                SkillTokenSource.Dispose();
            }
        }
        print(skillName);
        UsingSkill = null;
    }
    public void AddSkill(SkillBase skill)
    {
/*        if (skills.Contains(skill))
        {
            Debug.LogWarning($"There already is such skill in the skill list, agent: {gameObject.name}");
            return;
        }*/

        skills.Add(skill);
        skill.skillAgent = this;
        SkillAdded?.Invoke(skill);
    }
    public void RemoveSkill(SkillBase skill)
    {
        if (!skills.Contains(skill))
        {
            Debug.LogWarning($"There is no such skill in the skill list to remove, agent: {gameObject.name}");
            return;
        }
        skills.Remove(skill);
        SkillRemoved?.Invoke(skill);
    }
    public SkillBase GetSkill(SkillName skillName)
    {
        if (!skills.Any(x => x.skillName == skillName))
        {
            Debug.LogWarning($"No such skill in the skill list, agent: {gameObject.name}");
            return null;
        }
        SkillBase skill = null;
        foreach (SkillBase skillBase in skills)
        {
            if (skillBase.skillName == skillName)
            {
                skill = skillBase;
            }
        }
        return skill;
    }
    public IReadOnlyList<SkillBase> GetAllSkills()
    {
        return skills.AsReadOnly();
    }
    public bool ContainsSkill(SkillBase skill)
    {
        return skills.Contains(skill);
    }
    public async Task RotateToTarget(Vector3 Target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(Character.transform.position.DirectionTo(Target).Flat(), Vector3.up);
        while (Math.Round(Character.characterVisual.rotation.eulerAngles.y) != Math.Round(targetRotation.eulerAngles.y))
        {
            float t = Mathf.Clamp(Time.deltaTime * 4f, 0f, 0.99f);
            Character.characterVisual.rotation = Quaternion.Lerp(Character.characterVisual.rotation, targetRotation, t);
            Character.characterVisual.rotation = Quaternion.Euler(0, Character.characterVisual.localEulerAngles.y, 0f);

            await Task.Yield();
        }
    }
}
