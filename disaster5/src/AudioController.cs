using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;

namespace Disaster
{
    class AudioController
    {
        public static Music currentMusic;
        public static bool playing = false;

        public static List<Sound> playingSounds;
        public static List<Sound> soundsToRemove;

        public static void Init()
        {
            playingSounds = new List<Sound>();
            soundsToRemove = new List<Sound>();
        }

        public static void PlayMusic(Music music)
        {
            if (playing)
            {
                Raylib.StopMusicStream(currentMusic);
            }
            currentMusic = music;
            Raylib.PlayMusicStream(currentMusic);
            playing = true;
        }

        public static void StopMusic()
        {
            if (playing)
            {
                Raylib.StopMusicStream(currentMusic);
                playing = false;
            }
        }

        public static void Update()
        {
            Raylib.UpdateMusicStream(currentMusic);

            for (var i = 0; i < playingSounds.Count; i++)
            {
                if (!Raylib.IsSoundPlaying(playingSounds[i]))
                {
                    soundsToRemove.Add(playingSounds[i]);
                    Raylib.UnloadSound(playingSounds[i]);
                }
            }

            for (var i = 0; i < soundsToRemove.Count; i++)
            {
                playingSounds.Remove(soundsToRemove[i]);
            }

            soundsToRemove.Clear();
        }

        public static void PlaySound(string soundPath, float volume, float pitch)
        {
            if (volume == 1.0f && pitch == 1.0f)
            {
                var sound = Assets.Audio(soundPath);
                if (!sound.succeeded)
                {
                    Console.WriteLine($"failed to load sound {soundPath}");
                    return;
                }
                Raylib.PlaySoundMulti(sound.sound);
            } else
            {
                var loaded = Assets.LoadPath(soundPath, out string assetPath);
                if (!loaded)
                {
                    Console.WriteLine($"Can't find sound {soundPath}");
                    return;
                }

                var sound = Raylib.LoadSound(assetPath);
                Raylib.SetSoundPitch(sound, (float)pitch);
                Raylib.SetSoundVolume(sound, (float)volume);
                Raylib.PlaySound(sound);
                playingSounds.Add(sound);
            }
        }

        public static void StopAllSound()
        {
            Raylib.StopSoundMulti();
            for (var i = 0; i < playingSounds.Count; i++)
            {
                Raylib.StopSound(playingSounds[i]);
            }

            for (var i = 0; i < playingSounds.Count; i++)
            {
                Raylib.UnloadSound(playingSounds[i]);
            }
            playingSounds.Clear();
        }
    }
}
