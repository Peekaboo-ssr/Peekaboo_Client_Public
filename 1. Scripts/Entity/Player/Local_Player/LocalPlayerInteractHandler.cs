using System.Collections;
using UnityEngine;

public class LocalPlayerInteractHandler : MonoBehaviour
{ 
    [field:SerializeField] public GameObject CurInteractObj { get; private set; } // 현재 상호작용하는 오브젝트 
    [field: SerializeField] public bool CanInteractItem { get; private set; } // 아이템 상호작용 가능 여부
    [field: SerializeField] public bool CanInteractDoor { get; private set; } // 문 상호작용 가능 여부
    [field: SerializeField] public bool CanInteractGhost { get; private set; } // 귀신 상호작용 가능 여부
    [field: SerializeField] public bool CanInteractStore { get; private set; } // 상점 상호작용 가능 여부
    [field: SerializeField] public bool CanInteractExtract { get; private set; } // 추출 상점 상호작용 가능 여부
    [field: SerializeField] public bool CanInteractDiffSelector { get; private set; } // 난이도 상점 상호작용 가능 여부

    public Ghost targetGhost;

    [Header("Interact")]
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask ghostLayer;
    [SerializeField] private LayerMask storeLayer;
    [SerializeField] private LayerMask extractLayer;
    [SerializeField] private LayerMask diffSelectorLayer;
    [SerializeField] private float rayCheckDistance;
    [SerializeField] private float rayCheckDuration;

    private Item item;
    private Coroutine checkRayCoroutine;
    private WaitForSeconds rayCheckWaitForSeconds;

    private void Start()
    {
        rayCheckWaitForSeconds =  new WaitForSeconds(rayCheckDuration);
        GameManager.Instance.Player.EventHandler.OnInteractEvent += Interact;
    }

    private void Interact(EInteractType type)
    {
        switch(type)
        {
            case EInteractType.Item:
                GetItem();
                break;
            case EInteractType.Door:
                ToggleDoor();
                break;
            case EInteractType.Ghost:
                Attack();
                break;
            case EInteractType.Store:
                UIManager.Instance.OpenStoreUI();
                break;
            case EInteractType.Extract:
                UIManager.Instance.OpenExtractUI();
                break;
            case EInteractType.DiffSelector:
                UIManager.Instance.OpenDiffSelectUI();
                break;
        }
    }

    #region ray check

    public void StartCheckRay()
    {
        StopCheckRay();
        checkRayCoroutine = StartCoroutine(CheckRay());
    }

    public void StopCheckRay()
    {
        UIManager.Instance.UI_HUD.UI_PlayerInteract.ShowInteractText("");
        if (checkRayCoroutine != null)
            StopCoroutine(checkRayCoroutine);
    }

    private IEnumerator CheckRay()
    {
        while (true)
        {
            if (!GameManager.Instance.Player.gameObject.activeInHierarchy)
                yield break;
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            // 아이템과 상호작용 가능한 경우
            if (Physics.Raycast(ray, out hit, rayCheckDistance) && ExtensionMethods.IsSameLayer(itemLayer, hit.collider.gameObject.layer))
            {
                if (hit.collider.gameObject != CurInteractObj)
                {
                    CurInteractObj = hit.collider.gameObject;
                    item = CurInteractObj.GetComponent<Item>();
                }
                if (item.IsInteractable)
                {
                    UIManager.Instance.UI_HUD.UI_PlayerInteract.ShowInteractText("Get Item (E)");
                    CanInteractItem = true;
                }
            }
            // 문과 상호작용 가능한 경우
            else if (Physics.Raycast(ray, out hit, rayCheckDistance) && ExtensionMethods.IsSameLayer(doorLayer, hit.collider.gameObject.layer))
            {
                if (hit.collider.gameObject != CurInteractObj)
                    CurInteractObj = hit.collider.gameObject;
                UIManager.Instance.UI_HUD.UI_PlayerInteract.ShowInteractText("Open Door (E)");
                CanInteractDoor = true;
            }
            // 귀신과 상호작용 가능한 경우
            else if (Physics.Raycast(ray, out hit, rayCheckDistance) && ExtensionMethods.IsSameLayer(ghostLayer, hit.collider.gameObject.layer))
            {
                if (hit.collider.gameObject != CurInteractObj)
                    targetGhost = hit.collider.gameObject.GetComponent<Ghost>();
                if (targetGhost.IsDefeatable) // 퇴치 가능한 귀신인지 확인
                {
                    UIManager.Instance.UI_HUD.UI_PlayerInteract.ShowInteractText("Exorcise Ghost (E)");
                    CanInteractGhost = true;
                }
            }
            // 상점과 상호작용 가능한 경우
            else if (Physics.Raycast(ray, out hit, rayCheckDistance) 
                && ExtensionMethods.IsSameLayer(storeLayer, hit.collider.gameObject.layer)
                && !GameServerSocketManager.Instance.IsInStage
                )
            {
                if (hit.collider.gameObject != CurInteractObj)
                    CurInteractObj = hit.collider.gameObject;
                UIManager.Instance.UI_HUD.UI_PlayerInteract.ShowInteractText("Open Store (E)");
                CanInteractStore = true;
            }
            // 추출 상점과 상호작용 가능한 경우
            else if (Physics.Raycast(ray, out hit, rayCheckDistance) 
                && ExtensionMethods.IsSameLayer(extractLayer, hit.collider.gameObject.layer)
                && !GameServerSocketManager.Instance.IsInStage
                )
            {
                if (hit.collider.gameObject != CurInteractObj)
                    CurInteractObj = hit.collider.gameObject;
                UIManager.Instance.UI_HUD.UI_PlayerInteract.ShowInteractText("Open Extractor (E)");
                CanInteractExtract = true;
            }
            // 난이도 선택과 상호작용 가능한 경우
            else if (Physics.Raycast(ray, out hit, rayCheckDistance) 
                && ExtensionMethods.IsSameLayer(diffSelectorLayer, hit.collider.gameObject.layer)
                && !GameServerSocketManager.Instance.IsInStage
                )
            {
                if (hit.collider.gameObject != CurInteractObj)
                    CurInteractObj = hit.collider.gameObject;
                UIManager.Instance.UI_HUD.UI_PlayerInteract.ShowInteractText("Select Diff (E)");
                CanInteractDiffSelector = true;
            }
            else
            {
                UIManager.Instance.UI_HUD.UI_PlayerInteract.HideInteractText();
                CanInteractItem = false;
                CanInteractDoor = false;
                CanInteractGhost = false;
                CanInteractStore = false;
                CanInteractExtract = false;
                CanInteractDiffSelector = false;
            }
            yield return rayCheckWaitForSeconds;
        }
    }

    #endregion

    #region item
    // 아이템 줍기
    private void GetItem()
    {
        if (item != null && item.IsInteractable)
        {
            item.LocalPickUp();
            CurInteractObj = null;
        }
    }
    #endregion

    #region door
    // 열려있으면 닫기 <-> 닫혀있으면 열기
    private void ToggleDoor()
    {
        Door door = CurInteractObj.GetComponent<Door>();
        if(door != null)
        {
            door.ToggleRequest(GameManager.Instance.Player);
        }

        CurInteractObj = null;
    }
    #endregion

    #region Ghost
    private void Attack()
    {
        GameManager.Instance.Player.StateMachine.ChangeState(GameManager.Instance.Player.StateMachine.AttackState);
    }
    #endregion

}
