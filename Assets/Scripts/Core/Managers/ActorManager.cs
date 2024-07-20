using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.Core
{
    public class ActorManager : MonoBehaviour
    {
        public List<Actor> Actors { get; private set; }
        public GameObject MainActor { get; private set; }

        public void SetMainActor(GameObject t_player) => MainActor = t_player;

        private void Awake()
        {
            Actors = new List<Actor>();
        }
    }
}