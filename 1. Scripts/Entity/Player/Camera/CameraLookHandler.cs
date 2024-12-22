using TMPro;
using UnityEngine;

public class CameraLookHandler : MonoBehaviour
{
    [Header("# Camera 설정 값")]
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;

    [Header("Player Head")]
    [SerializeField] private Transform head;

    private Vector3 lookDir;
    private float camCurXRot;
    private LocalPlayer player;

    private void Awake()
    {
        player = GetComponentInParent<LocalPlayer>();
    }

    private void Start()
    {
        player.EventHandler.OnLookEvent += GetLookDir;
    }

    private void LateUpdate()
    {
        if(gameObject.activeInHierarchy)
            CameraLook();
    }

    private void GetLookDir(Vector3 value)
    {
        lookDir = value;
    }

    private void CameraLook()
    {
        float lookSensitivity = player.StatHandler.CurrentStat.LookSensivity;

        // 카메라 y축 회전 -> 좌우 화전
        camCurXRot += lookDir.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        transform.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        // 플레어이 y축 회전 -> 좌우 회전
        player.transform.eulerAngles += new Vector3(0, lookDir.x * lookSensitivity, 0);
    }
}