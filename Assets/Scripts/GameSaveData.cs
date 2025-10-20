using System;
using System.Collections.Generic;
using UnityEngine;

// Serializable save data for the entire game
[Serializable]
public class GameSaveData
{
    // Player State
    public SerializableVector3 playerPosition;
    public SerializableQuaternion playerRotation;
    public int playerHP;
    public bool isInCombat;


    // Inventory
    [Serializable]
    public class InventoryItemData
    {
        public string tagName;
        public int quantity;
    }
    public List<InventoryItemData> inventoryItems = new List<InventoryItemData>();
    public int selectedHotbarSlot = -1;


    // Ammo
    public int totalAmmo; // from Shooter script
    public int currentMag;


    // World State

    public List<string> pickedUpItemIDs = new List<string>(); // store unique IDs or names of pickups
    public List<string> openedDoorsOrIntercoms = new List<string>(); // store unique IDs/tags

    // Player Progress
    public string currentLevel;

    // Settings
    public float masterVolume;
    public float brightness;
    public float fov;

    //intercomController
    public Dictionary<string, bool> intercomStates = new Dictionary<string, bool>();

    //Enemy
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
    public int id;
    public int hp;
    public bool isAlive;
    public string currentState; // optional: idle, patrol, chase, attack
    public List<GameSaveData> enemies = new List<GameSaveData>();

}

// Helper structs for serializing Vector3/Quaternion
[Serializable]
public struct SerializableVector3
{
    public float x, y, z;
    public SerializableVector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
    public SerializableVector3(Vector3 v) { x = v.x; y = v.y; z = v.z; }
    public Vector3 ToVector3() => new Vector3(x, y, z);
}

[Serializable]
public struct SerializableQuaternion
{
    public float x, y, z, w;
    public SerializableQuaternion(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
    public SerializableQuaternion(Quaternion q) { x = q.x; y = q.y; z = q.z; w = q.w; }
    public Quaternion ToQuaternion() => new Quaternion(x, y, z, w);
}
