using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionManager : Singleton<OptionManager>
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    
    // 현재 볼륨 값 (0.0 ~ 1.0)
    private float bgmVolume = 0.8f;
    private float sfxVolume = 0.8f;
    private float voiceVolume = 0.8f;
    
    // UI 패널
    [SerializeField] private GameObject optionPanel;

    // 사운드 볼륨 슬라이더
    private Slider bgmSlider;
    private Slider sfxSlider;
    private Slider voiceSlider;

    // 튜토리얼 옵션
    private Toggle tutorialToggle;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == this)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
            LoadSettings();  // 저장된 설정 로드
            CreateOptionPanel();  // UI 생성
        }
    }

    private void Start()
    {
        ApplySettings();  // 설정 적용
    }

    #region Settings Data Management
    
    // 설정 저장
    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("VoiceVolume", voiceVolume);
        PlayerPrefs.Save();
        Debug.Log("OptionManager: 설정 저장 완료");
    }

    // 튜토리얼 설정 저장
    private void SaveTutorialSetting(bool isShown)
    {
        PlayerPrefs.SetInt("IsTutorialShown", isShown ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"OptionManager: 튜토리얼 설정 저장 - {isShown}");
    }

    // 설정 로드
    private void LoadSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 0.8f);
        Debug.Log("OptionManager: 설정 로드 완료");
    }

    // 설정 적용
    private void ApplySettings()
    {
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
        SetVoiceVolume(voiceVolume);
    }

    #endregion

    #region Volume Control

    public void SetBGMVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        if (audioMixer != null)
        {
            // 0~1 값을 -80~0 dB로 변환
            float db = bgmVolume > 0.0001f ? Mathf.Log10(bgmVolume) * 20 : -80f;
            audioMixer.SetFloat("BGM", db);
        }
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        if (audioMixer != null)
        {
            float db = sfxVolume > 0.0001f ? Mathf.Log10(sfxVolume) * 20 : -80f;
            audioMixer.SetFloat("SFX", db);
        }
    }

    public void SetVoiceVolume(float value)
    {
        voiceVolume = Mathf.Clamp01(value);
        if (audioMixer != null)
        {
            float db = voiceVolume > 0.0001f ? Mathf.Log10(voiceVolume) * 20 : -80f;
            audioMixer.SetFloat("Voice", db);
        }
    }

    // UI에서 호출 (Slider.onValueChanged)
    public void OnBGMSliderChanged(float value)
    {
        SetBGMVolume(value);
        SaveSettings();  // 즉시 저장
    }

    public void OnSFXSliderChanged(float value)
    {
        SetSFXVolume(value);
        SaveSettings();
    }

    public void OnVoiceSliderChanged(float value)
    {
        SetVoiceVolume(value);
        SaveSettings();
    }

    #endregion

    #region Tutorial Settings

    // UI에서 호출 (Toggle.onValueChanged)
    public void OnTutorialToggleChanged(bool isOn)
    {
        SaveTutorialSetting(isOn);
    }

    // 튜토리얼 표시 여부 확인
    public bool ShouldShowTutorial()
    {
        // IsTutorialShown이 false면 튜토리얼을 보여줘야 함
        return PlayerPrefs.GetInt("IsTutorialShown", 0) == 0;
    }

    #endregion

    #region UI Management

    private void CreateOptionPanel()
    {
        if (optionPanel != null) return;

        // 프리팹에서 로드 시도
        GameObject prefab = Resources.Load<GameObject>("UI/OptionPanel");
        
        if (prefab != null)
        {
            // Canvas 생성
            GameObject canvasObj = new GameObject("OptionCanvas");
            canvasObj.transform.SetParent(transform);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 900;
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // 프리팹 인스턴스화
            optionPanel = Instantiate(prefab, canvasObj.transform);
            
            // UI 컴포넌트 찾기
            FindUIComponents();
            
            optionPanel.SetActive(false);
            Debug.Log("OptionManager: OptionPanel 로드 완료");
        }
        else
        {
            Debug.LogWarning("OptionManager: OptionPanel 프리팹을 찾을 수 없습니다!");
        }
    }

    private void FindUIComponents()
    {
        if (optionPanel == null) return;

        // Slider 찾기 (이름으로 검색)
        bgmSlider = optionPanel.transform.Find("SoundTab/BGMSlider")?.GetComponent<Slider>();
        sfxSlider = optionPanel.transform.Find("SoundTab/SFXSlider")?.GetComponent<Slider>();
        voiceSlider = optionPanel.transform.Find("SoundTab/VoiceSlider")?.GetComponent<Slider>();
        tutorialToggle = optionPanel.transform.Find("TutorialTab/TutorialToggle")?.GetComponent<Toggle>();

        // 이벤트 연결
        if (bgmSlider != null)
        {
            bgmSlider.value = bgmVolume;
            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }
        if (voiceSlider != null)
        {
            voiceSlider.value = voiceVolume;
            voiceSlider.onValueChanged.AddListener(OnVoiceSliderChanged);
        }
        if (tutorialToggle != null)
        {
            tutorialToggle.isOn = PlayerPrefs.GetInt("IsTutorialShown", 0) == 1;
            tutorialToggle.onValueChanged.AddListener(OnTutorialToggleChanged);
        }
    }

    public void ShowOptionPanel()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
            
            // 현재 값으로 UI 업데이트
            if (bgmSlider != null) bgmSlider.value = bgmVolume;
            if (sfxSlider != null) sfxSlider.value = sfxVolume;
            if (voiceSlider != null) voiceSlider.value = voiceVolume;
            if (tutorialToggle != null) tutorialToggle.isOn = PlayerPrefs.GetInt("IsTutorialShown", 0) == 1;
        }
    }

    public void HideOptionPanel()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }
    }

    public void ToggleOptionPanel()
    {
        if (optionPanel != null)
        {
            bool isActive = optionPanel.activeSelf;
            if (isActive)
                HideOptionPanel();
            else
                ShowOptionPanel();
        }
    }

    #endregion

    #region Properties (다른 Manager에서 접근 가능)

    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;
    public float VoiceVolume => voiceVolume;
    public bool IsTutorialShown => PlayerPrefs.GetInt("IsTutorialShown", 0) == 1;

    #endregion
}