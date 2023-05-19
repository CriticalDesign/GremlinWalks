using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GremlinWalks
{
    internal class Gremlin
    {
        private Texture2D _gremlinSprite;
        private SpriteFont _gremlinFont;
        private int _rows, _cols, _currentFrame, _endCol, _animRow;
        private Vector2 _position, _direction;
        private float _speed;
        private string _name, _chatter;

        private bool _paceLeft;

        int timeSinceLastFrame = 0;
        int millisecondsPerFrame = 100;

        public Gremlin(Texture2D gremlinSprite, SpriteFont gremlinFont, string name, string chatter)
        {
            _gremlinSprite = gremlinSprite;
            _gremlinFont = gremlinFont;
            _name = name;
            _chatter = chatter;

            _rows = 5;
            _cols = 16;
            _currentFrame = 0;

            _animRow = 1;
            _endCol = 8;

            _direction = new Vector2(0,0);
            _position = new Vector2(200, 100);
            _speed = 100f;


            //walk variables
            _paceLeft = true;
        }

        public void Update(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;

                _currentFrame++;
                if (_currentFrame == _endCol)
                    _currentFrame = 0;
            }

            pace(new Vector2(100,100), new Vector2(500,100), gameTime);
        }

        public void pace(Vector2 startPace, Vector2 endPace, GameTime gameTime)
        {
            if (_position.X <= startPace.X)
            {
                _paceLeft = false;
            }
            if(_position.X >= endPace.X)
            {
                _paceLeft = true;
            }

            if(_paceLeft) { move(startPace, gameTime);  }
            else { move(endPace, gameTime); }

        }

        public void move(Vector2 target, GameTime gameTime)
        {
            //adpated from: https://stackoverflow.com/questions/12715096/xna-vector-math-movement
            Vector2 temp = (target - _position); // gets the difference between the target and position
            temp.Normalize();                   // sets the vector to unit vector
            temp *= _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;                  // sets the vector to be the length of moveSpeed
            float x = temp.X;
            float y = temp.Y;

            _position += new Vector2(x, y);
            //float xP, yP;
            //double angle = Math.Acos(((x * _direction.X) + (y * _direction.Y)) / (temp.Length() * _direction.Length())); //dot product finds the angle between temp and direction
            //xP = (float)(Math.Cos(angle) * (x - _direction.X) - Math.Sin(angle) * (y - _direction.Y) + x);
            //yP = (float)(Math.Sin(angle) * (x - _direction.X) - Math.Cos(angle) * (y - _direction.Y) + y);            // these lines rotate the point x,y around the direction vector by angle "angle"
            //return new Vector2(x, y);
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            int width = _gremlinSprite.Width / _cols;
            int height = _gremlinSprite.Height / _rows;
            int row = _animRow;
            int column = _currentFrame;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)_position.X, (int)_position.Y, width, height);

            SpriteEffects faceLeft = SpriteEffects.None;
            if(_paceLeft)
                faceLeft = SpriteEffects.FlipHorizontally;

            string text = _name + ": " + _chatter;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            spriteBatch.DrawString(_gremlinFont, text, new Vector2(_position.X - (int)(_gremlinFont.MeasureString(text).X / 2), _position.Y - 20), Color.White);
            spriteBatch.Draw(_gremlinSprite, _position, sourceRectangle, Color.White, 0, new Vector2(width/2, height/2), new Vector2(4,4), faceLeft, 0);
            spriteBatch.End();
        }

    }
}
