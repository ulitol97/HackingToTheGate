using UnityEngine;

namespace Game.Enemies
{
    public class LogEnemy : Enemy
    {
        public Transform target;
        public Transform homePosition;

        public float chaseRadius;

        public float attackRadius;

        // Start is called before the first frame update
        private void Start()
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        // Update is called once per frame
        private void Update()
        {
            CheckDistance();
        }

        private void CheckDistance()
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= chaseRadius && distanceToTarget > attackRadius)
            {
                // Approach but never more than attack radius.
                transform.position = Vector3.MoveTowards(transform.position, 
                    target.position, moveSpeed * Time.deltaTime);
            }
        }

    }
}
