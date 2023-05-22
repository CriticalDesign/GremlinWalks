using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace GremlinWalks
{
    internal class Gremlin
    {
        private Texture2D _gremlinSprite;
        private SpriteFont _gremlinFont;
        private int _rows, _cols, _currentFrame, _endCol, _animRow;
        private Vector2 _position, _chaseTarget, _direction;
        private float _speed, _scale, _circularAngle;
        private string _name, _chatter;
        private string _moveType;
        private Color _color;


        private bool _paceLeft;

        private List<Vector2> _path;
        private int _destination;

        int timeSinceLastFrame = 0;
        int millisecondsPerFrame = 100;

        int randomHTimeSinceLastFrame = 0;
        int randomHMillisecondsPerFrame = 2000;
        int randomVTimeSinceLastFrame = 0;
        int randomVMillisecondsPerFrame = 2000;
        int randomDeltaX = 0;
        int randomDeltaY = 0;

        public Gremlin(Texture2D gremlinSprite, SpriteFont gremlinFont, string name, string chatter, string moveType, Color color)
        {
            _gremlinSprite = gremlinSprite;
            _gremlinFont = gremlinFont;
            _name = name;
            _color = color;

            if (chatter != "" || chatter != string.Empty)
                chatter = "\"" + chatter + "\"";
            
            _chatter = chatter;

            
            _rows = 5;
            _cols = 16;
            _currentFrame = 0;

            if (moveType == "pace" || moveType == "circle" || moveType == "random" || moveType == "chase" || moveType == "bounce" || moveType == "path") 
            {
                _animRow = 1;
                _endCol = 8;
            }
            else
            {
                _animRow = 0;
                _endCol = 16;
            }

            _path = new List<Vector2>();
            _destination = -1;

            _position = new Vector2(0, 0);
            _direction = new Vector2(0, 0);

            _speed = 100f;
            _scale = 3.0f;

            _circularAngle = 0f;

            //walk variables
            _paceLeft = true;
            _moveType = moveType;
            _chaseTarget = _position;
        }

        public void SetStartPosition(Vector2 position)
        {
            _position = position;
        }

        public void SetChaseTarget(Vector2 chaseMe)
        {
            _chaseTarget = chaseMe;
        }

        public Vector2 GetPosition()
        {
            return _position;
        }

        public void SetChatter(string chatter)
        {
            _chatter = chatter;
        }

        public string GetChatter() { return _chatter; }

        public string GetName() { return _name; }

        public void SetDirection(Vector2 dir) { _direction = dir; }

        public void SetPath() { 
            _path.Add(new Vector2(400, 400));
            _path.Add(new Vector2(700, 400));
            _path.Add(new Vector2(700, 475));
            _path.Add(new Vector2(400, 475));
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

            if (_moveType == "pace")
                pace(new Vector2(50, 100), new Vector2(250, 100), gameTime);
            else if (_moveType == "random")
                RandomWalk(gameTime);
            else if (_moveType == "chase")
                move(_chaseTarget, gameTime);
            else if (_moveType == "bounce")
                bounce(gameTime);
            else if (_moveType == "path" || _moveType == "circle")
                path(gameTime);
        }

        //TODO: There are some issues in here, it's changing too frequently.
        public void RandomWalk(GameTime gameTime)
        {
            Random rng  = new Random();
            randomHTimeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (randomHTimeSinceLastFrame > randomHMillisecondsPerFrame)
            {
                randomHTimeSinceLastFrame -= randomHMillisecondsPerFrame;
                randomDeltaX = rng.Next((int)-_speed, (int)_speed);
            }

            randomVTimeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (randomVTimeSinceLastFrame > randomVMillisecondsPerFrame)
            {
                randomVTimeSinceLastFrame -= randomVMillisecondsPerFrame;
                randomDeltaY = rng.Next((int)-_speed, (int)_speed);
            }

            if (randomDeltaX > 0)
                _paceLeft = false;
            else
                _paceLeft = true;

            if (_position.X < 50 || _position.X > 750)
                randomDeltaX = -randomDeltaX;

            if (_position.Y < 50 || _position.Y > 550)
                randomDeltaY = -randomDeltaY;

            _position.X += (randomDeltaX * (float)gameTime.ElapsedGameTime.TotalSeconds);
            _position.Y += (randomDeltaY * (float)gameTime.ElapsedGameTime.TotalSeconds);

        }

        public void SetCircle(float radius)
        {
            float tempX = _position.X;
            float tempY = _position.Y;
            for(float i = (float)-Math.PI; i <= (float)Math.PI; i += 0.1f)
            {
                tempX -= radius * (float)Math.Sin(i);
                tempY += radius * (float)Math.Cos(i);
                _path.Add(new Vector2(tempX, tempY));
            }
            /*
            _circularAngle += (float)(gameTime.ElapsedGameTime.TotalSeconds * _speed/(radius * 25f));
            
            _position.X -= radius * (float)Math.Sin(_circularAngle);
            _position.Y += radius * (float)Math.Cos(_circularAngle);
            
            if(radius * (float)Math.Sin(_circularAngle) < 1)
                _paceLeft = false;
            else 
                _paceLeft = true; 
            */
        }

        public void pace(Vector2 startPace, Vector2 endPace, GameTime gameTime)
        {

            if (_position.X <= startPace.X)
                _paceLeft = false;

            if (_position.X >= endPace.X)
                _paceLeft = true;

            if (_paceLeft) 
                move(startPace, gameTime);  
            else
                move(endPace, gameTime); 
        }

        public void move(Vector2 target, GameTime gameTime)
        {
            //adpated from: https://stackoverflow.com/questions/12715096/xna-vector-math-movement
            Vector2 temp = (target - _position); // gets the difference between the target and position
            temp.Normalize();                   // sets the vector to unit vector
            temp *= _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;                  // sets the vector to be the length of moveSpeed
            float x = temp.X;
            float y = temp.Y;

            if (_position.X > target.X)
                _paceLeft = true;
            else
                _paceLeft = false;

            _position += new Vector2(x, y);

            //float xP, yP;
            //double angle = Math.Acos(((x * _direction.X) + (y * _direction.Y)) / (temp.Length() * _direction.Length())); //dot product finds the angle between temp and direction
            //xP = (float)(Math.Cos(angle) * (x - _direction.X) - Math.Sin(angle) * (y - _direction.Y) + x);
            //yP = (float)(Math.Sin(angle) * (x - _direction.X) - Math.Cos(angle) * (y - _direction.Y) + y);            // these lines rotate the point x,y around the direction vector by angle "angle"
            //return new Vector2(x, y);
        }


        public void bounce(GameTime gameTime)
        {
            //adpated from: https://stackoverflow.com/questions/12715096/xna-vector-math-movement
            //Vector2 temp = (_direction - _position); // gets the difference between the target and position
            _direction.Normalize();                   // sets the vector to unit vector
            //_direction = ;                  // sets the vector to be the length of moveSpeed
            //float x = _direction.X;
            //float y = _direction.Y;

            if (_position.X <  50 || _position.X > 300)
                _direction.X = -_direction.X;
            if (_position.Y < 300 || _position.Y > 500)
                _direction.Y = -_direction.Y;

            if (_direction.X > 0)
                _paceLeft = false;
            else
                _paceLeft = true;

            _position += _direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //float xP, yP;
            //double angle = Math.Acos(((x * _direction.X) + (y * _direction.Y)) / (temp.Length() * _direction.Length())); //dot product finds the angle between temp and direction
            //xP = (float)(Math.Cos(angle) * (x - _direction.X) - Math.Sin(angle) * (y - _direction.Y) + x);
            //yP = (float)(Math.Sin(angle) * (x - _direction.X) - Math.Cos(angle) * (y - _direction.Y) + y);            // these lines rotate the point x,y around the direction vector by angle "angle"
            //return new Vector2(x, y);
        }

        public void path(GameTime gameTime)
        {
            if(_destination == -1)
            {
                _destination = 0;
            }
            if (Vector2.Distance(_path[_destination], _position) < 5)
            {
                _destination++;
            }
            if(_destination >= _path.Count) { _destination = 0; }

            SetChatter(_path[_destination] + "");

            move(_path[_destination], gameTime);
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


            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            spriteBatch.DrawString(_gremlinFont, _name, new Vector2(_position.X - (int)(_gremlinFont.MeasureString(_name).X / 2), _position.Y - 80), _color);
            spriteBatch.DrawString(_gremlinFont, _chatter, new Vector2(_position.X - (int)(_gremlinFont.MeasureString(_chatter).X / 2), _position.Y - 60), Color.White);
            spriteBatch.Draw(_gremlinSprite, _position, sourceRectangle, Color.White, 0, new Vector2(width/2, height/2), new Vector2(_scale, _scale), faceLeft, 0);
            spriteBatch.End();
        }

    }
}
