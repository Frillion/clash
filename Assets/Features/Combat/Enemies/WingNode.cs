using System;
using Clash.Utillities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class WingNode : TreeChainNode 
{
    [HideInInspector]
    public int index;
    [HideInInspector]
    public SpriteRenderer spRenderer;

    public void Awake()
    {
        spRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 0, 0, 255);
        Gizmos.DrawWireSphere(transform.position, segmentLength);
    }
}
