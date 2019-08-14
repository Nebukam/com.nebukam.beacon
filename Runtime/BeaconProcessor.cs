using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Nebukam.Signals;
using Nebukam.Common;

namespace Nebukam.Beacon
{

    public abstract class BeaconProcessor : MonoBehaviour
    {

        public BeaconIntention intention;
        public BeaconController controller;

        // Start is called before the first frame update
        void Start()
        {

        }

        /*
        // Update is called once per frame
        void Update()
        {


        }
        */

        public abstract void Process( ref float3 position, ref float3 velocity, ref float3 lookAt );

    }
}