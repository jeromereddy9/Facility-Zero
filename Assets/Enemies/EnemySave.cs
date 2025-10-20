using UnityEngine;
using System.Linq;

public class EnemySave : MonoBehaviour, ISavable
{
    private Enemy enemy;
    private Animator animator;

    //[Header("Unique ID")]
    //public int id; // assign manually in Inspector or via script

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();
    }

    public void SaveData(GameSaveData data)
    {
        if (enemy == null) return;

        var enemyData = new GameSaveData
        {
            id = enemy.id,
            position = new SerializableVector3(transform.position),
            rotation = new SerializableQuaternion(transform.rotation),
            hp = enemy.CurrentHP,
            isAlive = enemy.CurrentHP > 0,
            currentState = GetCurrentState()
        };

        data.enemies.Add(enemyData);
    }

    public void LoadData(GameSaveData data)
    {
        var enemyData = data.enemies.FirstOrDefault(e => e.id == this.enemy.id);
        if (enemyData == null) return;

        transform.position = enemyData.position.ToVector3();
        transform.rotation = enemyData.rotation.ToQuaternion();

        enemy.CurrentHP = enemyData.hp;

        if (!enemyData.isAlive)
        {
            Destroy(enemy.gameObject); // Or play death animation
        }
        else
        {
            SetAnimatorState(enemyData.currentState);
        }
    }

    private string GetCurrentState()
    {
        if (animator.GetBool("isPatrolling")) return "Patrolling";
        if (animator.GetBool("isChasing")) return "Chasing";
        if (animator.GetBool("isAttacking")) return "Attacking";
        return "Idle";
    }

    private void SetAnimatorState(string state)
    {
        animator.SetBool("isPatrolling", state == "Patrolling");
        animator.SetBool("isChasing", state == "Chasing");
        animator.SetBool("isAttacking", state == "Attacking");
    }
}
