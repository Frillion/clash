using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clash.Utillities
{
    public class TreeChainNode : MonoBehaviour
    {
        public bool rootNode;
        [HideInInspector] public float segmentLength;
        public List<TreeChainNode> children;

        private Vector2 PointConstraint(Vector3 constraintPosition, Vector3 constrainedPosition)
        {
            Vector2 dir = constraintPosition - constrainedPosition;
            Vector2 newPos = constraintPosition - (Vector3)dir.normalized * segmentLength;
            return dir.magnitude > segmentLength ? newPos : constrainedPosition;
        }
        
        public void Resolve()
        {
            foreach (var current in children)
            {
                current.transform.position = PointConstraint(transform.position, current.transform.position);
            }
        }
    }
}
