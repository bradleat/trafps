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
    /// <summary>
    /// Audio class used for creating sounds/ background music in a game
    /// </summary>
    public class Audio
    {
        // Audio objects
        AudioEngine engine;
        SoundBank soundBank;
        WaveBank waveBank;
        Cue cue;
        AudioCategory musicCategory;

        /// <summary>
        /// Initialises the Audio engine
        /// </summary>
        /// <param name="audioFilePath">A string containing the file path to your Xact file ( make sure to put ".xgs" at the end of the string i.e - "Content\\example.xgs"</param>
        public Audio(string audioFilePath)
        {
            // Initialize audio objects.
            engine = new AudioEngine(audioFilePath);
            //"Content\\Audio\\PlaySound.xgs");
            soundBank = new SoundBank(engine, "Content\\Sound Bank.xsb");
            // Create streaming wave bank.
            waveBank = new WaveBank(engine, "Content\\Wave Bank.xwb");
            // must call update
            engine.Update();

        }
        /// <summary>
        /// Used to get the category from the Xact file
        /// </summary>
        /// <param name="categoryname">A string of the name of the category i.e "Music"</param>
        /// <returns>Returns an AudioCategory</returns>
        public AudioCategory GetCategory(string categoryname)
        {
            musicCategory = engine.GetCategory(categoryname);
            return musicCategory;
        }
        /// <summary>
        /// Get the cue from the Xact file using a string. This should be the first line in your game, in Initialise() 
        /// </summary>
        /// <param name="sound"> A string of the name of the cue in your Xact file</param>
        /// <returns>Returns the cue of a given string from your Xact file</returns>
        public Cue GetCue(string sound)
        {
            cue = soundBank.GetCue(sound);
            return cue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cue"></param>
        /// <param name="apply3d"></param>
        /// <param name="listener"></param>
        /// <param name="emitter"></param>
        /// <returns></returns>
        public Cue Play(Cue cue, bool apply3d, AudioListener listener, AudioEmitter emitter)
        {
            if (cue.IsStopped)
                cue = soundBank.GetCue(cue.Name);

            if (cue.IsPrepared)
            {
                if (apply3d)
                    cue.Apply3D(listener, emitter);
                cue.Play();
            }
            return cue;
        }
        public void Pause(Cue cue)
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
        public Cue Stop(Cue cue)
        {
            cue.Stop(AudioStopOptions.AsAuthored);
            return cue;
        }
        public float ChangeVolume(AudioCategory category, float volume, bool increase, float maxVolume)
        {
            if (increase)
            {
                volume = MathHelper.Clamp(volume + 0.01f, 0.0f, maxVolume);
            }
            else
                volume = MathHelper.Clamp(volume - 0.01f, 0.0f, maxVolume);

            category.SetVolume(volume);
            return volume;
        }

        public void Apply3D(Cue cue, AudioListener listener, AudioEmitter emitter)
        {
            cue.Apply3D(listener, emitter);
        }
        public void Apply3DPosition(Cue cue, AudioListener listener, AudioEmitter emitter,
             Vector3 listenerPosition, Vector3 emitterPosition)
        {
            listenerPosition = listener.Position;
            emitterPosition = emitter.Position;
            cue.Apply3D(listener, emitter);
        }
        public void Apply3DVelocity(Cue cue, AudioListener listener, AudioEmitter emitter, Vector3 emitterVelocity, Vector3 listenerVelocity)
        {
            listenerVelocity = listener.Velocity;
            emitterVelocity = emitter.Velocity;
            cue.Apply3D(listener, emitter);
        }
        public void Apply3DAll(Cue cue, AudioListener listener, AudioEmitter emitter, Vector3 listenerPosition,
            Vector3 emitterPosition, Vector3 listenerVelocity, Vector3 emitterVelocity)
        {
            listenerPosition = listener.Position;
            emitterPosition = emitter.Position;
            listener.Velocity = listener.Velocity;
            emitter.Velocity = emitter.Velocity;
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

