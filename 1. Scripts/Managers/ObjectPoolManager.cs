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
        public int size;  // 초기 풀 크기
    }

    public List<Pool> pools;
    private Dictionary<string, ObjectPool<MonoBehaviour>> poolDic;

    // 부모 오브젝트
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
                collectionCheck: false,  // 동일 오브젝트 중복 방지 체크 비활성화
                defaultCapacity: pool.size,  // 초기 풀 크기 설정
                maxSize: pool.size  // 최대 풀 크기 설정
            );

            poolDic.Add(pool.code, newPool);
        }
    }

    public MonoBehaviour SpawnFromPool(string code, Vector2 position, Quaternion rotation)
    {
        if (!poolDic.ContainsKey(code))
        {
            Debug.LogWarning($"'{code}'에 해당하는 풀이 존재하지 않습니다.");
            return null;
        }

        MonoBehaviour objectToSpawn = poolDic[code].Get();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // 이름을 기준으로 부모 설정
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
            objectToSpawn.transform.SetParent(transform); // 기본 부모로 설정
        }

        return objectToSpawn;
    }

    public void ReturnToPool(string code, MonoBehaviour obj)
    {
        if (!poolDic.ContainsKey(code))
        {
            Debug.LogWarning($"'{code}'에 해당하는 풀이 존재하지 않습니다.");
            return;
        }

        poolDic[tag].Release(obj);
    }
}
