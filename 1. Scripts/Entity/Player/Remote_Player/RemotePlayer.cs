using UnityEngine;

public class RemotePlayer : Player
{
    public RemotePlayerNetworkHandler NetworkHandler { get; private set; }
    public RemotePlayerMovement Movement {  get; private set; }

    [SerializeField] private GameObject playerSight;
    [SerializeField] private Camera remoteCamera;
    [SerializeField] private AudioListener audioListener;

    public bool IsDie = false;  

    protected override void Awake()
    {
        base.Awake();
        NetworkHandler = GetComponent<RemotePlayerNetworkHandler>();
        Movement = GetComponent<RemotePlayerMovement>();
        audioListener = GetComponentInChildren<AudioListener>();

        RemotePlayerInit();
    }

    public void RemotePlayerInit()
    {
        NetworkHandler.Init(this);
        Movement.Init(this);
    }

    public void OnSpectate()
    {
        playerSight.SetActive(true);
        remoteCamera.enabled = true;
        audioListener.enabled = true;   
    }

    public void OffSpectate()
    {
        playerSight.SetActive(false);
        remoteCamera.enabled = false;
        audioListener.enabled = false;
    }

    protected override void PlayerDie(Player player)
    {
        base.PlayerDie(player);

        OffSpectate();
    }
}
