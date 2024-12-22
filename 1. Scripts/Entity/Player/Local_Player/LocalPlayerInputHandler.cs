using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayerInputHandler : MonoBehaviour
{
    public PlayerInput PlayerInput { get; private set; }

    private bool isCursorLock;
    private bool isWalk;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        UnLockCursor();
    }

    #region Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        LocalPlayer player = GameManager.Instance.Player;

        if (!isCursorLock && context.performed)
        {
            player.StateMachine.ChangeState(player.StateMachine.MoveState);
            player.EventHandler.CallMoveEvent(new Vector3(value.x, 0f, value.y));
            isWalk = true;
        }
        else if (context.canceled) 
        {
            player.StateMachine.ChangeState(player.StateMachine.IdleState);
            player.EventHandler.CallMoveEvent(Vector3.zero);
            isWalk = false;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LocalPlayer player = GameManager.Instance.Player;
        if (!isCursorLock)
        {
            player.EventHandler.CallLookEvent(context.ReadValue<Vector2>());
        }
        else
        {
            player.EventHandler.CallLookEvent(Vector2.zero);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        LocalPlayer player = GameManager.Instance.Player;
        if (!isCursorLock && context.started)
        {
            player.EventHandler.CallJumpEvent();
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        LocalPlayer player = GameManager.Instance.Player;
        if (!isCursorLock && context.performed)
        {
            player.StartDrainStamina(); 
        }
        else if (context.canceled)
        {
            if (isWalk)
                player.StateMachine.ChangeState(player.StateMachine.MoveState);
            else
                player.StateMachine.ChangeState(player.StateMachine.IdleState);

            player.StopDrainStamina();
            player.StartRecoveryStamina();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        LocalPlayer player = GameManager.Instance.Player;
        if (!isCursorLock && context.started)
        {
            if (player.InteractHandler.CanInteractItem)
            {
                player.EventHandler.CallInteractEvent(EInteractType.Item);
            }
            else if (player.InteractHandler.CanInteractDoor)
            {
                player.EventHandler.CallInteractEvent(EInteractType.Door);
            }
            else if (player.InteractHandler.CanInteractGhost)
            {
                player.EventHandler.CallInteractEvent(EInteractType.Ghost);
            }
            else if (player.InteractHandler.CanInteractStore)
            {
                player.EventHandler.CallInteractEvent(EInteractType.Store);
            }
            else if (player.InteractHandler.CanInteractExtract)
            {
                player.EventHandler.CallInteractEvent(EInteractType.Extract);
            }
            else if (player.InteractHandler.CanInteractDiffSelector)
            {
                player.EventHandler.CallInteractEvent(EInteractType.DiffSelector);
            }
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (!isCursorLock && context.started)
        {
            InventoryManager.Instance.Inventory.ThrowItemRequest();
        }
    }

    public void OnMouseScroll(InputAction.CallbackContext context)
    {
        if (!isCursorLock && context.started)
        {
            float scrollDeltaY = context.ReadValue<Vector2>().y;
            if (scrollDeltaY > 0)
            {
                InventoryManager.Instance.Inventory.ChangeSelectedItemRequest(true); // 휠 위로 올렸을 때 -> 인벤토리 왼쪽 이동
            }
            else if (scrollDeltaY < 0)
            {
                InventoryManager.Instance.Inventory.ChangeSelectedItemRequest(false); // 휠 아래로 올렸을 때 -> 인벤토리 오른쪽 이동
            }
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (!isCursorLock && context.started)
        {
            InventoryManager.Instance.Inventory.UseItemRequest();
        }
    }

    public void OnSetting(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //if (!isCursorLock)
            //    UIManager.Instance.OpenSettingUI();
            //else
            //    UIManager.Instance.OpenHUDUI();
        }
    }
    #endregion

    #region Cursor
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLock = true;
    }

    public void UnLockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLock = false;
    }
    #endregion
}