using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ParticleSandbox.Simulation;
using System.Collections.Generic;
using System;

namespace ParticleSandbox;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _groundTexture;

    private ParticleSystem _particleSystem;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _particleSystem = new ParticleSystem();
        _particleSystem.InitializeParticles();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _groundTexture = new Texture2D(GraphicsDevice, 1, 1);
        _groundTexture.SetData(new[] { Color.White });
        _particleSystem.LoadParticleTexture(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        float gravity = 500f;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float groundY = GraphicsDevice.Viewport.Height - 40;

        _particleSystem.Update(deltaTime, gravity, groundY);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        float groundY = GraphicsDevice.Viewport.Height - 40;

        _spriteBatch.Draw(
            _groundTexture,
            new Rectangle(
                0,
                (int)groundY,
                GraphicsDevice.Viewport.Width,
                4
            ),
            Color.White
        );

        _particleSystem.DrawParticles(_spriteBatch);

        _spriteBatch.End();


        base.Draw(gameTime);
    }
}
