using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ParticleSandbox.Simulation;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;

namespace ParticleSandbox;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private List<Particle> _particles = new();
    private Texture2D _particleTexture;
    private Texture2D _groundTexture;
    private const float ParticleRadius = 12f;
    private const float Restitution = 0.8f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    private Texture2D CreateCircleTexture(int radius)
    {
        int diameter = radius * 2;

        Texture2D texture = new Texture2D(
            GraphicsDevice,
            diameter,
            diameter
        );

        Color[] colors = new Color[diameter * diameter];

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                int dx = x - radius;
                int dy = y - radius;

                bool insideCircle = dx * dx + dy * dy <= radius * radius;

                colors[y * diameter + x] = insideCircle
                    ? Color.White
                    : Color.Transparent;
            }
        }

        texture.SetData(colors);

        return texture;

    }

    protected override void Initialize()
    {
        Random random = new Random();
        for (int i = 0; i < 10; i++)
        {
            _particles.Add(new Particle
            {
                Position = new Vector2(150 + i * 50, 100),
                Velocity = new Vector2(random.Next(-200, 201), random.Next(-400, 501)),
                Acceleration = Vector2.Zero,
                Mass = 1f
            });
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _particleTexture = CreateCircleTexture((int)ParticleRadius);
        _groundTexture = new Texture2D(GraphicsDevice, 1, 1);
        _groundTexture.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        float gravity = 500f;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (Particle particle in _particles)
        {

            Vector2 gravityForce = new Vector2(
                0,
                particle.Mass * gravity
            );

            particle.ApplyForce(gravityForce);
            particle.Update(deltaTime);

            float groundY = GraphicsDevice.Viewport.Height - 40;
            particle.ResolveGroundCollision(groundY, ParticleRadius, Restitution);
        }

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

        foreach (Particle particle in _particles)
        {
            _spriteBatch.Draw(
                _particleTexture,
                particle.Position,
                null,
                Color.White,
                0f,
                new Vector2(ParticleRadius),
                1f,
                SpriteEffects.None,
                0f
            );
        }

        _spriteBatch.End();


        base.Draw(gameTime);
    }
}
