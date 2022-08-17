using Jotunn.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses
{
    class Vortex : MonoBehaviour
    {        
        public float range = 5f;
        public Vector3 position;

        public Character character;
        public TimedDestruction td;


        public void Awake()
        {
            td = base.GetComponent<TimedDestruction>();
            position = transform.position + Vector3.up;
        }

        public void Update()
        {
            if (character != null)
            {
                foreach (Character character in GetEnemies())
                {
                    if (!character.IsBoss() && !character.IsStaggering())
                    {
                        Vector3 vector2 = Vector3.Normalize(character.transform.position - position);
                        character.Stagger(vector2);
                    }

                    float distance = Vector3.Distance(character.transform.position, position);
                    if (distance < 2f)
                    {
                        td.DestroyNow();
                    }
                    else
                    {
                        Vector3 vector2 = Vector3.Normalize(position - character.transform.position);
                        float num2 = 1.5f;
                        character.transform.position = character.transform.position + vector2 * num2 * Time.deltaTime;
                    }
                }
            }
        }

        public void OnDestroy()
        {
            GameObject prefab = PrefabManager.Instance.GetPrefab("fx_eikthyr_stomp");
            Object.Instantiate(prefab, transform.position, Quaternion.identity);

            foreach (Character ch in GetEnemies())
            {
                HitData hitData = new HitData();
                hitData.m_damage.m_lightning = 20f;
                hitData.SetAttacker(character);
                ch.Damage(hitData);
            }
        }

        public IEnumerable<Character> GetEnemies()
        {
            List<Character> list = new List<Character>();
            Character.GetCharactersInRange(transform.position, range, list);
            return list.Where(ch => BaseAI.IsEnemy(character, ch));
        }
    }
}
