using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clash.Features.Combat
{
    [RequireComponent(typeof(EnemyHealthComponent))]
    [RequireComponent(typeof(IdComponent))]
    public class Crow : Spawnable, IEnemy, IProjectileOwner
    {
        private IdComponent _id;
        private Vector2 _spawnPosition;
        [SerializeField] private AnimationCurve smoothing;
        [SerializeField] private float movementAnimationDuration;
        [SerializeField] private ProjectileType type;
        [SerializeField] private float cooldown;
        private float _animationTime;

        private CrowBodyAnim _bodyAnimator;
        private List<WingAnimation> _wingAnimators;

        private EnemyHealthComponent _health;
        

        private void Awake()
        {
            _bodyAnimator = GetComponentInChildren<CrowBodyAnim>();
            _wingAnimators = GetComponentsInChildren<WingAnimation>().ToList();
            _health = GetComponent<EnemyHealthComponent>();
            _id = GetComponent<IdComponent>();
        }
        

        public override void Spawn()
        {
            _animationTime = 0;
            _bodyAnimator.Init();
            _health.Init();
            _wingAnimators.ForEach(animator => animator.Init());

            
            base.Spawn();
        }

        public override void Despawn()
        {
            TokenSystem.Instance.Cancel(_id.ID.ToString());
            base.Despawn();
        }

        public IdComponent GetIdReference()
        {
            return _id;
        }

        public async UniTask ShootLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(cooldown, cancellationToken: token);
                await TimeManager.Instance.PauseGuard(token);
                ProjectileManager.Instance.SpawnProjectile(this, GameManager.Instance.playerTransform.position, type);
            }
        }

        public IHealth GetHealthComponent()
        {
            return _health;
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
                if (transform.position == (Vector3)dest)
                {
                    ShootLoop(TokenSystem.Instance.GetToken(_id.ID.ToString()).Token).Forget();
                    return;
                }

                await UniTask.NextFrame(cancellationToken: token);
            }
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public void Damage(float damage)
        {
            _health.Damage(damage);
        }
    }
}

