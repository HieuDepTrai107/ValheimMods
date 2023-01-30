using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;
using static EnhancedBosses.Main;
using Log = Jotunn.Logger;

namespace EnhancedBosses
{
    public class Boss : MonoBehaviour
    {
        public Character character;
        public Humanoid humanoid;
        public MonsterAI monsterAI;
        public BaseAI baseAI;
        public ZNetView zNetView;
        public Vector3 position;
        public Minimap.PinData pin;

        public virtual string bossName { get; set; }

        public virtual List<CustomAttack> customAttacks { get; set; }

        public virtual void Awake()
        {
            character = base.GetComponent<Character>();
            humanoid = base.GetComponent<Humanoid>();
            monsterAI = base.GetComponent<MonsterAI>();
            baseAI = base.GetComponent<BaseAI>();
            zNetView = base.GetComponent<ZNetView>(); 

            position = character.transform.position;
            pin = PinManager.AddBossPin(position);
            pinsList.Add(this);
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

        public virtual bool Process_Attack(Attack attack)
        {
            var weapon = attack.m_weapon;

            foreach (var customAttack in customAttacks)
            {
                if (weapon.m_dropPrefab != null ? weapon.m_dropPrefab.name == customAttack.name : weapon.m_shared.m_name == customAttack.name)
                {
                    customAttack.OnAttackTriggered(character, monsterAI);

                    if (customAttack.stopOriginalAttack)
                    {
                        return false;
                    }

                    return true;
                }
            }

            return true;
        }

        public virtual bool CanUseAttack(ItemDrop.ItemData item)
        {
            foreach (var customAttack in customAttacks)
            {
                if (item.m_dropPrefab != null ? item.m_dropPrefab.name == customAttack.name : item.m_shared.m_name == customAttack.name)
                {
                    return customAttack.CanUseAttack(character, monsterAI);
                }
            }

            return true;
        }

        public virtual void SetupCharacter()
        {

        }

        public void SetupCustomAttacks()
        {
            List<GameObject> items = new();

            foreach (CustomAttack attack in customAttacks)
            {
                ItemInfo itemInfo = cfg[bossName][attack.name];

                GameObject gameObject = attack.Setup();
                ItemDrop item = gameObject.GetComponent<ItemDrop>();
                item.m_itemData.m_shared.m_aiAttackInterval = itemInfo.Cooldown;

                if (itemInfo.Enabled)
                {
                    items.Add(gameObject);
                }
            }

            PrefabManager.Cache.GetPrefab<Humanoid>(bossName).m_defaultItems = items.ToArray();
        }
    }
}
