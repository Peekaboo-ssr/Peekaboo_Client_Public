using System.Collections.Generic;
using UnityEngine;

public class WaitingRoom : MonoBehaviour
{
    List<RoomBtn> roomList = new List<RoomBtn>();
    [SerializeField] private RoomBtn roomBtnPrefab;
    [SerializeField] private Transform roomTransform;
    public void InitList()
    {
        foreach(RoomBtn roomBtn in roomList)
        {
            Destroy(roomBtn.gameObject);
        }
        roomList.Clear();
    }

    public void CreateRoom(string gameSessionId, string roomName, int numberOfPlayer, int latency)
    {
        // RoomBtn 생성
        RoomBtn newBtn = Instantiate(roomBtnPrefab,roomTransform);
        newBtn.Init(gameSessionId, roomName, numberOfPlayer, latency);
        roomList.Add(newBtn);
    }
}
