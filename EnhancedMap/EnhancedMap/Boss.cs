using UnityEngine;

namespace EnhancedMap
{
	public class Boss : MonoBehaviour
    {
        public Character character;
        public Vector3 position;
        public Minimap.PinData pin;

		public void Awake()
		{
			character = gameObject.GetComponent<Character>();
			position = character.transform.position;
			pin = PinManager.AddBossPin(position);
			Main.bosses.Add(this);
		}

		public void OnDeath()
		{
			if (pin != null)
			{
				Minimap.instance.RemovePin(pin);
			}
		}

		public bool IsPositionChanges()
		{
			return character.transform.position != position;
		}

		public void Move()
		{
			if (pin != null)
			{
				Minimap.instance.RemovePin(pin);
			}

			pin = PinManager.AddBossPin(position);
			position = character.transform.position;
		}
	}
}
