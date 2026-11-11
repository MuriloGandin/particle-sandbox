using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace ParticleSandbox.Simulation
{
    public class ParticleSystem
    {
        public const float ParticleRadius = 12f;
        private const float Restitution = 0.8f;
        private readonly List<Particle> _particles = new();
        private Texture2D _particleTexture;

        private void CreateParticle(Vector2 position, Vector2 acceleration, Vector2 velocity, float mass)
        {
            _particles.Add(new Particle
            {
                Position = position,
                Velocity = velocity,
                Acceleration = acceleration,
                Mass = mass
            });
        }

        public void InitializeParticles()
        {
            _particles.Clear();
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                CreateParticle(
                    position : new Vector2(150 + random.Next(0, 200), 100),
                    velocity : new Vector2(random.Next(-200, 201), random.Next(-400, 501)),
                    acceleration : Vector2.Zero,
                    mass : 1f
                    );
            }
        }

        public void Update(float deltaTime, float gravity, float groundY)
        {
            foreach (Particle particle in _particles)
            {

                Vector2 gravityForce = new Vector2(
                    0,
                    particle.Mass * gravity
                );

                particle.ApplyForce(gravityForce);
                particle.Update(deltaTime);

                particle.ResolveGroundCollision(groundY, ParticleRadius, Restitution);
                CheckParticlesCollisions();
            }
        }

        public void DrawParticles(SpriteBatch sprite)
        {
            foreach (Particle particle in _particles)
            {
                sprite.Draw(
                    _particleTexture,
                    particle.Position,
                    null,
                    Color.White,
                    0f,
                    new Vector2(ParticleSystem.ParticleRadius),
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        public void LoadParticleTexture(GraphicsDevice graphicsDevice)
        {
            _particleTexture = CreateCircleTexture((int) ParticleSystem.ParticleRadius, graphicsDevice);
        }

        private Texture2D CreateCircleTexture(int radius, GraphicsDevice graphicsDevice)
        {
            int diameter = radius * 2;

            Texture2D texture = new Texture2D(
                graphicsDevice,
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

        private void CheckParticlesCollisions()
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                for (int j = i + 1; j < _particles.Count; j++)
                {
                    Particle p1 = _particles[i];
                    Particle p2 = _particles[j];
                    Vector2 delta = p2.Position - p1.Position;
                    float distanceSquared = delta.LengthSquared();
                    float minDistance = ParticleRadius * 2;
                    if (distanceSquared < minDistance * minDistance)
                    {
                        ResolveParticleCollision(p1, p2);
                    }
                }
            }
        }

        private void ResolveParticleCollision(Particle p1, Particle p2)
        {
            Vector2 delta = p2.Position - p1.Position;
            float distance = delta.Length();
            float mindistance = ParticleRadius * 2;

            if (distance >= mindistance)
                return;

            Vector2 direction = delta / distance;

            float overlap = mindistance - distance;
            p1.Position -= direction * (overlap / 2);
            p2.Position += direction * (overlap / 2); 

            Vector2 relativeVelocity = p2.Velocity - p1.Velocity;

            float speedAlongNormal = Vector2.Dot(relativeVelocity, direction);

            if (speedAlongNormal > 0)
                return;

            float impulseMagnitude = -(1 + Restitution) * speedAlongNormal / 2f;

            Vector2 impulse = impulseMagnitude * direction;

            p1.Velocity -= impulse;
            p2.Velocity += impulse;
        }
    }
}
