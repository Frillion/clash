using System;
using System.Threading;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clash.Features.Combat
{
    [RequireComponent(typeof(IdComponent))]
    public class ProjectileBase : Spawnable, IProjectile 
    {
        private static readonly int Velocity = Shader.PropertyToID("_velocity");
        private static readonly int Color1 = Shader.PropertyToID("_color");
        private IdComponent _id;
        public float acceleration;
        public float initialVelocity;
        public float stretchStrength;
        private float _acceleration;
        private bool _reflected;
        private float _velocity;
        private Vector2 _velocityDir;
        private SpriteRenderer _renderer;

        protected void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _id = GetComponent<IdComponent>();
            _velocity = initialVelocity;
            _acceleration = acceleration;
            _reflected = false;
        }

        protected void FixedUpdate()
        {
            _velocity += _acceleration * TimeManager.Instance.fixedDeltaTime;
            transform.position += (Vector3)(_velocityDir * (_velocity * TimeManager.Instance.fixedDeltaTime));
            _renderer.material.SetVector(Velocity,(Vector4)_velocityDir * (_velocity * stretchStrength));
            if (!_renderer.isVisible) Despawn(); 
        }

        public override void Despawn()
        {
            _reflected = false;
            _velocity = initialVelocity;
            _renderer.material.SetVector(Velocity,(Vector4)_velocityDir * _velocity);
            _renderer.material.SetColor(Color1, new Color(1,1,1,1));
            base.Despawn();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("slash"))
            {
                ProjectileManager.Instance.NotifyReflection(this); 
            }

            if (other.gameObject.CompareTag("Player") && !_reflected)
            {
                other.gameObject.GetComponent<IHealth>().Damage(10);
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                if (!(ProjectileManager.Instance.IsOwner(this, 
                        other.gameObject.GetComponent<IProjectileOwner>()) && _reflected))
                    return;
                
                EnemySystem.Instance.DamageEnemy(other.gameObject.GetComponent<IEnemy>().GetIdReference().ID, 2);
            }
        }

        public IdComponent GetIdReference()
        {
            return _id;
        }

        public async UniTask OnReflected(Vector2 origin, CancellationToken token)
        {
            _velocity = 0;
            _acceleration = 0;
            _renderer.material.SetColor(Color1, new Color(1,0,0,1));
            TimeManager.Instance.PauseFor(100, token).Forget();
            await UniTask.Delay(100, DelayType.DeltaTime, cancellationToken:token);
        
            if (token.IsCancellationRequested)
                return;

            var dir = (origin - (Vector2)transform.position).normalized;
            SetTrajectory(dir);
            _reflected = true;
            _velocity = initialVelocity * 3;
            _acceleration = acceleration;
        }

        public void SetTrajectory(Vector2 trajectory)
        {
            _velocityDir = trajectory;
        }
    }
}