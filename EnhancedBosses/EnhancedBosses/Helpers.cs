using Jotunn.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Log = Jotunn.Logger;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace EnhancedBosses
{
    class Helpers
    {
        public static void PrintWeapons(string name)
        {
            var prefab = PrefabManager.Cache.GetPrefab<GameObject>(name);
            Humanoid humanoid = prefab.GetComponent<Humanoid>();
            foreach (var item in humanoid.m_defaultItems)
            {
                Log.LogWarning(item.name);
            }
        }

        public static bool isEikthyr(Character character)
        {
            return character.name.Replace("(Clone)", string.Empty).Equals("Eikthyr");
        }

        public static bool isEikthyr(Humanoid character)
        {
            return character.name.Replace("(Clone)", string.Empty).Equals("Eikthyr");
        }

        public static bool isElder(Character character)
        {
            return character.name.Contains("gd_king");
        }

        public static bool isBonemass(Character character)
        {
            return character.name.Contains("Bonemass");
        }

        public static bool isModer(Character character)
        {
            return character.name.Contains("Dragon");
        }

        public static bool isYagluth(Character character)
        {
            return character.name.Contains("GoblinKing");
        }

        public static bool isGreydwarfShaman(Character character)
        {
            return character.name.Contains("GreydwarfShaman");
        }

        public static void PlayEffect(string prefabN, Vector3 pos)
        {
            GameObject prefab = ZNetScene.instance.GetPrefab(prefabN);
            if (prefab != null)
            {
                Object.Instantiate<GameObject>(prefab, pos, Quaternion.identity);
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

        public static GameObject GetRandomCreature(List<string> creatures)
        {
            System.Random r = new System.Random();
            int index = r.Next(creatures.Count);
            return ZNetScene.instance.GetPrefab(creatures[index]);
        }

        public static void SpawnCreatures(Character summoner, List<string> creatures, int min = 1, int max = 3)
        {
            int count = GetCreaturesCount(summoner, min, max);
            for (int i = 0; i < count; i++)
            {
                GameObject prefab = GetRandomCreature(creatures);
                Spawn(summoner, prefab);
            }
        }

        public static void Spawn(Character summoner, GameObject prefab)
        {
            Vector3 pVec = summoner.transform.position + summoner.transform.forward * 4f;
            GameObject go = Object.Instantiate(prefab, pVec, Quaternion.identity);
            Character ch = go.GetComponent<Character>();
            if (ch != null)
            {
                ch.m_baseAI.SetTargetInfo(Player.m_localPlayer.GetZDOID());
                ch.m_baseAI.SetHuntPlayer(true);
                ch.m_level = Random.Range(1, 3);
                Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_Potion_stamina_medium"), ch.transform.position, Quaternion.identity);
                Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_WishbonePing"), ch.transform.position, Quaternion.identity);
            }
        }

        public static int GetCreaturesCount(Character character, int min, int max)
        {
            float hp = character.GetHealthPercentage();
            int k = (int)(max * (1 - hp)) + (int)Mathf.Round(min * hp);
            return k;
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

        public static List<Player> FindPlayers(Vector3 point, float radius = 20f)
        {
            var players = new List<Player>();
            Player.GetPlayersInRange(point, radius, players);
            return players;
        }

        public static List<Character> FindEnemies(Character character, float range = 20f)
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

        public static TreeBase FindNearTree(Vector3 position, float radius)
        {
            TreeBase nearTree = null;
            float minDistance = 10000;
            var colliders = Physics.OverlapSphere(position, radius);
            foreach (var collider in colliders)
            {
                if (collider?.gameObject?.GetComponentInParent<TreeBase>() is TreeBase tree)
                {
                    float distance = Vector3.Distance(tree.transform.position, position);
                    if (tree != nearTree && distance < minDistance)
                    {
                        nearTree = tree;
                        minDistance = distance;
                    }
                }
            }

            return nearTree;
        }

        public static void DestroyTree(TreeBase tree)
        {
            tree.m_destroyedEffect.Create(tree.transform.position, tree.transform.rotation, tree.transform);
            tree.SpawnLog(Vector3.zero);
            List<GameObject> dropList = tree.m_dropWhenDestroyed.GetDropList();
            for (int i = 0; i < dropList.Count; i++)
            {
                Vector2 vector = Random.insideUnitCircle * 0.5f;
                Vector3 position = tree.transform.position + Vector3.up * tree.m_spawnYOffset + new Vector3(vector.x, tree.m_spawnYStep * (float)i, vector.y);
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
                Object.Instantiate(dropList[i], position, rotation);
            }
            tree.gameObject.SetActive(value: false);
            tree.m_nview.Destroy();
        }
        public static Dictionary<string, Dictionary<string, Main.ItemInfo>> DeserializeFile(string filename)
        {
            string path = Path.Combine(Main.ModPath, filename);
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Main.ItemInfo>>>(json);
        }
    }
}
