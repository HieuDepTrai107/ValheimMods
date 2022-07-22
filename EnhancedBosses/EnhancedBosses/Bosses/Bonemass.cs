using EnhancedBosses.StatusEffects;
using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class Bonemass : MonoBehaviour
    {
        public Character character;

        public static List<string> BonemassCreatures = new List<string>()
        {
            "Draugr",
            "Draugr_Elite",
            "Blob",
            "BlobElite"
        };

        public void Awake()
        {
            character = gameObject.GetComponent<Character>();
        }

        public void Update()
        {
            if (character.m_nview.IsOwner())
            {
                foreach (var player in Utils.FindPlayers(character.transform.position, 10f))
                {
                    if (Main.BonemassTripEffect.Value)
                    {
                        SE_Trip statusEffect1 = ScriptableObject.CreateInstance<SE_Trip>();
                        player.GetSEMan().AddStatusEffect(statusEffect1, true);
                    }    

                    SE_Slow statusEffect2 = ScriptableObject.CreateInstance<SE_Slow>();
                    player.GetSEMan().AddStatusEffect(statusEffect2, true);
                }
            }
        }

        public bool Process_Attack(Attack attack)
        {
            var weapon = attack.m_weapon;

            if (weapon.m_shared.m_name == "bonemass_attack_summon")
            {
                SpawnMinions();
                return false;
            }
            else if (weapon.m_dropPrefab == Main.BonemassAOE)
            {
                AOEDebuffs();
            }

            return true;
        }

        public void SpawnMinions()
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            var min = Main.BonemassMinMinions.Value + (playersCount - 1) * Main.BonemassMinionsMultiplier.Value;
            var max = Main.BonemassMaxMinions.Value + (playersCount - 1) * Main.BonemassMinionsMultiplier.Value;
            Utils.SpawnCreatures(character, BonemassCreatures, min, max);
        }

        public void AOEDebuffs()
        {
            foreach (var player in Utils.FindPlayers(character.transform.position))
            {
                player.UnequipItem(player.m_leftItem);
                player.UnequipItem(player.m_rightItem);
            }
        }

        public static GameObject BonemassSummon()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("bonemass_attack_summon", "bonemass_attack_aoe");
            gameObject.name = "bonemass_attack_summon";
            ItemDrop.ItemData.SharedData shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;
            shared.m_aiAttackInterval = Main.BonemassSummonCooldown.Value;
            shared.m_name = "bonemass_attack_summon";
            return gameObject;
        }
    }
}
