using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HealthControll))]
public class HealthControllBehavior : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HealthControll myTarget = (HealthControll)target;
        if (GUILayout.Button("Kill"))
        {
            myTarget.Kill();
        }
    }
}
