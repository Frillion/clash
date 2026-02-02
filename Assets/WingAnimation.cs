using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WingAnimation : MonoBehaviour
{
    private List<WingNode> _wingNodes;
    private int _currentIndex;

    public void Awake()
    {
        _wingNodes = GetComponentsInChildren<WingNode>().ToList();
        _wingNodes.Sort((node1, node2) =>
        {
            var dist1 = Vector2.Distance(node1.transform.position, transform.position);
            var dist2 = Vector2.Distance(node2.transform.position, transform.position);

            return dist1.CompareTo(dist2);
        });

        _currentIndex = 0;
        _wingNodes.ForEach(node => {
            node.index = _currentIndex;
            _currentIndex++;
        });
    }
    
    
}
