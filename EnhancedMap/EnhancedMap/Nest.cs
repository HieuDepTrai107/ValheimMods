using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedMap
{
	public class Nest : MonoBehaviour
	{
		public Destructible destructible;
		public static Minimap.PinData pin;

		public void Awake()
		{
			destructible = gameObject.GetComponent<Destructible>();
			pin = PinManager.AddNestPin(destructible.transform.position); 
			// Main.nests.Add(this);
		}

		public void Destroy()
		{
			if (pin != null && Minimap.instance != null)
			{
				Minimap.instance.RemovePin(pin);
            }
		}
	}
}
