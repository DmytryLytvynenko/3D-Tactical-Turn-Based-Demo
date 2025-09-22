using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UFOController))]
public class UFOControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UFOController myTarget = (UFOController)target;
        if (GUILayout.Button("PlaceDots"))
        {
            myTarget.PlaceDots();
        }
        if (GUILayout.Button("NameDots"))
        {
            myTarget.NameDots();
        }
    }
}
