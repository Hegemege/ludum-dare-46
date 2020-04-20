using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities
{
    public class RandomAudioClip : MonoBehaviour
    {
        public List<AudioClip> Clips;

        private AudioSource _source;
        private bool _played;

        public float RandomPitchMin = 1f;
        public float RandomPitchMax = 1f;

        void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (_source.isPlaying)
            {
                return;
            }
            else
            {
                if (_played)
                {
                    _played = false;
                    _source.Stop();
                    gameObject.SetActive(false);
                }
                else
                {
                    _source.clip = Clips[Random.Range(0, Clips.Count)];
                    _source.pitch = Random.Range(RandomPitchMin, RandomPitchMax);
                    _source.Play();
                    _played = true;
                }
            }
        }
    }
}
