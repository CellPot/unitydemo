using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject prefabToSpawn;
    [Tooltip("Интервал спавна объектов (0 - без интервала)")]
    public float repeatInterval;
    [Tooltip("Максимальное количество объектов всего")]
    public int maxObjectsTotal = 1;
    [Tooltip("Максимальное количество объектов одновременно в сцене")]
    public int simultaneousSpawnedObjects = 1;
    [Tooltip("Счетчик количества заспавненных объектов всего")]
    public int spawnedObjectsCntTotal;
    [Tooltip("Количество объектов в сцене")]
    public int objectsAtScene;

    private void Update()
    {
        objectsAtScene = gameObject.transform.childCount;
    }
    void Start()
    {

        if (!prefabToSpawn.CompareTag("Player"))
        {
            if (repeatInterval == 0)
            {
                SpawnObject();
            }
            else if (repeatInterval > 0 )
            {
                InvokeRepeating("SpawnObject", 0.0f, repeatInterval);
            }
        }
        
    }
    public GameObject SpawnObject()
    {
        if (prefabToSpawn!= null && spawnedObjectsCntTotal < maxObjectsTotal && objectsAtScene < simultaneousSpawnedObjects)
        {
            
            GameObject spawnedObject = Instantiate(prefabToSpawn, transform.position, Quaternion.identity,gameObject.transform);
            spawnedObjectsCntTotal++;
            objectsAtScene = gameObject.transform.childCount;
            print($"{prefabToSpawn.name} is spawned at {this.transform.position.ToString()}");
            return spawnedObject;

        }
        return null;
    }

}
