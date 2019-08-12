using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Nebukam.Signals;

namespace Nebukam.Beacon
{

    public class BeaconIntention : MonoBehaviour
    {

        public Transform goal = null;

        protected bool m_goalLocationDirty = false;
        protected float3 m_goalLocation = float3(false);
        protected RSignal<float3> s_goalLocationChanged = new RSignal<float3>();

        public bool goalLocationDirty { get { return m_goalLocationDirty; } }
        public float3 goalLocation
        {
            get { return m_goalLocation; }
            set
            {
                if (m_goalLocation.x == value.x
                    && m_goalLocation.y == value.y
                    && m_goalLocation.z == value.z)
                {
                    return;
                }

                m_goalLocation = value;
                m_goalLocationDirty = true;
            }
        }
        public IRSignal<float3> goalLocationChanged { get { return s_goalLocationChanged; } }


        // Start is called before the first frame update
        void Start()
        {

        }

        protected virtual void OnGoalLocationChanged()
        {

        }

        // Update is called once per frame
        void Update()
        {

            if(goal != null)
            {
                goalLocation = goal.position;
            }

            if (m_goalLocationDirty)
            {
                m_goalLocationDirty = false;
                s_goalLocationChanged.Dispatch(ref m_goalLocation);
                OnGoalLocationChanged();
            }

            //Check if situation meets intentions

        }
    }
}