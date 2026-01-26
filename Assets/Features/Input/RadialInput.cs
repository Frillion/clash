using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Clash.Utillities;

public class RadialInput : SingletonMonoBehaviour<RadialInput>
{
    private Vector2 _pivot;
    private Vector2 _mouseWorldPosition;
    private float _angle;
    
    [FormerlySerializedAs("_radius")] [SerializeField]
    private float radius;

    [HideInInspector]
    public Vector2 inputPosition;
    [HideInInspector]
    public Vector2 direction;

    [CanBeNull] private Camera _mainCam;

    protected new void Awake()
    {
        base.Awake();
        _mainCam = Camera.main;
    }

    private void Start()
    {
        _pivot = transform.position;
        _mouseWorldPosition = transform.right;
        inputPosition = transform.right * radius;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_mainCam) return;
        _mouseWorldPosition = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        direction = (_mouseWorldPosition - _pivot).normalized;
        inputPosition = direction * radius;
    }
}
