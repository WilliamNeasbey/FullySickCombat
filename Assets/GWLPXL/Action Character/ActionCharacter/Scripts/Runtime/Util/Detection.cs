using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// goal, most re-used methods so dont need to re-write things
/// </summary>
namespace GWLPXL.ActionCharacter
{
    public enum DirectionType
    {
        None = 0,
        Front = 10,
        Back = 20,
        Right = 30,
        Left = 40,
        Any = 50,
        Above = 60,
        Below = 70
    }
    [System.Serializable]
    public struct HitDetect
    {
        public Vector3 VectorDirection;
        public DirectionType Direction;
        public HitDetect(Vector3 vd, DirectionType d)
        {
            VectorDirection = vd;
            Direction = d;
        }

    }
    [System.Serializable]
    public struct InputDetect
    {
        public Vector3 VectorDirection;
        public AxisMovementType Direction;
        public InputDetect(Vector3 vd, AxisMovementType d)
        {
            VectorDirection = vd;
            Direction = d;
        }

    }

    /// <summary>
    /// wrapper class for commonly used detection options in unity
    /// </summary>
    public static class Detection
    {
        static RaycastHit hit;
        static RaycastHit[] hits;
        static Ray ray;
        static RaycastHit2D hit2d;
        static NavMeshHit navhit;
        static Collider[] colls = new Collider[1];
        static Vector3 zero = new Vector3(0, 0, 0);
        static Ray[] rays;
        static float angle;
        static Vector3 targetDir;
        static float dot;

        //static HitDetect[] directions = new HitDetect[0];
        static DirectionType hitkey = DirectionType.None;
        static int hitdetectangle = 45;

        static AxisMovementType inputkey = AxisMovementType.Neutral;
        static int inputdetectangle = 45;
        static HitDetect[] directions = new HitDetect[0];

        /// <summary>
        /// float closest returns sqmag
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="targets"></param>
        /// <param name="closest"></param>
        /// <param name="closestT"></param>
        public static void FindClosest(Vector3 origin, Collider[] targets, out float closest, out Collider closestT)
        {
            closest = Mathf.Infinity;
            closestT = null;
            for (int i = 0; i < targets.Length; i++)
            {
                Vector3 dir = targets[0].transform.position - origin;
                float sqrdmag = dir.sqrMagnitude;
                if (sqrdmag < closest)
                {
                    closest = sqrdmag;
                    closestT = targets[i];
                }
            }
        }

        public static void FindClosest(Vector3 origin, Vector3 forward, float sightangle, List<Transform> targets, out float closest, out Transform closestT)
        {
            closest = Mathf.Infinity;
            closestT = null;
            for (int i = 0; i < targets.Count; i++)
            {
                Vector3 dir = targets[0].position - origin;
                float sqrdmag = dir.sqrMagnitude;
                if (sqrdmag < closest && Detection.HasSight(origin, forward, targets[i].position, sightangle))
                {
                    closest = sqrdmag;
                    closestT = targets[i];
                }
            }
        }
        public static DirectionType DetectHitDirection(Transform self, Vector3 other, float angle = 45f)
        {

            hitkey = DirectionType.None;
            directions = new HitDetect[6]
            {
                new HitDetect(self.forward, DirectionType.Front),
                new HitDetect(self.right, DirectionType.Right),
                new HitDetect(-self.right, DirectionType.Left),
                new HitDetect(-self.forward, DirectionType.Back),
                new HitDetect(self.up, DirectionType.Above),
                new HitDetect(-self.up, DirectionType.Below)

            };
            bool hasSight = false;
            for (int i = 0; i < directions.Length; i++)
            {
                hasSight = Detection.HasSight(self, directions[i].VectorDirection, other, angle);
                if (hasSight)
                {
                    hitkey = directions[i].Direction;
                    break;
                }
            }

            return hitkey;
        }
        public static DirectionType DetectHitDirection(Transform self, Transform other, float angle = 45f)
        {

            hitkey = DirectionType.None;
            HitDetect[] directions = new HitDetect[6]
            {
                new HitDetect(self.forward, DirectionType.Front),
                new HitDetect(self.right, DirectionType.Right),
                new HitDetect(-self.right, DirectionType.Left),
                new HitDetect(-self.forward, DirectionType.Back),
                new HitDetect(self.up, DirectionType.Above),
                new HitDetect(-self.up, DirectionType.Below)

            };
            bool hasSight = false;
            for (int i = 0; i < directions.Length; i++)
            {
                hasSight = Detection.HasSight(self, directions[i].VectorDirection, other, angle);
                if (hasSight)
                {
                    hitkey = directions[i].Direction;
                    break;
                }
            }

            return hitkey;
        }
        public static Vector3 ConvertDirection(DirectionType type, Transform transform)
        {
            switch (type)
            {
                case DirectionType.Above:
                    return transform.up;
                case DirectionType.Back:
                    return -transform.forward;
                case DirectionType.Below:
                    return -transform.up;
                case DirectionType.Front:
                    return transform.forward;
                case DirectionType.Left:
                    return -transform.right;
                case DirectionType.Right:
                    return transform.right;
            }
            return zero;
        }

