using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class SoundManager : Singleton<SoundManager>
{
    public enum BGMType
    {
        BGM_Lobby,
        BGM_Game,
    }

    public enum SFXType
    {
        Player_Footwalk,
        Player_MakeItem,
        Player_Button,
    }

    [SerializeField] private AudioClip[] _bgmClips;
    [SerializeField] private AudioClip[] _sfxClips_Player;
    [SerializeField] private AudioClip[] _sfxClips_Enemy;
    [SerializeField] private AudioClip[] _voiceClips_Jangsanbeom;

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _voiceSource;

    protected override void Awake()
    {
        base.Awake();

        if(Instance == this)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
            InitSoundManager();
        }
    }

    private void Start()
    {
        PlayBGM(BGMType.BGM_Lobby);
    }

    public void InitSoundManager()
    {
        if(_bgmSource == null)
        {
            GameObject audioObject = new GameObject("@Sound_BGM");
            audioObject.transform.parent = transform;
            _bgmSource = audioObject.AddComponent<AudioSource>();
        }
        else
            _bgmSource.gameObject.name = "@Sound_BGM";

        if(_sfxSource == null)
        {
            GameObject audioObject = new GameObject("@Sound_SFX");
            audioObject.transform.parent = transform;
            _sfxSource = audioObject.AddComponent<AudioSource>();
        }
        else
            _sfxSource.gameObject.name = "@Sound_SFX";

        if(_voiceSource == null)
        {
            GameObject audioObject = new GameObject("@Sound_Voice");
            audioObject.transform.parent = transform;
            _voiceSource = audioObject.AddComponent<AudioSource>();
        }
        else
            _voiceSource.gameObject.name = "@Sound_Voice";

        _bgmSource.loop = true;
        _bgmSource.volume = 1.0f;
        _sfxSource.loop = false;
        _sfxSource.volume = 1.0f;
        _voiceSource.loop = false;
        _voiceSource.volume = 1.0f;
    }

    public void PlayBGM(BGMType bgmType)
    {
        if(_bgmSource.isPlaying)
            _bgmSource.Stop();

        _bgmSource.clip = _bgmClips[(int)bgmType];
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        if (_bgmSource.isPlaying)
            _bgmSource.Stop();
    }

    public void PlaySFX(SFXType sfxType)
    {
        _sfxSource.PlayOneShot(_sfxClips_Player[(int)sfxType]);
    }

    public void PlaySFX(EnemyType enemyType)
    {
        _sfxSource.PlayOneShot(_sfxClips_Enemy[(int)enemyType]);
    }

    public void PlayVoice_Jangsanbeom()
    {
        int random = UnityEngine.Random.Range(0, _voiceClips_Jangsanbeom.Length);
        _voiceSource.PlayOneShot(_voiceClips_Jangsanbeom[random]);
    }

    public void TestVoice()
    {
        PlayVoice_Jangsanbeom();
    }
}
