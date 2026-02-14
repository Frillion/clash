using System;
using JetBrains.Annotations;
using UnityEngine;

public class CloudNode : MonoBehaviour, IMetaCubeNode
{
    private SpriteRenderer _rend;

    private void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public Material GetSharedMaterial()
    {
        return _rend.sharedMaterial;
    }

    public Material GetMaterial()
    {
        return _rend.material;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }
}
