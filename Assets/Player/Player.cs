using UnityEngine;

public class Player : MonoBehaviour
{
    public int HP = 300;

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            print("Player Dead");
        }
        else
        {
            print("Player Hit");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieAttackHand"))
        {
            TakeDamage(15);
        }
    }
}
