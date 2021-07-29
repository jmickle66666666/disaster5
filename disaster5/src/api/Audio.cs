using Jurassic;
using Jurassic.Library;
using Raylib_cs;
using System;
namespace DisasterAPI
{
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
            
            Disaster.MusicController.PlayMusic(music);
        }

        public static int maxChannels = 64;

        [JSFunction(Name = "playSound")]
        [FunctionDescription("Play a given audio file once")]
        [ArgumentDescription("audioPath", "path to the audio asset to play")]
        public void PlaySound(string audioPath)
        {
            Raylib.PlaySoundMulti(Disaster.Assets.Audio(audioPath));
        }
    }
}