using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemySpawner myTarget = (EnemySpawner)target;
        if (GUILayout.Button("FindSpawnTile"))
        {
            myTarget.FindSpawnTile();
        }
        if (GUILayout.Button("FindSpawnTileX1000"))
        {
            myTarget.FindSpawnTileX1000();
        }
    }
}
