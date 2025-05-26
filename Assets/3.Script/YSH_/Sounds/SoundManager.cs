using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;

    // 씬 인덱스에 맞는 BGM 클립을 저장하는 리스트.
    // 인덱스 0은 씬 0번의 BGM, 인덱스 1은 씬 1번의 BGM.
    public List<AudioClip> scenesClip = new List<AudioClip>();

    // BGM(배경음악)을 재생하는 AudioSource
    public AudioSource BGMaudio;
    // SFX(효과음)을 재생하는 AudioSource
    public AudioSource SFXaudio;
    // 버튼 클릭 효과음 클립

    [SerializeField] public AudioClip ButtonClip;
    // 버튼 효과음 클립의 Resources 폴더 내 경로 (Inspector에서 설정 가능)
    [SerializeField] public string buttonClipPath = "ButtonSound"; // 기본 경로 설정 (Resources 폴더 내)

    [Header("Volume UI")]
    public GameObject soundUI; // SoundUI 전체
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

            // AudioSource가 없다면 생성
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length < 2)
            {
                BGMaudio = gameObject.AddComponent<AudioSource>();
                BGMaudio.playOnAwake = false;

                SFXaudio = gameObject.AddComponent<AudioSource>();
                SFXaudio.playOnAwake = false;
            }
            else
            {
                BGMaudio = sources[0];
                SFXaudio = sources[1];
            }

            // AudioSource 무조건 활성화
            BGMaudio.enabled = true;
            SFXaudio.enabled = true;

            // ButtonClip 로드
            if (ButtonClip == null)
            {
                ButtonClip = Resources.Load<AudioClip>(buttonClipPath);
                if (ButtonClip == null)
                {
                    Debug.LogWarning("ButtonClip 로드 실패! 경로 확인 필요: " + buttonClipPath);
                }
            }

            // 슬라이더 초기값 세팅 및 이벤트 연결
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

            // 씬 로드 이벤트 연결
            SceneManager.sceneLoaded += OnSceneLoaded;

            // 게임 시작 시 사운드 UI 숨기기
            if (soundUI != null)
            {
                soundUI.SetActive(false);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 사운드 UI를 보이게 함
    public void ShowSoundUI()
    {
        if (soundUI != null && !soundUI.activeSelf)
        {
            soundUI.SetActive(true);
            OnButtonSound(); // 효과음 추가
        }
    }

    // 사운드 UI를 숨김
    public void HideSoundUI()
    {
        if (soundUI != null && soundUI.activeSelf)
        {
            soundUI.SetActive(false);
            OnButtonSound(); // 효과음 추가
        }
    }

    // 씬이 로드될 때마다 호출되는 함수입니다.
    public void OnSceneLoaded(Scene scene, LoadSceneMode loadScene)
    {
        if (SFXaudio == null)
        {
            Debug.LogError("SFXaudio가 null입니다!");
            return;
        }

        // 씬 로드 시 SFXaudio가 활성화되어 있는지 체크
        if (!SFXaudio.gameObject.activeInHierarchy)
        {
            SFXaudio.gameObject.SetActive(true);
        }

        if (!SFXaudio.enabled)
        {
            SFXaudio.enabled = true;
        }

        // BGM이 정상적으로 작동하는지 확인하고, PlaySceneBgm() 호출
        PlaySceneBgm(); // 새로 로드된 씬에 맞는 BGM을 항상 재생

        // 버튼에 효과음 추가
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
    public void ButtonSoundsCall(GameObject parent)
    {
        Button[] buttons = parent.GetComponentsInChildren<Button>(true); // 비활성 포함
        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(OnButtonSound); // 중복 방지
            btn.onClick.AddListener(OnButtonSound);
        }
    }

    // 현재 씬에 맞는 BGM을 재생하는 함수입니다.
    public void PlaySceneBgm()
    {
        // 현재 활성화된 씬의 인덱스를 가져옵니다.
        int Scene_i = SceneManager.GetActiveScene().buildIndex;

        // 씬 인덱스가 scenesClip 리스트의 범위 내에 있고, 해당 씬의 BGM 클립이 null이 아닌 경우
        if (Scene_i < scenesClip.Count && scenesClip[Scene_i] != null)
        {
            BGMaudio.Stop();                         // 기존에 재생 중인 BGM을 정지합니다.
            BGMaudio.clip = scenesClip[Scene_i];      // 해당 씬의 BGM 클립으로 설정합니다.
            BGMaudio.loop = true;                     // BGM을 반복 재생하도록 설정합니다.
            BGMaudio.Play();                         // BGM을 재생합니다.
        }
    }

    // 버튼 클릭 효과음을 재생하는 함수입니다.
    public void OnButtonSound()
    {
        if (SFXaudio == null)
        {
            Debug.LogError("SFXaudio가 null입니다!");
            return;
        }

        // AudioSource가 비활성화된 경우 강제로 활성화
        if (!SFXaudio.enabled)
        {
            SFXaudio.enabled = true;
        }

        // AudioSource가 속한 게임 오브젝트가 비활성화된 경우 활성화
        if (!SFXaudio.gameObject.activeInHierarchy)
        {
            SFXaudio.gameObject.SetActive(true);
        }

        // PlayOneShot 전에 AudioSource가 정상적으로 활성화되었는지 다시 확인
        if (SFXaudio.isActiveAndEnabled && !SFXaudio.isPlaying) // AudioSource가 활성화되고 재생 중이 아닌 경우에만 재생
        {
            if (ButtonClip != null)
            {
                SFXaudio.PlayOneShot(ButtonClip, SFXaudio.volume); // 볼륨을 3으로 설정 (조정 가능)
            }
            else
            {
                Debug.LogWarning("ButtonClip이 null입니다!");
            }
        }
    }
    
    // BGM 볼륨을 슬라이더로 조절하는 함수
    public void SetBGMVolume(float volume)
    {
        if (BGMaudio != null)
            BGMaudio.volume = volume;
    }

    // SFX 볼륨을 슬라이더로 조절하는 함수
    public void SetSFXVolume(float volume)
    {
        if (SFXaudio != null)
            SFXaudio.volume = volume;
    }
}