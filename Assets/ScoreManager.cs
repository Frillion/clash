using Clash.Utillities;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager> 
{
    public float displayMultiplier = 1;
    private float _realScore;

    private new void Awake()
    {
        base.Awake();
        Init(); // Temporary before adding launching system
    }

    public void Init()
    {
        _realScore = 0;
        displayMultiplier = Mathf.Max(1, displayMultiplier);
    }

    public float GetScore()
    {
        return _realScore * displayMultiplier;
    }

    public void AddScore(float score)
    {
        _realScore += score;
    }
}

