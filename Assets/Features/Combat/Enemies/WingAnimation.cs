using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clash.Utillities;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WingAnimation : MonoBehaviour
{
    private List<WingNode> _wingNodes;
    private int _currentIndex;
    [SerializeField] private float animationStrength;
    [SerializeField] private float animationSpeed;
    [SerializeField] private float kinematicRadius;

    private CancellationTokenSource _cancelChain;

    private readonly Queue<TreeChainNode> _chainQueue = new();

    private void GrabAndSetupNodes()
    {
        _wingNodes = GetComponentsInChildren<WingNode>().ToList();
        _wingNodes.Sort((node1, node2) =>
        {
            var dist1 = Vector2.Distance(node1.transform.position, transform.position);
            var dist2 = Vector2.Distance(node2.transform.position, transform.position);

            return dist2.CompareTo(dist1);
        });

        _currentIndex = 0;
        _wingNodes.ForEach(node =>
        {
            node.segmentLength = kinematicRadius;
            node.index = _currentIndex;
            _currentIndex++;
        });
    }

    public void Awake()
    {
        GrabAndSetupNodes();
        _cancelChain = CancellationTokenSource.CreateLinkedTokenSource(
            this.GetCancellationTokenOnDestroy(),CancellationToken.None);
        
        ResolveWingChain(_cancelChain.Token).Forget();
        Animate(_cancelChain.Token).Forget();
    }

    private async UniTask Animate(CancellationToken token)
    {
        var root = _wingNodes.First(node => node.rootNode);
        while (!token.IsCancellationRequested)
        {
            var offset = Mathf.Round(Mathf.Sin(Time.time * Mathf.Deg2Rad * 360 * animationSpeed)) * animationStrength;
            await root.MoveSelfAndChildren(new Vector3(0, offset, 0), 400, token);
        }
    }

    private async UniTask ResolveWingChain(CancellationToken token)
    {
        _chainQueue.Enqueue(_wingNodes.First(node => node.rootNode));
        while (_chainQueue.Count > 0 && !token.IsCancellationRequested)
        {
            var current = _chainQueue.Dequeue();
            current.children.ForEach(node => { _chainQueue.Enqueue(node);});
            current.Resolve();
            await UniTask.NextFrame(PlayerLoopTiming.Update, token);
        }

        ResolveWingChain(token).Forget();
    }

    private void OnValidate()
    {
        GrabAndSetupNodes();
    }
}
