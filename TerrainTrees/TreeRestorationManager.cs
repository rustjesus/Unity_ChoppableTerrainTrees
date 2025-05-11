using System.Collections.Generic;
using UnityEngine;

public static class TreeRestorationManager
{
    public static List<(Vector3 worldPosition, int prototypeIndex)> removedTrees = new();

    public static void TrackRemovedTree(Vector3 worldPosition, int prototypeIndex)
    {
        removedTrees.Add((worldPosition, prototypeIndex));
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
                SetTerrainCoppableTrees.RespawnTree(tree.worldPosition, tree.prototypeIndex);
            }

            removedTrees.Clear();
        }
    }
#endif
}
