using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;

namespace GremlinWalks
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Song _music;

        private Random _rng;

        private Gremlin _gretl, _kurt, _liesl, _marta, _louisa, _brigitta, _friedrich;

        private List<Gremlin> _gremlins;

        int _chaseTarget;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gremlins = new List<Gremlin>();
            _rng = new Random();
            _chaseTarget = -1;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _music = Content.Load<Song>("chiptune"); //https://pixabay.com/music/video-games-chiptune-grooving-142242/
            MediaPlayer.Play(_music);

            _louisa = new Gremlin(Content.Load<Texture2D>("gremlinSprites"), Content.Load<SpriteFont>("gremlinFont"), "Louisa", "I'm chasing.", "chase", Color.Yellow);
            _louisa.SetStartPosition(new Vector2(50, 400));
            _gretl = new Gremlin(Content.Load<Texture2D>("gremlinSprites"), Content.Load<SpriteFont>("gremlinFont"), "Gretl", "I'm pacing.", "pace", Color.White);
            _gretl.SetStartPosition(new Vector2(100, 150));
            _gretl.SetPace(new Vector2(50, 150), new Vector2(250, 150));
            _kurt = new Gremlin(Content.Load<Texture2D>("gremlinSprites"), Content.Load<SpriteFont>("gremlinFont"), "Kurt", "I'm idle.", "idle", Color.Orange);
            _kurt.SetStartPosition(new Vector2(550, 200));
            _liesl = new Gremlin(Content.Load<Texture2D>("gremlinSprites"), Content.Load<SpriteFont>("gremlinFont"), "Liesl", "I'm circling.", "circle", Color.Red);
            _liesl.SetStartPosition(new Vector2(450, 200));
            _liesl.SetCircle(10f);
            _marta = new Gremlin(Content.Load<Texture2D>("gremlinSprites"), Content.Load<SpriteFont>("gremlinFont"), "Marta", "I'm random.", "random", Color.Pink);
            _marta.SetStartPosition(new Vector2(700, 500));
            _brigitta = new Gremlin(Content.Load<Texture2D>("gremlinSprites"), Content.Load<SpriteFont>("gremlinFont"), "Brigitta", "I'm bouncing.", "bounce", Color.Green);
            _brigitta.SetStartPosition(new Vector2(150, 400));
            _brigitta.SetDirection(new Vector2(_rng.Next(0,800), _rng.Next(0,600)));
            _friedrich = new Gremlin(Content.Load<Texture2D>("gremlinSprites"), Content.Load<SpriteFont>("gremlinFont"), "Friedrich", "I'm following a path.", "path", Color.Purple);
            _friedrich.SetStartPosition(new Vector2(400, 400));
            _friedrich.SetPath();


            //_gremlins.Add(_louisa);
            //_gremlins.Add(_marta);
            //_gremlins.Add(_brigitta);
            _gremlins.Add(_gretl);
            _gremlins.Add(_kurt);
            _gremlins.Add(_liesl);
            _gremlins.Add(_friedrich); 


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach(Gremlin gremlin in _gremlins)
            {
                gremlin.Update(gameTime);
            }

            if (_chaseTarget == -1 || Vector2.Distance(_louisa.GetPosition(), _gremlins[_chaseTarget].GetPosition()) < 75)
            {
                _chaseTarget = _rng.Next(1, _gremlins.Count);
                _louisa.SetChatter("\"I'm chasing.\" (" + _gremlins[_chaseTarget].GetName() + ")");
            }
            _louisa.SetChaseTarget(_gremlins[_chaseTarget].GetPosition());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (Gremlin gremlin in _gremlins)
            {
                gremlin.Draw(_spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}