using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class WingRenderHelper : MonoBehaviour
{
   private static readonly int Neighbors = Shader.PropertyToID("_neighbors");
   private static readonly int NumberOfNodes = Shader.PropertyToID("_numberOfNodes");
   private List<SpriteRenderer> _wingNodes;

   private void Awake()
   {
      _wingNodes = GetComponentsInChildren<SpriteRenderer>().ToList();
      _wingNodes.Sort((node1, node2) =>
      {
         var dist1 = Vector2.Distance(node1.transform.position, transform.position);
         var dist2 = Vector2.Distance(node2.transform.position, transform.position);

         return dist1.CompareTo(dist2);
      });

   }

   private void Update()
   {
      foreach (var node in _wingNodes)
      {
         node.material.SetVectorArray(Neighbors, _wingNodes.ConvertAll(nd => (Vector4)nd.transform.position).ToArray());
         node.material.SetFloat(NumberOfNodes, _wingNodes.Count);
      } 
   }
}
