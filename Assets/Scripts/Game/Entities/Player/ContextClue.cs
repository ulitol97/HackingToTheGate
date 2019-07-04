using System;
using UnityEngine;

namespace Game.Entities.Player
{
    public class ContextClue : MonoBehaviour
    {
        public GameObject contextClue;

        public void Enable()
        {
            contextClue.SetActive(true);
        }

        public void Disable()
        {
            contextClue.SetActive(false);
        }
    }
}
