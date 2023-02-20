using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhunkyPhrogs.Core;

namespace PhunkyPhrogs.Environment
{
    public class PlatformSpawner : MonoBehaviour
    {
        public GameObject[] easySections;
        public GameObject[] mediumSections;
        public GameObject[] hardSections;

        public float distanceBetweenSections;

        private Phrog _phrog;
        private bool canSpawn = true;

        // Start is called before the first frame
        void Start()
        {
            _phrog = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Phrog>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_phrog._distance % distanceBetweenSections < 0.2 && !_phrog.grounded && canSpawn)
            {
                Instantiate(easySections[Random.Range(0, easySections.Length)], transform);
                canSpawn = false;
            }
            else if (_phrog._distance % distanceBetweenSections > 0.5 && !canSpawn)
            {
                canSpawn = true;
            }

        }
    }
}