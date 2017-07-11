using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    /// <summary>
    /// Add this event when you would like to start or stop emission a Particle System Emitter.
    /// </summary>
    [CustomEvent("StartStop Emitter", "Particle")]
    public class StartStopEmitter : EventKey
    {        
        [SerializeField]
        private ParticleSystem[] _Particles;
        [SerializeField]
        private bool _Emission = true;

        [ExposeProperty(101, "Emission", "start emit or stop emit")]
        public bool Emission { get { return _Emission; } set { _Emission = value; } }

        [ExposeProperty(102, "Particles", "Particles to  enable or disable emission")]
        public ParticleSystem[] Particles { get { return _Particles; } set { _Particles = value; } }        

        public override void FireEvent()
        {
            if (_Particles != null && _Particles.Length > 0)
            {
                for (int i = 0; i < _Particles.Length; i++)
                {
                    if (_Particles[i] != null)
                    {
                        var emission = _Particles[i].emission;
                        emission.enabled = _Emission;                                        
                    }
                }
            }          
        }
    }
}