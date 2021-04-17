using Jurassic;
using Jurassic.Library;
using SDL2;
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
            SDL_mixer.Mix_PlayMusic(Disaster.Assets.Music(audioPath), 1);
        }

        public static int maxChannels = 64;

        [JSFunction(Name = "playSound")]
        [FunctionDescription("Play a given audio file once")]
        [ArgumentDescription("audioPath", "path to the audio asset to play")]
        public void PlaySound(string audioPath)
        {
            var channel = SDL_mixer.Mix_PlayChannel(-1, Disaster.Assets.Audio(audioPath), 0);
            if (channel == -1)
            {
                Console.WriteLine(SDL.SDL_GetError());
            }
        }
    }
}