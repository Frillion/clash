using System;
using System.Threading;
using Clash.Features.Combat.Enemies;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectileBase : Spawnable, IProjectile 
{
    private static readonly int Velocity = Shader.PropertyToID("_velocity");
    public Guid ID;
    public float acceleration;
    public float initialVelocity;
    public float stretchStrength;
    private float _acceleration;
    private float _velocity;
    private Vector2 _velocityDir;
    private SpriteRenderer _renderer;

    protected void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _velocity = initialVelocity;
        _acceleration = acceleration;
    }

    protected void FixedUpdate()
    {
        _velocity += _acceleration * Time.fixedDeltaTime;
        transform.position += (Vector3)(_velocityDir * (_velocity * Time.fixedDeltaTime));
        _renderer.material.SetVector(Velocity,(Vector4)_velocityDir * (_velocity * stretchStrength));
        if (!_renderer.isVisible) Despawn(); 
    }

    public override void Despawn()
    {
        _velocity = initialVelocity;
        _renderer.material.SetVector(Velocity,(Vector4)_velocityDir * _velocity);
        base.Despawn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("slash"))
        {
            Debug.Log("Reflect detected");
           ProjectileManager.Instance.NotifyReflection(this); 
        }
    }

    public Guid GetGuid()
    {
        return ID;
    }

    public void SetGuid(Guid id) { ID = id; }

    public async UniTask OnReflected(Vector2 origin, CancellationToken token)
    {
        _velocity = 0;
        _acceleration = 0;
        await UniTask.Delay(100, DelayType.DeltaTime, cancellationToken:token);
        
        if (token.IsCancellationRequested)
            return;

        var dir = (origin - (Vector2)transform.position).normalized;
        SetTrajectory(dir);
        _velocity = initialVelocity * 3;
        _acceleration = acceleration;
    }

    public void SetTrajectory(Vector2 trajectory)
    {
        _velocityDir = trajectory;
    }
}
