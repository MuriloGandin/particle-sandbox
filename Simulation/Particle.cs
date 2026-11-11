using Microsoft.Xna.Framework;

namespace ParticleSandbox.Simulation;

public class Particle
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Vector2 Acceleration { get; set; }
    private Vector2 _accumulatedForce;

    public float Mass { get; set; }

    public void Update(float deltaTime)
    {
        Acceleration = _accumulatedForce / Mass;

        Velocity += Acceleration * deltaTime;
        Position += Velocity * deltaTime;

        _accumulatedForce = Vector2.Zero;
    }

    public void ApplyForce(Vector2 force)
    {
        _accumulatedForce += force;
    }

    public void ResolveGroundCollision(float groundY, float radius, float restitution)
    {
        if (Position.Y + radius >= groundY)
        {
            Position = new Vector2(
                Position.X,
                groundY - radius
            );

            if (Velocity.Y > 0)
            {
                Velocity = new Vector2(
                    Velocity.X,
                    -Velocity.Y * restitution
                );
            }
        }
    }
}