using EnhancedBosses.Bosses;
using Jotunn.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Log = Jotunn.Logger;
using BepInEx.Bootstrap;

namespace EnhancedBosses
{
    public static class Helpers
    {
        public static void PrintWeapons(string name)
        {
            Humanoid humanoid = PrefabManager.Cache.GetPrefab<Humanoid>(name);
            foreach (var item in humanoid.m_defaultItems)
            {
                Log.LogWarning(item.name);
            }
        }

        public static void PrintWeapons(GameObject prefab)
        {
            Humanoid humanoid = prefab.GetComponent<Humanoid>();
            foreach (var item in humanoid.m_defaultItems)
            {
                Log.LogWarning(item.name);
            }
        }

        public static void PrintDamages(this ItemDrop.ItemData.SharedData shared)
        {
            var damages = shared.m_damages;
            var damagesPerLevel = shared.m_damagesPerLevel;

            Log.LogWarning("\n\n\n\n\n\n");
            if (damages.m_blunt != 0)
            {
                Log.LogWarning($"Blunt: {damages.m_blunt}");
            }
            if (damagesPerLevel.m_blunt != 0)
            {
                Log.LogWarning($"BluntPerLevel: {damagesPerLevel.m_blunt}");
            }

            if (damages.m_chop != 0)
            {
                Log.LogWarning($"Chop: {damages.m_chop}");
            }
            if (damagesPerLevel.m_chop != 0)
            {
                Log.LogWarning($"ChopPerLevel: {damagesPerLevel.m_chop}");
            }

            if (damages.m_damage != 0)
            {
                Log.LogWarning($"Damage: {damages.m_damage}");
            }
            if (damagesPerLevel.m_damage != 0)
            {
                Log.LogWarning($"DamagePerLevel: {damagesPerLevel.m_damage}");
            }

            if (damages.m_fire != 0)
            {
                Log.LogWarning($"Fire: {damages.m_fire}");
            }
            if (damagesPerLevel.m_fire != 0)
            {
                Log.LogWarning($"FirePerLevel: {damagesPerLevel.m_fire}");
            }

            if (damages.m_frost != 0)
            {
                Log.LogWarning($"Frost: {damages.m_frost}");
            }
            if (damagesPerLevel.m_frost != 0)
            {
                Log.LogWarning($"FrostPerLevel: {damagesPerLevel.m_frost}");
            }

            if (damages.m_lightning != 0)
            {
                Log.LogWarning($"Lightning: {damages.m_lightning}");
            }
            if (damagesPerLevel.m_lightning != 0)
            {
                Log.LogWarning($"LightningPerLevel: {damagesPerLevel.m_lightning}");
            }

            if (damages.m_pickaxe != 0)
            {
                Log.LogWarning($"Pickaxe: {damages.m_pickaxe}");
            }
            if (damagesPerLevel.m_pickaxe != 0)
            {
                Log.LogWarning($"PickaxePerLevel: {damagesPerLevel.m_pickaxe}");
            }

            if (damages.m_pierce != 0)
            {
                Log.LogWarning($"Pierce: {damages.m_pierce}");
            }
            if (damagesPerLevel.m_pierce != 0)
            {
                Log.LogWarning($"PiercePerLevel: {damagesPerLevel.m_pierce}");
            }

            if (damages.m_poison != 0)
            {
                Log.LogWarning($"Poison: {damages.m_poison}");
            }
            if (damagesPerLevel.m_poison != 0)
            {
                Log.LogWarning($"PoisonPerLevel: {damagesPerLevel.m_poison}");
            }

            if (damages.m_slash != 0)
            {
                Log.LogWarning($"Slash: {damages.m_slash}");
            }
            if (damagesPerLevel.m_slash != 0)
            {
                Log.LogWarning($"SlashPerLevel: {damagesPerLevel.m_slash}");
            }

            if (damages.m_spirit != 0)
            {
                Log.LogWarning($"Spirit: {damages.m_spirit}");
            }
            if (damagesPerLevel.m_spirit != 0)
            {
                Log.LogWarning($"SpiritPerLevel: {damagesPerLevel.m_spirit}");
            }
            Log.LogWarning("\n\n\n\n\n\n");
        }

        public static void PrintDamages(this ItemDrop.ItemData item)
        {
            item.m_shared.PrintDamages();
        }

