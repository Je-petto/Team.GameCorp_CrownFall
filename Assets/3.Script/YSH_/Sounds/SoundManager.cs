using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct SceneBGMEntry
{
    public string sceneName;      // 씬 이름
    public AudioClip bgmClip;     // 해당 씬의 BGM
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;

    [Header("Scene BGM Settings")]
    [Tooltip("씬 이름과 해당 씬의 BGM을 매핑합니다.")]
    public List<SceneBGMEntry> sceneBGMs = new List<SceneBGMEntry>();

    private Dictionary<string, AudioClip> sceneBGMMap = new Dictionary<string, AudioClip>();

    public AudioSource BGMaudio;
    public AudioSource SFXaudio;

    [SerializeField] public AudioClip ButtonClip;
    [SerializeField] public string buttonClipPath = "ButtonSound";

    [Header("Volume UI")]
    public GameObject soundUI;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("숨길 씬 이름")]
    public List<string> scenesToHideUI = new List<string> { "InGameScene" };

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // AudioSource 세팅
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length < 2)
            {
                BGMaudio = gameObject.AddComponent<AudioSource>();
                SFXaudio = gameObject.AddComponent<AudioSource>();
                BGMaudio.playOnAwake = false;
                SFXaudio.playOnAwake = false;
            }
            else
            {
                BGMaudio = sources[0];
                SFXaudio = sources[1];
            }

            BGMaudio.enabled = true;
            SFXaudio.enabled = true;

            // ButtonClip 로드
            if (ButtonClip == null)
            {
                ButtonClip = Resources.Load<AudioClip>(buttonClipPath);
            }

            // 볼륨 슬라이더 초기화
            if (bgmSlider != null)
            {
                bgmSlider.value = BGMaudio.volume;
                bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            }

            if (sfxSlider != null)
            {
                sfxSlider.value = SFXaudio.volume;
                sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            }

            // Dictionary 초기화
            foreach (var entry in sceneBGMs)
            {
                if (!sceneBGMMap.ContainsKey(entry.sceneName))
                    sceneBGMMap.Add(entry.sceneName, entry.bgmClip);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;

            if (soundUI != null)
                soundUI.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowSoundUI()
    {
        if (soundUI != null && !soundUI.activeSelf)
        {
            soundUI.SetActive(true);
            OnButtonSound();
        }
    }

    public void HideSoundUI()
    {
        if (soundUI != null && soundUI.activeSelf)
        {
            soundUI.SetActive(false);
            OnButtonSound();
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SoundManager] Scene loaded: {scene.name}");
        PlaySceneBgm();

        // 모든 버튼에 효과음 연결
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button btn in buttons)
        {
            if (btn.gameObject.scene.IsValid())
            {
                btn.onClick.RemoveListener(OnButtonSound);
                btn.onClick.AddListener(OnButtonSound);
            }
        }
    }

    public void PlaySceneBgm()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (sceneBGMMap.TryGetValue(currentSceneName, out AudioClip clip) && clip != null)
        {
            BGMaudio.Stop();
            BGMaudio.clip = clip;
            BGMaudio.loop = true;
            BGMaudio.Play();
        }
        else
        {
            Debug.LogWarning($"[{currentSceneName}]에 해당하는 BGM이 없습니다.");
        }
    }

    public void ButtonSoundsCall(GameObject parent)
    {
        Button[] buttons = parent.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(OnButtonSound);
            btn.onClick.AddListener(OnButtonSound);
        }
    }

    public void OnButtonSound()
    {
        if (SFXaudio == null || ButtonClip == null) return;

        if (!SFXaudio.enabled) SFXaudio.enabled = true;
        if (!SFXaudio.gameObject.activeInHierarchy) SFXaudio.gameObject.SetActive(true);

        if (SFXaudio.isActiveAndEnabled)
        {
            SFXaudio.PlayOneShot(ButtonClip, SFXaudio.volume);
        }
    }

    public void SetBGMVolume(float volume)
    {
        if (BGMaudio != null)
            BGMaudio.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (SFXaudio != null)
            SFXaudio.volume = volume;
    }
}
