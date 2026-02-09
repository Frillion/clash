using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clash.Features.Combat
{
    public interface IHealth
    {
        public Guid GetID();
        public void SetGuid(Guid id);
        public float GetCurrentHp();
        public void Damage(float damage);
        public void Death();
    }

    public class PlayerHealthComponent : MonoBehaviour, IHealth
    {
        private Guid _id;
        public float health;
        private float _health;

        public void Awake()
        {
            _health = health;
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
            return _health;
        }

        public void Damage(float damage)
        {
            _health -= damage;
        }

        public void Death()
        {
            throw new NotImplementedException();
        }
    }
}

