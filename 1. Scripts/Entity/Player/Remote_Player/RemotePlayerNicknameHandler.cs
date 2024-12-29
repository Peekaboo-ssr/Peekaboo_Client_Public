using System.Collections;
using TMPro;
using UnityEngine;

public class RemotePlayerNicknameHandler : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI TXT_Nickname;

    private Coroutine nicknameCoroutine;
    private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(.1f);

    private void OnDisable()
    {
        StopNicknameCo();
    }

    public void Init(string nickName)
    {
        TXT_Nickname.enabled = false;
        TXT_Nickname.text = nickName;
    }

    public void StopNicknameCo()
    {
        if (nicknameCoroutine != null)
            StopCoroutine(NickNameCoroutine());

        TXT_Nickname.enabled = false;
    }

    public void StartNicknameCo()
    {
        StopNicknameCo();
        nicknameCoroutine = StartCoroutine(NickNameCoroutine());
    }
    
    private IEnumerator NickNameCoroutine()
    {
        TXT_Nickname.enabled = true;

        while(true)
        {
            if(GameManager.Instance.Player != null)
            {
                Vector3 dirToLocalPlayer = TXT_Nickname.transform.position- GameManager.Instance.Player.CameraTransform.position;
                dirToLocalPlayer.y = 0;
                TXT_Nickname.transform.rotation = Quaternion.LookRotation(dirToLocalPlayer);
            }

            yield return waitForSeconds;
        }
    }
}
