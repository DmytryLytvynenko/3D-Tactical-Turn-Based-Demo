using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public int ActiveCharacterAmount { get { return active.Count; } }
    [SerializeField] private Button switchTurnButton;

    [SerializeField] private CameraControl cameraControl;
    private List<Character> all = new List<Character>();
    private List<Character> active = new List<Character>();

/*    public List<Character> AllCharacters => all.AsReadOnly().ToList();
    public List<Character> AllAwaken => active.AsReadOnly().ToList();*/
    public static event Action EnemiesTurnStarted;
    public static event Action EnemiesTurnEnded;

    private void OnEnable()
    {
        TurnSwitcher.TurnSwitched += OnTurnSwitched;
    }
    private void OnDisable()
    {
        TurnSwitcher.TurnSwitched -= OnTurnSwitched;
    }
    public void AddCharacter(Character character)
    {
        all.Add(character);
        character.CharacterDied += OnCharacterDied;
    }
    public void AddCharacterToActive(Character character)
    {
        active.Add(character);
        character.CharacterDied += OnCharacterDied;
    }
    public void RemoveCharacter(Character character)
    {
        all.Remove(character);
        active.Remove(character);
        character.CharacterDied -= OnCharacterDied;
    }
    public void RemoveCharacterFromActive(Character character)
    {
        if (active.Contains(character))
        {
            active.Remove(character);
        }
        character.CharacterDied -= OnCharacterDied;
    }
    public void OnTurnSwitched()
    {
        OnTurnSwitchedAsync();
    }
    public IEnumerator OnTurnSwitchedRoutine()
    {
        foreach (Character character in active)
        {
            cameraControl.SetAnchor(character.transform);
            yield return new WaitUntil(() => Mathf.Abs((cameraControl.transform.position - character.transform.position).magnitude) < 0.5f);

            //do active staff
            character.MakeTurn();
            yield return new WaitUntil(() => character.FinishedTurn);
            yield return new WaitForSeconds(0.5f);
        }
        cameraControl.SetDefalutAnchor();
        switchTurnButton.interactable = true;
    }
    public async void OnTurnSwitchedAsync()
    {
        EnemiesTurnStarted?.Invoke();
        switchTurnButton.interactable = false;
        foreach (Character character in active)
        {
            if (character.IsDead)
            {
                continue;
            }
            cameraControl.SetAnchor(character.transform);
            await TaskUtils.WaitUntil(() => (cameraControl.transform.position - character.transform.position).magnitude < 0.5f);

            //do active staff
            character.MakeTurn();
            await TaskUtils.WaitUntil(() => character.FinishedTurn);
            await Task.Delay(500);
        }
        DestroyDeadEnimies();
        cameraControl.SetDefalutAnchor();
        switchTurnButton.interactable = true;
        EnemiesTurnEnded?.Invoke();
    }

    private void DestroyDeadEnimies()
    {
        Character temp;
        for (int i = 0; i < active.Count; i++)
        {
            if (active[i].IsDead)
            {
                temp = active[i];
                active.RemoveAt(i);
                Destroy(temp.gameObject);
                i--;
            }
        }
    }

    private void OnCharacterDied(Character character)
    {
        //RemoveCharacterFromActive(character);
    }
}
