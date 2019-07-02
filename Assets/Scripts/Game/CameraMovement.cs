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
        /// Transform property (position and rotation) of the game element the camera should follow.
        /// </summary>
        public Transform target;

        /// <summary>
        /// A multiplying factor to the camera's following speed.
        /// </summary>
        /// <remarks>A default value of "0.12" is enough for an average speed.</remarks>
        [System.ComponentModel.DefaultValue(0.12)]
        public float smoothing;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        /// <summary>
        /// Function called on each frame the CameraMovement script is present into the game.
        /// Checks for user controller input to manage future movement operations.
        /// </summary>
        /// <remarks>GetAxisRaw allows digital input instead of analog input, either the movement signal is sent or not.
        /// </remarks>
        void Update()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Function called on each frame the CameraMovement script is present into the game.
        /// It is executed after the instructions specified in Update. <see cref="Update"/>
        /// </summary>
        private void LateUpdate()
        {
            if (transform.position != target.position)
            {
                Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
            }
        }
    }
}
