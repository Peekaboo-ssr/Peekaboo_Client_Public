using UnityEngine;
using UnityEngine.InputSystem;

public class SpectateInputHandler : MonoBehaviour
{
    private SpectateHandler spectateHandler;

    private void Awake()
    {
        spectateHandler = GetComponent<SpectateHandler>();
    }

    public void OnSetting(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // UI Setting 띄우기
        }
    }

    public void OnPageUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            spectateHandler.TranslateCamera(true);
        }
    }

    public void OnPageDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            spectateHandler.TranslateCamera(false);
        }
    }
}
