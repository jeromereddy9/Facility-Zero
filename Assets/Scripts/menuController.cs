using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using TMPro;

public class menuController : MonoBehaviour
{

    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volumetextvalue = null;
    [SerializeField] private Slider volumeslider = null;
    [SerializeField] private float defaultValue = 20;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("level To Load")]
    public string _newGameLevel;
    private string LeveltoLoad;
    [SerializeField] private GameObject noSavedGameDialogue = null;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Text BrightnessText = null;
    [SerializeField] private Slider BrightnessSlider = null;
    [SerializeField] private int defaultBrightness = 1;

    private float _brightnessLevel;


    public void NewGameDialog()
    {

        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialogYES()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            LeveltoLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(LeveltoLoad);

        }
        else
        {
            noSavedGameDialogue.SetActive(true);

        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        volumetextvalue.text = value.ToString("0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());

    }
    public void setbrightness(float brightness)
    {
        _brightnessLevel = brightness;
        BrightnessText.text = brightness.ToString();
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("MasterBrightness", _brightnessLevel);

    }
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultValue;
            volumeslider.value = defaultValue;
            volumetextvalue.text = defaultValue.ToString("0");
            VolumeApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}

