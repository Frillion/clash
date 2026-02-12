using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clash.Features.Combat
{
    [RequireComponent(typeof(IdComponent))]
    public class ProjectileSpawner : MonoBehaviour, IProjectileOwner
    {
        [SerializeField] private ProjectileType type;
        [SerializeField] private float cooldown;
        [SerializeField] private Transform target;
        private CancellationTokenSource _shotCancel;
        private IdComponent _id;

        public void Awake()
        {
            _shotCancel?.Cancel();
            _shotCancel?.Dispose();
            _shotCancel =
                CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                    CancellationToken.None);

            _id = GetComponent<IdComponent>();
            ShootLoop(_shotCancel.Token).Forget();
        }

        public async UniTask ShootLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(cooldown, cancellationToken: token);
                await TimeManager.Instance.PauseGuard(token);
                ProjectileManager.Instance.SpawnProjectile(this, target.position, type);
            }
        }

        public IdComponent GetIdReference()
        {
            return _id;
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }
    }
}

