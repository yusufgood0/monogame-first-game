using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Threading;
using first_game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace first_game
{
    internal class Slider
    {
        public static void SliderUpdate(MouseState _mouseState, MouseState _previousMouseState, ref float sliderValue, float minValue, float maxValue, Rectangle sliderSize)
        {
            if (sliderSize.Contains(_previousMouseState.Position) && _mouseState.LeftButton == ButtonState.Pressed)
            {
                sliderValue = Math.Max(Math.Min(((float)(_mouseState.X - sliderSize.X) / sliderSize.Width) * (maxValue - minValue) + minValue, maxValue), minValue);
            }
        }

        public static void SliderDraw(SpriteBatch _spriteBatch, float sliderValue, float minValue, float maxValue, Rectangle sliderSize, Color backgroundColor, Color sliderColor, string text, float textScale, float visualSliderValueMultiplier)
        {
            string visualSliderValue = ((int)(sliderValue * visualSliderValueMultiplier)).ToString();
            _spriteBatch.Begin();
            _spriteBatch.DrawString(
            Game1.titleFont,
            visualSliderValue,
            new(sliderSize.Right - Game1.titleFont.MeasureString(visualSliderValue).X * textScale, sliderSize.Y - Game1.titleFont.LineSpacing * textScale),
            backgroundColor,
            0,
            new(),
            textScale,
            0,
            .99f
            );

            _spriteBatch.DrawString(
                Game1.titleFont,
                text,
                new(sliderSize.X, sliderSize.Y - Game1.titleFont.LineSpacing * textScale),
                backgroundColor,
                0,
                new(),
                textScale,
                0,
                .99f
                );
            _spriteBatch.Draw(
                Game1.blankTexture,
                sliderSize,
                null,
                backgroundColor,
                0,
                new(),
                0,
                .98f
                );
            _spriteBatch.Draw(
                Game1.blankTexture,
                new(sliderSize.X, sliderSize.Y, (int)(sliderSize.Width / (maxValue - minValue) * (sliderValue - minValue)), sliderSize.Height),
                null,
                sliderColor,
                0,
                new(),
                0,
                .99f
                );
            _spriteBatch.End();

        }
    }
}
