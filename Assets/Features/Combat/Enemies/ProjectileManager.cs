using System;
using System.Collections.Generic;
using System.Threading;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Clash.Features.Combat
{
    public interface IProjectile
    {
        IdComponent GetIdReference();
        UniTask OnReflected(Vector2 origin, CancellationToken token);
        void SetTrajectory(Vector2 trajectory);
    }

    public interface IProjectileOwner
    {
        IdComponent GetIdReference();
        UniTask ShootLoop(CancellationToken token);
        Vector2 GetPosition();
    }

    public enum ProjectileType
    {
        Base
    }

    public class ProjectileManager : SingletonMonoBehaviour<ProjectileManager>
    {
        [SerializeField] private ProjectileBase basicProjectilePrefab;
        private ObjectPool<ProjectileBase> _baseProjectilePool;
        private readonly Dictionary<Guid, IProjectileOwner> _projectileToOwner = new();

        private void ResetPools(bool createNew = true)
        {
            PoolManager.Instance.Remove(basicProjectilePrefab.name);
            if (createNew)
            {
                _baseProjectilePool = new ObjectPool<ProjectileBase>().CreateObjectPool(basicProjectilePrefab, null, 15U);
            }
        }

        protected new void Awake()
        {
            base.Awake();
            ResetPools();

            TokenSystem.Instance.AddToken(nameof(ProjectileManager),
                CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                    CancellationToken.None));
        }

        public void NotifyReflection(IProjectile projectile)
        {
            projectile.OnReflected(_projectileToOwner[projectile.GetIdReference().ID].GetPosition(), 
            TokenSystem.Instance.GetToken(nameof(ProjectileManager)).Token).Forget();
        }

        public void SpawnProjectile(IProjectileOwner owner, Vector2 target, ProjectileType type)
        {
            IProjectile proj = type switch
            {
                ProjectileType.Base => _baseProjectilePool.Spawn(owner.GetPosition()),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            proj.GetIdReference().ID = Guid.NewGuid();
            _projectileToOwner.Add(proj.GetIdReference().ID, owner);
            proj.SetTrajectory((target-owner.GetPosition()).normalized);
        }

        public bool IsOwner(IProjectile projectile, IProjectileOwner owner)
        {
            return _projectileToOwner[projectile.GetIdReference().ID].GetIdReference().ID
                   == owner.GetIdReference().ID;
        }
    }
}