using System;
using System.Collections.Generic;
using System.Threading;
using Clash.Extensions;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clash.Features.Combat
{
    public interface IEnemy
    {
        Guid GetId();
        void SetId(Guid id);
        void SetOrigin(Vector2 origin);
        UniTask MoveTo(Vector2 dest, CancellationToken token);
    }

    public class EnemySystem : SingletonMonoBehaviour<EnemySystem>
    {
        public List<Transform> spawnPoints;
        public List<Transform> destinationPoints;

        [SerializeField] private Crow crowPrefab;
        private ObjectPool<Crow> _crowSpawner;
        private readonly List<Crow> _activeCrows = new();

        private CancellationTokenSource _enemyBehaviourTokenSource;

        [SerializeField] private int spawnDelayMS;

        private new void Awake() // Temporary For Testing
        {
            base.Awake();
            Init();
        } 

        public void Init()
        {
            _enemyBehaviourTokenSource?.Cancel();
            _enemyBehaviourTokenSource?.Dispose();
            _enemyBehaviourTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                    CancellationToken.None);
            
            ResetPools();
            SpawnLoop(_enemyBehaviourTokenSource.Token).Forget();
        }

        private async UniTask SpawnLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var spawn = spawnPoints.Random();
                var newSpawn = _crowSpawner.Spawn(spawn.position);
                newSpawn.SetOrigin(spawn.position);
                newSpawn.MoveTo(destinationPoints.Random().position, token).Forget(); // Temporary Test, Change
                _activeCrows.Add(newSpawn);
                
                await UniTask.Delay(spawnDelayMS,
                    DelayType.Realtime, PlayerLoopTiming.Update,
                    token);
            }
        }

        private void ResetPools(bool createNew = true)
        {
            PoolManager.Instance.Remove(crowPrefab.name);
            if (createNew)
            {
                _crowSpawner = new ObjectPool<Crow>().CreateObjectPool(crowPrefab);
            }
        }
    }
}

