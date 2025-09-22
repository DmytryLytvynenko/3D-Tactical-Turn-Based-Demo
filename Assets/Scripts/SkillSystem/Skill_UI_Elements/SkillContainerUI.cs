using System.Collections.Generic;
using UnityEngine;

public class SkillContainerUI : MonoBehaviour
{
    [SerializeField] SkillAgent player;
    [SerializeField] private float horizontalStep = 90f;
    [SerializeField] private RectTransform firstButtonPos;
    [SerializeField] private Transform skillsSubcontainer;
    [SerializeField] private GameObject SkillBlockingPanel;
    [SerializeField] private GameObject SkillButtonPrefab;

    private List<SkillUIButton> skillButtons = new List<SkillUIButton>();
    private void Awake()
    {
        InitializeSkillsOnStart();
    }

    private void OnEnable()
    {
        player.SkillAdded += OnSkillAdded;
        player.SkillRemoved += OnSkillRemoved;
        CharacterManager.EnemiesTurnStarted += OnEnemiesTurnStarted;
        CharacterManager.EnemiesTurnEnded += OnEnemiesTurnEnded;
    }
    private void OnDisable()
    {
        player.SkillAdded -= OnSkillAdded;
        player.SkillRemoved -= OnSkillRemoved;
        CharacterManager.EnemiesTurnStarted -= OnEnemiesTurnStarted;
        CharacterManager.EnemiesTurnEnded -= OnEnemiesTurnEnded;
    }
    private void InitializeSkillsOnStart()
    {
        skillButtons.Clear();
        foreach (SkillBase skill in player.GetAllSkills())
        {
            CreateSkillUIButton(skill);
        }
        UpdateSkillPosition();
    }

    public void AddSkill(SkillBase _skill)
    {
        CreateSkillUIButton(_skill);
        UpdateSkillPosition();
    }
    public void RemoveSkill(SkillName skillName)
    {
        for (int i = 0; i < skillButtons.Count; i++)
        {
            if (skillButtons[i].SkillName == skillName)
            {
                Destroy(skillButtons[i].gameObject);
                skillButtons.RemoveAt(i);
                i--;
            }
        }
        UpdateSkillPosition();
    }
    public void UpdateSkillPosition()
    {
        Vector3 buttonPos = new Vector3(firstButtonPos.position.x + (skillButtons.Count - 1) / 2.0f * -horizontalStep, firstButtonPos.position.y, firstButtonPos.position.z);
        for (int i = 0; i < skillButtons.Count; i++)
        {
            skillButtons[i].transform.position = buttonPos;
            buttonPos.x += horizontalStep;
        }
    }
    private void CreateSkillUIButton(SkillBase skill)
    {
        SkillUIButton newSkillButton = Instantiate(SkillButtonPrefab, skillsSubcontainer).GetComponent<SkillUIButton>();
        newSkillButton.SkillName = skill.skillName;
        newSkillButton.Skill = skill;
        newSkillButton.SkillAgent = player;
        newSkillButton.SubscibeToEvents();
        skillButtons.Add(newSkillButton);
    }
    private void OnSkillAdded(SkillBase skill)
    {
        AddSkill(skill);
    }
    private void OnSkillRemoved(SkillBase skill)
    {
        RemoveSkill(skill.skillName);
    }    
    private void OnEnemiesTurnStarted()
    {
        SkillBlockingPanel.SetActive(true);
    }    
    private void OnEnemiesTurnEnded()
    {
        SkillBlockingPanel.SetActive(false);
    }
}

