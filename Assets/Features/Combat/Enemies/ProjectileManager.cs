using System;
using System.Collections.Generic;
using System.Linq;
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
        void Despawn();
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
        private readonly List<IProjectile> _activeProjectiles = new();
        private readonly List<Guid> _orphanedProjectiles = new();

        private void ResetPools(bool createNew = true)
        {
            PoolManager.Instance.Remove(basicProjectilePrefab.name);
            if (createNew)
            {
                _baseProjectilePool = new ObjectPool<ProjectileBase>().CreateObjectPool(basicProjectilePrefab, null, 15U);
            }
        }

        public void Init()
        {
            ResetPools();

            TokenSystem.Instance.AddToken(nameof(ProjectileManager),
                CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                    CancellationToken.None));
        }

        public void NotifyReflection(IProjectile projectile)
        {
            _projectileToOwner.TryGetValue(projectile.GetIdReference().ID, out var owner);
            if (owner == null)
            {
                projectile.OnReflected(new Vector2(0, 1),
                    TokenSystem.Instance.GetToken(nameof(ProjectileManager)).Token).Forget();
                
                return;
            }

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
            _activeProjectiles.Add(proj);
        }

        public void Adopt(IProjectileOwner newOwner)
        {
            _orphanedProjectiles.ForEach(projectile => _projectileToOwner.TryAdd(projectile, newOwner));
        }

        public void ClearOwner(IProjectileOwner owner)
        {
            List<Guid> keysToRemove = new();
            foreach (var keyValuePair in _projectileToOwner
                         .Where(keyValuePair => keyValuePair.Value.GetIdReference().ID == owner.GetIdReference().ID))
            {
                keysToRemove.Add(keyValuePair.Key);
                if(!_orphanedProjectiles.Contains(keyValuePair.Key)) _orphanedProjectiles.Add(keyValuePair.Key);
            }
            
            keysToRemove.ForEach(key => _projectileToOwner.Remove(key));
        }

        public void Clear()
        {
            _activeProjectiles.ForEach(projectile => projectile.Despawn());
            _activeProjectiles.Clear();
            _projectileToOwner.Clear();
        }

        public bool IsOwner(IProjectile projectile, IProjectileOwner owner)
        {
            _projectileToOwner.TryGetValue(projectile.GetIdReference().ID, out var storedOwner);
            if (storedOwner == null)
            {
                return false;
            }

            return storedOwner.GetIdReference().ID == owner.GetIdReference().ID;
        }
    }
}