using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TagsTargets : MonoBehaviour
{
    [SerializeField] private List<CharacterTags> tags = new List<CharacterTags>();
    [SerializeField] private List<CharacterTags> targets = new List<CharacterTags>();
    public IReadOnlyList<CharacterTags> Targets => targets;
    public IReadOnlyList<CharacterTags> Tags => tags;

    public bool TargetsContains(List<CharacterTags> tags)
    {
        return targets.Any(item => tags.Contains(item));
    }
    public bool TargetsContains(IReadOnlyList<CharacterTags> tags)
    {
        return targets.Any(item => tags.Contains(item));
    }
    public bool TagsContains(CharacterTags target)
    {
        return tags.Contains(target);
    }
    public List<Character> SortByTargetPriorityAndDistance(Vector3 selfPosition ,List<Character> visibleCharacters)
    {
        return visibleCharacters
            .OrderBy(character =>
            {
                int priority = character.tagsTargets.Tags
                    .Select(tag => targets.IndexOf(tag))
                    .Where(index => index != -1)
                    .DefaultIfEmpty(int.MaxValue)
                    .Min();

                return priority;
            })
            .ThenBy(character =>
            {
                // Считаем расстояние только если персонаж имеет тег из targets
                int priority = character.tagsTargets.Tags
                    .Select(tag => targets.IndexOf(tag))
                    .Where(index => index != -1)
                    .DefaultIfEmpty(int.MaxValue)
                    .Min();

                return priority == int.MaxValue
                    ? float.MaxValue // если тегов нет — не сортируем по расстоянию
                    : Vector3.Distance(selfPosition, character.transform.position);
            })
            .ToList();
    }

}

public enum CharacterTags
{
    Sheep,
    Archer,
    Player,
    Bandit,
    Animal,
    Human,
    All,
    None
}
