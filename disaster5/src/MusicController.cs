using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;

namespace Disaster
{
    class MusicController
    {
        public static Music currentMusic;
        public static bool playing = false;

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
        }
    }
}
