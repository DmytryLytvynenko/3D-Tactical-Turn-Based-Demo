using System;
using System.Collections.Generic;

public class TurnSwitcher 
{
    public static int TurnCounter { get; private set; } = 0;
    public static event Action TurnSwitched;

    public static void SwitchTurn()
    {
        TurnSwitched?.Invoke();
        TurnCounter++;
    }
}
