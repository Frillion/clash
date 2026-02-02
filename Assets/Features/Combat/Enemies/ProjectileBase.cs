using System;
using Clash.Utillities;
using UnityEngine;

public class ProjectileBase : Spawnable
{
    private static readonly int Velocity = Shader.PropertyToID("_velocity");
    public float accelleration;
    public float velocity;
    private Vector2 _velocityDir;
    private Material mat;

    private void Awake()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _velocityDir = new Vector2(1f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        velocity += accelleration * Time.deltaTime;
        transform.position += (Vector3)(_velocityDir * (velocity * Time.deltaTime));
        mat.SetVector(Velocity,(Vector4)_velocityDir * velocity);
    }
}
