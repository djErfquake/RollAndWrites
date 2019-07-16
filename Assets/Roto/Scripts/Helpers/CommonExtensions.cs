using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonExtensions
{

    /// <summary>
    /// Shuffles a list of anything
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The List</param>
    public static void ShuffleList<T>(this IList<T> list, int seed = -1)
    {
        System.Random rnd = (seed == -1) ? new System.Random() : new System.Random(seed);

        int n = list.Count;
        while (n > 1)
        {
            int k = (rnd.Next(0, n) % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }




    /// <summary>
    /// Modifies the specified components in a Vector3 to the specified values.
    /// 
    /// Example Use:
    ///     Vector3 v = new Vector3(1, 2, 3);
    ///     v = v.With(y: 15, z: 7);
    ///     // v is now a Vector3(1, 15, 7);
    /// </summary>
    /// <param name="x">if present, the original Vector3.x will be set to this</param>
    /// <param name="y">if present, the original Vector3.y will be set to this</param>
    /// <param name="z">if present, the original Vector3.z will be set to this</param>
    /// <returns></returns>
    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
    }



    /// <summary>
    /// Modifies the specified components in a Vector2 to the specified values.
    /// 
    /// Example Use:
    ///     Vector2 v = new Vector2(1, 2);
    ///     v = v.With(y: 15);
    ///     // v is now a Vector3(1, 15);
    /// </summary>
    /// <param name="x">if present, the original Vector2.x will be set to this</param>
    /// <param name="y">if present, the original Vector2.y will be set to this</param>
    /// <returns></returns>
    public static Vector2 With(this Vector2 original, float? x = null, float? y = null)
    {
        return new Vector2(x ?? original.x, y ?? original.y);
    }



}
