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
        SFX_Button,
        SFX_Walk,
        SFX_Rustle,
        SFX_Die,
    }

    [SerializeField] private AudioClip[] _bgmClips;
    [SerializeField] private AudioClip[] _sfxClips;
    [SerializeField] private AudioClip[] _voiceClips_Jangsanbeom;

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _voiceSource;

    private void Start()
    {
        Init();
        PlayBGM(BGMType.BGM_Lobby);
    }

    public void Init()
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
        _sfxSource.loop = false;
        _voiceSource.loop = false;
    }

    public void PlayBGM(BGMType bgmType)
    {
        _bgmSource.clip = _bgmClips[(int)bgmType];
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void PlaySFX(SFXType sfxType)
    {
        _sfxSource.PlayOneShot(_sfxClips[(int)sfxType]);
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
