#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using System.Collections.Generic;
using UnityEngine;

public static class TreeRestorationManager
{
    public static List<(Vector3 worldPosition, int prototypeIndex, float widthScale, float heightScale)> removedTrees = new();

    public static void TrackRemovedTree(Vector3 worldPosition, int prototypeIndex, float widthScale, float heightScale)
    {
        removedTrees.Add((worldPosition, prototypeIndex, widthScale, heightScale));
    }

    public static void UntrackRestoredTree(Vector3 worldPosition, int prototypeIndex)
    {
        removedTrees.RemoveAll(tree =>
            tree.prototypeIndex == prototypeIndex &&
            Vector3.Distance(tree.worldPosition, worldPosition) < 0.1f); // small tolerance
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    static void RegisterEditorEvents()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorSceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            RestoreTrees();
        }
    }

    private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        RestoreTrees();
    }

    private static void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
    {
        RestoreTrees();
    }

    private static void RestoreTrees()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        foreach (var tree in removedTrees)
        {
            Vector3 relative = tree.worldPosition - terrainPos;
            Vector3 normalizedPos = new Vector3(
                relative.x / terrainSize.x,
                relative.y / terrainSize.y,
                relative.z / terrainSize.z
            );

            SetTerrainCoppableTrees.RespawnTree(normalizedPos, tree.prototypeIndex, tree.widthScale, tree.heightScale);
        }

        removedTrees.Clear();
    }
#endif
}
