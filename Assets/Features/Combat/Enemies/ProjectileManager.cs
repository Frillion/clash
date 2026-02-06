using System;
using System.Collections.Generic;
using System.Threading;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Clash.Features.Combat.Enemies
{
    public interface IProjectile
    {
        Guid GetGuid();
        void SetGuid(Guid id);
        UniTask OnReflected(Vector2 origin, CancellationToken token);
        void SetTrajectory(Vector2 trajectory);
    }

    public interface IProjectileOwner
    {
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
        private CancellationTokenSource _projectileTokenSource;

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
            
            if (_projectileTokenSource != null)
            {
                _projectileTokenSource.Cancel();
                _projectileTokenSource.Dispose();
            }
            
            _projectileTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                    CancellationToken.None);
        }

        public void NotifyReflection(IProjectile projectile)
        {
            projectile.OnReflected(_projectileToOwner[projectile.GetGuid()].GetPosition(), 
                _projectileTokenSource.Token).Forget();
        }

        public void SpawnProjectile(IProjectileOwner owner, Vector2 target, ProjectileType type)
        {
            IProjectile proj = type switch
            {
                ProjectileType.Base => _baseProjectilePool.Spawn(owner.GetPosition()),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            proj.SetGuid(Guid.NewGuid());
            _projectileToOwner.Add(proj.GetGuid(), owner);
            proj.SetTrajectory((target-owner.GetPosition()).normalized);
        }
    }
}