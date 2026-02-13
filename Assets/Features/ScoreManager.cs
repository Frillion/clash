using System.Threading;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager> 
{
    public float displayMultiplier = 1;
    private float _realScore;

    public void Init()
    {
        _realScore = 0;
        displayMultiplier = Mathf.Max(1, displayMultiplier);

        TokenSystem.Instance.AddToken(nameof(ScoreManager),
            CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(),
                CancellationToken.None));
        
        TickSurvivalTime(TokenSystem.Instance.GetToken(nameof(ScoreManager)).Token).Forget();
    }

    private async UniTask TickSurvivalTime(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            _realScore += Time.deltaTime;
            await UniTask.NextFrame(cancellationToken: token);
        }
    }

    public float GetScore()
    {
        return Mathf.RoundToInt(_realScore * displayMultiplier);
    }

    public void AddScore(float score)
    {
        _realScore += score;
    }
}

