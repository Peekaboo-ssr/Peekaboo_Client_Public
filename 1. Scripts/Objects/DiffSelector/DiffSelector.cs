using UnityEngine;

public class DiffSelector : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnNextDay += Init;
        GameManager.Instance.DiffSelector = this;
    }

    private void Init()
    {
        gameObject.SetActive(true);
    }
}
