using System;
using UnityEngine;

namespace Clash.Features.Combat
{
    [RequireComponent(typeof(IdComponent))]
    public class EnemyHealthComponent : MonoBehaviour, IHealth
    {
        private IdComponent _id;
        [SerializeField] private int totalHits = 1;
        private int _hitsLeft;

        public void Init()
        {
            _hitsLeft = Mathf.Max(1, totalHits);
            _id = GetComponent<IdComponent>();
        }

        public IdComponent GetIdReference()
        {
            return _id;
        }

        public float GetCurrentHp()
        {
            return _hitsLeft;
        }

        public void Damage(float damage)
        {
            _hitsLeft -= (int)MathF.Round(damage);
            if (_hitsLeft <= 0)
            {
                Death();
            }
        }

        public void Death()
        {
            EnemySystem.Instance.NotifyDeath(_id.ID);
        }
    }
}
