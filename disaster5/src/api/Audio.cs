using Jurassic;
using Jurassic.Library;
using Raylib_cs;
using System;
namespace DisasterAPI
{
    [ClassDescription("Functions for playing sounds! neat")]
    public class Audio : ObjectInstance {
        public Audio(ScriptEngine engine) : base(engine) {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "playMusic")]
        [FunctionDescription("Loop a given audio file")]
        [ArgumentDescription("audioPath", "path to the audio asset to play")]
        public void PlayMusic(string audioPath)
        {
            var music = Disaster.Assets.Music(audioPath);
            if (music.succeeded)
            {
                Disaster.AudioController.PlayMusic(music.music);
            }
        }

        public static int maxChannels = 64;

        [JSFunction(Name = "playSound")]
        [FunctionDescription("Play a given audio file once")]
        [ArgumentDescription("audioPath", "path to the audio asset to play")]
        [ArgumentDescription("volume", "(optional) volume to play audio at. 1 = default")]
        [ArgumentDescription("pitch", "(optional) pitch to play audio at. 1 = default")]
        public void PlaySound(string audioPath, double volume = 1.0, double pitch = 1.0)
        {
            Disaster.AudioController.PlaySound(audioPath, (float)volume, (float)pitch);
        }

        [JSFunction(Name = "setMainVolume")]
        [FunctionDescription("Set overall audio volume")]
        [ArgumentDescription("volume", "volume to set. 1.0 = default")]
        public void SetMainVolume(double volume)
        {
            Raylib.SetMasterVolume((float) volume);
        }

        [JSFunction(Name = "stopAllSound")]
        [FunctionDescription("Stop all playing sounds, except music")]
        public void StopAllSounds()
        {
            Disaster.AudioController.StopAllSound();
        }

        [JSFunction(Name = "stopMusic")]
        [FunctionDescription("Stop the playing music")]
        public void StopMusic()
        {
            Disaster.AudioController.StopMusic();
        }

    }
}