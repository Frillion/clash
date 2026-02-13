using System;
using Clash.Features.Combat;
using Clash.Utillities;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
   public Action OnGameStart;
   public Action OnGameEnd;

   public void Start()
   {
      OnGameStart += ProjectileManager.Instance.Init;
      OnGameStart += EnemySystem.Instance.Init;
      OnGameStart += TimeManager.Instance.Init;
      OnGameStart += ScoreManager.Instance.Init;

      OnGameEnd += ProjectileManager.Instance.Clear;
      OnGameEnd += EnemySystem.Instance.Clear;
      
      StartGame();
   }

   public void StartGame()
   {
      OnGameStart?.Invoke();
   }

   public void End()
   {
      OnGameEnd?.Invoke();
   }
}
