using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;
    [SerializeField] private bool isDontDestroy;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            if (transform.parent.childCount == 1)
            {
                Destroy(transform.parent);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (!isDontDestroy) return;

        if (transform.parent != null && transform.root != null) // �ش� ������Ʈ�� �ڽ� ������Ʈ���
        {
            DontDestroyOnLoad(this.transform.root.gameObject); // �θ� ������Ʈ�� DontDestroyOnLoad ó��
        }
        else
        {
            DontDestroyOnLoad(this.gameObject); // �ش� ������Ʈ�� �� ���� ������Ʈ��� �ڽ��� DontDestroyOnLoad ó��
        }
    }
}