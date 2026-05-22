using UnityEngine;

public class MonsterDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHP playerHP = collision.GetComponent<PlayerHP>();

            if (playerHP != null)
            {
                playerHP.TakeDamage(damage);
            }
        }
    }
}