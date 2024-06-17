using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SLC.Bad4Business.Core
{
    public class DetectionModule : MonoBehaviour
    {
        public float radius = 10.0f;
        public float angle = 50.0f;

        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;

        public GameObject player;

        public LayerMask targetMask;
        public LayerMask obstructionMask;

        public bool isPlayerVisible;


        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");


        }
    }
}
