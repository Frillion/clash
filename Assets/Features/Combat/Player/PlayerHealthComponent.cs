using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clash.Features.Combat
{
    public interface IHealth
    {
        public void Init();
        public IdComponent GetIdReference();
        public float GetCurrentHp();
        public void Damage(float damage);
        public void Death();
    }

    public class PlayerHealthComponent : MonoBehaviour, IHealth
    {
        private IdComponent _id;
        public float health;
        private float _health;

        public static Action OnDamaged;

        public void Awake()
        {
           GameManager.Instance.OnGameStart += Init; 
        }

        public void Init()
        {
            _id = GetComponent<IdComponent>();
            _health = health;
        }

        public IdComponent GetIdReference()
        {
            return _id;
        }

        public float GetCurrentHp()
        {
            return _health;
        }

        public void Damage(float damage)
        {
            TimeManager.Instance.SetTimeScaleFor(0.5f, 300).Forget();
            OnDamaged?.Invoke();
            _health -= damage;
            if (_health <= 0)
            {
                Death();
            }
        }

        public void Death()
        {
            GameManager.Instance.End();
        }
    }
}