        static Vector3 target;
        public static Vector3 DetectClosestTarget(Vector3 start, Vector3 fwd, float Radius, float angle, LayerMask Mask, List<string> filtertags)
        {

            target = new Vector3(0, 0, 0);
 
            Debug.DrawLine(start, start + fwd * Radius, Color.red);
            //not working as intended

            Collider[] all = Detection.SphereOverlapAll(start, Radius, Mask);

            List<Transform> possible = new List<Transform>();
            for (int i = 0; i < all.Length; i++)
            {

                List<string> tags = filtertags;
                Collider collider = all[i];
                if (tags.Count > 0)
                {
                    for (int j = 0; j < tags.Count; j++)
                    {
                        string tag = tags[j];
                        if (collider.gameObject.CompareTag(tag))
                        {
                            possible.Add(all[j].transform);
                        }
                    }
                }
                else
                {
                    possible.Add(collider.transform);
                }


            }

            //eventually make better.
            possible = possible.OrderBy(
            x => Vector3.Distance(start, x.transform.position)
            ).ToList();

            if (possible.Count > 0)
            {
                for (int i = 0; i < possible.Count; i++)
                {
                    if (Detection.HasSight(start, fwd, possible[i].position, angle) == false) continue;

                    Vector3 dir = possible[i].position - start;
                   // RaycastHit hit = Detection.CapsuleCast(start, start + headoffset, Radius, dir.normalized, Radius, Mask);

                    if (hit.collider != null)
                    {

                        target = hit.point;
                        target.y = start.y;
                       // t1.transform.forward = target - t1.position;
                        break;
                    }
                }
                //priority
                //ignores the 360 effect
            }

            return target;
        }
        public static DirectionType DetectHitDirection(Transform self, Transform other)
        {

            hitkey = DirectionType.None;
            HitDetect[] directions = new HitDetect[6]
            {
                new HitDetect(self.forward, DirectionType.Front),
                new HitDetect(self.right, DirectionType.Right),
                new HitDetect(-self.right, DirectionType.Left),
                new HitDetect(-self.forward, DirectionType.Back),
                new HitDetect(self.up, DirectionType.Above),
                new HitDetect(-self.up, DirectionType.Below)

            };
            bool hasSight = false;
            for (int i = 0; i < directions.Length; i++)
            {
                hasSight = Detection.HasSight(self, directions[i].VectorDirection, other, hitdetectangle);
                if (hasSight)
                {
                    hitkey = directions[i].Direction;
                    break;
                }
            }

            return hitkey;
        }


        public static AxisMovementType DetectInputMoveDirection(Transform self, Vector3 targetPos)
        {
            inputkey = AxisMovementType.Neutral;
            InputDetect[] directions = new InputDetect[4]
            {
                new InputDetect(self.forward, AxisMovementType.Forward),
                new InputDetect(self.right, AxisMovementType.Right),
                new InputDetect(-self.right, AxisMovementType.Left),
                new InputDetect(-self.forward, AxisMovementType.Backward),

            };
            bool hasSight = false;
            Vector3 selfpos = self.position;
            for (int i = 0; i < directions.Length; i++)
            {

                hasSight = Detection.HasSight(selfpos, directions[i].VectorDirection, targetPos, inputdetectangle);
                if (hasSight)
                {
                    inputkey = directions[i].Direction;
                    break;
                }
            }

            return inputkey;
        }
        public static Vector3 RandomNavmeshLocation(Transform agnetobject, float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += agnetobject.position;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                finalPosition = hit.position;
            }
            return finalPosition;
        }
        public static Vector3 DirectionToClosestEdge(Transform agentobject)
        {
            if (NavMesh.FindClosestEdge(agentobject.position, out navhit, NavMesh.AllAreas))
            {
                DrawCircle(agentobject.position, hit.distance, Color.red);
                Debug.DrawRay(navhit.position, Vector3.up, Color.red);
                return navhit.position - agentobject.position;
            }
            return zero;
        }

