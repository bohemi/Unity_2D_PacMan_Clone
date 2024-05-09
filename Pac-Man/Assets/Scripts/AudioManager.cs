using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sounds[] sounds;

    private void Awake()
    {
        foreach (Sounds s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;

            s.source.volume = s.volume;
        }
    }

    private void Start()
    {
        play("game_start", false);
    }

    public void play(string name, bool stop)
    {
        Sounds s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();

        if(stop)
        {
            s.source.Stop();
        }
    }
}
