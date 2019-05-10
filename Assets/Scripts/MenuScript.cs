using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public AudioMixer mixer;

    public GameObject menu, deathScreen;

    public Slider masterVol, musicVol, sfxVol;

    private bool isPaused = false;

    private float DbCoef(float cf) {
        float dB;
        if (Mathf.Abs(cf) > 0.0001f) dB = 20.0f * Mathf.Log10(cf);
        else dB = -144.0f;
        return dB;
    }

    public void SetMasterVol(float val) {
        mixer.SetFloat("MasterVolume", DbCoef(val));
    }

    public void SetMusicVol(float val) {
        mixer.SetFloat("MusicVolume", DbCoef(val));
    }

    public void SetSFXVol(float val) {
        mixer.SetFloat("SFXVolume", DbCoef(val));
    }

    public void ShowDeathScreen() {
        deathScreen.SetActive(true);
        menu.SetActive(false);
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Start is called before the first frame update
    void Start()
    {
        HideCursor();

        masterVol.onValueChanged.AddListener(SetMasterVol);
        musicVol.onValueChanged.AddListener(SetMusicVol);
        sfxVol.onValueChanged.AddListener(SetSFXVol);
    }


    void ShowCursor() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void HideCursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause() {
        Time.timeScale = 0.0f;
        menu.SetActive(true);
        isPaused = true;
        ShowCursor();
    }

    public void UnPause() {
        Time.timeScale = 1.0f;
        menu.SetActive(false);
        isPaused = false;
        HideCursor();
    }

    private void togglePause() {
        if (isPaused) {
            UnPause();
        } else {
            Pause();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            togglePause();
        }
    }
}
