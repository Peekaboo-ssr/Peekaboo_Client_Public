using UnityEngine;

public class RemotePlayerMovement : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }

    private RemotePlayer player;
    private Vector3 targetPosition;
    private Quaternion targetBodyRotation;
    private Quaternion targetCamRotation;

    private void Awake()
    {
        Rigidbody = GetComponentInChildren<Rigidbody>();
    }

    private void Start()
    {
        player.EventHandler.OnMoveEvent += UpdateTargetPosition;
        player.EventHandler.OnLookEvent += UpdateTargetRotation;
        player.EventHandler.OnJumpEvent += Jump;
    }

    public void Init(RemotePlayer player)
    {
        this.player = player;
    }

    private void Update()
    {
        Rigidbody.MovePosition(Vector3.Lerp(Rigidbody.position, targetPosition, Time.deltaTime * 10f));
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetBodyRotation, Time.deltaTime * 10f);
        player.CameraTransform.localRotation = Quaternion.Slerp(player.CameraTransform.localRotation, targetCamRotation, Time.deltaTime * 10f);
    }

    public void UpdateTargetPosition(Vector3 position)
    {
        targetPosition = new Vector3(position.x, transform.position.y, position.z);       
    }

    public void UpdateTargetRotation(Vector3 rotation)
    {
        targetBodyRotation = Quaternion.Euler(new Vector3(0, rotation.y, 0));
        targetCamRotation = Quaternion.Euler(new Vector3(rotation.x, 0, 0));
    }

    public void Jump()
    {
        Rigidbody.AddForce(Vector3.up * player.PlayerSO.JumpForce, ForceMode.Impulse);
    }
}
