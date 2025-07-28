using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource[] sources;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Dictionary<string, AudioClip> soundLib = new();

    public enum AudioMode
    {
        ONESHOT, SINGLE, MUSIC
    }

    public void Start()
    {
        float generalVolume = 1;// * SaveDataManager.GetMasterVolume() * SaveDataManager.GetGeneralVolume();
        foreach (var source in sources)
            source.volume = generalVolume;

        musicSource.volume = 1;// * SaveDataManager.GetMasterVolume() * SaveDataManager.GetMusicVolume();

        var clips = Resources.LoadAll<AudioClip>("Audio");
        string debugOutput = "";
        foreach (var clip in clips)
        {
            soundLib.Add(clip.name, clip);
            debugOutput += $"\t{clip.name}\n";
        }

        Debug.Log("Loaded the following sfx:\n" + debugOutput);
    }

    public void PlaySound(string name, AudioMode mode = AudioMode.ONESHOT, float pitch = 1)
    {
        if (!soundLib.ContainsKey(name)) {
            Debug.LogWarning($"Attempted to play sound with name {name}. Sound does not exist");
            return;
        }

        var sound = soundLib[name];

        var source = sources[0];
        foreach (var cur in sources)
        {
            if (!cur.isPlaying)
            {
                source = cur;
                break;
            }
        }

        source.pitch = pitch;

        switch (mode)
        {
            case AudioMode.ONESHOT:
                source.PlayOneShot(sound);
                break;
            case AudioMode.SINGLE:
                foreach (var cur in sources)
                {
                    if (cur.clip == sound)
                    {
                        source = cur;
                        break;
                    }
                }

                source.clip = sound;
                source.Play();
                break;
            case AudioMode.MUSIC:
                musicSource.clip = sound;
                musicSource.Play();
                break;
        }
    }
}
