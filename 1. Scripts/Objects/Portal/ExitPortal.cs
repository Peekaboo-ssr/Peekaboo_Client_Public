using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    public LayerMask targetLayer;
    private void OnCollisionEnter(Collision collision)
    {
        if(ExtensionMethods.IsSameLayer(targetLayer, collision.gameObject.layer))
        {
            if (collision.gameObject.TryGetComponent(out LocalPlayerNetworkHandler localPlayerNetworkHandler))
            {
                localPlayerNetworkHandler.StateChangeRequest(CharacterState.Exit);
                GameManager.Instance.CanRestartGame = true;
            }
        }
    }
}
