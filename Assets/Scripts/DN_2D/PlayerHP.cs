using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public int maxHP = 3;
    public int currentHP;

    public GameObject gameOverText;

    void Start()
    {
        currentHP = maxHP;

        // 시작할 때 게임오버 텍스트 숨김
        gameOverText.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log("플레이어 체력 : " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("게임 오버");

        gameOverText.SetActive(true);

        // 플레이어 움직임 정지
        Time.timeScale = 0f;
    }
}
