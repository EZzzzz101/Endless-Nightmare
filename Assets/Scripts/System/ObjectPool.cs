using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;          // 池子标签（要和怪物预制体的Tag一致）
        public GameObject prefab;   // 怪物预制体
        public int preloadCount;    // 预生成数量
    }

    public List<Pool> pools = new List<Pool>();
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 遍历所有配置的池子，预生成物体
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.preloadCount; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false); // 预生成时失活
                objectQueue.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectQueue);
        }
    }

    // 【核心方法1】从池子取出物体
    public GameObject GetFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"找不到标签为 {tag} 的对象池！");
            return null;
        }

        // 从队列取出
        GameObject objToUse = poolDictionary[tag].Dequeue();

        // 激活并设置位置
        objToUse.SetActive(true);
        objToUse.transform.position = position;
        objToUse.transform.rotation = rotation;

        // 取出后立刻放回队列末尾（如果下次取的时候它还是激活的，说明池子不够用了，会自动扩容）
        poolDictionary[tag].Enqueue(objToUse);

        return objToUse;
    }

    // 【核心方法2】把物体放回池子（由EnemyBase调用）
    public void ReturnToPool(string tag, GameObject obj)
    {
        obj.SetActive(false);
        // 不需要手动Enqueue，因为它已经在Start的队列里了
    }
}