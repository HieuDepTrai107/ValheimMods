using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class Yagluth : MonoBehaviour
    {
        public System.Random rnd = new System.Random();
        public Character character;

        public static List<string> YagluthCreatures = new List<string>()
        {
            "Lox",
            "GoblinBrute",
            "GoblinShaman"
        };
        public void Awake()
        {
            character = gameObject.GetComponent<Character>();
        }

        public bool Process_Attack(Attack attack)
        {
            var weapon = attack.m_weapon;

            if (weapon.m_shared.m_name == "GoblinKing_Summon")
            {
                SpawnMinions();
                return false;
            }

            return true;
        }

        public void SpawnMinions()
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            var min = Main.YagluthMinMinions.Value + (playersCount - 1) * Main.YagluthMinionsMultiplier.Value;
            var max = Main.YagluthMaxMinions.Value + (playersCount - 1) * Main.YagluthMinionsMultiplier.Value;
            Helpers.SpawnCreatures(character, YagluthCreatures, min, max);
        }

        public static void YagluthSummon()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("GoblinKing_Summon", "GoblinKing_Nova");
            ItemDrop.ItemData.SharedData shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;
            shared.m_name = "GoblinKing_Summon";
            PrefabManager.Instance.AddPrefab(gameObject);
        }
    }
}
