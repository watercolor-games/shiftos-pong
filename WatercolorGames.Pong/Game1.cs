using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plex.Engine.GraphicsSubsystem;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using Plex.Engine;
using Microsoft.Xna.Framework.Audio;

namespace WatercolorGames.Pong
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private SpriteFont _scoreFont = null;
        private SpriteFont _timeLeftFont = null;
        private SpriteFont _bodyFont = null;

        private double _timeLeftSeconds = 60;
        private int _level = 1;
        private long _codepoints = 0;

        private SoundEffect _typesound = null;
        private SoundEffect _writesound = null;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private float _playerPaddleY = 0.5F;
        private float _cpuPaddleY = 0.5F;

        private float _ballX = 0.5F;
        private float _ballY = 0.5F;

        private const float _paddleHeight = 0.15F;
        private const float _paddleWidth = 0.05F;

        private const float _ballSize = 0.1F;
        private const float _ballSizeLandscape = 0.05F;

        private Rectangle _ballRect = new Rectangle();
        private Rectangle _playerRect = new Rectangle();
        private Rectangle _cpuRect = new Rectangle();

        private const long _cpLevelBeatReward = 1;
        private const long _cpCpuBeatReward = 2;

        private string _countdownHead = "Countdown header";
        private string _countdownDesc = "Countdown description";

        private double _countdown = 3;

        private const float _paddleHeightLandscape = 0.20F;
        private const float _paddleWidthLandscape = 0.025F;

        private float _ballVelYStart = 0.0025F;
        private float _ballVelX = 0.0025F;
        private float _ballVelY = -0.0025F;
        private float _cpuSpeed = 0.5F;

        private int _gameState = 0;

        private double _countdownBeepTimer = 0;

        private bool _isLandscape = false;

        private GraphicsContext _gfx = null;

        private Activity1 _activity = null;

        public Game1(Activity1 activity)
        {
            _activity = activity;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _gfx = new GraphicsContext(GraphicsDevice, spriteBatch, 0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

            _bodyFont = Content.Load<SpriteFont>("Fonts/MainBody");
            _timeLeftFont = Content.Load<SpriteFont>("Fonts/TimeLeft");
            _scoreFont = Content.Load<SpriteFont>("Fonts/Score");

            _typesound = Content.Load<SoundEffect>("SFX/typesound");
            _writesound = Content.Load<SoundEffect>("SFX/writesound");


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private int _frames = 0;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            //In case the player rotates their screen...
            _gfx.Width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            _gfx.Height = GraphicsDevice.PresentationParameters.BackBufferHeight;

            //Now we need to figure out if we're landscape or not.
            _isLandscape = GraphicsDevice.PresentationParameters.DisplayOrientation == DisplayOrientation.LandscapeLeft || GraphicsDevice.PresentationParameters.DisplayOrientation == DisplayOrientation.LandscapeLeft;


            switch (_gameState)
            {
                case 0: //intro
                    foreach(var touch in TouchPanel.GetState())
                    {
                        if(touch.State == TouchLocationState.Released)
                        {
                            _countdownHead = "Ready to play?";
                            _countdownDesc = $"Level {_level} - you can earn:\n\n{_cpCpuBeatReward * _level} CP for beating the computer.\n{_cpLevelBeatReward * _level} CP for surviving.";
                            _gameState = 2;
                            break;
                        }
                    }
                    break;
                case 1: //In-game.
                    _frames++;

                    TouchLocation previous = default(TouchLocation);


                    foreach (var touch in TouchPanel.GetState())
                    {
                        if (!touch.TryGetPreviousLocation(out previous))
                            continue;

                        var delta = previous.Position - touch.Position;
                        _playerPaddleY -= (delta.Y / _gfx.Width);

                    }

                    float ballSize = (_isLandscape) ? _ballSizeLandscape : _ballSize;
                    float paddleSize = (_isLandscape) ? _paddleWidthLandscape : _paddleWidth;
                    float paddleHeight = (_isLandscape) ? _paddleHeightLandscape : _paddleHeight;

                    _ballRect.Width = (int)Math.Round(MathHelper.Lerp(0, _gfx.Width, ballSize));
                    _ballRect.Height = (int)Math.Round(MathHelper.Lerp(0, _gfx.Height, ballSize));
                    _ballRect.X = (int)Math.Round(MathHelper.Lerp(0, _gfx.Width, _ballX - (ballSize / 2)));
                    _ballRect.Y = (int)MathHelper.Clamp((float)Math.Round(MathHelper.Lerp(0, _gfx.Height, _ballY - (ballSize / 2))), 0, _gfx.Height);

                    _playerRect.Width = (int)MathHelper.Lerp(0, _gfx.Width, paddleSize);
                    _playerRect.Height = (int)MathHelper.Lerp(0, _gfx.Height, paddleHeight);

                    _cpuRect.Width = _playerRect.Width;
                    _cpuRect.Height = _playerRect.Height;

                    float screenMarginX = 0.01f;

                    _playerRect.X = (int)MathHelper.Lerp(0, _gfx.Width, screenMarginX);
                    _cpuRect.X = (int)MathHelper.Lerp(0, _gfx.Width, (1F - screenMarginX) - paddleSize);

                    _playerRect.Y = (int)MathHelper.Lerp(0, _gfx.Height, _playerPaddleY - (paddleHeight / 2));
                    _cpuRect.Y = (int)MathHelper.Lerp(0, _gfx.Height, _cpuPaddleY - (paddleHeight / 2));



                    if (_ballRect.Top <= 0)
                    {
                        _ballY = ballSize / 2;
                        _ballVelY = -_ballVelY;
                    }

                    if (_ballRect.Bottom >= _gfx.Height)
                    {
                        _ballY = 1f - (ballSize / 2F);
                        _ballVelY = -_ballVelY;
                    }


                    if (_cpuRect.Intersects(_ballRect))
                    {
                        float ballInPaddle = _ballY - (_cpuPaddleY - (paddleHeight / 2));
                        float hitLocationPercentage = ballInPaddle / paddleHeight;



                        _ballVelX = -_ballVelX;
                        _ballVelY = MathHelper.Lerp(-_ballVelYStart, _ballVelYStart, hitLocationPercentage);
                        _typesound.Play();
                    }

                    if (_playerRect.Intersects(_ballRect))
                    {
                        float ballInPaddle = _ballY - (_playerPaddleY - (paddleHeight / 2));
                        float hitLocationPercentage = ballInPaddle / paddleHeight;

                        _ballX = screenMarginX + paddleSize + (ballSize / 2);

                        _ballVelX = -_ballVelX;
                        _ballVelY = MathHelper.Lerp(-_ballVelYStart, _ballVelYStart, hitLocationPercentage);
                        _typesound.Play();
                    }

                    if (_ballRect.Left >= _gfx.Width)
                    {
                        _ballX = 0.85F;
                        _ballY = 0.5F;
                        _codepoints += (_cpCpuBeatReward * _level);
                        _ballVelX = -_ballVelX;
                        _countdownHead = "You Beat the Computer!";
                        _countdownDesc = $"{_cpCpuBeatReward * _level} Codepoints rewarded.";
                        _gameState++;
                    }

                    if (_ballRect.Right <= 0)
                    {
                        _ballX = 0.5F;
                        _ballY = 0.5F;
                        _ballVelX = 0.0025F;
                        _ballVelY = 0.0025F;
                        _ballVelYStart = 0.0025F;
                        _playerPaddleY = 0.5F;
                        _cpuPaddleY = 0.5F;
                        _countdownHead = "You Lost!";
                        _countdownDesc = $"You missed out on {_codepoints} Codepoints!";
                        _codepoints = 0;
                        _timeLeftSeconds = 60;
                        _level = 1;
                        _gameState++;
                        return;
                    }



                    _ballX += _ballVelX;
                    _ballY += _ballVelY;

                    _timeLeftSeconds -= gameTime.ElapsedGameTime.TotalSeconds;

                    if(_timeLeftSeconds < 0)
                    {
                        _timeLeftSeconds = 60;
                        _ballVelYStart *= 2;
                        _ballVelX *= 2;
                        _codepoints += _cpLevelBeatReward * _level;
                        _countdownHead = $"Level {_level} Complete";
                        _level++;
                        _countdownDesc = $"Now on Level {_level} - you can earn:\n\n{_cpCpuBeatReward * _level} CP for beating the computer.\n{_cpLevelBeatReward * _level} CP for surviving.";
                        _gameState++;
                    }

                    if (_frames >= 7)
                    {
                        _frames = 0;
                        return;
                    }

                    if (_cpuPaddleY > _ballY)
                    {
                        _cpuPaddleY -= (_ballVelYStart * _cpuSpeed);
                    }
                    else if (_cpuPaddleY < _ballY)
                    {
                        _cpuPaddleY += (_ballVelYStart * _cpuSpeed);
                    }


                    _playerPaddleY = MathHelper.Clamp(_playerPaddleY, paddleHeight / 2, 1 - (paddleHeight / 2));
                    break;
                case 2: //Countdown.
                    _countdownBeepTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if(_countdownBeepTimer>=1)
                    {
                        _writesound.Play();
                        _countdownBeepTimer = 0;
                    }
                    _countdown -= gameTime.ElapsedGameTime.TotalSeconds;
                    if(_countdown < 0)
                    {
                        _gameState--;
                        _countdown = 3;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _gfx.Device.Clear(Color.Black);

            _gfx.BeginDraw();

            switch (_gameState)
            {
                case 0:
                    string welcome = _activity.ApplicationContext.Resources.GetString(Resource.String.WelcomeToPong);

                    var measure = TextRenderer.MeasureText(welcome, _bodyFont, (int)(_gfx.Width / 1.25), WrapMode.Words);

                    _gfx.DrawString(welcome, (_gfx.Width - (int)(_gfx.Width / 1.25)) / 2, (_gfx.Height - (int)measure.Y) / 2, Color.White, _bodyFont, TextAlignment.Center, (int)(_gfx.Width / 1.25), WrapMode.Words);

                    break;
                case 1:

                    //Draw the two paddles.
                    _gfx.DrawRectangle(_playerRect.X, _playerRect.Y, _playerRect.Width, _playerRect.Height, Color.White);
                    _gfx.DrawRectangle(_cpuRect.X, _cpuRect.Y, _cpuRect.Width, _cpuRect.Height, Color.White);

                    //Draw the ball.
                    _gfx.DrawCircle(_ballRect.X + (_ballRect.Width / 2), _ballRect.Y + (_ballRect.Width / 2), _ballRect.Width / 2, Color.White);
                    break;
                case 2:
                    var headMeasure = TextRenderer.MeasureText(_countdownHead, _bodyFont, (int)(_gfx.Width / 1.25), WrapMode.Words);

                    _gfx.DrawString(_countdownHead, (_gfx.Width - (int)(_gfx.Width / 1.25)) / 2, _gfx.Height / 4, Color.White, _bodyFont, TextAlignment.Center, (int)(_gfx.Width / 1.25), WrapMode.Words);
                    _gfx.DrawString(_countdownDesc, (_gfx.Width - (_gfx.Width/2)) / 2, (_gfx.Height / 4) + (int)headMeasure.Y + 30, Color.White, _scoreFont, TextAlignment.Center, _gfx.Width/2, WrapMode.Words);



                    string countdownText = Math.Round(_countdown).ToString();
                    var countdownMeasure = _timeLeftFont.MeasureString(countdownText);

                    _gfx.Batch.DrawString(_timeLeftFont, countdownText, new Vector2((_gfx.Width - countdownMeasure.X) / 2, (_gfx.Height - countdownMeasure.Y) / 2), Color.White);



                    break;
            }
            //Measure the "Seconds Left" counter.
            string secondsleft = $"{Math.Round(_timeLeftSeconds)} Seconds Left";

            //Render the seconds left counter
            _gfx.DrawString(secondsleft, (_gfx.Width - (_gfx.Width / 2)) / 2, 20, Color.White, _timeLeftFont, TextAlignment.Center, _gfx.Width / 2, WrapMode.Words);

            //Level text
            string level = $"Level: {_level}";
            var lMeasure = TextRenderer.MeasureText(level, _scoreFont, 0, WrapMode.None);

            //render level text
            _gfx.DrawString(level, 20, (_gfx.Height - (int)lMeasure.Y) - 20, Color.White, _scoreFont, TextAlignment.Left, 0, WrapMode.None);

            //Codepoints text
            string codepoints = $"{_codepoints} Codepoints";
            var cMeasure = TextRenderer.MeasureText(codepoints, _scoreFont, 0, WrapMode.None);

            //render codepoints text
            _gfx.DrawString(codepoints, (_gfx.Width - (int)cMeasure.X)-20, (_gfx.Height - (int)lMeasure.Y) - 20, Color.White, _scoreFont, TextAlignment.Left, 0, WrapMode.None);



            _gfx.EndDraw();

            base.Draw(gameTime);
        }
    }
}
