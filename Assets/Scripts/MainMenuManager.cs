using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private VolumeProfile globalVolume;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Transform settingsTransform;
    [SerializeField] private Transform playTransform;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private AudioSource impactSoft;
    [SerializeField] private AudioSource impactHeavy;
    [SerializeField] private GameObject difficultyCanvas;

    [Header("SETTINGS")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeValueText;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TextMeshProUGUI brightnessValueText;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private int qualityIndex;
    private Resolution[] resolutions;
    private int currentResolutionIndex;
    private List<string> options = new List<string>();
    private bool isFullscreen = true;
    private bool inSettings = false;
    private int enemyCount;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + "@" + (int)resolutions[i].refreshRateRatio.value;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);


        LoadSettings();
    }

    public void PlayButton()
    {
        PlayRandomPitch(impactHeavy, false);

        MoveCamera();
    }
    private void MoveCamera()
    {
        mainCamera.transform.DOMove(playTransform.position, moveDuration);
        mainCamera.transform.DORotate(playTransform.eulerAngles, moveDuration);

        difficultyCanvas.SetActive(true);
    }
    public void SettingsButton()
    {
        PlayRandomPitch(impactHeavy, false);

        inSettings = true;

        mainCamera.transform.DOMove(settingsTransform.position, moveDuration);
        mainCamera.transform.DORotate(settingsTransform.eulerAngles, moveDuration);
    }
    public void BackButton()
    {
        PlayRandomPitch(impactHeavy, false);

        inSettings = false;

        mainCamera.transform.DOMove(mainTransform.position, moveDuration);
        mainCamera.transform.DORotate(mainTransform.eulerAngles, moveDuration);

        ApplySettingsChanges();
    }
    public void QuitButton()
    {
        PlayRandomPitch(impactHeavy, false);

        Application.Quit();
    }
    public void DifficultyButton(int count)
    {
        PlayRandomPitch(impactHeavy, false);

        enemyCount = count;

        ApplySettingsChanges();

        StartCoroutine(BeginGame());
    }
    private IEnumerator BeginGame()
    {
        yield return new WaitForSeconds(0.25f);

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    private void ApplySettingsChanges()
    {
        PlayerPrefs.SetFloat("Volume", Mathf.Round(volumeSlider.normalizedValue * 10f) / 10f);
        print("Volume: " + PlayerPrefs.GetFloat("Volume"));

        PlayerPrefs.SetFloat("Sensitivity", Mathf.Round(sensitivitySlider.normalizedValue * 10f) / 10f);
        print("Sensitivity: " + PlayerPrefs.GetFloat("Sensitivity"));

        PlayerPrefs.SetFloat("Brightness", brightnessSlider.normalizedValue * 1.5f - 0.5f); // f(x) = 1.5x - 0.5
        print("Brightness: " + PlayerPrefs.GetFloat("Brightness"));

        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
        print("Resolution: " + resolutionDropdown.value);

        PlayerPrefs.SetInt("Quality", qualityIndex);
        print("Quality: " + PlayerPrefs.GetInt("Quality"));

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        print("Fullscreen: " + PlayerPrefs.GetInt("Fullscreen"));

        PlayerPrefs.SetInt("Difficulty", enemyCount);
        print("Difficulty: " + PlayerPrefs.GetInt("Difficulty"));
    }
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            volumeSlider.normalizedValue = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            volumeSlider.value = (volumeSlider.maxValue - volumeSlider.minValue) / 2;
        }
        volumeValueText.text = volumeSlider.value.ToString();

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity") * 100;
        }
        else
        {
            sensitivitySlider.value = 50;
        }
        sensitivityValueText.text = sensitivitySlider.value.ToString();

        if (PlayerPrefs.HasKey("Brightness"))
        {
            float brightness = PlayerPrefs.GetFloat("Brightness");
            brightnessSlider.normalizedValue = 2f * (brightness + 0.5f) * 0.33f; // f^-1(x) = 2(x + 0.5)/3
        }
        else
        {
            brightnessSlider.value = 33;
        }
        brightnessValueText.text = brightnessSlider.value.ToString();

        if (PlayerPrefs.HasKey("Resolution"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("Resolution");
            Resolution resolution = resolutions[currentResolutionIndex];
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        else
        {
            Resolution resolution = resolutions[currentResolutionIndex];
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        if (PlayerPrefs.HasKey("Quality"))
        {
            qualityIndex = PlayerPrefs.GetInt("Quality");
            QualitySettings.SetQualityLevel(qualityIndex);
        }
        else
        {
            qualityIndex = 2;
            QualitySettings.SetQualityLevel(qualityIndex);
        }
        graphicsDropdown.value = qualityIndex;

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            isFullscreen = PlayerPrefs.GetInt("Fullscreen") == 1 ? true : false;
            Screen.fullScreen = isFullscreen;
        }
        else
        {
            Screen.fullScreen = isFullscreen;
        }
        fullscreenToggle.isOn = isFullscreen;
    }
    private void PlayRandomPitch(AudioSource audioSource, bool isInSettings)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        if(!audioSource.isPlaying && (inSettings || !isInSettings))
            audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        PlayRandomPitch(impactSoft, true);

        volumeValueText.text = volume.ToString();
    }
    public void SetSensitivity(float sensitivity)
    {
        PlayRandomPitch(impactSoft, true);

        sensitivityValueText.text = sensitivity.ToString();
    }
    public void SetBrightness(float brightness)
    {
        PlayRandomPitch(impactSoft, true);

        brightnessValueText.text = brightness.ToString();

        if (globalVolume.TryGet(out LiftGammaGain lgg))
        {
            lgg.gamma.Override(new Vector4(1f, 1f, 1f, brightnessSlider.normalizedValue * 1.5f - 0.5f)); // f(x) = 1.5x - 0.5
        }
    }
    public void SetQuality(int qualityIndex)
    {
        PlayRandomPitch(impactSoft, true);

        this.qualityIndex = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetResolution(int resolutionIndex)
    {
        PlayRandomPitch(impactSoft, true);

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        PlayRandomPitch(impactSoft, true);

        this.isFullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }
}
