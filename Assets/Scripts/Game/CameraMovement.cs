using System;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The CameraMovement class extends the functionality of a Unity camera component by making the camera
    /// locate a target to follow and move towards that target if needed.
    /// </summary>
    public class CameraMovement : MonoBehaviour
    {
        /// <summary>
        /// Transform object containing the coordinates the camera should follow.
        /// </summary>
        public Transform target;

        /// <summary>
        /// A multiplying factor to the camera's following speed.
        /// </summary>
        /// <remarks>A default value of "0.12" is enough for an average speed.</remarks>
        [System.ComponentModel.DefaultValue(0.12)]
        public float smoothing;

        /// <summary>
        /// Vector containing the X and Y highest values the camera position can take.
        /// </summary>
        public Vector2 maxBoundaries;
        
        /// <summary>
        /// Vector containing the X and Y lowest values the camera position can take. 
        /// </summary>
        public Vector2 minBoundaries;

        /// <summary>
        /// Function called on each frame the CameraMovement script is present into the game.
        /// It is is charge of arranging the camera position updates./>
        /// </summary>
        /// <remarks>LateUpdate was used instead of Update to give time to all game objects (including
        /// the camera target) to update their positions before the camera follows them.
        /// </remarks>
        private void LateUpdate()
        {
            FollowTarget();
        }

        /// <summary>
        /// Updates the camera position by interpolating the targets position and the camera's current position.
        /// The camera's Z position is kept the same because only 2 dimensions are needed for the game.
        /// </summary>
        private void FollowTarget()
        {
            if (transform.position != target.position)
            {
                Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
                CheckCameraBoundaries(ref targetPosition);
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
            }
        }

        /// <summary>
        /// Receives a vector and checks that it's coordinated do not surpass the boundaries specified.
        /// <see cref="minBoundaries"/> <see cref="maxBoundaries"/>
        /// If the input vector's coordinates are out of bounds, corrects them to the closest in bounds position.
        /// </summary>
        /// <param name="targetPosition">Vector containing the coordinated to be adjusted</param>
        private void CheckCameraBoundaries(ref Vector3 targetPosition)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBoundaries.x, maxBoundaries.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBoundaries.y, maxBoundaries.y);
        }
    }
}
