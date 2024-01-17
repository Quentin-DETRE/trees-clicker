using UnityEngine;
using UnityEngine.UI;


public class UIManager : BaseManager
{
    public static UIManager Instance;

    private GameObject currentUI;
    private GameObject pauseUI;

    [SerializeField]
    private GameObject startUIPrefab;
    [SerializeField]
    private GameObject gameUIPrefab;
    [SerializeField]
    private GameObject pauseUIPrefab;

    public Slider masterSlider;
    public Slider musiqueSlider;
    public Slider SFXSlider;
    private void Awake()
    {
        if (!CheckSingletonInstance(this, ref Instance))
        {
            return; // Instance déjà existante, la nouvelle est détruite
        }
    }

    private void Start()
    {
        GameManager.OnStateChanged += HandleStateChange;
        HandleStateChange(GameManager.Instance.State);
    }

    private void OnDestroy()
    {
        GameManager.OnStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Start:
                LoadUI(startUIPrefab);
                break;
            case GameState.Playing:
                LoadUI(gameUIPrefab);
                HidePauseMenu();
                break;
            case GameState.Pause:
                ShowPauseMenu();
                break;
        }
    }

    private void LoadUI(GameObject uiPrefab)
    {
        if (currentUI != null && GameManager.Instance.State != GameState.Pause)
        {
            Destroy(currentUI);
        }

        if (uiPrefab != null)
        {
            currentUI = Instantiate(uiPrefab);
            if (uiPrefab == startUIPrefab)
            {
                masterSlider = currentUI.transform.Find("SettingPanel/OpaqueBackground/MasterVolume").GetComponent<Slider>();
                masterSlider.value = OptionsManager.Instance.masterSliderValue;
                musiqueSlider = currentUI.transform.Find("SettingPanel/OpaqueBackground/MusicVolume").GetComponent<Slider>();
                musiqueSlider.value = OptionsManager.Instance.musiqueSliderValue;
                SFXSlider = currentUI.transform.Find("SettingPanel/OpaqueBackground/SFXVolume").GetComponent<Slider>();
                SFXSlider.value = OptionsManager.Instance.SFXSliderValue;
            }
            if (uiPrefab == gameUIPrefab)
            {

            }
            if (uiPrefab == pauseUIPrefab)
            {
                masterSlider = currentUI.transform.Find("OpaqueBackground/MasterVolume").GetComponent<Slider>();
                masterSlider.value = OptionsManager.Instance.masterSliderValue;
                musiqueSlider = currentUI.transform.Find("OpaqueBackground/MusicVolume").GetComponent<Slider>();
                musiqueSlider.value = OptionsManager.Instance.musiqueSliderValue;
                SFXSlider = currentUI.transform.Find("OpaqueBackground/SFXVolume").GetComponent<Slider>();
                SFXSlider.value = OptionsManager.Instance.SFXSliderValue;

            }
        }
    }

    private void ShowPauseMenu()
    {
        if (pauseUI == null && pauseUIPrefab != null)
        {
            LoadUI(pauseUIPrefab);
            DontDestroyOnLoad(pauseUI);
        }

        pauseUI.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        GameManager.Instance.UpdateState(GameState.Playing);
    }
    
    public void StartGame()
    {
        GameManager.Instance.UpdateState(GameState.Playing);
    }

    public void MenuGame()
    {
        GameManager.Instance.UpdateState(GameState.Start);
    }

    public void PauseGame()
    {
        GameManager.Instance.UpdateState(GameState.Pause);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}