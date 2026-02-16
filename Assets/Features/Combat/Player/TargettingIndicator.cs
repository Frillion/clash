using System;
using System.Collections.Generic;
using Clash.Features.Audio;
using UnityEngine;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

namespace Clash.Features.Combat
{
    public class TargettingIndicator : MonoBehaviour
    {
        [SerializeField] private float slashDuration;
        [SerializeField] private float cooldown;
        [SerializeField] private SlashLifetime slashPrefab;
        [SerializeField] private AudioSettings clip;
        private float _timeSinceLastSlash;
        private ObjectPool<SlashLifetime> _slashPool;

        private void Awake()
        {
            _slashPool = new ObjectPool<SlashLifetime>().CreateObjectPool(
                slashPrefab,
                initialPoolSize:2
            );
            
            _timeSinceLastSlash = cooldown;
        }

        // Update is called once per frame
        private void Update()
        {
            _timeSinceLastSlash += Time.deltaTime;
            if (Input.GetMouseButtonDown(0) && _timeSinceLastSlash >= cooldown)
            {
                var newSlash = _slashPool.Spawn(transform.position);
                AudioManager.Instance.Play(clip).Forget();
                newSlash.transform.right = RadialInput.Instance.direction;
                newSlash.Initialize(slashDuration);
                _timeSinceLastSlash = 0;
            }

            transform.position = RadialInput.Instance.inputPosition;
        }
    }
}

