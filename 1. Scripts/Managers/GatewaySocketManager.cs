using System;
using UnityEngine;
public class GatewaySocketManager : TCPSocketManagerBase<GatewaySocketManager>
{
    [field: SerializeField] public bool IsResponse { get; set; } = false;
    [field: SerializeField] public bool IsLoginSuccess { get; set; } = false;
    [field: SerializeField] public bool LoginInProgress { get; set; } = false;
    [SerializeField] private string customGatewayIP = "3.38.96.26";

    private void Start()
    {
        //port = (int)EPort.Gateway;

        Init(ip, port);
        Connect();
    }

    #region Gateway_Response

    public void LoginResponse(GamePacket gamePacket)
    {
        if (!LoginInProgress) return;
        var response = gamePacket.LoginResponse;
        IsResponse = true;
        Debug.Log($"LoginfailCode : {response.GlobalFailCode}\nUserId : {response.UserId}\nToken : {response.Token}");
        if (response.GlobalFailCode == GlobalFailCode.AuthenticationFailed) IsLoginSuccess = false;
        else
        {
            IsLoginSuccess = true;
            NetworkManager.Instance.Token = response.Token;
            NetworkManager.Instance.UserId = response.UserId;
        }
        LoginInProgress = false;
    }
    #endregion
}