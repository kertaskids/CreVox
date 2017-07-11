using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you would like to Emit a number of particles.
    /// </summary>
    [CustomEvent("Emit", "Particle")]
    public class Emit : EventKey
    {
        [SerializeField]
        private ParticleSystem _Particle;        
        [SerializeField]
        private int _Count;
        [SerializeField]
        private bool _ClearParticles = false;

        [ExposeProperty(101, "Count", "Number of particles")]
        public int Count { get { return _Count; } set { _Count = value; } }

        [ExposeProperty(102, "Clear", "Removes all particles from the particle emitter before emit.")]
        public bool ClearParticles { get { return _ClearParticles; } set { _ClearParticles = value; } }

        [ExposeProperty(103, "Particle", "Particle System")]
        public ParticleSystem Particle { get { return _Particle; } set { _Particle = value; } }        

        public override void FireEvent()
        {
            if (_Particle != null)
            {
                if (_ClearParticles) _Particle.Clear();
                if (_Count > 0)
                    _Particle.Emit(_Count);
            }           
        }
    }
}