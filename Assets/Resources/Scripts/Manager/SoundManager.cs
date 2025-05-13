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
            InitializeSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        RangedAttack.OnFireBoltAttack += HandleFireBolt;
        Character.OnHit += HandleHit;
    }

    private void OnDisable()
    {
        RangedAttack.OnFireBoltAttack += HandleFireBolt;
        Character.OnHit -= HandleHit;
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

        // apply initial volumes
        ApplyAllVolumes();
    }

    private void ApplyAllVolumes()
    {
        foreach (var sound in sounds)
        {
            float master = sound.category == SoundCategory.BGM ? BGMMasterVolume : SFXMasterVolume;
            sound.source.volume = sound.volume * master;
        }
    }

    private void HandleHit()
    {
        PlaySound("Hit_SFX");
    }
    private void HandleFireBolt()
    {
        PlaySound("FireBolt_SFX");
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
    public void PauseSound(string _name)
    {
        if (soundDictionary.TryGetValue(_name, out SoundData sound))
        {
            sound.source.Pause();
        }
        else
        {
            Debug.LogWarning($"SoundManager: Sound '{_name}' not found!");
        }
    }
    public void ResumeSound(string _name)
    {
        if (soundDictionary.TryGetValue(_name, out SoundData sound))
        {
            sound.source.UnPause();
        }
        else
        {
            Debug.LogWarning($"SoundManager: Sound '{_name}' not found!");
        }
    }
    public void SetMasterBGMVolume(float volume)
    {
        BGMMasterVolume = Mathf.Clamp01(volume);
        foreach (var sound in sounds)
        {
            if (sound.category == SoundCategory.BGM)
                sound.source.volume = sound.volume * BGMMasterVolume;
        }
    }

    /// <summary> Set overall master volume for all SFX (0-1). </summary>
    public void SetVolume(string name, float volume)
    {
        if (!TryGetSound(name, out var s)) return;
        s.volume = Mathf.Clamp01(volume);
        float master = s.category == SoundCategory.BGM ? BGMMasterVolume : SFXMasterVolume;
        s.source.volume = s.volume * master;
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
