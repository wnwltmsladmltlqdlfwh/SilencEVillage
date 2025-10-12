using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOption : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Toggle _bgmMuteToggle;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Toggle _sfxMuteToggle;
    [SerializeField] private Slider _voiceSlider;
    [SerializeField] private Toggle _voiceMuteToggle;

    private void Awake()
    {
        _bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        //_bgmMuteToggle.onValueChanged.AddListener(SetBGMMute);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        //_sfxMuteToggle.onValueChanged.AddListener(SetSFXMute);
        _voiceSlider.onValueChanged.AddListener(SetVoiceVolume);
        //_voiceMuteToggle.onValueChanged.AddListener(SetVoiceMute);
    }

    public void SetBGMVolume(float value)
    {
        _audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
        // TODO : 볼륨값 저장 
    }
    public void SetBGMMute(bool value)
    {
        _audioMixer.SetFloat("BGM", value ? -80 : 0);
    }

    public void SetSFXVolume(float value)
    {
        _audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
    }
    public void SetSFXMute(bool value)
    {
        _audioMixer.SetFloat("SFX", value ? -80 : 0);
    }


    public void SetVoiceVolume(float value)
    {
        _audioMixer.SetFloat("Voice", Mathf.Log10(value) * 20);
    }
    public void SetVoiceMute(bool value)
    {
        _audioMixer.SetFloat("Voice", value ? -80 : 0);
    }
}
