using System;
using System.Threading;
using Clash.Features.Combat.Enemies;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour, IProjectileOwner
{
    [SerializeField] private ProjectileType type;
    [SerializeField] private float cooldown;
    [SerializeField] private Transform target;
    private CancellationTokenSource _shotCancel;

    public void Awake()
    {
        _shotCancel =
            CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                CancellationToken.None);
        ShootLoop(_shotCancel.Token).Forget();
    }

    private async UniTask ShootLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.WaitForSeconds(cooldown, cancellationToken: token);
            ProjectileManager.Instance.SpawnProjectile(this, target.position, type);
        }
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }
}
