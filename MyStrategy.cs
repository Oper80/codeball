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

    class Vec3D
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }

            public Vec3D(double x, double y, double z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }

            public double Len()
            {
                return Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            }

            public Vec3D Normalize()
            {
                double d = (1.0 / this.Len());

                return new Vec3D(this.X * d, this.Y * d, this.Z * d);
            }

            public static Vec3D operator +(Vec3D v1, Vec3D v2)
            {
                return new Vec3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
            }
            
                public static Vec3D operator -(Vec3D v1, Vec3D v2)
            {
                return new Vec3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
            }

            public static Vec3D operator * (Vec3D v, double m)
            {
                return new Vec3D(v.X * m, v.Y * m, v.Z * m);
            }
            
            public static void CollideRobots (Robot a, Robot b)
            {
                const double mass = 1 / ROBOT_MASS;
                Vec3D vecA = new Vec3D(a.x, a.y, a.z);
                Vec3D vecB = new Vec3D(b.x, b.y, b.z);
                Vec3D deltaPosition = vecB - vecA;
                double distance = deltaPosition.Len();
                double penetration = a.radius + b.radius - distance;
                if (penetration > 0)
                {
                    Vector3D normal = deltaPosition.Normalize();
                    vecA -= normal * penetration * mass;
                    vecB -= normal * penetration * mass;

                }
            }






let delta_velocity = dot(b.velocity - a.velocity, normal)
+ b.radius_change_speed - a.radius_change_speed
if delta_velocity< 0:
let impulse = (1 + random(MIN_HIT_E, MAX_HIT_E)) * delta_velocity* normal
a.velocity += impulse* k_a
b.velocity -= impulse* k_b

        }

        public void Act(Robot me, Rules rules, Game game, Action action)
        {
            
        }
    }
}
