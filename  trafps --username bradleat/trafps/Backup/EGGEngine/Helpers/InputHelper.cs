using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EGGEngine.Helpers
{
    public class InputHelper
    {
        /// <summary>
        /// Returns true if the given key is pressed and false otherwise.
        /// </summary>
        /// <param name="key">Key being checked</param>
        /// <returns>Boolean value representing whether the key is down or not</returns>
        public bool KeyDown(Keys key)
        {
            return (Keyboard.GetState().IsKeyDown(key));
        }

        /// <summary>
        /// Returns true if the given array of keys are all pressed and 
        /// false otherwise.
        /// </summary>
        /// <param name="keys">Array of keys being checked</param>
        /// <returns>Boolean value representing whether the keys are down or not</returns>
        public bool KeysDown(Keys[] keys)
        {
            bool isDown = true;

            for (int i = 0; i < keys.Length; ++i)
                isDown &= Keyboard.GetState().IsKeyDown(keys[i]);

            return isDown;
        }

        /// <summary>
        /// Returns true if the given button is pressed and false otherwise
        /// </summary>
        /// <param name="button">Button being checked</param>
        /// <returns>Boolean value representing whether the button is down or not</returns>
        public bool ButtonDown(Buttons button)
        {
            return (GamePad.GetState(PlayerIndex.One).IsButtonDown(button));
        }

        /// <summary>
        /// Returns true if all of the given buttons are pressed and false
        /// otherwise.
        /// </summary>
        /// <param name="buttons">Array of buttons being checked</param>
        /// <returns>Boolean value representing whether the buttons are down or not</returns>
        public bool ButtonsDown(Buttons[] buttons)
        {
            bool isDown = true;

            for (int i = 0; i < buttons.Length; ++i)
                isDown &= GamePad.GetState(PlayerIndex.One).IsButtonDown(buttons[i]);

            return isDown;
        }

        /// <summary>
        /// Sets the vibration.
        /// </summary>
        /// <param name="leftMotor">Low frequency vibration</param>
        /// <param name="rightMotor">High frequency vibration</param>
        public void SetVibration(float leftMotor, float rightMotor)
        {
            if(leftMotor > 0f || rightMotor > 0f)
                GamePad.SetVibration(PlayerIndex.One, leftMotor, rightMotor);
        }

        /// <summary>
        /// Returns the X value of the thumbstick being used.
        /// </summary>
        /// <param name="thumbstick">The thumbstick being manipulated</param>
        /// <returns>Float value representing the direction the thumbstick is being pushed in the x direction</returns>
        public float GetThumbstickX(string thumbstick)
        {
            float retVal = 0f;

            if (thumbstick.Equals("Left") || thumbstick.Equals("left"))
                retVal = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
            else if (thumbstick.Equals("Right") || thumbstick.Equals("right"))
                retVal = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X;
            else
                throw new System.ArgumentException("Invalid choice for thumbstick",
                    "thumbstick");

            return retVal;
        }

        /// <summary>
        /// Returns the Y value of the thumbstick being used.
        /// </summary>
        /// <param name="thumbstick">The thumbstick being manipulated</param>
        /// <returns>Float value representing the direction the thumbstick is being pushed in the y direction</returns>
        public float GetThumbstickY(string thumbstick)
        {
            float retVal = 0f;

            if (thumbstick.Equals("Left") || thumbstick.Equals("left"))
                retVal = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
            else if (thumbstick.Equals("Right") || thumbstick.Equals("right"))
                retVal = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y;
            else
                throw new System.ArgumentException("Invalid choice for thumbstick",
                    "thumbstick");

            return retVal;
        }
    }
}
