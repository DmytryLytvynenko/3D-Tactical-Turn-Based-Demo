using UnityEngine;

public class TurnSwitcherUI : MonoBehaviour
{
    public void SwitchTurn()
    {
        if (Player.UsingSkill || Player.InstancePlayer.Moving)
        {
            Debug.Log("Player is busy");
            return; 
        }
            
        TurnSwitcher.SwitchTurn();
    }
}
