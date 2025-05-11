using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetTerrainCoppableTrees : MonoBehaviour
{
    [SerializeField] private GameObject[] choppablePrefabs;

    void Start()
    {
        Terrain terrain = Terrain.activeTerrain;
        TreeInstance[] trees = terrain.terrainData.treeInstances;
        TreePrototype[] prototypes = terrain.terrainData.treePrototypes;
        Vector3 terrainSize = terrain.terrainData.size;

        GameObject parent = new GameObject("ChoppableTrees");

        for (int i = 0; i < trees.Length; i++)
        {
            TreeInstance tree = trees[i];
            TreePrototype prototype = prototypes[tree.prototypeIndex];
            string treeName = prototype.prefab.name;

            GameObject matchingPrefab = null;
            foreach (GameObject prefab in choppablePrefabs)
            {
                if (prefab.name == treeName)
                {
                    matchingPrefab = prefab;
                    break;
                }
            }

            if (matchingPrefab == null)
                continue;

            Vector3 worldPosition = Vector3.Scale(tree.position, terrainSize) + terrain.transform.position;
            Quaternion rotation = Quaternion.AngleAxis(tree.rotation * Mathf.Rad2Deg, Vector3.up);

            GameObject treeObj = Instantiate(matchingPrefab, worldPosition, rotation, parent.transform);
            treeObj.GetComponent<WoodCuttingTree>().isSpawnedTree = true;
            treeObj.GetComponent<AI_Health>().prototypeIndex = tree.prototypeIndex;

            // Add NavMeshObstacle
            NavMeshObstacle obstacle = treeObj.AddComponent<NavMeshObstacle>();
            obstacle.carving = true;
            obstacle.carveOnlyStationary = true;

            // Match collider from prefab
            Collider coll = matchingPrefab.GetComponent<Collider>();
            if (coll != null)
            {
                if (coll is CapsuleCollider capsule)
                {
                    obstacle.shape = NavMeshObstacleShape.Capsule;
                    obstacle.center = capsule.center;
                    obstacle.radius = capsule.radius;
                    obstacle.height = capsule.height;
                }
                else if (coll is BoxCollider box)
                {
                    obstacle.shape = NavMeshObstacleShape.Box;
                    obstacle.center = box.center;
                    obstacle.size = box.size;
                }
                else
                {
                    Debug.LogWarning("Unsupported collider type on " + matchingPrefab.name);
                }
            }
            else
            {
                Debug.LogWarning(matchingPrefab.name + " is missing a Collider.");
            }
        }
    }
    public static void RespawnTree(Vector3 worldPosition, int prototypeIndex = 0, float widthScale = 1f, float heightScale = 1f, float colorVariation = 0.2f)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPosition = terrain.transform.position;

        // Convert world position to normalized terrain local position (0-1)
        Vector3 relativePos = (worldPosition - terrainPosition);
        Vector3 normalizedPos = new Vector3(
            relativePos.x / terrainSize.x,
            0, // Y is not used for placement
            relativePos.z / terrainSize.z
        );

        TreeInstance newTree = new TreeInstance
        {
            position = normalizedPos,
            prototypeIndex = prototypeIndex,
            widthScale = widthScale,
            heightScale = heightScale,
            color = Color.Lerp(Color.white, new Color(1f, 1f, 1f), colorVariation),
            lightmapColor = Color.white
        };

        List<TreeInstance> trees = new List<TreeInstance>(terrain.terrainData.treeInstances);
        trees.Add(newTree);
        terrain.terrainData.treeInstances = trees.ToArray();
    }

}
