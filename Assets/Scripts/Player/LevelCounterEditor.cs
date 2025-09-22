using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelCounter))]
public class LevelCounterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelCounter myTarget = (LevelCounter)target;
        if (GUILayout.Button("Add XP Points"))
        {
            myTarget.AddXP();
        }
    }
}
