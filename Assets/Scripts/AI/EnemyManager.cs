using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.Bad4Business.AI
{
    public class EnemyManager : MonoBehaviour
    {
        public List<EnemyController> Enemies { get; private set; }
        public int NumberOfEnemiesTotal { get; private set; }
        public int NumberOfEnemiesRemaining => Enemies.Count;

        private void Awake()
        {
            Enemies = new List<EnemyController>();
        }

        public void RegisterEnemy(EnemyController t_instance)
        {
            Enemies.Add(t_instance);

            NumberOfEnemiesTotal++;
        }

        public void UnregisterEnemy(EnemyController t_instance)
        {
            Enemies.Remove(t_instance);
        }
    }
}