        public static void PrintDamages(this Aoe aoe)
        {
            var damages = aoe.m_damage;
            var damagesPerLevel = aoe.m_damagePerLevel;

            Log.LogWarning("\n\n\n\n\n\n");
            if (damages.m_blunt != 0)
            {
                Log.LogWarning($"Blunt: {damages.m_blunt}");
            }
            if (damagesPerLevel.m_blunt != 0)
            {
                Log.LogWarning($"BluntPerLevel: {damagesPerLevel.m_blunt}");
            }

            if (damages.m_chop != 0)
            {
                Log.LogWarning($"Chop: {damages.m_chop}");
            }
            if (damagesPerLevel.m_chop != 0)
            {
                Log.LogWarning($"ChopPerLevel: {damagesPerLevel.m_chop}");
            }

            if (damages.m_damage != 0)
            {
                Log.LogWarning($"Damage: {damages.m_damage}");
            }
            if (damagesPerLevel.m_damage != 0)
            {
                Log.LogWarning($"DamagePerLevel: {damagesPerLevel.m_damage}");
            }

            if (damages.m_fire != 0)
            {
                Log.LogWarning($"Fire: {damages.m_fire}");
            }
            if (damagesPerLevel.m_fire != 0)
            {
                Log.LogWarning($"FirePerLevel: {damagesPerLevel.m_fire}");
            }

            if (damages.m_frost != 0)
            {
                Log.LogWarning($"Frost: {damages.m_frost}");
            }
            if (damagesPerLevel.m_frost != 0)
            {
                Log.LogWarning($"FrostPerLevel: {damagesPerLevel.m_frost}");
            }

            if (damages.m_lightning != 0)
            {
                Log.LogWarning($"Lightning: {damages.m_lightning}");
            }
            if (damagesPerLevel.m_lightning != 0)
            {
                Log.LogWarning($"LightningPerLevel: {damagesPerLevel.m_lightning}");
            }

            if (damages.m_pickaxe != 0)
            {
                Log.LogWarning($"Pickaxe: {damages.m_pickaxe}");
            }
            if (damagesPerLevel.m_pickaxe != 0)
            {
                Log.LogWarning($"PickaxePerLevel: {damagesPerLevel.m_pickaxe}");
            }

            if (damages.m_pierce != 0)
            {
                Log.LogWarning($"Pierce: {damages.m_pierce}");
            }
            if (damagesPerLevel.m_pierce != 0)
            {
                Log.LogWarning($"PiercePerLevel: {damagesPerLevel.m_pierce}");
            }

            if (damages.m_poison != 0)
            {
                Log.LogWarning($"Poison: {damages.m_poison}");
            }
            if (damagesPerLevel.m_poison != 0)
            {
                Log.LogWarning($"PoisonPerLevel: {damagesPerLevel.m_poison}");
            }

            if (damages.m_slash != 0)
            {
                Log.LogWarning($"Slash: {damages.m_slash}");
            }
            if (damagesPerLevel.m_slash != 0)
            {
                Log.LogWarning($"SlashPerLevel: {damagesPerLevel.m_slash}");
            }

            if (damages.m_spirit != 0)
            {
                Log.LogWarning($"Spirit: {damages.m_spirit}");
            }
            if (damagesPerLevel.m_spirit != 0)
            {
                Log.LogWarning($"SpiritPerLevel: {damagesPerLevel.m_spirit}");
            }
            Log.LogWarning("\n\n\n\n\n\n");

        }

        public static void PrintDamages(this HitData.DamageTypes damageTypes)
        {
            Log.LogWarning("\n\n");
            if (damageTypes.m_blunt != 0)
            {
                Log.LogWarning($"Blunt: {damageTypes.m_blunt}");
            }

            if (damageTypes.m_chop != 0)
            {
                Log.LogWarning($"Chop: {damageTypes.m_chop}");
            }

            if (damageTypes.m_damage != 0)
            {
                Log.LogWarning($"Damage: {damageTypes.m_damage}");
            }

            if (damageTypes.m_fire != 0)
            {
                Log.LogWarning($"Fire: {damageTypes.m_fire}");
            }

            if (damageTypes.m_frost != 0)
            {
                Log.LogWarning($"Frost: {damageTypes.m_frost}");
            }

            if (damageTypes.m_lightning != 0)
            {
                Log.LogWarning($"Lightning: {damageTypes.m_lightning}");
            }

            if (damageTypes.m_pickaxe != 0)
            {
                Log.LogWarning($"Pickaxe: {damageTypes.m_pickaxe}");
            }

            if (damageTypes.m_pierce != 0)
            {
                Log.LogWarning($"Pierce: {damageTypes.m_pierce}");
            }

            if (damageTypes.m_poison != 0)
            {
                Log.LogWarning($"Poison: {damageTypes.m_poison}");
            }


            if (damageTypes.m_slash != 0)
            {
                Log.LogWarning($"Slash: {damageTypes.m_slash}");
            }

            if (damageTypes.m_spirit != 0)
            {
                Log.LogWarning($"Spirit: {damageTypes.m_spirit}");
            }

            Log.LogWarning("\n\n");
        }

        public static bool isGreydwarfShaman(Character character)
        {
            return character.name.Contains("GreydwarfShaman");
        }

        public static void PlayEffect(string prefabName, Vector3 pos)
        {
            GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
            if (prefab != null)
            {
                Object.Instantiate(prefab, pos, Quaternion.identity);
            }
        }

        public static Tuple<Vector3, Vector3> GetTargetPosition(Character c)
        {
            Transform head = c.m_head;
            if (c.m_head != null)
            {
                Vector3 position = head.position;
                Vector3 position2 = head.position;
                position2.Set(position2.x, position2.y - 0.4f, position2.z);
                position.Set(position.x, position.y - 0.4f, position.z);
                return new Tuple<Vector3, Vector3>(position, position2);
            }

            return null;
        }


