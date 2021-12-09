using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Nebukam.Common;

namespace Nebukam.Beacon
{

    [AddComponentMenu("Nebukam/Beacon/Beacon Controller")]
    [DisallowMultipleComponent]
    public class BeaconController : MonoBehaviour
    {

        [Header("Settings")]
        [Tooltip("Horizon plane")]
        public AxisPair plane = AxisPair.XY;

        [Header("Intentions")]
        [Tooltip("If left empty, will be fetched automatically using GetComponent()")]
        public BeaconIntention beaconIntentions;
        [Tooltip("When to update intentions in regard to the processing stack")]
        public UpdateOrder updateOrder = UpdateOrder.Before;

        [Header("Velocity")]
        [Tooltip("Speed (normalized heading * speed)")]
        public float speed = 1.0f;
        public float dropOffDistance = 2f;
        public AnimationCurve dropOffPattern;

        [Header("LookAt")]
        [Tooltip("What does this gameobject looks at")]
        public LookAt lookAt = LookAt.Direction;
#if UNITY_EDITOR
        [Nebukam.Common.Editor.ConditionalField("lookAt", LookAt.CustomTarget)]
#endif
        public Transform lookAtTarget = null;

        [Header("Processors")]
        [Tooltip("List of processors this controller will apply, in order")]
        public BeaconProcessor[] beaconProcessors;

        // Start is called before the first frame update
        void Start()
        {
            if (beaconIntentions == null)
                beaconIntentions = GetComponent<BeaconIntention>();

            if (beaconProcessors != null)
            {
                BeaconProcessor processor;
                for (int i = 0, count = beaconProcessors.Length; i < count; i++)
                {
                    processor = beaconProcessors[i];
                    if (processor == null) { continue; }
                    processor.intention = beaconIntentions;
                    processor.controller = this;
                }
            }

        }

        // Update is called once per frame
        void Update()
        {

            if (beaconIntentions == null) { return; }

#if UNITY_EDITOR
            if (updateOrder == 0)
            {
                Debug.LogWarning("updateOrder is neither set to Before nor After on " + this + " : the intentions will never be updated.");
            }
#endif

            if ((updateOrder & UpdateOrder.Before) != 0)
                beaconIntentions.Tick();

            float3 position = transform.position, goal = beaconIntentions.goalLocation;
            float3 heading = normalize(goal - position);
            float sqDistToGoal = distancesq(position, goal);

            float alteredSpeed = speed;

            //Check if we should sample dropoff pattern based on current distance to goal.
            if (dropOffDistance > 0f && dropOffPattern != null)
            {
                float sqDropOff = lengthsq(dropOffDistance);
                if (sqDistToGoal <= sqDropOff)
                {
                    alteredSpeed = speed * dropOffPattern.Evaluate(1f - sqDistToGoal / sqDropOff);
                }
            }

            float3 velocity = heading * alteredSpeed;
            IntentionState iState = beaconIntentions.currentState;

            if (beaconProcessors != null)
            {
                BeaconProcessor processor;
                for (int i = 0, count = beaconProcessors.Length; i < count; i++)
                {
                    processor = beaconProcessors[i];
                    if (processor == null
                        || !processor.enabled
                        || (processor.allowedStates & iState) == 0)
                    {
                        continue;
                    }

                    processor.Apply(
                        ref position,
                        ref velocity,
                        ref heading,
                        sqDistToGoal);
                }
            }

            transform.position = position;

            if (lookAt != LookAt.Nothing)
            {
                float3 dir = float3(0f);

                if (lookAt == LookAt.Goal)
                    dir = normalize(goal - position);
                else if (lookAt == LookAt.Direction)
                    dir = heading;
                else if (lookAt == LookAt.CustomTarget && lookAtTarget != null)
                    dir = normalize((float3)lookAtTarget.position - position);

                if (plane == AxisPair.XY)
                    transform.up = float3(dir.x, dir.y, 0f);
                else
                    transform.forward = dir;
            }

            if ((updateOrder & UpdateOrder.After) != 0)
                beaconIntentions.Tick();

        }

    }
}