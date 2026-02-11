using System;
using UnityEngine;

namespace Clash.Features.Combat
{
    public class EnemyHealthComponent : MonoBehaviour, IHealth
    {
        private Guid _id;
        [SerializeField] private int totalHits = 1;
        private int _hitsLeft;

        public void Init()
        {
            _hitsLeft = Mathf.Max(1, totalHits);
        }

        public Guid GetID()
        {
            return _id;
        }

        public void SetGuid(Guid id)
        {
            _id = id;
        }

        public float GetCurrentHp()
        {
            return _hitsLeft;
        }

        public void Damage(float damage)
        {
            _hitsLeft -= (int)MathF.Round(damage);
        }

        public void Death()
        {
            throw new NotImplementedException();
        }
    }
}
