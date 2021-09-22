//This code is build on my own ,and tutorial by Brackeys.

using System;
using UnityEngine;

namespace General
{
    public class AudioManager : SingletonMonoBehavior<AudioManager>
    {
        public Sound[] sounds;

        private static AudioManager _instance;

        // Start is called before the first frame update
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            foreach (var sound in sounds)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;

                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
            }
        }

        public void Play(string soundName)
        {
            var s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogError("找不到此音效：" + soundName);
                return;
            }

            s.source.Play();
        }
    }
}