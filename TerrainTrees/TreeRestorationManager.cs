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
    [UnityEditor.InitializeOnLoadMethod]
    static void RegisterExitHandler()
    {
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
        {
            foreach (var tree in removedTrees)
            {
                SetTerrainCoppableTrees.RespawnTree(tree.worldPosition, tree.prototypeIndex, tree.widthScale, tree.heightScale);
            }

            removedTrees.Clear();
        }
    }
#endif
}
