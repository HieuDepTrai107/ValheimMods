using UnityEngine;

namespace EnhancedBosses
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

		public void UpdatePosition()
		{
			position = character.transform.position;

			if (pin != null)
			{
				pin.m_pos = position;
			}
		}
	}
}
