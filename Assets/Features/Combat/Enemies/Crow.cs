using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clash.Features.Combat
{
    public class Crow : Spawnable, IEnemy
    {
        private Guid _id;
        private Vector2 _spawnPosition;
        [SerializeField] private AnimationCurve smoothing;
        [SerializeField] private float movementAnimationDuration;
        private float _animationTime;

        private CrowBodyAnim _bodyAnimator;
        private List<WingAnimation> _wingAnimators;
        
        public Guid GetId()
        {
            return _id;
        }

        private void Awake()
        {
            _bodyAnimator = GetComponentInChildren<CrowBodyAnim>();
            _wingAnimators = GetComponentsInChildren<WingAnimation>().ToList();
        }

        public override void Spawn()
        {
            _animationTime = 0;
            _bodyAnimator.Init();
            _wingAnimators.ForEach(animator => animator.Init());
            base.Spawn();
        }

        public void SetId(Guid id)
        {
            _id = id;
        }

        public void SetOrigin(Vector2 origin)
        {
            _spawnPosition = origin;
        }

        public async UniTask MoveTo(Vector2 dest, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _animationTime += Time.deltaTime;
                if (_animationTime >= movementAnimationDuration) return;
                
                transform.position = 
                    Vector2.Lerp(_spawnPosition, dest, smoothing.Evaluate(_animationTime));
                await UniTask.NextFrame(cancellationToken: token);
            }
        }
    }
}

