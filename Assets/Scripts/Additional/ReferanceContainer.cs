using UnityEngine;

public class ReferanceContainer : MonoBehaviour
{
    public static UFOController UFOController { get; private set; }
    public static Player Player { get; private set; }
    public static CharacterManager CharacterManager { get; private set; }

    public static void FindReferances()
    {
        Player = FindFirstObjectByType<Player>();
        UFOController = FindFirstObjectByType<UFOController>();
        CharacterManager = FindFirstObjectByType<CharacterManager>();
    }
}
