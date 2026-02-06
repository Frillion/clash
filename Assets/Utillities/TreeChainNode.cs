using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clash.Utillities
{
    public class TreeChainNode : MonoBehaviour
    {
        public bool rootNode;
        [HideInInspector] public Vector3 initialPosition;
        [HideInInspector] public float segmentLength;
        [Range(0.0f, 100.0f)]
        public float influence;
        public List<TreeChainNode> children;

        public void Awake()
        {
            initialPosition = transform.position;
        }

        private Vector2 PointConstraint(Vector3 constraintPosition, Vector3 constrainedPosition, float strength)
        {
            Vector2 dir = constraintPosition - constrainedPosition;
            Vector2 newPos = constraintPosition - (Vector3)dir.normalized * segmentLength;
            return dir.magnitude > segmentLength ? Vector2.Lerp(constrainedPosition, newPos,strength/100) : constrainedPosition;
        }

        public async UniTask MoveSelfAndChildren(Vector3 anchor, int delay, CancellationToken token, bool ignoreFirst = true, float strength = 100)
        {
            if (!ignoreFirst)
            {
                await UniTask.Delay(delay, DelayType.DeltaTime, cancellationToken: token);
            }

            await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken: token);
            if (token.IsCancellationRequested) return;
            transform.position = initialPosition + anchor * strength/100;
            foreach (var current in children)
            {
                current.MoveSelfAndChildren(anchor, delay, token, false, current.influence).Forget();
            }
        }

        public void Resolve()
        {
            foreach (var current in children)
            {
                current.transform.position = PointConstraint(transform.position, current.transform.position, current.influence);
            }
        }
    }
}
