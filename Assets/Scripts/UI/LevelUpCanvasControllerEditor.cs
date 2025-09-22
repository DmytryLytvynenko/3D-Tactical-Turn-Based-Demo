using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelUpCanvasController))]
public class LevelUpCanvasControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelUpCanvasController myTarget = (LevelUpCanvasController)target;
        if (GUILayout.Button("Appear"))
        {
            myTarget.AnimateButtonsAppear();
        }
    }
}
