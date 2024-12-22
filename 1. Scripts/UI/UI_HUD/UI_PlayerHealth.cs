using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHealth : MonoBehaviour
{
    [SerializeField] private Dictionary<int, bool> hearts = new Dictionary<int, bool>(); // bool값 true면 alive
    [SerializeField] private Image[] healths = new Image[3];
    [SerializeField] private Color aliveColor;
    [SerializeField] private Color dieColor;

    private void Start()
    {
        UIManager.Instance.UI_HUD.UI_PlayerHealth = this;
        InitDictionary();
    }

    private void InitDictionary()
    {
        for(int i = 0; i < healths.Length; i++)
        {
            hearts[i] = true;
        }
    }

    public void InitUI(int value)
    {
        for (int i = 0; i < healths.Length; i++)
        {
            if (value <= 0)
            {
                healths[i].color = dieColor;
                continue;
            }

            healths[i].color = aliveColor;
            hearts[i] = true;
            value -= 1;
        }
    }

    public void UpdatePlayerHealthUI(int curHeart)
    {
        // 이미 죽은경우 return 
        if (GetCurHealthUI() <= 0)
            return;

        // 비활성화 시켜야 할 UI 개수
        int damage = GetCurHealthUI() - curHeart;

        // 배열의 오른쪽부터 검사. 
        for (int i = healths.Length - 1; i >= 0; i--)
        {
            if (damage <= 0)
                break;

            if (hearts[i])
            {
                healths[i].color = dieColor;
                hearts[i] = false;
                damage -= 1;
            }
        }
    }

    // 상점에서 Heart 구매시
    public void GainHeart(int value)
    {
        // 이미 Heart가 꽉찬 경우 return
        if (GetCurHealthUI() >= healths.Length)
            return;

        for (int i = healths.Length-1; i >=0; i--)
        {
            if (value <= 0)
                break;

            if (!hearts[i])
            {
                healths[i].color = aliveColor;
                hearts[i] = true;
                value -= 1;
            }
        }
    }

    // 현재 활성화 되어있는 Heart UI 개수
    private int GetCurHealthUI()
    {
        int cnt = 0;
        foreach (var playerHealth in hearts)
        {
            if (playerHealth.Value)
                cnt++;
        }
        return cnt;
    }
}
