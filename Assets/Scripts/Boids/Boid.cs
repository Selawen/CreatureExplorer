using UnityEngine;

namespace BoidSystem
{
    public class Boid : MonoBehaviour
    {
        internal Vector3 velocity;
        internal Vector3 position;

        private void Update()
        {
            transform.Translate(position - transform.position);
        }
    }
}
