using System;
using System.Collections.Generic;
using System.Threading;
using Clash.Utillities;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class TokenSystem : SingletonMonoBehaviour<TokenSystem> 
{
    private readonly Dictionary<string, CancellationTokenSource> _tokenRegistry = new();

    public void AddToken(string tokenName, CancellationTokenSource token)
    {
        _tokenRegistry.Add(tokenName, token);
    }

    public CancellationTokenSource GetToken(string tokenName)
    {
        return _tokenRegistry[tokenName];
    }

    public void Cancel(string tokenName)
    {
        if (!_tokenRegistry.TryGetValue(tokenName, out var tokenSource)) return;
        tokenSource.Cancel();
        tokenSource.Dispose();
        _tokenRegistry.Remove(tokenName);
    }

    public void CancelAll()
    {
        foreach (var cancellationTokenSource in _tokenRegistry)
        {
            cancellationTokenSource.Value.Cancel();
            cancellationTokenSource.Value.Dispose();
        }
        
        _tokenRegistry.Clear();
    }
}
