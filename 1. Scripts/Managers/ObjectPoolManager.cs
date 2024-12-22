using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [System.Serializable]
    public class Pool
    {
        public string code;
        public MonoBehaviour prefab;
        public int size;  // �ʱ� Ǯ ũ��
    }

    public List<Pool> pools;
    private Dictionary<string, ObjectPool<MonoBehaviour>> poolDic;

    // �θ� ������Ʈ
    public Transform ghostsParent;
    public Transform itemsParent;

    protected override void Awake()
    {
        base.Awake();
        poolDic = new Dictionary<string, ObjectPool<MonoBehaviour>>();

        foreach (var pool in pools)
        {
            var newPool = new ObjectPool<MonoBehaviour>(
                createFunc: () => {
                    MonoBehaviour obj = Instantiate(pool.prefab);
                    obj.gameObject.SetActive(false);
                    return obj;
                },
                actionOnGet: (obj) => {
                    obj.gameObject.SetActive(true);
                },
                actionOnRelease: (obj) => {
                    obj.gameObject.SetActive(false);
                },
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,  // ���� ������Ʈ �ߺ� ���� üũ ��Ȱ��ȭ
                defaultCapacity: pool.size,  // �ʱ� Ǯ ũ�� ����
                maxSize: pool.size  // �ִ� Ǯ ũ�� ����
            );

            poolDic.Add(pool.code, newPool);
        }
    }

    public MonoBehaviour SpawnFromPool(string code, Vector2 position, Quaternion rotation)
    {
        if (!poolDic.ContainsKey(code))
        {
            Debug.LogWarning($"'{code}'�� �ش��ϴ� Ǯ�� �������� �ʽ��ϴ�.");
            return null;
        }

        MonoBehaviour objectToSpawn = poolDic[code].Get();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // �̸��� �������� �θ� ����
        if (objectToSpawn.name.Contains("GST"))
        {
            objectToSpawn.transform.SetParent(ghostsParent);
        }
        else if (objectToSpawn.name.Contains("ITM"))
        {
            objectToSpawn.transform.SetParent(itemsParent);
        }
        else
        {
            objectToSpawn.transform.SetParent(transform); // �⺻ �θ�� ����
        }

        return objectToSpawn;
    }

    public void ReturnToPool(string code, MonoBehaviour obj)
    {
        if (!poolDic.ContainsKey(code))
        {
            Debug.LogWarning($"'{code}'�� �ش��ϴ� Ǯ�� �������� �ʽ��ϴ�.");
            return;
        }

        poolDic[tag].Release(obj);
    }
}
