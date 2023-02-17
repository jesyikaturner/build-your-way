using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{

    // https://forum.unity.com/threads/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code.241052/ -- Smooth P -- Smooth.Foundations
    public static void Shuffle<T>(this List<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}