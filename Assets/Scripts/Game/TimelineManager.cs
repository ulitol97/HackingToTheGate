using UnityEngine;
using UnityEngine.Playables;

namespace Game
{
    public class TimelineManager : MonoBehaviour
    {
        private bool fix = false;
        public Animator playerAnimator;
        public RuntimeAnimatorController playerAnimController;
        public PlayableDirector director;
        
        // Start is called before the first frame update
        private void OnEnable()
        {
            playerAnimController = playerAnimator.runtimeAnimatorController;
            playerAnimator.runtimeAnimatorController = null;
        }

        // Update is called once per frame
        private void Update()
        {
            if (director.state != PlayState.Playing && !fix)
            {
                fix = true;
                playerAnimator.runtimeAnimatorController = playerAnimController;
            }
        }
    }
}
