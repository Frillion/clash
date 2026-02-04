using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clash.Utillities
{
    public class TreeChainNode : MonoBehaviour
    {
        public bool rootNode;
        [HideInInspector] public float segmentLength;
        [Range(0.0f, 100.0f)]
        public float influence;
        public List<TreeChainNode> children;

        private Vector2 PointConstraint(Vector3 constraintPosition, Vector3 constrainedPosition, float strength)
        {
            Vector2 dir = constraintPosition - constrainedPosition;
            Vector2 newPos = constraintPosition - (Vector3)dir.normalized * segmentLength;
            return dir.magnitude > segmentLength ? Vector2.Lerp(constrainedPosition, newPos,strength/100) : constrainedPosition;
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
