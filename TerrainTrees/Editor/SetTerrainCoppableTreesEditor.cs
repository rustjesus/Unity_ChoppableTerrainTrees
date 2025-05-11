using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SetTerrainCoppableTrees))]
public class SetTerrainCoppableTreesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SetTerrainCoppableTrees script = (SetTerrainCoppableTrees)target;
        if (GUILayout.Button("Bake Choppable Trees"))
        {
            script.BakeChoppableTrees();
        }
    }
}
