using System;
using System.Collections.Generic;
using UnityEngine;
using Clash.Utillities;
using Unity.VisualScripting;

public class TargettingIndicator : MonoBehaviour
{
    [SerializeField] private float slashDuration;
    [SerializeField] private float cooldown;
    [SerializeField] private SlashLifetime slashPrefab;
    private ObjectPool<SlashLifetime> _slashPool;

    private void Awake()
    {
        _slashPool = new ObjectPool<SlashLifetime>().CreateObjectPool(
            slashPrefab,
            initialPoolSize:2
            );
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var newSlash = _slashPool.Spawn(transform.position);
            newSlash.transform.right = RadialInput.Instance.direction;
            newSlash.Initialize(slashDuration);
        }

        transform.position = RadialInput.Instance.inputPosition;
    }
}
