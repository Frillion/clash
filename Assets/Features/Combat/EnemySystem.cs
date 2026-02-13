using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clash.Extensions;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clash.Features.Combat
{
    public interface IEnemy
    {
        IdComponent GetIdReference();
        IHealth GetHealthComponent();
        void SetOrigin(Vector2 origin);
        UniTask MoveTo(Vector2 dest, CancellationToken token);
    }

    public class EnemySystem : SingletonMonoBehaviour<EnemySystem>
    {
        public Transform playerTransform;
        public List<Transform> spawnPoints;
        public List<Transform> destinationPoints;

        [SerializeField] private Crow crowPrefab;
        private ObjectPool<Crow> _crowSpawner;
        private readonly List<Crow> _activeCrows = new();


        [SerializeField] private int spawnDelayMSStart;
        [SerializeField] private AnimationCurve spawnRateScaling;
        [SerializeField] private float scalingDuration;
        [SerializeField] private int spawnDelayMSMin;
        private int _currentSpawnDelayMS;

        public void Init()
        {
            TokenSystem.Instance.AddToken(nameof(EnemySystem),
                CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                    CancellationToken.None));

            _currentSpawnDelayMS = spawnDelayMSStart;
            
            ResetPools();
            SpawnLoop(TokenSystem.Instance.GetToken(nameof(EnemySystem)).Token).Forget();
        }

        public void DamageEnemy(Guid id, float damage)
        {
            var crowToDamage = _activeCrows.First(crow => crow.GetIdReference().ID == id);
            crowToDamage.GetHealthComponent().Damage(damage);
        }

        public void NotifyDeath(Guid id)
        {
            var crowToDie = _activeCrows.First(crow => crow.GetIdReference().ID == id);
            crowToDie.Despawn();
            ProjectileManager.Instance.ClearOwner(crowToDie);
            _activeCrows.Remove(crowToDie);
        }

        public void Clear()
        {
            _activeCrows.ForEach(crow => crow.Despawn());
            _activeCrows.Clear();
        }


        private async UniTask SpawnLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var spawn = spawnPoints.Random();
                var newSpawn = _crowSpawner.Spawn(spawn.position);
                newSpawn.GetIdReference().ID = Guid.NewGuid();
                newSpawn.SetOrigin(spawn.position);
                
                TokenSystem.Instance.AddToken(newSpawn.GetIdReference().ID.ToString(),
                    CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                        CancellationToken.None));
                
                newSpawn.MoveTo(destinationPoints.Random().position, token).Forget(); 
                _activeCrows.Add(newSpawn);
                ProjectileManager.Instance.Adopt(newSpawn);
                
                await UniTask.Delay(_currentSpawnDelayMS,
                    DelayType.Realtime, PlayerLoopTiming.Update,
                    token);
                
                _currentSpawnDelayMS = (int)Mathf.Lerp(spawnDelayMSStart, spawnDelayMSMin,
                    spawnRateScaling.Evaluate(TimeManager.Instance.totalTime / scalingDuration));
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

