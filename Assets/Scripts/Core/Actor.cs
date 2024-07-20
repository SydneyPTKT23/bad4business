using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class Actor : MonoBehaviour
    {
        // Dictates the "team" of the actor. Actors with the same affiliation are friendly with each other.
        public int affiliation;

        // The origin point other actors will aim at when attacking.
        public Transform aimPoint;

        private ActorManager m_actorManager;

        private void Start()
        {
            m_actorManager = FindObjectOfType<ActorManager>();

            // Register this instance to actor manager.
            if (!m_actorManager.Actors.Contains(this))
            {
                m_actorManager.Actors.Add(this);
            }
        }
    }
}
