using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        // 1) 현재 마스터 볼륨을 슬라이더에 반영
        bgmSlider.value = Shared.SoundManager.BGMMasterVolume;
        sfxSlider.value = Shared.SoundManager.SFXMasterVolume;

        // 저장된 값 → 슬라이더
        bgmSlider.value = PlayerPrefs.GetFloat("BGMMasterVolume", Shared.SoundManager.BGMMasterVolume);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXMasterVolume", Shared.SoundManager.SFXMasterVolume);

        // 2) 슬라이더 값이 바뀔 때마다 호출되도록 리스너 등록
        bgmSlider.onValueChanged.AddListener(Shared.SoundManager.SetMasterBGMVolume);
        sfxSlider.onValueChanged.AddListener(Shared.SoundManager.SetMasterSFXVolume);
    }
}
