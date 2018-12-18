using static System.Math;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Com.CodeGame.CodeBall2018.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeBall2018.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        const int ROBOT_MIN_RADIUS = 1;
        const double ROBOT_MAX_RADIUS = 1.05;
        const int ROBOT_MAX_JUMP_SPEED = 15;
        const int ROBOT_ACCELERATION = 100;
        const int ROBOT_NITRO_ACCELERATION = 30;
        const int ROBOT_MAX_GROUND_SPEED = 30;
        const int ROBOT_ARENA_E = 0;
        const int ROBOT_RADIUS = 1;
        const int ROBOT_MASS = 2;
        const int TICKS_PER_SECOND = 60;
        const int MICROTICKS_PER_TICK = 100;
        const int RESET_TICKS = 2 * TICKS_PER_SECOND;
        const double BALL_ARENA_E = 0.7;
        const int BALL_RADIUS = 2;
        const int BALL_MASS = 1;
        const double MIN_HIT_E = 0.4;
        const double MAX_HIT_E = 0.5;
        const int MAX_ENTITY_SPEED = 100;
        const int MAX_NITRO_AMOUNT = 100;
        const int START_NITRO_AMOUNT = 50;
        const double NITRO_POINT_VELOCITY_CHANGE = 0.6;
        const int NITRO_PACK_X = 20;
        const int NITRO_PACK_Y = 1;
        const int NITRO_PACK_Z = 30;
        const double NITRO_PACK_RADIUS = 0.5;
        const int NITRO_PACK_AMOUNT = 100;
        const int NITRO_RESPAWN_TICKS = 10 * TICKS_PER_SECOND;
        const int GRAVITY = 30;

    
            
  
        public static void CollideRobots (Robot a, double jumpA, Robot b, double jumpB = 0)
        {
            const double mass = 1 / ROBOT_MASS;
            Vector3D vecA = new Vector3D(a.x, a.y, a.z);
            Vector3D vecB = new Vector3D(b.x, b.y, b.z);
            Vector3D aVel = new Vector3D(a.velocity_x, a.velocity_y, a.velocity_z);
            Vector3D bVel = new Vector3D(b.velocity_x, b.velocity_y, b.velocity_z);
            Vector3D deltaPosition = vecB - vecA;
            double distance = deltaPosition.Length;
            double penetration = a.radius + b.radius - distance;
            if (penetration > 0)
            {
                Vector3D normal = deltaPosition;
                normal.Normalize();
                vecA -= normal * penetration * mass;
                vecB -= normal * penetration * mass;
                double deltaVelocity = Vector3D.DotProduct(bVel - aVel, normal) + jumpB - jumpA;
                if (deltaVelocity < 0)
                {
                    System.Random rnd = new System.Random();
                    Vector3D impulse = (1 + MIN_HIT_E + rnd.NextDouble() * 0.1) * deltaVelocity * normal;
                    aVel += impulse * mass;
                    bVel -= impulse * mass;
                }

            }
            a.velocity_x = aVel.X;
            a.velocity_y = aVel.Y;
            a.velocity_z = aVel.Z;
            b.velocity_x = bVel.X;
            b.velocity_y = bVel.Y;
            b.velocity_z = bVel.Z;
        }

        public static void CollideBall(Robot a, double jumpA, Ball b)
        {
            const double k_a = (1 / ROBOT_MASS) / ((1 / ROBOT_MASS) + (1 / BALL_MASS));
            const double k_b = (1 / BALL_MASS) / ((1 / ROBOT_MASS) + (1 / BALL_MASS));
            Vector3D vecA = new Vector3D(a.x, a.y, a.z);
            Vector3D vecB = new Vector3D(b.x, b.y, b.z);
            Vector3D aVel = new Vector3D(a.velocity_x, a.velocity_y, a.velocity_z);
            Vector3D bVel = new Vector3D(b.velocity_x, b.velocity_y, b.velocity_z);
            Vector3D deltaPosition = vecB - vecA;
            double distance = deltaPosition.Length;
            double penetration = a.radius + b.radius - distance;
            if (penetration > 0)
            {
                Vector3D normal = deltaPosition;
                normal.Normalize();
                vecA -= normal * penetration * k_a;
                vecB -= normal * penetration * k_b;
                double deltaVelocity = Vector3D.DotProduct(bVel - aVel, normal) - jumpA;
                if (deltaVelocity < 0)
                {
                    System.Random rnd = new System.Random();
                    Vector3D impulse = (1 + MIN_HIT_E + rnd.NextDouble() * 0.1) * deltaVelocity * normal;
                    aVel += impulse * k_a;
                    bVel -= impulse * k_b;
                }

            }
            a.velocity_x = aVel.X;
            a.velocity_y = aVel.Y;
            a.velocity_z = aVel.Z;
            b.velocity_x = bVel.X;
            b.velocity_y = bVel.Y;
            b.velocity_z = bVel.Z;
        }

 



        public void Act(Robot me, Rules rules, Game game, Action action)
        {
            
        }
    }
}
