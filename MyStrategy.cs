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

        public static Vector3D DanToPlane(Vector3D point, Vector3D pointOnPlane, Vector3D planeNormal)
        {
            return Vector3D.DotProduct(point - pointOnPlane, planeNormal) * planeNormal;
        }

        public static Vector3D DanToSphereInner(Vector3D point, Vector3D sphereCenter, double sphereRadius)
        {
            Vector3D norm = sphereCenter - point;
            norm.Normalize();
            return (sphereRadius - (point - sphereCenter).Length) * norm;
        }

        public static Vector3D DanToSphereOuter(Vector3D point, Vector3D sphereCenter, double sphereRadius)
        {
            Vector3D norm = point - sphereCenter;
            norm.Normalize();
            return ((point - sphereCenter).Length - sphereRadius) * norm;
        }

        public static Vector3D Min (Vector3D v1, Vector3D v2)
        {
            if (v1.Length >= v2.Length)
            {
                return v1;
            }
            return v2;
        }

        public static Vector3D DanToArenaQuarter(Vector3D point, Arena arena)
        {
            // Ground
            Vector3D dan = DanToPlane(point, new Vector3D(0, 0, 0), new Vector3D(0, 1, 0));
            // Ceiling
            dan = Min(dan, DanToPlane(point, new Vector3D(0, arena.height, 0), new Vector3D(0, -1, 0)));
            // Side x
            dan = Min(dan, DanToPlane(point, new Vector3D(arena.width / 2, 0, 0), new Vector3D(-1, 0, 0)));
            // Side z (goal)
            dan = Min(dan, DanToPlane(point, new Vector3D(0, 0, (arena.depth / 2) + arena.goal_depth), new Vector3D(0, 0, -1)));
            // Side z
            Vector3D v = new Vector3D(point.X, point.Y, 0) - new Vector3D((arena.goal_width / 2) - arena.goal_top_radius, arena.goal_height - arena.goal_top_radius, 0);
            if (point.X >= (arena.goal_width / 2) + arena.goal_side_radius || point.Y >= arena.goal_height + arena.goal_side_radius ||
            (v.X > 0 && v.Y > 0 && v.Length >= arena.goal_top_radius + arena.goal_side_radius))
            {
                dan = Min(dan, DanToPlane(point, new Vector3D(0, 0, arena.depth / 2), new Vector3D(0, 0, -1)));
            }
            // Side x & ceiling (goal)
            if (point.Z >= (arena.depth / 2) + arena.goal_side_radius)
            {   // x
                dan = Min(dan, DanToPlane(point, new Vector3D(arena.goal_width / 2, 0, 0), new Vector3D(-1, 0, 0)));
                // y
                dan = Min(dan, DanToPlane(point, new Vector3D(0, arena.goal_height, 0), new Vector3D(0, -1, 0)));
            }




        }


        
       








        public static Vector3D DanToArena(Vector3D point)
        {
            bool negateX = point.X < 0;
            bool negateZ = point.Z < 0;
            if (negateX)
            {
                point.X = -point.X;
            }
            if (negateZ)
            {
                point.Z = -point.Z;
            }
            Vector3D result = DanToArenaQurter(point);
            if (negateX)
            {
                result.X = -result.X;
            }
            if (negateZ)
            {
                result.Z = -result.Z;
            }
            return result;
        }


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

        public static void CollideArena(Ball b)
        {

        }

        function collide_with_arena(e: Entity):
let distance, normal = dan_to_arena(e.position)
let penetration = e.radius - distance
if penetration > 0:
e.position += penetration* normal
let velocity = dot(e.velocity, normal) - e.radius_change_speed
if velocity< 0:
e.velocity -= (1 + e.arena_e) * velocity* normal
return normal
return None



        public void Act(Robot me, Rules rules, Game game, Action action)
        {
                
        }
    }
}
