using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IMetaCubeNode
{
   Vector2 GetPosition();
   Material GetSharedMaterial();
   Material GetMaterial();
   GameObject GetObject();
}

public class WingRenderHelper : MonoBehaviour
{
   private static readonly int Neighbors = Shader.PropertyToID("_neighbors");
   private static readonly int NumberOfNodes = Shader.PropertyToID("_numberOfNodes");
   private List<IMetaCubeNode> _wingNodes;
   
   #if UNITY_EDITOR
   public void UpdateNodes()
   {
      _wingNodes = GetComponentsInChildren<IMetaCubeNode>().ToList();
      _wingNodes.Sort((node1, node2) =>
      {
         var dist1 = Vector2.Distance(node1.GetPosition(), transform.position);
         var dist2 = Vector2.Distance(node2.GetPosition(), transform.position);

         return dist1.CompareTo(dist2);
      });
      
      foreach (var node in _wingNodes)
      {
         var nodeRenderer = node.GetObject().GetComponent<SpriteRenderer>();
         nodeRenderer.sharedMaterial.SetVectorArray(Neighbors, _wingNodes.ConvertAll(nd => (Vector4)nd.GetPosition()).ToArray());
         nodeRenderer.sharedMaterial.SetFloat(NumberOfNodes, _wingNodes.Count);
      } 
   }
   #endif

   private void Awake()
   {
      _wingNodes = GetComponentsInChildren<IMetaCubeNode>().ToList();
      _wingNodes.Sort((node1, node2) =>
      {
         var dist1 = Vector2.Distance(node1.GetPosition(), transform.position);
         var dist2 = Vector2.Distance(node2.GetPosition(), transform.position);

         return dist1.CompareTo(dist2);
      });

   }

   private void Update()
   {
      foreach (var node in _wingNodes)
      {
         node.GetMaterial().SetVectorArray(Neighbors, _wingNodes.ConvertAll(nd => (Vector4)nd.GetPosition()).ToArray());
         node.GetMaterial().SetFloat(NumberOfNodes, _wingNodes.Count);
      } 
   }
}
