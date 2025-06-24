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
    internal class Button
    {
        Rectangle _Rectangle;
        Texture2D _Texture;
        Color _backgroundColor;
        Color _hoverColor;
        SpriteFont _spriteFont;
        float _fontScale;
        string _text;
        Vector2 _textPosition;
        public Button(Rectangle Rectangle, Texture2D Texture, Color backgroundColor, Color pressColor, SpriteFont spriteFont, float fontScale, string text)
        {
            _Rectangle = Rectangle;
            _Texture = Texture;
            _backgroundColor = backgroundColor;
            _hoverColor = pressColor;
            _spriteFont = spriteFont;
            _fontScale = fontScale;
            _text = text;
            _textPosition = new Vector2(_Rectangle.Center.X - (_spriteFont.MeasureString(text).X * _fontScale) / 2, _Rectangle.Center.Y - (_spriteFont.LineSpacing * _fontScale) / 2);
        }

        bool buttonHover(Vector2 vector)
        {
            Point point = new((int)vector.X, (int)vector.Y);
            if (_Rectangle.Contains(point))
            {
                return true;
            }
            return false;
        }
        bool buttonHover(MouseState mouseState)
        {
            if (_Rectangle.Contains(mouseState.Position))
            {
                return true;
            }
            return false;
        }
        public bool buttonPressed(MouseState mouseState, MouseState previousMouseState)
        {
            if (buttonHover(mouseState) && General.OnLeftButtonPress())
            {
                return true;
            }
            return false;
        }
        public void Draw(SpriteBatch _spriteBatch, MouseState mouseState, MouseState previousMouseState)
        {
            Color color;
            if (buttonHover(mouseState))
            {
                color = _hoverColor;
            }
            else
            {
                color = _backgroundColor;
            }
            _spriteBatch.Draw(_Texture, _Rectangle, null, color, 0, new(), 0, .99f);
            _spriteBatch.DrawString(_spriteFont, "Restart", _textPosition, Color.White, 0, new(), _fontScale, 0, 1);
        }
    }
}
