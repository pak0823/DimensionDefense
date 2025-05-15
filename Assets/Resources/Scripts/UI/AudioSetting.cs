using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        // 1) ���� ������ ������ �����̴��� �ݿ�
        bgmSlider.value = Shared.SoundManager.BGMMasterVolume;
        sfxSlider.value = Shared.SoundManager.SFXMasterVolume;

        // ����� �� �� �����̴�
        bgmSlider.value = PlayerPrefs.GetFloat("BGMMasterVolume", Shared.SoundManager.BGMMasterVolume);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXMasterVolume", Shared.SoundManager.SFXMasterVolume);

        // 2) �����̴� ���� �ٲ� ������ ȣ��ǵ��� ������ ���
        bgmSlider.onValueChanged.AddListener(Shared.SoundManager.SetMasterBGMVolume);
        sfxSlider.onValueChanged.AddListener(Shared.SoundManager.SetMasterSFXVolume);
    }
}
