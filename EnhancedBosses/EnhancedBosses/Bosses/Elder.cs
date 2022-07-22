using UnityEngine;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnhancedBosses.StatusEffects;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class Elder : MonoBehaviour
    {
        public Character character;
        public MonsterAI monsterAI;
        public BaseAI baseAI;

        public List<string> ElderCreatures = new List<string>()
        {
            "Greydwarf_Elite",
            "Greydwarf_Shaman"
        };

        public int treeHP = Main.ElderHeal.Value;
        public float teleportTimer = Main.ElderTeleportCooldown.Value;
        public float healTimer = Main.ElderHealCooldown.Value;
        public float teleportDistance = Main.ElderTeleportationDistance.Value;

        public bool isHealing = false;
        public bool isTeleporting = false;

        public void Awake()
        {
            character = gameObject.GetComponent<Character>();
            monsterAI = gameObject.GetComponent<MonsterAI>();
            baseAI = gameObject.GetComponent<BaseAI>();
        }

        public void Update()
        {
            if (character.m_nview.IsOwner())
            {
                teleportTimer -= Time.deltaTime;
                if (teleportTimer <= 0f)
                {
                    Teleport();
                }

                healTimer -= Time.deltaTime;
                if (healTimer <= 0f)
                {
                    EatTree();
                }
            } 
        }
        
        public bool Process_Attack(Attack attack)
        {
            var weapon = attack.m_weapon;

            if (weapon.m_shared.m_name == "gd_king_summon")
            {
                SpawnMinions();
                return false;
            }
            else if (weapon.m_dropPrefab == Main.ElderRoots)
            {
                SpawnDeathsquitos();
            }

            if (character.GetHealthPercentage() < 0.5f)
            {
                if (weapon.m_shared.m_name == "gd_king_shield")
                {
                    CastShield();
                    return false;
                }
            }

            return true;
        }

        public void SpawnDeathsquitos()
        {
            Vector3 position = character.transform.position;

            int count = 3;
            float radius = 10f;

            for (int i = 0; i < count; i++)
            {
                Vector3 vector = new Vector3(position.x + Random.Range(-radius, radius), position.y, position.z + Random.Range(-radius, radius));
                SpawnDeathsquito(vector);
            }
        }

        public void SpawnDeathsquito(Vector3 position)
        {
            GameObject prefab = ZNetScene.instance.GetPrefab("Deathsquito");

            CharacterTimedDestruction td = prefab.GetComponent<CharacterTimedDestruction>();
            if (td != null)
            {
                td.m_timeoutMin = 10f;
                td.m_timeoutMax = td.m_timeoutMin;
                td.m_triggerOnAwake = true;
                td.enabled = true;
            }

            GameObject go = Object.Instantiate(prefab, position, Quaternion.identity);
            Character ch = go.GetComponent<Character>();
            
            ch.transform.localScale = 0.4f * Vector3.one;
            ch.m_faction = Character.Faction.ForestMonsters;
            ch.m_baseAI.SetHuntPlayer(true);
            ch.m_level = Random.Range(1, 3);

            var drop = ch.GetComponent<CharacterDrop>();
            drop.m_drops.Clear();
            drop.m_dropsEnabled = false;

            Humanoid humanoid = ch as Humanoid;
            humanoid.m_defaultItems = new GameObject[]
            {
                DeathsquitoSting()
            };

            Object.Instantiate(ZNetScene.instance.GetPrefab("fx_float_hitwater"), ch.transform.position, Quaternion.identity);
            
        }

        public void CastShield()
        {
            SE_Shielded se_shielded = ScriptableObject.CreateInstance<SE_Shielded>();
            se_shielded.m_character = character;
            character.GetSEMan().AddStatusEffect(se_shielded);
        }

        async public void EatTree()
        {
            if (character.GetHealthPercentage() < 0.5f)
            {
                TreeBase tree = Utils.FindNearTree(character.transform.position, 20f);
                if (tree != null)
                {
                    isHealing = true;
                    healTimer = Main.ElderHealCooldown.Value;
                    character.m_zanim.SetTrigger("scream");
                    await Task.Delay(200);
                    character.Heal(treeHP);
                    Utils.DestroyTree(tree);
                    isHealing = false;
                }
            }
        }

        async public void Teleport()
        {
            if (baseAI.HaveTarget())
            {
                var target = baseAI.GetTargetCreature();
                if (target.IsPlayer())
                {
                    Vector3 targetPosition = target.transform.position;
                    float distanceToTarget = Vector3.Distance(targetPosition, character.transform.position);
                    if (distanceToTarget > teleportDistance)
                    {
                        var tree = Utils.FindNearTree(targetPosition, 20f);
                        if (tree != null)
                        {
                            if (Vector3.Distance(targetPosition, tree.transform.position) < distanceToTarget)
                            {
                                isTeleporting = true;
                                baseAI.StopMoving();
                                teleportTimer = Main.ElderTeleportCooldown.Value;
                                character.m_zanim.SetTrigger("spawn");
                                await Task.Delay(500);
                                character.transform.position = tree.transform.position;
                                baseAI.StopMoving();
                                character.Heal(treeHP);
                                Utils.DestroyTree(tree);
                                isTeleporting = false;
                            }
                        }
                    }
                }
            }
        }

        public void SpawnLog()
        {
            var position = character.transform.position + character.transform.forward * 5f + character.transform.up * 10f;
            GameObject gameObject = Object.Instantiate(ZNetScene.instance.GetPrefab("beech_log"), position, character.transform.rotation);
            gameObject.transform.Rotate(-90, 90, 0);

            // Rigidbody component = gameObject.GetComponent<Rigidbody>();
            // component.AddForce(EnhancedBossesPlugin.vector.Value * EnhancedBossesPlugin.force.Value, ForceMode.Impulse);

            // var test = gameObject.AddComponent<TimedDestruction>();
            // test.m_timeout = 1f;
        }

        public void SpawnMinions()
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            var min = Main.ElderMinMinions.Value + (playersCount - 1) * Main.ElderMinionsMultiplier.Value;
            var max = Main.ElderMaxMinions.Value + (playersCount - 1) * Main.ElderMinionsMultiplier.Value;
            Utils.SpawnCreatures(character, ElderCreatures, min, max);
        }

        public GameObject DeathsquitoSting()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("ElderDeathsquito_sting", "Deathsquito_sting");
            ItemDrop.ItemData.SharedData shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;
            shared.m_name = "ElderDeathsquito_sting";
            shared.m_damages.m_blunt = 0f;
            shared.m_damages.m_chop = 0f;
            shared.m_damages.m_damage = 0f;
            shared.m_damages.m_fire = 0f;
            shared.m_damages.m_frost = 0f;
            shared.m_damages.m_spirit = 0f;
            shared.m_damages.m_slash = 0f;
            shared.m_damages.m_pierce = 10f;
            shared.m_damages.m_pickaxe = 0f;
            shared.m_damages.m_lightning = 0f;
            return gameObject;
        }

        public static GameObject ElderSummon()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("gd_king_summon", "gd_king_rootspawn");
            ItemDrop.ItemData.SharedData shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;
            shared.m_aiAttackInterval = Main.ElderSummonCooldown.Value;
            shared.m_name = "gd_king_summon";
            return gameObject;
        }

        public static GameObject ElderShield()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("gd_king_shield", "gd_king_rootspawn");
            ItemDrop.ItemData.SharedData shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;
            shared.m_aiAttackInterval = Main.ElderShieldCooldown.Value;
            shared.m_name = "gd_king_shield";
            return gameObject;
        }
    }
}
