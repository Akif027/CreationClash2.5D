using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    // Singleton instance
    private static ObjectPool _instance;
    public static ObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObjectPool>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<ObjectPool>();
                    singletonObject.name = typeof(ObjectPool).ToString() + " (Singleton)";

                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    // Ensure the instance is destroyed when the application quits
    private void OnApplicationQuit()
    {
        _instance = null;
    }

    // Define PooledObject class as before
    [System.Serializable]
    public class PooledObject
    {
        public string name;
        public GameObject prefab;
        public Transform parent;
        public int initialSize = 10;
        public int maxSize = 20;
    }

    public List<PooledObject> pooledObjects;
    private Dictionary<string, Queue<GameObject>> availableObjects = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, HashSet<GameObject>> reservedObjects = new Dictionary<string, HashSet<GameObject>>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);  // Keeps the pool persistent across scenes
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        foreach (PooledObject pooledObject in pooledObjects)
        {
            availableObjects[pooledObject.name] = new Queue<GameObject>();
            reservedObjects[pooledObject.name] = new HashSet<GameObject>();

            for (int i = 0; i < pooledObject.initialSize; i++)
            {
                GameObject obj = Instantiate(pooledObject.prefab);
                if (pooledObject.parent != null)
                {
                    obj.transform.SetParent(pooledObject.parent);
                }

                obj.SetActive(false);
                availableObjects[pooledObject.name].Enqueue(obj);
            }
        }
    }

    public GameObject GetPooledObject(string name)
    {
        if (availableObjects.ContainsKey(name))
        {
            if (availableObjects[name].Count > 0)
            {
                GameObject obj = availableObjects[name].Dequeue();
                reservedObjects[name].Add(obj);
                obj.SetActive(true);
                return obj;
            }
            else
            {
                foreach (PooledObject pooledObject in pooledObjects)
                {
                    if (pooledObject.name == name && reservedObjects[name].Count < pooledObject.maxSize)
                    {
                        GameObject obj = Instantiate(pooledObject.prefab);
                        if (pooledObject.parent != null)
                        {
                            obj.transform.SetParent(pooledObject.parent);
                        }

                        reservedObjects[name].Add(obj);
                        obj.SetActive(true);
                        return obj;
                    }
                }
            }
        }
        Debug.LogError($"Max size reached for {name} or object not found.");
        return null;
    }

    public void ReturnObjectToPool(GameObject obj, string name)
    {
        obj.SetActive(false);
        reservedObjects[name].Remove(obj);
        availableObjects[name].Enqueue(obj);
    }

    private void OnDestroy()
    {
        foreach (var pair in reservedObjects)
        {
            foreach (var obj in pair.Value)
            {
                Destroy(obj);
            }
        }
    }
}
