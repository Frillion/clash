using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WingRenderHelper : MonoBehaviour
{
   private static readonly int Neighbors = Shader.PropertyToID("_neighbors");
   private static readonly int NumberOfNodes = Shader.PropertyToID("_numberOfNodes");
   private List<WingNode> _wingNodes;
   
   #if UNITY_EDITOR
   public void UpdateNodes()
   {
      _wingNodes = GetComponentsInChildren<WingNode>().ToList();
      _wingNodes.Sort((node1, node2) =>
      {
         var dist1 = Vector2.Distance(node1.transform.position, transform.position);
         var dist2 = Vector2.Distance(node2.transform.position, transform.position);

         return dist1.CompareTo(dist2);
      });
      
      foreach (var node in _wingNodes)
      {
         node.spRenderer.sharedMaterial.SetVectorArray(Neighbors, _wingNodes.ConvertAll(nd => (Vector4)nd.transform.position).ToArray());
         node.spRenderer.sharedMaterial.SetFloat(NumberOfNodes, _wingNodes.Count);
      } 
   }
   #endif

   private void Awake()
   {
      _wingNodes = GetComponentsInChildren<WingNode>().ToList();
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
         node.spRenderer.material.SetVectorArray(Neighbors, _wingNodes.ConvertAll(nd => (Vector4)nd.transform.position).ToArray());
         node.spRenderer.material.SetFloat(NumberOfNodes, _wingNodes.Count);
      } 
   }
}
