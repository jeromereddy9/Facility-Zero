using UnityEngine;
using FacilityZero.PlayerHealthController;

public class PlayerSave : MonoBehaviour, ISavable
{
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Called automatically by saveManager when saving
    public void SaveData(GameSaveData data)
    {
        // Save position and rotation
        data.playerPosition = new SerializableVector3(transform.position);
        data.playerRotation = new SerializableQuaternion(transform.rotation);

        // Save current HP (directly from your script)
        if (playerHealth != null)
            data.playerHP = playerHealth.HP;

        // Example: combat flag if you use it elsewhere
        data.isInCombat = false;
    }

    // Called automatically by saveManager when loading
    public void LoadData(GameSaveData data)
    {
        // Apply position and rotation
        transform.position = data.playerPosition.ToVector3();
        transform.rotation = data.playerRotation.ToQuaternion();

        // Restore HP
        if (playerHealth != null)
        {
            playerHealth.HP = Mathf.Clamp(data.playerHP, 0, playerHealth.maxHP);

            // Update UI (health bar & colors)
            if (playerHealth.GetComponent<UnityEngine.UI.Slider>() != null)
                playerHealth.Heal(0); // this re-syncs UI visuals without changing HP
        }
    }
}
