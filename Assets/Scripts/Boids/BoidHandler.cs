using System.Collections.Generic;
using UnityEngine;

namespace BoidSystem
{
    public class BoidHandler : MonoBehaviour
    {
        [SerializeField] private string boidGroupName = "Boids";

        [SerializeField] private ushort flockingPercentile = 100;
        [SerializeField] private ushort velocityMatchPercentile = 8;
        [SerializeField] private ushort avoidDistance = 20;
        [SerializeField] private ushort avoidancePercentile = 5;
        [SerializeField] private ushort destinationSeekingPercentile = 10;
        [SerializeField] private ushort steeringInfluence = 2;

        [SerializeField] private ushort maximumSpeed = 12;

        [SerializeField] private ushort boidAmount = 10;

        [SerializeField] private Boid boidPrefab;

        [SerializeField] private Transform customDestination;
        
        // steerObject is used to determine the direction the group moves to. It's based on the forward direction.
        [SerializeField] private Transform steerObject;

        private List<Boid> boids = new();
        private Transform boidGroup;

        private void Awake()
        {
            CreateBoids();
        }
        // Start is called before the first frame update
        void Start()
        {
            InitializePosition();
        }

        // Update is called once per frame
        void Update()
        {
            foreach(Boid boid in boids)
            {
                MoveBoidToNewPosition(boid);
            }
        }

        public void CreateBoids()
        {
            boidGroup = new GameObject(boidGroupName).transform;
            for (int i = 0; i < boidAmount; i++)
            {
                boids.Add(Instantiate(boidPrefab, boidGroup));
            }
        }

        public void InitializePosition()
        {
            #region
            foreach(Boid boid in boids)
            {
                boid.position = new Vector3(Random.Range(-20, 20), Random.Range(5, 15), Random.Range(-20, 20));
            }
            #endregion
            // To do: Random start positions for boids
            // Region above is for debug purposes only and should be replaced by a defined area.
        }

        public void MoveBoidToNewPosition(Boid boid)
        {
            // To do: consider the forward direction of the boids and make them turn to change direction
            // also consider not having influence from boids behind (out of sight).
            Vector3 flock = Flock(boid);
            Vector3 avoid = Avoid(boid);
            Vector3 matchVelocity = MatchVelocity(boid);
            Vector3 toDestination = MoveToDestination(boid);
            Vector3 steer = SteerEffect();

            boid.velocity += flock + avoid + matchVelocity + toDestination + steer;
            LimitVelocity(boid);
            boid.velocity *= Time.deltaTime;
            boid.position += boid.velocity;
        }

        private Vector3 Flock(Boid boid)
        {
            Vector3 perceivedCenter = Vector3.zero;

            foreach(Boid other in boids)
            {
                if(other != boid)
                {
                    perceivedCenter += other.position;
                }
            }
            perceivedCenter /= (boids.Count - 1);

            return (perceivedCenter - boid.position) / flockingPercentile;
        }

        private Vector3 Avoid(Boid boid)
        {
            Vector3 avoidance = Vector3.zero;
            foreach(Boid other in boids)
            {
                if(other != boid)
                {
                    if ((other.position - boid.position).sqrMagnitude < Mathf.Pow(avoidDistance, 2))
                    {
                        avoidance -= (other.position - boid.position);
                    }
                }
            }
            return avoidance / avoidancePercentile;
        }

        private Vector3 MatchVelocity(Boid boid)
        {
            Vector3 perceivedVelocity = Vector3.zero;

            foreach (Boid other in boids)
            {
                if (other != boid)
                {
                    perceivedVelocity += other.velocity;
                }
            }

            perceivedVelocity /= (boids.Count - 1);

            return (perceivedVelocity - boid.velocity) / velocityMatchPercentile;
        }
        
        private Vector3 SteerEffect()
        {
            if (!steerObject)
            {
                return Vector3.zero;
            }
            return steerObject.forward * steeringInfluence;
        }

        private Vector3 MoveToDestination(Boid boid)
        {
            if (!customDestination)
            {
                return Vector3.zero;
            }
            return (customDestination.position - boid.position) / destinationSeekingPercentile;
        }

        private void LimitVelocity(Boid boid)
        {
            if(boid.velocity.sqrMagnitude > Mathf.Pow(maximumSpeed, 2))
            {
                boid.velocity = (boid.velocity / boid.velocity.magnitude) * maximumSpeed;
            }
        }
    }
}
