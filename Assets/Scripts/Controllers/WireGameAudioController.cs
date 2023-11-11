using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class WireGameAudioController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem mParticleSystem;
        [SerializeField] private AudioSource onWireAttach, particleSpark;
        [SerializeField] private List<AudioClip> particleSparkClips;

        private void Start()
        {
            if (!mParticleSystem || !onWireAttach || !particleSpark)
                throw new Exception("Check attached components!!");
            mParticleSystem.Stop();
        }

        private void Update()
        {
            var randIndex = Random.Range(0, particleSparkClips.Count);

            if (mParticleSystem.particleCount > 5)
                if (!particleSpark.isPlaying)
                {
                    particleSpark.clip = particleSparkClips[randIndex];
                    particleSpark.Play();
                }
        }

        public void SetParticlePlayState(bool enable)
        {
            if (enable)
                mParticleSystem.Play();
            else
                mParticleSystem.Stop();
        }
    }
}