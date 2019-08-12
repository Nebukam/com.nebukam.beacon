using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Nebukam.Signals;

namespace Nebukam.Beacon
{

    [DisallowMultipleComponent]
    public class BeaconController : MonoBehaviour
    {

        public BeaconIntention beaconIntentions;
        public BeaconProcessor[] beaconProcessors;

        public float speed = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            if (beaconIntentions == null)
            {
                beaconIntentions = GetComponent<BeaconIntention>();
            }
        }

        // Update is called once per frame
        void Update()
        {

            if(beaconIntentions == null) { return; }

            float3 position = transform.position;
            float3 lookAt = normalize(beaconIntentions.goalLocation - position);
            float3 velocity = lookAt * speed;

            if(beaconProcessors != null)
            {
                BeaconProcessor processor;
                for (int i = 0, count = beaconProcessors.Length; i < count; i++)
                {
                    processor = beaconProcessors[i];
                    if (!processor.enabled) { continue; }
                    processor.Process(ref position, ref velocity, ref lookAt);
                }
            }

            transform.position = position;

        }

    }
}