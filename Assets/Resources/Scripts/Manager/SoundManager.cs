using UnityEngine;
using System.Collections.Generic;
public class SoundManager : MonoBehaviour
{
    [Header("Sound Configurations")]
    public List<SoundData> sounds = new List<SoundData>();

    [Header("Master Volume Controls")]
    [Range(0f, 1f)] public float BGMMasterVolume = 1f;
    [Range(0f, 1f)] public float SFXMasterVolume = 1f;

    private Dictionary<string, SoundData> soundDictionary;

    private void Awake()
    {
        if (Shared.SoundManager == null)
        {
            Shared.SoundManager = this;
            DontDestroyOnLoad(gameObject);
            
            BGMMasterVolume = PlayerPrefs.GetFloat("BGMMasterVolume", BGMMasterVolume);
            SFXMasterVolume = PlayerPrefs.GetFloat("SFXMasterVolume", SFXMasterVolume);

            InitializeSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        Character.OnHit += HandleHit;
        ProjectileAttack.OnFireBoltAttack += HandleFireBolt;
        ProjectileAttack.OnArrowAttack += HandleArrow;
        RangeAttack.OnLightiningAttack += HandleLightning;
        RangeAttack.OnBoomAttack += HandleBoom;
    }

    private void OnDisable()
    {
        Character.OnHit -= HandleHit;
        ProjectileAttack.OnFireBoltAttack -= HandleFireBolt;
        ProjectileAttack.OnArrowAttack -= HandleArrow;
        RangeAttack.OnLightiningAttack -= HandleLightning;
        RangeAttack.OnBoomAttack -= HandleBoom;

    }

    private void InitializeSounds()
    {
        soundDictionary = new Dictionary<string, SoundData>();
        foreach (var sound in sounds)
        {
            // Create an AudioSource for each sound
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
            soundDictionary[sound.name] = sound;
        }

        ApplyAllVolumes();
    }

    private void ApplyAllVolumes()
    {
        foreach (var s in sounds)
        {
            float master = (s.category == SoundCategory.BGM)
                ? BGMMasterVolume
                : SFXMasterVolume;
            s.source.volume = s.volume * master;
        }
    }

    private void HandleHit()
    {
        PlaySoundDelayed("Hit_SFX",0.1f);
    }
    private void HandleFireBolt()
    {
        PlaySoundDelayed("FireBolt_SFX",0.1f);
    }
    private void HandleArrow()
    {
        PlaySoundDelayed("Arrow_SFX",0.1f);
    }
    private void HandleLightning()
    {
        PlaySoundDelayed("Lightning_SFX",0.2f);
    }
    private void HandleBoom()
    {
        PlaySoundDelayed("Boom_SFX",0.5f);
    }


    public void PlaySound(string _name)
    {
        if (soundDictionary.TryGetValue(_name, out SoundData sound))
        {
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning($"SoundManager: Sound '{_name}' not found!");
        }
    }
    public void PlaySoundDelayed(string name, float delay)
    {
        if (TryGetSound(name, out var s))
        {
            // AudioSettings.dspTime 기반으로 딜레이 재생
            s.source.PlayDelayed(delay);
        }
    }
    public void StopSound(string _name)
    {
        if (soundDictionary.TryGetValue(_name, out SoundData sound))
        {
            sound.source.Stop();
        }
        else
        {
            Debug.LogWarning($"SoundManager: Sound '{_name}' not found!");
        }
    }
    public void SetMasterBGMVolume(float volume)
    {
        BGMMasterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("BGMMasterVolume", BGMMasterVolume);
        PlayerPrefs.Save();
        foreach (var sound in sounds)
        {
            if (sound.category == SoundCategory.BGM)
                sound.source.volume = sound.volume * BGMMasterVolume;
        }
    }
    public void SetMasterSFXVolume(float volume)
    {
        SFXMasterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXMasterVolume", SFXMasterVolume);
        PlayerPrefs.Save();
        foreach (var sound in sounds)
        {
            if (sound.category == SoundCategory.SFX)
                sound.source.volume = sound.volume * SFXMasterVolume;
        }
    }
    public void StopAllInCategory(SoundCategory _category)
    {
        foreach (var sound in sounds)
        {
            if (sound.category == _category)
                sound.source.Stop();
        }
    }
    public void SetCategoryVolume(SoundCategory _category, float _volume)
    {
        _volume = Mathf.Clamp01(_volume);
        foreach (var sound in sounds)
        {
            if (sound.category == _category)
            {
                sound.volume = _volume;
                sound.source.volume = _volume;
            }
        }
    }

    private bool TryGetSound(string name, out SoundData sound)
    {
        if (soundDictionary.TryGetValue(name, out sound)) return true;
        Debug.LogWarning($"SoundManager: Sound '{name}' not found!");
        return false;
    }

}



[System.Serializable]
public class SoundData
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = false;
    public SoundCategory category;
    [HideInInspector] public AudioSource source;
}


/// <summary>
/// Categories for grouping sounds.
/// </summary>
public enum SoundCategory
{
    BGM,
    SFX
}
