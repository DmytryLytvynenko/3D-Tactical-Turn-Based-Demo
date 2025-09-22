using System.Collections.Generic;
using UnityEngine;

public class ResetDataToDefault : MonoBehaviour
{
    [SerializeField] private List<CharacterData> characterDatas = new();
    [SerializeField] private List<SkillData> skillDatas = new();
    [SerializeField] private List<SkillBase> skills = new();
    [SerializeField] private LevelUPCardCollection cards;

    private void Awake()
    {
        ReferanceContainer.FindReferances();
        cards.Init();
        foreach (var data in characterDatas)
        {
            data.ResetToDefault();
        }
        foreach (var data in skillDatas)
        {
            data.ResetToDefault();
        }
        foreach (var skill in skills)
        {
            skill.OnStart();
        }
    }
    private void OnDisable()
    {
        foreach (var skill in skills)
        {
            skill.OnEnd();
        }
    }
}