        static void DrawCircle(Vector3 center, float radius, Color color)
        {
            Vector3 prevPos = center + new Vector3(radius, 0, 0);
            for (int i = 0; i < 30; i++)
            {
                float angle = (float)(i + 1) / 30.0f * Mathf.PI * 2.0f;
                Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                Debug.DrawLine(prevPos, newPos, color);
                prevPos = newPos;
            }
        }
        public static bool IsAgentOnNavMesh(Transform agentObject)
        {
            Vector3 agentPosition = agentObject.position;

            // Check for nearest point on navmesh to agent, within onMeshThreshold
            if (NavMesh.SamplePosition(agentPosition, out navhit, 1f, NavMesh.AllAreas))
            {
                // Check if the positions are vertically aligned
                if (Mathf.Approximately(agentPosition.x, navhit.position.x)
                    && Mathf.Approximately(agentPosition.z, navhit.position.z))
                {
                    // Lastly, check if object is below navmesh
                    return agentPosition.y >= navhit.position.y;
                }
            }

            return false;
        }
        public static RaycastHit CapsuleCast(Vector3 p0, Vector3 p1, float radius, Vector3 dir, float distance, LayerMask layer)
        {
            Physics.CapsuleCast(p0, p1, radius, dir, out hit, distance, layer);
            return hit;
        }
        public static bool SimpleCapsuleCast(Vector3 p0, Vector3 p1, float radius, Vector3 dir, float distance, LayerMask layer)
        {
            return Physics.CapsuleCast(p0, p1, radius, dir, distance, layer);
          

        }
        public static bool SimpleCapsuleCast(Vector3 p0, Vector3 p1, float radius, Vector3 dir, float distance, LayerMask layer, QueryTriggerInteraction q = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.CapsuleCast(p0, p1, radius, dir, distance, layer);
          

        }
        public static bool SimpleSpherecast(Vector3 pos, float radius, Vector3 direction, float length, LayerMask layermask)
        {
            return Physics.SphereCast(pos, radius, direction, out hit, length, layermask);
        }
        public static RaycastHit SimpleSpherecastHit(Vector3 pos, float radius, Vector3 direction, float length, LayerMask layermask)
        {
            Physics.SphereCast(pos, radius, direction, out hit, length, layermask);
            return hit;
        }
        public static int CapsuleOverlapAllNonAlloc(Vector3 floor, Vector3 head, float radius, LayerMask mask)
        {
            return Physics.OverlapCapsuleNonAlloc(floor, head, radius, colls, mask);
        }
        public static Collider[] CapsuleOverlapAll(Vector3 floor, Vector3 head, float radius, LayerMask mask)
        {
            return Physics.OverlapCapsule(floor, head, radius, mask);
        }
        public static Collider[] SphereOverlapAll(Vector3 pos, float radius, LayerMask mask, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            colls = Physics.OverlapSphere(pos, radius, mask, query);
            return colls;
        }
        public static Collider[] SphereOverlapAll(Vector3 pos, float radius, LayerMask mask, List<string> filteredTags, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
        {
            colls = SphereOverlapAll(pos, radius, mask, query);
            if (filteredTags != null)
            {
                List<Collider> filtered = new List<Collider>();
                for (int i = 0; i < colls.Length; i++)
                {
                    filtered.Add(colls[i]);
                }
                for (int i = 0; i < filtered.Count; i++)
                {
                    string tag = filtered[i].gameObject.tag;
                    for (int j = 0; j < filteredTags.Count; j++)
                    {
                        if (CommonFunctions.WordEquals(tag, filteredTags[j]))
                        {
                            //remove this
                            filtered.RemoveAt(i);
                            break;
                        }
                       
                    }
                }

                return filtered.ToArray();
            }

            return colls;
        }
        public static RaycastHit[] SphereCastHitAll(Vector3 pos, float radius, Vector3 direction, float length, LayerMask layermask)
        {
            hits = Physics.SphereCastAll(pos, radius, direction, length, layermask);
            return hits;
        }

        public static bool SimpleRaycast(Ray ray, float length, LayerMask layermask)
        {
            return Physics.Raycast(ray, length, layermask);
        }
        public static bool SimpleRaycast(Ray ray, float length)
        {
            return Physics.Raycast(ray, length);
        }
        public static bool SimpleRaycast(Vector3 startpos, Vector3 direction, float length, LayerMask layermask)
        {
            ray = new Ray(startpos, direction);
            return SimpleRaycast(ray, length, layermask);
        }
        public static RaycastHit SimpleRaycastHit(Vector3 startpos, Vector3 direction, float length, LayerMask layermask)
        {
            ray = new Ray(startpos, direction);
            Physics.Raycast(ray, out hit, length, layermask);
            return hit;
        }
        public static RaycastHit SimpleRaycastHit(Vector3 startpos, Vector3 direction, float length)
        {
            ray = new Ray(startpos, direction);
            Physics.Raycast(ray, out hit, length);
            return hit;
        }
        public static bool SimpleRaycast2D(Vector2 startpos, Vector2 direction, float distance, LayerMask layermask)
        {

            hit2d = Physics2D.Raycast(startpos, direction, distance, layermask);
            return hit2d.collider != null;
        }
        public static RaycastHit2D SimpleRaycastHit2D(Vector2 startpos, Vector2 direction, float distance, LayerMask layermask)
        {
            hit2d = Physics2D.Raycast(startpos, direction, distance, layermask);
            return hit2d;
        }


        public static bool HasLineOfSight(Transform viewer, Transform viewee, LayerMask blockingLayers, float maxRange, int verticalRays = 3, float verticalRayStep = 1)
        {
            Vector3 direction = viewee.transform.position - viewer.transform.position;
            direction.Normalize();
            float verticalstart = 0;
            rays = new Ray[verticalRays];
            for (int i = 0; i < rays.Length; i++)
            {
                rays[i] = new Ray(viewer.transform.position + Vector3.up * verticalstart, direction);//refactor this out to be a direction
                verticalstart += verticalRayStep;
            }
            for (int i = 0; i < rays.Length; i++)
            {
                if (Detection.SimpleRaycast(rays[i], maxRange, blockingLayers))
                {
                    return true;
                }
            }

            return false;

        }
        public static bool HasLineOfSight(Transform viewer, Transform viewee, float maxRange, int verticalRays = 3, float verticalRayStep = 1)
        {
            Vector3 direction = viewee.transform.position - viewer.transform.position;
            direction.Normalize();
            float verticalstart = 0;
            rays = new Ray[verticalRays];
            for (int i = 0; i < rays.Length; i++)
            {
                rays[i] = new Ray(viewer.transform.position + Vector3.up * verticalstart, direction);//should also include horizontal?why not just spherecast?
                verticalstart += verticalRayStep;
            }
            for (int i = 0; i < rays.Length; i++)
            {
                if (Detection.SimpleRaycast(rays[i], maxRange))
                {
                    return true;
                }
            }

            return false;

        }


        public static bool HasSight(Transform user, Vector3 userforward, Vector3 target, float forwardSightAngle, PhysicsType type = PhysicsType.Physics3D)
        {
            angle = 0;
            targetDir = target - user.transform.position;
            targetDir = targetDir.normalized;

            switch (type)
            {
                case PhysicsType.Physics3D:
                    dot = Vector3.Dot(targetDir, userforward);
                    angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                    break;
                case PhysicsType.Physics2D:
                    angle = Vector2.Angle(userforward, target);
                    break;
            }

            return angle <= forwardSightAngle;
        }
        public static bool HasSight(Transform user, Vector3 userforward, Transform target, float forwardSightAngle, PhysicsType type = PhysicsType.Physics3D)
        {
            return HasSight(user, userforward, target.position, forwardSightAngle, type);
          
        }
        public static List<Collider> GetLockOnTargets(Transform user, float Radis, LayerMask Mask, DirectionType[] Direction, bool ordered = true)
        {
            Collider[] colls = Detection.SphereOverlapAll(user.position, Radis, Mask);
            List<Collider> Possible = new List<Collider>();
            ILockOnTarget self = user.GetComponent<ILockOnTarget>();

            bool any = false;
            for (int i = 0; i < Direction.Length; i++)
            {
                if (Direction[i] == DirectionType.Any)
                {
                    any = true;
                }
            }

            if (any)
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    ILockOnTarget lockon = colls[i].GetComponent<ILockOnTarget>();
                    if (lockon != null && lockon != self)
                    {
                        Possible.Add(colls[i]);
                    }
             
                }
            }
            else
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    for (int j = 0; j < Direction.Length; j++)
                    {
                        DirectionType type = Detection.DetectHitDirection(user, colls[i].transform);
                        if (type == Direction[j])
                        {
                            ILockOnTarget lockon = colls[i].GetComponent<ILockOnTarget>();
                            if (lockon != null && lockon != self)
                            {
                                Possible.Add(colls[i]);
                                break;
                            }
                        }

                    }
                    

                }
            }

            if (ordered)
            {
                Possible = Possible.OrderBy(
                x => Vector3.Distance(user.position, x.transform.position)
                ).ToList();
            }
            
            return Possible;
        }

        public static bool HasSight(Vector3 userPos, Vector3 userforward, Vector3 targetPos, float forwardSightAngle, PhysicsType type = PhysicsType.Physics3D)
        {
            angle = 0;
            targetDir = targetPos - userPos;
            targetDir = targetDir.normalized;

            switch (type)
            {
                case PhysicsType.Physics3D:
                    dot = Vector3.Dot(targetDir, userforward);
                    angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                    break;
                case PhysicsType.Physics2D:
                    angle = Vector2.Angle(userforward, targetPos);
                    break;
            }

            return angle <= forwardSightAngle;
        }
    }

}