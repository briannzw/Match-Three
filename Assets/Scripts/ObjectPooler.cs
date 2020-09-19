using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public static ObjectPooler instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        AddIntoPool();
    }

    private void AddIntoPool()
    {
        pools.ForEach((pool) =>
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            int i = 0;
            while(i < pool.size)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);

                objectPool.Enqueue(obj);
                i++;
            }

            poolDictionary.Add(pool.tag, objectPool);
        });
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Object with tag " + tag + " doesnt exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        if (objectToSpawn.activeSelf == true)
        {
            poolDictionary[tag].Enqueue(objectToSpawn);
            return SpawnFromPool(tag, position, rotation);
        }
        else
        {
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            poolDictionary[tag].Enqueue(objectToSpawn);
        }

        return objectToSpawn;
    }
}
