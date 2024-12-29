using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [System.Serializable]
    public class Pool
    {
        public EPoolObjectType type;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<int, ObjectPool<GameObject>> poolDictionary;

    // 부모 오브젝트
    public Transform DiedPlayerPoolTransform;
    public Transform EffectTransform;

    protected override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<int, ObjectPool<GameObject>>();

        foreach (var pool in pools)
        {
            var newPool = new ObjectPool<GameObject>(
                createFunc: () => {
                    GameObject obj = Instantiate(pool.prefab);
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

            poolDictionary.Add((int)pool.type, newPool);
        }
    }

    public GameObject SpawnFromPool(EPoolObjectType type)
    {
        if (!poolDictionary.ContainsKey((int)type))
        {
            Debug.LogWarning($"'{type}'에 해당하는 풀이 존재하지 않습니다.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[(int)type].Get();

        switch (type)
        {
            default:
                objectToSpawn.transform.SetParent(transform); // 기본 부모로 설정
                break;
            case EPoolObjectType.DiedPlayer:
                objectToSpawn.transform.SetParent(DiedPlayerPoolTransform);
                break;
            case EPoolObjectType.DiedEffect:
                objectToSpawn.transform.SetParent(EffectTransform);
                break;
        }
        return objectToSpawn;
    }

    public void ReturnToPool(EPoolObjectType type, GameObject obj)
    {
        if (!poolDictionary.ContainsKey((int)type))
        {
            Debug.LogWarning($"'{type}'에 해당하는 풀이 존재하지 않습니다.");
            return;
        }
        poolDictionary[(int)type].Release(obj);
    }

    public void ResetDiedPlayer()
    {
        // Died Player 끄기
        for (int i = 0; i < DiedPlayerPoolTransform.childCount; ++i)
        {
            Transform child = DiedPlayerPoolTransform.GetChild(i);
            if (child != null)
            {
                child.gameObject.SetActive(false); 
            }
        }
        // Died Effect 끄기
        for (int i = 0; i < EffectTransform.childCount; ++i)
        {
            Transform child = EffectTransform.GetChild(i);
            if (child != null)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
