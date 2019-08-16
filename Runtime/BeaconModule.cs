using System.Collections.Generic;
using UnityEngine;
using Nebukam.JobAssist;

namespace Nebukam.Beacon
{
    public abstract class BeaconModule<T> : SingletonBehaviour<T>
        where T : Component, new()
    {

        public delegate void OnProcessorComplete(IProcessor processor);

        protected List<IProcessor> m_processes = new List<IProcessor>();
        protected Dictionary<IProcessor, OnProcessorComplete> m_processesCallbacks = new Dictionary<IProcessor, OnProcessorComplete>();

        protected override void Init()
        {
            
        }

        protected override void Tick(float delta)
        {
            for (int i = 0, count = m_processes.Count; i < count; i++)
                m_processes[i].Schedule(delta);
        }

        protected override void LateTick(float delta)
        {
            IProcessor p;
            OnProcessorComplete callback;
            for (int i = 0, count = m_processes.Count; i < count; i++)
            {
                p = m_processes[i];
                if (p.TryComplete() && m_processesCallbacks.TryGetValue(p, out callback))
                    callback(p);
            }
        }
        
        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
            if (!disposing) { return; }

            IProcessor p;
            IProcessorGroup pGroup;
            for(int i = 0, count = m_processes.Count; i < count; i++)
            {
                p = m_processes[i];
                pGroup = p as IProcessorGroup;
                if (pGroup != null)
                    pGroup.DisposeAll();
                else
                    p.Dispose();
            }

            m_processes.Clear();
            m_processes = null;

            m_processesCallbacks.Clear();

        }

        protected override void FixedTick()
        {
            
        }

    }
}
