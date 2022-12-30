using UnityEngine;
using Log = Jotunn.Logger;

namespace TradingSkill
{
    static class Helpers
    {
        public static void PrintChildren(this GameObject gameObject)
        {
            gameObject.transform.PrintChildren();
        }

        public static void PrintChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Log.LogWarning(transform.GetChild(i).name);
            }

            Log.LogWarning("\n\n\n");
        }
    }
}
