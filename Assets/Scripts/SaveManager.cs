using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;

namespace FacilityZero.Manager
{
    public class saveManager : MonoBehaviour
    {
        public static saveManager Instance { get; private set; }
        private string saveFilePath => Path.Combine(Application.persistentDataPath, "savegame.json");

        [Header("Optional UI Button Reference")]
        public Button loadButton; // Assign your load button in the Inspector (optional)

        private GameSaveData _pendingLoadData = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // Optional: Hook up button if assigned
                if (loadButton != null)
                    loadButton.onClick.AddListener(LoadGame);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            // Press 'O' to save the game
            if (Input.GetKeyDown(KeyCode.O))
            {
                SaveGame();
            }
        }

        // ---------------- SAVE ----------------
        public void SaveGame()
        {
            var saveData = new GameSaveData();

            // Save global settings
            saveData.masterVolume = AudioListener.volume;
            saveData.currentLevel = SceneManager.GetActiveScene().name;

            // Let all ISavable objects populate the data
            ISavable[] savables = FindAllSavables();
            foreach (var s in savables)
                s.SaveData(saveData);

            // Write JSON to disk
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, json);

            Debug.Log($"? Game saved to: {saveFilePath}");
        }

        // ---------------- LOAD ----------------
        public void LoadGame()
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.LogWarning("?? No save file found!");
                return;
            }

            string json = File.ReadAllText(saveFilePath);
            GameSaveData loaded = JsonUtility.FromJson<GameSaveData>(json);

            // If scene differs, load it first then apply the save
            if (!string.IsNullOrEmpty(loaded.currentLevel) && SceneManager.GetActiveScene().name != loaded.currentLevel)
            {
                _pendingLoadData = loaded;
                SceneManager.sceneLoaded += OnSceneLoadedApplySave;
                SceneManager.LoadScene(loaded.currentLevel);
            }
            else
            {
                ApplySaveDataToScene(loaded);
            }
        }

        private void OnSceneLoadedApplySave(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedApplySave;

            if (_pendingLoadData != null)
            {
                ApplySaveDataToScene(_pendingLoadData);
                _pendingLoadData = null;
            }
        }

        private void ApplySaveDataToScene(GameSaveData data)
        {
            // Apply global settings
            AudioListener.volume = data.masterVolume;

            // Restore all savable objects
            ISavable[] savables = FindAllSavables();
            foreach (var s in savables)
                s.LoadData(data);

            Debug.Log("? Save data successfully applied to scene.");
        }

        // Utility: find all ISavable objects in scene
        private ISavable[] FindAllSavables()
        {
            List<ISavable> list = new List<ISavable>();
            MonoBehaviour[] all = FindObjectsOfType<MonoBehaviour>();
            foreach (var mb in all)
                if (mb is ISavable s) list.Add(s);
            return list.ToArray();
        }
    }

}