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

        private PlayerController pc;
        private bool canSpawn = true;

        // Start is called before the first frame
        void Start()
        {
            pc = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (pc._distance % distanceBetweenSections < 0.2 && !pc.IsGrounded() && canSpawn)
            {
                Instantiate(easySections[Random.Range(0, easySections.Length)], transform);
                canSpawn = false;
            }
            else if (pc._distance % distanceBetweenSections > 0.5 && !canSpawn)
            {
                canSpawn = true;
            }

        }
    }
}