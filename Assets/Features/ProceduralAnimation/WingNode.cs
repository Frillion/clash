using System;
using Clash.Utillities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class WingNode : TreeChainNode, IMetaCubeNode
{
    [HideInInspector]
    public int index;

    [HideInInspector] public SpriteRenderer spRenderer;

    public new void Awake()
    {
        base.Awake();
        spRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 0, 0, 255);
        Gizmos.DrawWireSphere(transform.position, segmentLength);
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public Material GetSharedMaterial()
    {
        return spRenderer.sharedMaterial;
    }

    public Material GetMaterial()
    {
        return spRenderer.material;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }
}
