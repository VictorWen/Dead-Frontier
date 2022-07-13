using System.Collections.Generic;
using UnityEngine;

public class GenerationUtils 
{
    public static T GetRandom<T>(HashSet<T> set)
    {
        int id = Random.Range(0, set.Count - 1);
        int count = 0;
        foreach (T v in set)
        {
            if (count == id)
                return v;
            count++;
        }
        throw new UnityException("This should never happen");
    }
}
