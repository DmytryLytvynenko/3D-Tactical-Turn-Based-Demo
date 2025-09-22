using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class ExpirienceContainer : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<int, GameObject> XPPercentage;
    [SerializeField] private Vector2 XPObjectCount;

    Character Character;
    private void OnEnable()
    {
        Character = GetComponent<Character>();
        Character.CharacterDied += OnCharacterDied;
    }
    private void OnDisable()
    {
        Character.CharacterDied -= OnCharacterDied;
    }

    private void OnCharacterDied(Character character)
    {
        var keys = XPPercentage.Keys.OrderByDescending(k => k).ToList();
        List<int> percentages = new List<int>();
        percentages.Add(keys[0]);
        for (int j = 1; j <= keys.Count - 1; j++)
        {
            percentages.Add(percentages[j - 1] + keys[j]);
        }
        int count = Random.Range((int)XPObjectCount.x, (int)XPObjectCount.y);
        for (int i = 0; i < count; i++)
        {
            int randomPercentage = Random.Range(0,100);
            for (int j = 0; j < percentages.Count; j++)
            {
                if (randomPercentage <= percentages[j])
                {
                    if (j > 0)
                    {
                        randomPercentage = percentages[j] - percentages[j - 1];
                    }
                    else
                    {
                        randomPercentage = percentages[j];
                    }
                    break;
                }
            }
            XPPercentage.TryGetValue(randomPercentage, out GameObject VFX);
            Instantiate(VFX, Character.characterCenter.position, Quaternion.identity);
        }
    }
}
