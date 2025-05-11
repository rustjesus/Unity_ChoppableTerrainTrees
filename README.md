Choppable terrain tree instancing for Unity:
You will need to make your own "Tree" script to chop them down, this is just the instancing code that allows you to use terrain trees the way you want. 
You will need a 9/10 skill for this and it will require some refactoring.
EXAMPLE USE:


                    float removedWidth, removedHeight;
                    if (SetTerrainCoppableTrees.RemoveTreeInstance(transform.position, out removedWidth, out removedHeight))
                    {
                        TreeRestorationManager.TrackRemovedTree(transform.position, prototypeIndex, removedWidth, removedHeight);
                        StartCoroutine(DelayRespawn(removedWidth, removedHeight));
                    }
                    
    IEnumerator DelayRespawn(float widthScale, float heightScale)
    {
        yield return new WaitForSeconds(treeSpawner.treeRespawnTime);
        TreeRestorationManager.UntrackRestoredTree(transform.position, prototypeIndex);
        SetTerrainCoppableTrees.RespawnTree(gameObject.transform.position, prototypeIndex, widthScale, heightScale);
    }
