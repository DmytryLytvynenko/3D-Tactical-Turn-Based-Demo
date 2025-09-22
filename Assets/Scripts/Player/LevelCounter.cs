using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCounter : MonoBehaviour
{
    [field: SerializeField] public static int LevelCount { get; private set; } = 0;

    [SerializeField] private Image levelBar;
    [SerializeField] private TextMeshProUGUI levelCounter;
    [SerializeField] private int maxLevel;
    [SerializeField] private float firstLevelXP = 7.5f;
    [SerializeField] private float fillLerpRate = 6f;
    [SerializeField] private int testXPAmount = 10;
    private int currentXPPoints = 0;
    private float currentXPPercentage = 0;
    private int goalXPPoints = 100;
    public static event Action LevelIncreased;
    private void Start()
    {
        goalXPPoints = (int)(firstLevelXP*(2*Mathf.Pow(2,LevelCount/6)));
    }
    private void Update()
    {
        levelBar.fillAmount = Mathf.Lerp(levelBar.fillAmount, currentXPPercentage, fillLerpRate * Time.deltaTime);
    }
    public void OnLevelReached()
    {
        LevelCount++;
        LevelIncreased?.Invoke();
        currentXPPoints -= goalXPPoints;
        //goalXPPoints = 100;
        goalXPPoints = (int)Mathf.Round(firstLevelXP * (2 * Mathf.Pow(2.0f, LevelCount / 6.0f))); //goalXPPoints func
        levelCounter.text = (LevelCount + 1).ToString();
        if (currentXPPoints >= goalXPPoints)
        {
            OnLevelReached();
        }
        currentXPPercentage = (float)currentXPPoints / (float)goalXPPoints;
        if (LevelCount == maxLevel)
        {
            currentXPPercentage = 1;
        }
    }
    public void PickupXpObject(XPObject xp)
    {
        currentXPPoints += xp.XPPoints;
        currentXPPercentage = (float)currentXPPoints / (float)goalXPPoints;
        if (currentXPPoints >= goalXPPoints)
        {
            OnLevelReached();
        }
    }
    public void AddXP()
    {
        if (LevelCount == maxLevel)
        {
            return;
        }
        currentXPPoints += testXPAmount;
        currentXPPercentage = (float)currentXPPoints / (float)goalXPPoints;
        if (currentXPPoints >= goalXPPoints)
        {
            OnLevelReached();
        }
    }
}
