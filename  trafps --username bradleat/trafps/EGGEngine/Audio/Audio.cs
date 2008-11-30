using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace EGGEngine.Audio
{
    class Audio
    {
        // Audio objects
        AudioEngine engine;
        SoundBank soundBank;
        WaveBank waveBank;
        Cue cue;
        AudioCategory musicCategory;

        // 3D audio objects
        AudioEmitter emitter = new AudioEmitter();
        AudioListener listener = new AudioListener();


        public Audio(string audioFilePath, string initialsound, AudioCategory category, string categoryname, int offset, short packetsize)
        {
            // Initialize audio objects.
            engine = new AudioEngine(audioFilePath);
                //"Content\\Audio\\PlaySound.xgs");
            soundBank = new SoundBank(engine, "Content\\Audio\\Sound Bank.xsb");
            // Create streaming wave bank.
            waveBank = new WaveBank(engine, "Content\\Audio\\Wave Bank.xwb", offset, packetsize);

            category = engine.GetCategory(categoryname);
            // must call update
            engine.Update();

            cue = soundBank.GetCue(initialsound);
            cue.Play();

        }
        public Cue GetCue(string sound)
        {
            cue = soundBank.GetCue(sound);
            return cue;
        }
        public void PlaySound(Cue cue)
        {
            cue.Play();
        }
        public void PauseSound(Cue cue)
        {
            if (cue.IsPaused)
            {
                cue.Resume();
            }
            else if (cue.IsPlaying)
            {
                cue.Pause();
            }
            else
            {
                // If stopped, create a new cue.
                cue = soundBank.GetCue(cue.Name);
                cue.Play();
            }
        }
        public void StopSound(Cue cue)
        {
            cue.Stop(AudioStopOptions.AsAuthored);
        }
        public void ChangeVolume(AudioCategory category, float volume, bool increase, float maxVolume)
        {
            if (increase)
            {
                volume = MathHelper.Clamp(+0.01f, 0.0f, maxVolume);
            }
            else
                volume = MathHelper.Clamp(-0.01f, 0.0f, maxVolume);

            category.SetVolume(volume);
        }

        public void Play3DSound(AudioListener listener, AudioEmitter emitter, 
            Cue cue, Vector3 listenerPosition, Vector3 emitterPosition)
        {
            listener.Position = listenerPosition;
            emitter.Position = emitterPosition;
            cue.Apply3D(listener, emitter);
        }
        public void ChangeEmitterVelocity(AudioEmitter emitter, float maxVelocity, bool increase, float amount)
        {
            if (increase)
                emitter.Velocity = new Vector3(maxVelocity * amount, 0.0f, 0.0f);
            else
                emitter.Velocity = new Vector3(-maxVelocity * amount, 0.0f, 0.0f);
        }
        public void ChangeListenerVelocity(AudioListener listener, float maxVelocity, bool increase, float amount)
        {
            if (increase)
                listener.Velocity = new Vector3(maxVelocity * amount, 0.0f, 0.0f);
            else
                listener.Velocity = new Vector3(-maxVelocity * amount, 0.0f, 0.0f);
        }
        public void Update()
        {
            engine.Update();
        }

    }
}
