using System.Threading;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clash.Features.Audio
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        public AudioListener listener;

        [SerializeField] private AudioPlayer playerPrefab;
        private ObjectPool<AudioPlayer> _playerPool;

        public void Init()
        {
            TokenSystem.Instance.AddToken(nameof(AudioManager),
                CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                    CancellationToken.None));
            
            ResetPools();
        }

        private void ResetPools(bool createNew = true)
        {
            PoolManager.Instance.Remove(playerPrefab.name);
            if (createNew)
            {
                _playerPool = new ObjectPool<AudioPlayer>().CreateObjectPool(playerPrefab);
            }
        }

        public async UniTask Play(AudioSettings clip, CancellationToken? token = null)
        {
            token ??= TokenSystem.Instance.GetToken(nameof(AudioManager)).Token;

            while (!token.Value.IsCancellationRequested)
            {
                var player = _playerPool.Spawn(listener.transform.position);
                player.PlayOneShot(clip.clip);
                player.source.volume = 0.2f;
                player.source.pitch = Random.Range(clip.minPitch, clip.maxPitch);

                await UniTask.WaitForSeconds(clip.clip.length * clip.lengthPercent, cancellationToken: token.Value);
                player.Despawn();
                return;
            }
        }
    }
}

