using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [SerializeField] private Button start, sound, contnue, home; // UI buttons
    [SerializeField] private Slider soundSlider;                // Volume slider

    [SerializeField] TMP_Dropdown rows, columns;                // Dropdowns for selecting grid size

    [SerializeField] private Sprite soundOn, soundOff;          // Icons for sound button (on/off)

    [SerializeField] GameObject startPanel;                     // Start menu panel

    [SerializeField] private GameManager manager;               // Reference to GameManager

    float inactiveTimer; // Tracks inactivity for hiding the sound slider

    private void Awake()
    {
        CheckContinue(); // Enable/disable continue button based on save file
    }

    // Checks if a save file exists to enable the "Continue" button
    void CheckContinue()
    {
        if (SaveManager.DoesSaveFileExist())
        {
            contnue.gameObject.SetActive(true);
        }
        else
        {
            contnue.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // Add listeners for UI interactions
        soundSlider.onValueChanged.AddListener(SoundValueChanged);
        start.onClick.AddListener(StartGame);
        contnue.onClick.AddListener(ContinueGame);
        sound.onClick.AddListener(OpenSound);
        home.onClick.AddListener(ReturnHome);
    }

    private void OnDisable()
    {
        // Remove listeners when disabled to prevent memory leaks
        soundSlider.onValueChanged.RemoveAllListeners();
        start.onClick.RemoveAllListeners();
        contnue.onClick.RemoveAllListeners();
        sound.onClick.RemoveAllListeners();
        home.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        // Track inactivity and hide slider if unused for 3 seconds
        inactiveTimer += Time.deltaTime;
        if (inactiveTimer >= 3f)
        {
            soundSlider.gameObject.SetActive(false);
        }
    }

    // Called when the sound slider value changes
    void SoundValueChanged(float value)
    {
        SoundManager.instance.SetVolume(value);
        inactiveTimer = 0; // Reset inactivity timer

        // Update sound button icon depending on slider value
        if (value == 0)
        {
            sound.image.sprite = soundOff;
        }
        else
        {
            sound.image.sprite = soundOn;
        }
    }

    // Toggles the sound slider panel
    void OpenSound()
    {
        SoundManager.instance.PlaySFX("button");

        if (soundSlider.gameObject.activeSelf)
        {
            soundSlider.gameObject.SetActive(false);
        }
        else
        {
            inactiveTimer = 0; // Reset inactivity when opened
            soundSlider.gameObject.SetActive(true);
        }
    }

    // Continues a saved game
    void ContinueGame()
    {
        startPanel.SetActive(false);
        manager.ContinueGame();
        SoundManager.instance.PlaySFX("button");
    }

    // Starts a new game with selected rows & columns
    void StartGame()
    {
        startPanel.SetActive(false);
        SoundManager.instance.PlaySFX("button");
        manager.DistributeCards(int.Parse(rows.captionText.text), int.Parse(columns.captionText.text));
    }

    // Returns to home/start menu
    void ReturnHome()
    {
        SoundManager.instance.PlaySFX("button");
        startPanel.SetActive(true);
        manager.ResetGame();
        CheckContinue(); // Re-check if continue button should be enabled
    }
}
