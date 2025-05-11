Choppable terrain tree instancing for Unity:
You will need to make your own "Tree" script to chop them down, this is just the instancing code that allows you to use terrain trees the way you want. 
You will need a 9/10 skill for this and it will require some refactoring.
EXAMPLE USE:


private void SomeFunc()
{

                    float removedWidth, removedHeight;
                    if (SetTerrainCoppableTrees.RemoveTreeInstance(transform.position, out removedWidth, out removedHeight))
                    {
                        TreeRestorationManager.TrackRemovedTree(transform.position, prototypeIndex, removedWidth, removedHeight);
                        StartCoroutine(DelayRespawn(removedWidth, removedHeight));
                    }
}
                    
IEnumerator DelayRespawn(float widthScale, float heightScale)
{
    yield return new WaitForSeconds(treeSpawner.treeRespawnTime);

    Terrain terrain = Terrain.activeTerrain;
    if (terrain == null) yield break;

    Vector3 terrainPos = terrain.transform.position;
    Vector3 terrainSize = terrain.terrainData.size;
    Vector3 worldPos = transform.position;

    // Convert world position to normalized terrain coordinates
    Vector3 relative = worldPos - terrainPos;
    Vector3 normalizedPos = new Vector3(
        relative.x / terrainSize.x,
        relative.y / terrainSize.y,
        relative.z / terrainSize.z
    );

    TreeRestorationManager.UntrackRestoredTree(worldPos, prototypeIndex);
    SetTerrainCoppableTrees.RespawnTree(normalizedPos, prototypeIndex, widthScale, heightScale);
}

