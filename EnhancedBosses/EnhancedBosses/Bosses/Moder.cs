using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class Moder : MonoBehaviour
    {
        public Character character;

        public static List<string> ModerCreaturesAir = new List<string>()
        {
            "Hatchling"
        };

        public static List<string> ModerCreaturesGround = new List<string>()
        {
            "Wolf"
        };

        public void Awake()
        {
            character = gameObject.GetComponent<Character>();
        }

        public bool Process_Attack(Attack attack)
        {
            var weapon = attack.m_weapon;

            if (weapon.m_shared.m_name == "dragon_summon")
            {
                SpawnAir();
                return false;
            }
            else if (weapon.m_dropPrefab == Main.ModerTaunt && character.IsOnGround())
            {
                SpawnGround();
            }
            else if (weapon.m_dropPrefab == Main.ModerColdbreath)
            {

            }

            return true;
        }

        public void SpawnAir()
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            var min = Main.ModerMinMinions.Value + (playersCount - 1) * Main.ModerMinionsMultiplier.Value;
            var max = Main.ModerMaxMinions.Value + (playersCount - 1) * Main.ModerMinionsMultiplier.Value;
            Helpers.SpawnCreatures(character, ModerCreaturesAir, min, max);
        }

        public void SpawnGround()
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            var min = Main.ModerMinMinions.Value + (playersCount - 1) * Main.ModerMinionsMultiplier.Value;
            var max = Main.ModerMaxMinions.Value + (playersCount - 1) * Main.ModerMinionsMultiplier.Value;
            Helpers.SpawnCreatures(character, ModerCreaturesGround, min, max);
        }

        public static void ModerSummon()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("dragon_summon", "dragon_spit_shotgun");
            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = "dragon_summon";
            PrefabManager.Instance.AddPrefab(gameObject);
        }
    }
}
