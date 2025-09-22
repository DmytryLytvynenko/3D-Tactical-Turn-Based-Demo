using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionPointsController : MonoBehaviour
{
    [SerializeField] private float horizontalStep = 90f;
    [SerializeField] private Transform center;
    [SerializeField] private List<ActionPoint> actionPoints = new List<ActionPoint>();

    [SerializeField] private int maxPointsCount = 0;
    [SerializeField] private int currentPointsCount = 0;
    [SerializeField] private CharacterData playerData;
    [SerializeField] private CharacterStats playerStats;

    private List<Task> tasks = new List<Task>();
    private void Start()
    {
        maxPointsCount = playerData.MaxActionPoints;
        UpdatePointsPosition();
        RestoreActionPoints();
    }

    private void OnEnable()
    {
        playerData.MaxActionPointsChanged += OnMaxActionPointsChanged;
        playerStats.StatsRestored += OnActionPointsRestored;
        playerStats.ActionPointsUsed += OnActionPointsUsed;
    }

    private void OnDisable()
    {
        playerData.MaxActionPointsChanged -= OnMaxActionPointsChanged;
        playerStats.StatsRestored -= OnActionPointsRestored;
        playerStats.ActionPointsUsed -= OnActionPointsUsed;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerData.ChangeParameter(CharacterDataParameter.ActionPoints, playerData.MaxActionPoints + 1);
        }
    }
    public void UpdatePointsPosition()
    {
        Vector3 buttonPos = new Vector3(center.localPosition.x + (maxPointsCount - 1) / 2.0f * -horizontalStep, center.localPosition.y, center.localPosition.z);
        for (int i = 0; i < maxPointsCount; i++)
        {
            actionPoints[i].Activate();
            actionPoints[i].transform.localPosition = buttonPos;
            buttonPos.x += horizontalStep;
        }
        for (int i = maxPointsCount;i < actionPoints.Count; i++)
        {
            actionPoints[i].Deactivate();
        }
    }
    private async void RestoreActionPoints()
    {
        foreach (ActionPoint actionPoint in actionPoints)
        {
            actionPoint.StopCurrentScaleTask();
        }
        await Task.Delay(50);
        currentPointsCount = playerStats.ActionPoints;
        for (int i = 0;i < currentPointsCount; i++)
        {
            actionPoints[i].AnimateAppearence();
            await Task.Delay(200);
        }
        tasks.Clear();
    }
    private void OnMaxActionPointsChanged(int maxPoints)
    {
        maxPointsCount = maxPoints;
        UpdatePointsPosition();
    }
    private void OnActionPointsUsed(int usedPoints)
    {
        int actualPoints = currentPointsCount - usedPoints;
        for (int i = currentPointsCount - 1; i > actualPoints - 1; i--)
        {
            actionPoints[i].AnimateDisappearence();
        }
        currentPointsCount = actualPoints;
    }

    private void OnActionPointsRestored()
    {
        RestoreActionPoints();
    }

}
