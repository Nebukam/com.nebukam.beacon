using Unity.Mathematics;
using UnityEngine;

namespace Nebukam.Beacon
{
    [AddComponentMenu("Nebukam/Beacon/Simple Move Processor")]
    public class SimpleMoveProcessor : BeaconProcessor
    {

        private void Awake()
        {

        }

        public override void Apply(
            ref float3 position,
            ref float3 velocity,
            ref float3 heading,
            float sqDistToGoal)
        {

            position += velocity * Time.deltaTime;

        }

    }
}