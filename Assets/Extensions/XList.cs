using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Clash.Extensions
{
    public static class XList
    {
        private static readonly Random _random = new Random();
        
        public static T Random<T>(this List<T> ls)
        {
            var index = _random.Next(ls.Count);
            return ls[index];
        }
    }
}
