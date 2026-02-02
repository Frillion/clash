using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class WingNode : MonoBehaviour
{
    [HideInInspector]
    public int index;
    [HideInInspector]
    public SpriteRenderer spRenderer;
    [FormerlySerializedAs("Radius")] public float radius;

    public void Awake()
    {
        spRenderer = GetComponent<SpriteRenderer>();
    }

    public void ApplyForce(Vector2 force)
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 0, 0, 255);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
