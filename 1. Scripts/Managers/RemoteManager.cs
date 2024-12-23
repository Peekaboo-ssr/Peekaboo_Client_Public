using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RemoteManager : Singleton<RemoteManager>
{
    [SerializeField] RemotePlayer remotePalyer;
    [SerializeField] Transform playerTrans;
    [SerializeField] Transform ghostTrans;
    [SerializeField] Transform itemTrans;
    [SerializeField] List<Door> doors;
    public Dictionary<string, RemotePlayer> PlayerDictionary = new Dictionary<string, RemotePlayer>();
    Dictionary<uint, Ghost> ghostDictionary = new Dictionary<uint, Ghost>();
    Dictionary<uint, Item> itemDictionary = new Dictionary<uint, Item>();
    Dictionary<uint, Door> doorDictionary = new Dictionary<uint, Door>();

    public event Action<string> OnPlayerDie;

    public bool isFailSesstion;

    protected override void Awake()
    {
        base.Awake();
        InitDoor();
        GameManager.Instance.OnFailSession += (() => isFailSesstion = true);
    }
    private void InitDoor()
    {
        for (int i = 1; i <= doors.Count; i++)
        {
            doors[i - 1].doorId = (uint)i;
            doorDictionary.Add((uint)i, doors[i - 1]);
        }
    }

    #region PlayerDictionary
    public async void RevivalRemotePlayer(Vector3 pos) // Remote Player 부활 (객체 키기)
    {
        await UniTask.WaitForSeconds(5f);
        foreach (var remotePlayer in PlayerDictionary)
        {
            remotePlayer.Value.gameObject.SetActive(true);
            remotePlayer.Value.OffSpectate();
            remotePlayer.Value.Rigidbody.MovePosition(pos);
            remotePlayer.Value.IsDie = false;
        }
    }

    public bool IsAliveRemotePlayer() // 1명이라도 살아있으면 true
    {
        foreach (var remotePlayer in PlayerDictionary.Values)
        {
            if (!remotePlayer.IsDie)
                return true;
        }
        return false;
    }

    public int GetAliveRemotePlayerNum()
    {
        int cnt = 0;
        foreach (var remotePlayer in PlayerDictionary.Values)
        {
            if (!remotePlayer.IsDie)
                cnt++;
        }
        return cnt;

    }
    #endregion

    #region Player
    // CreateGameResponse
    public void CreatePlayer(string id)
    {
        if (PlayerDictionary.ContainsKey(id)) return;
        RemotePlayer player = Instantiate(remotePalyer, playerTrans);
        player.Rigidbody.MovePosition(playerTrans.gameObject.transform.position);
        player.name = id.ToString();
        player.SetUserID(id);
        PlayerDictionary.Add(id, player);
    }
    public void UpdateRemotePlayerMovement(string id, Position pos, Rotation rot)
    {
        if (!PlayerDictionary.ContainsKey(id)) return;
        PlayerDictionary[id].NetworkHandler.MovementSync(pos.ToVector3(), rot.ToVector3());
    }
    public void UpdateRemotePlayerState(string id, CharacterState state)
    {
        if (id == NetworkManager.Instance.UserId && GameManager.Instance.Player.gameObject.activeInHierarchy)
        {
            GameManager.Instance.Player.NetworkHandler.StateChangeSync(state);
            return;
        }

        if(!PlayerDictionary.ContainsKey(id)) return;

        if (state == CharacterState.Died)
        {
            PlayerDictionary[id].IsDie = true;
            OnPlayerDie?.Invoke(id);
        }

        if (state == CharacterState.Exit || PlayerDictionary[id].gameObject.activeInHierarchy) // 현재 살아있는 경우에만 State 동기화
        {
            try
            {
                PlayerDictionary[id].NetworkHandler.StateSync(state);
            }
            catch (Exception)
            {
                Debug.LogError($"{state} State Change Notification 오류 발생");
                throw;
            }
        }
    }

    public void BlockInteractionPlayer()
    {
        GameManager.Instance.Player.NetworkHandler.BlockInteraction();
    }

    public void DisconnectPlayer(string id)
    {
        RemotePlayer deletePlayer = PlayerDictionary[id];

        if (!PlayerDictionary[id].IsDie)
            OnPlayerDie?.Invoke(id);

        PlayerDictionary.Remove(id);
        Destroy(deletePlayer.gameObject);
    }
    #endregion

    #region Ghost
    public void CreateGhost(GhostInfo ghostInfo)
    {
        int typeId = (int)ghostInfo.GhostTypeId;
        Ghost ghost = Instantiate(DataManager.Instance.GhostDataDictionary[typeId], ghostTrans);
        ghost.transform.position = ghostInfo.MoveInfo.Position.ToVector3();

        if (!isFailSesstion)
            ghost.Init(ghostInfo.GhostId, false);
        else
            ghost.Init(ghostInfo.GhostId, true);


        //ghost.Init(false, ghostId);
        if (!ghostDictionary.ContainsKey(ghostInfo.GhostId))
            ghostDictionary.Add(ghostInfo.GhostId, ghost);
    }
    /// <summary>
    /// Host와 hostx 모두 서버와의 통신으로 ghost 생성
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    //public void CreateRemoteGhost(uint id, uint type)
    //{
    //    // TODO : GhostSpawnManager.Instance.
    //    Ghost ghost = Instantiate(DataManager.Instance.GhostDataDictionary[(int)type], ghostTrans);
    //    ghost.name = id.ToString();
    //    //ghost.Init(false, ghostId);
    //    if (!ghostDictionary.ContainsKey(id))
    //        ghostDictionary.Add(id, ghost);
    //}
    public void UpdateRemoteGhostMovement(uint id, Position pos, Rotation rot)
    {
        ghostDictionary[id].NetworkHandler.HandleMoveNotification(pos.ToVector3(), Quaternion.Euler(rot.ToVector3()));
    }
    public void DeleteGhost(uint id)
    {
        Ghost deleteGhost = ghostDictionary[id];
        ghostDictionary.Remove(id);
        Destroy(deleteGhost.gameObject);
    }
    public void UpdateGhostState(uint id, CharacterState state)
    {
        // 귀신은 Host, Remote 모두 애니메이션 실행
        ghostDictionary[id].NetworkHandler.StateSync(state);
    }
    public void UpdateGhostSpecialState(uint id, GhostSpecialState state, bool isOn)
    {
        // 귀신은 Host, Remote 모두 특수상태 실행
        ghostDictionary[id].NetworkHandler.SpecialStateSync(state, isOn);
    }
    #endregion

    #region Item
    /// 미사용 함수 Item Id 생성을 서버에서 생성
    /*
    public void InitStageData()
    {
        InitItem();
        GamePacket packet = new GamePacket();
        packet.SpawnInitialDataResponse = new C2S_SpawnInitialDataResponse();

        foreach (var item in itemDictionary.Values)
        {
            ItemInfo itemInfo = new ItemInfo();
            Position pos = new Position();

            itemInfo.ItemId = (uint)item.Id;
            itemInfo.ItemTypeId = (uint)item.ItemData.Type;

            pos.X = item.transform.position.x;
            pos.Y = item.transform.position.y;
            pos.Z = item.transform.position.z;
            itemInfo.Position = pos;

            packet.SpawnInitialDataResponse.ItemInfos.Add(itemInfo);
        }

        // 셋팅 다하면 위 내용들을 바탕으로 ghostDatas, itemDatas를 서버에 전달
        GameServerSocketManager.Instance.Send(packet);
    }
    private void InitItem()
    {
        // 아이템 생성 및 배치
        Dictionary<uint, ItemInfo> itemInfo = new Dictionary<uint, ItemInfo>();

        // DifficultyTable에서 뽑을 숫자 선택
        int itemCnt = Random.Range(
            // 각 명칭을 하나의 변수로 관리
            (int)DataManager.Instance.StageData[(int)GameManager.Instance.DiffId - 1]["MinSoulItemNumber"],
            (int)DataManager.Instance.StageData[(int)GameManager.Instance.DiffId - 1]["MaxSoulItemNumber"]
            );

        for (int i = 0; i < itemCnt; i++)
        {
            int idx = Random.Range(0, DataManager.Instance.SoulItemData.Count);
            int typeId = (int)DataManager.Instance.SoulItemData[idx]["ItemTypeId"];
            CreateItem(typeId);
        }
    }
    // id없이 아이템 생성 (Init시에만 실행)
    public void CreateItem(int typeId)
    {
        Item item = Instantiate(DataManager.Instance.ItemDataDictionary[typeId], itemTrans);
        // item 정보 입력
        item.Init(itemIdx);
        if (!itemDictionary.ContainsKey(itemIdx))
            itemDictionary.Add(itemIdx++, item);
    }
    */
    // remoteItem을 생성해주는 경우
    public void CreateItem(ItemInfo itemInfo)
    {
        uint id = itemInfo.ItemId;
        if (itemDictionary.ContainsKey(id)) return;

        int typeId = (int)itemInfo.ItemTypeId;
        Vector3 pos = itemInfo.Position.ToVector3();
        Item item = Instantiate(DataManager.Instance.ItemDataDictionary[typeId], itemTrans);
        item.Init(id);
        item.Rigidbody.MovePosition(pos);
        itemDictionary.Add(id, item);
    }
    public Item GetItem(uint id)
    {
        return itemDictionary[id];
    }
    public void UpdateRemotePlayerGetItem(string userId, uint itemId)
    {
        if (!PlayerDictionary.ContainsKey(userId) || !itemDictionary.ContainsKey(itemId)) return;
        PlayerDictionary[userId].NetworkHandler.ItemGetSync(itemDictionary[itemId]);
    }
    public void UpdateRemotePlayerChangeItem(string userId)
    {
        if (NetworkManager.Instance.UserId.Equals(userId)) return;
        if (!PlayerDictionary.ContainsKey(userId)) return;
        PlayerDictionary[userId].NetworkHandler.ItemBareHandChangeSync();
    }
    public void UpdateRemotePlayerChangeItem(string userId, uint itemId)
    {
        if (NetworkManager.Instance.UserId.Equals(userId)) return;
        if (!PlayerDictionary.ContainsKey(userId) || !itemDictionary.ContainsKey(itemId)) return;
        PlayerDictionary[userId].NetworkHandler.ItemChangeSync(itemDictionary[itemId]);
    }
    public void UseItem(string userId, uint itemId)
    {
        if (!PlayerDictionary.ContainsKey(userId) || !itemDictionary.ContainsKey(itemId)) return;
        PlayerDictionary[userId].NetworkHandler.ItemUseSync(itemDictionary[itemId]);
    }
    public void DisuseItem(string userId, uint itemId)
    {
        if (NetworkManager.Instance.UserId.Equals(userId))
        {
            InventoryManager.Instance.Inventory.DisuseItem(itemId);
            GameManager.Instance.Player.NetworkHandler.ItemDisuseSync(itemDictionary[itemId]);
            return;
        }
        if (!PlayerDictionary.ContainsKey(userId) || !itemDictionary.ContainsKey(itemId)) return;
        PlayerDictionary[userId].NetworkHandler.ItemDisuseSync(itemDictionary[itemId]);
    }
    public void DiscardItem(string userId, uint itemId)
    {
        if (!PlayerDictionary.ContainsKey(userId) || !itemDictionary.ContainsKey(itemId)) return;
        Transform dropPos = PlayerDictionary[userId].ItemHoldHandler.ItemHoldTransform;
        PlayerDictionary[userId].NetworkHandler.ItemDiscardSync(itemDictionary[itemId], dropPos);
    }
    public void DeleteItem(uint itemId)
    {
        InventoryManager.Instance.Inventory.DeleteItem(itemId);
        foreach (var player in PlayerDictionary.Values)
            player.NetworkHandler.ItemDeleteSync(itemDictionary[itemId]);
        
        Item deleteItem = itemDictionary[itemId];
        itemDictionary.Remove(itemId);
        Destroy(deleteItem.gameObject);
    }
    #endregion

    #region Door
    public void UpdateRemoteDoorToggle(uint doorId, DoorState doorState)
    { 
        doorDictionary[doorId].RemoteToggle(doorState);
    }
    #endregion
}