        public static void PrintItems(Character character)
        {
            Humanoid humanoid = (Humanoid)character;
            foreach (GameObject item in humanoid.m_defaultItems)
            {
                Log.LogWarning(item.gameObject.name);
            }
        }

        public static void PrintCooldowns(string prefabName)
        {
            var prefab = PrefabManager.Cache.GetPrefab<GameObject>(prefabName);
            Humanoid humanoid = prefab.GetComponent<Humanoid>();
            foreach (var gameObject in humanoid.m_defaultItems)
            {
                var item = gameObject.GetComponent<ItemDrop>();
                Log.LogWarning($"{gameObject.name} : {item.m_itemData.m_shared.m_aiAttackInterval}");
            }
        }

        public static List<Player> FindPlayers(Vector3 point, float radius)
        {
            var players = new List<Player>();
            Player.GetPlayersInRange(point, radius, players);
            return players;
        }

        public static List<Character> FindEnemies(this Character character, float range)
        {
            var enemies = new List<Character>();
            foreach (Character ch in Character.GetAllCharacters())
            {
                if (BaseAI.IsEnemy(character, ch) && (ch.transform.position - character.transform.position).magnitude <= range)
                {
                    enemies.Add(ch);
                }
            }
            return enemies;
        }

        public static void SetCooldown(GameObject prefab, float newCooldown)
        {
            var item = prefab.GetComponent<ItemDrop>().m_itemData;
            item.m_shared.m_aiAttackInterval = newCooldown;
        }

        public static float Lerp(ref float value, float target, float delta)
        {
            if (value != target)
            {
                if (value < target)
                {
                    value = Mathf.Min(target, value + delta);
                }
                else
                {
                    value = Mathf.Max(target, value - delta);
                }
            }
            
            return value / target;
        }

        public static Dictionary<string, Dictionary<string, Main.ItemInfo>> DeserializeFile(string filename)
        {
            string path = Path.Combine(Main.ModPath, filename);
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Main.ItemInfo>>>(json);
        }

        public static Boss GetBoss(this Character character)
        {
            var name = character.name.Replace("(Clone)", string.Empty);

            if (name.Equals("Eikthyr"))
            {
                return character.gameObject.GetComponent<Eikthyr>();
            }
            else if (name.Equals("gd_king"))
            {
                return character.gameObject.GetComponent<Elder>();
            }
            else if (name.Equals("Bonemass"))
            {
                return character.gameObject.GetComponent<Bonemass>();
            }
            else if (name.Equals("Dragon"))
            {
                return character.gameObject.GetComponent<Moder>();
            }
            else if (name.Equals("GoblinKing"))
            {
                return character.gameObject.GetComponent<Yagluth>();
            }

            return null;
        }

        public static Boss GetBoss(this BaseAI baseAI)
        {
            return baseAI.m_character.GetBoss();
        }

        public static IEnumerable<Character> GetEnemiesInRange(this Character character, float range)
        {
            List<Character> list = new List<Character>();
            Character.GetCharactersInRange(character.transform.position, range, list);
            return list.Where(ch => BaseAI.IsEnemy(ch, character));
        }

        public static IEnumerable<Character> GetAlliesInRange(this Character character, float range, int count = 0)
        {
            List<Character> list = new List<Character>();
            Character.GetCharactersInRange(character.transform.position, range, list);
            if (count != 0)
            {
                return list.Where(ch => !BaseAI.IsEnemy(ch, character)).Take(count);
            }

            return list.Where(ch => !BaseAI.IsEnemy(ch, character));
        }

        public static void PrintStartEffects(this ItemDrop item)
        {
            foreach (var effect in item.m_itemData.m_shared.m_startEffect.m_effectPrefabs)
            {
                Log.LogWarning(effect.m_prefab.name);
            }
        }

        public static void PrintHitEffects(this ItemDrop item)
        {
            foreach (var effect in item.m_itemData.m_shared.m_hitEffect.m_effectPrefabs)
            {
                Log.LogWarning(effect.m_prefab.name);
            }
        }

        public static bool IsRRRCoreInstalled()
        {
            return Chainloader.PluginInfos.ContainsKey("com.alexanderstrada.rrrcore") && Chainloader.PluginInfos.ContainsKey("com.alexanderstrada.rrrmonsters");
        }

        public static bool IsMonsterLabzInstalled()
        {
            return Chainloader.PluginInfos.ContainsKey("MonsterLabZ");
        }

        public static SE_CustomShield GetShield(this Character character, string shieldName = "shield")
        {
            return character.GetSEMan().GetStatusEffects().Find(effect => effect.name.ToLower().Contains(shieldName)) as SE_CustomShield;
        }

        public static bool HaveShield(this Character character)
        {
            return character.GetSEMan().GetStatusEffects().Any(effect => effect.name.ToLower().Contains("shield"));
        }
    }
}
