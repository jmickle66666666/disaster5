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

        [JSFunction(Name = "playMusic")] public void PlayMusic(string audioPath)
        {
            SDL_mixer.Mix_PlayMusic(Disaster.Assets.Music(audioPath), 1);
        }

        [JSFunction(Name = "playSound")] public void PlaySound(string audioPath)
        {
            SDL_mixer.Mix_PlayChannel(0, Disaster.Assets.Audio(audioPath), 0);
        }
    }
}