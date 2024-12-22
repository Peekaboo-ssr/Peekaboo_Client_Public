using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    // 캐싱 데이터
    [field : SerializeField] public List<Ghost> GhostData { get; private set; }
    [field : SerializeField] public List<Item> ItemData { get; private set; }
    
    // Remote에서 사용할 딕셔너리 데이터
    public Dictionary<int, Ghost> GhostDataDictionary { get; private set; } = new Dictionary<int, Ghost>();
    public Dictionary<int, Item> ItemDataDictionary { get; private set; } = new Dictionary<int, Item>();

    // CSV 파싱 데이터
    public List<Dictionary<string, object>> StageData { get; private set; }
    public List<Dictionary<string, object>> SoulItemData { get; private set; }
    public List<Dictionary<string, object>> StoreData { get; private set; }
    public bool DataReady = false;

    private void Start()
    {
        InitCSVData();
        InitCacheData();
        DataReady = true;
    }


    private void InitCSVData()
    {
        StageData = CSVReader.Read("CSV/DifficultyTable");
        SoulItemData = CSVReader.Read("CSV/SoulItemTable");
        StoreData = CSVReader.Read("CSV/ShopTable");
    }

    // [TypeId : Key / 객체(Ghost, Item) : Value]를 기반으로 Dictionary 생성
    // TypeEnum == 데이터의 typeid와 동기화 필요
    private void InitCacheData()
    {
        foreach(var ghost in GhostData)
        {
            // 예외 처리
            if (GhostDataDictionary.ContainsKey((int)ghost.GhostType)) continue;
            GhostDataDictionary.Add((int)ghost.GhostType, ghost);
        }

        foreach(var item in ItemData)
        {
            // 예외 처리
            if(ItemDataDictionary.ContainsKey((int)item.ItemData.Type)) continue;
            ItemDataDictionary.Add((int)item.ItemData.Type, item);
        }
    }
}