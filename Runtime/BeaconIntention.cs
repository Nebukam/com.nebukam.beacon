using Nebukam.Signals;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Nebukam.Beacon
{
    [AddComponentMenu("Nebukam/Beacon/Beacon Intention")]
    public class BeaconIntention : MonoBehaviour
    {

        /// 
        /// Fields
        /// 

#if UNITY_EDITOR
        public IntentionState state = 0;            
#endif

    [Header("Goal")]
        [Tooltip("Sets the intention's goal to be a Transform.\n" +
            "Otherwise, set the goalLocation property through code")]
        public Transform goal = null;
        [Tooltip("Distance tolerance to determine whether the goal has been reached or not.")]
        public float goalReachTolerance = 0.1f;

        protected bool m_goalLocationDirty = false;
        protected float3 m_goalLocation = float3(false);
        protected IntentionState m_currentState = IntentionState.IDLE;
        protected RSignal<float3> s_goalLocationChanged = new RSignal<float3>();
        protected Signal<IntentionState, IntentionState> s_stateChanged = new Signal<IntentionState, IntentionState>();

        /// 
        /// Properties
        /// 

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
        
        public IntentionState currentState
        {
            get { return m_currentState; }
            protected set
            {
                if(m_currentState == value) { return; }
                IntentionState was = m_currentState;
                m_currentState = value;
                OnStateChanged(was);
            }
        }

        public IRSignal<float3> goalLocationChanged { get { return s_goalLocationChanged; } }
        public ISignal<IntentionState, IntentionState> stateChanged { get { return s_stateChanged; } }

        /// 
        /// Methods
        /// 

        // Start is called before the first frame update
        void Start()
        {

        }

        protected virtual void OnStateChanged(IntentionState was)
        {
            s_stateChanged.Dispatch(m_currentState, was);
#if UNITY_EDITOR
            state = m_currentState;
#endif
        }

    protected virtual void OnGoalLocationChanged()
        {
            s_goalLocationChanged.Dispatch(ref m_goalLocation);
        }
        
        public virtual void Tick()
        {

            if (goal != null)
            {
                goalLocation = goal.position;
            }

            if (m_goalLocationDirty)
            {
                m_goalLocationDirty = false;
                OnGoalLocationChanged();
            }

            CheckSituation();

        }

        /// <summary>
        /// Assess the current situation vs intentions and update current state accordingly.
        /// </summary>
        public virtual void CheckSituation()
        {
            float3 pos = transform.position;
            if(distance(goalLocation, pos) <= goalReachTolerance)
            {
                if (m_currentState == IntentionState.SEEK)
                {
                    currentState = IntentionState.REACHED;
                    currentState = IntentionState.IDLE;
                }
            }
            else
            {
                currentState = IntentionState.SEEK;
            }
        }
    }
}