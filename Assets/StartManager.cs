using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [SerializeField] private Button start, sound;
    [SerializeField] private Slider soundSlider;

    [SerializeField] TMP_Dropdown rows, columns;

    [SerializeField] private Sprite soundOn,soundOff;

    [SerializeField] GameObject startPanel;

    [SerializeField] private GameManager manager;

    float inactiveTimer;
    private void OnEnable()
    {
        soundSlider.onValueChanged.AddListener(SoundValueChanged);

        start.onClick.AddListener(StartGame);
        sound.onClick.AddListener(OpenSound);
    }
    private void OnDisable()
    {
        soundSlider.onValueChanged.RemoveAllListeners();
    }
    private void Update()
    {
        inactiveTimer += Time.deltaTime;

        if (inactiveTimer >= 3f)
        {

            soundSlider.gameObject.SetActive(false);
        }

    }

    void SoundValueChanged(float value)
    {
        SoundManager.instance.SetVolume(value); 
        inactiveTimer = 0;
        if (value == 0)
        {
            sound.image.sprite = soundOff;
        }
        else
        {
            sound.image.sprite = soundOn;
        }
    }


    void OpenSound()
    {
        if (soundSlider.gameObject.activeSelf)
        {
            soundSlider.gameObject.SetActive(false);
        }
        else
        {
            inactiveTimer = 0;

            soundSlider.gameObject.SetActive(true);
        }
    }


    void StartGame()
    {
        startPanel.SetActive(false);
        SoundManager.instance.PlaySFX("button");
        manager.DistributeCards(int.Parse(rows.captionText.text),int.Parse(columns.captionText.text));
    }

    void ResetToStart()
    {

    }

}
