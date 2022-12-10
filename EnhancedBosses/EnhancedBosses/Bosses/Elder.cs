using UnityEngine;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnhancedBosses.StatusEffects;
using System.Linq;
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
            character = base.GetComponent<Character>();
            monsterAI = base.GetComponent<MonsterAI>();
            baseAI = base.GetComponent<BaseAI>();
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
                
            }

            else if (weapon.m_dropPrefab == Main.ElderScream)
            {
                
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
                TreeBase tree = Helpers.FindNearTree(character.transform.position, 20f);
                if (tree != null)
                {
                    isHealing = true;
                    healTimer = Main.ElderHealCooldown.Value;
                    character.m_zanim.SetTrigger("scream");
                    await Task.Delay(200);
                    character.Heal(treeHP);
                    Helpers.DestroyTree(tree);
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
                        var tree = Helpers.FindNearTree(targetPosition, 20f);
                        if (tree != null)
                        {
                            if (Vector3.Distance(targetPosition, tree.transform.position) < distanceToTarget)
                            {
                                isTeleporting = true;
                                baseAI.StopMoving();
                                character.m_zanim.SetTrigger("spawn");
                                await Task.Delay(500);
                                character.transform.position = tree.transform.position;
                                baseAI.StopMoving();
                                character.Heal(treeHP);
                                Helpers.DestroyTree(tree);
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

        public void Spawn(float radius = 10f)
        {
            Vector3 position = character.transform.position;
            Vector3 vector = new Vector3(position.x + Random.Range(-radius, radius), position.y, position.z + Random.Range(-radius, radius));

            GameObject prefab = Helpers.GetRandomCreature(ElderCreatures);
            GameObject go = Object.Instantiate(prefab, vector, Quaternion.identity);
            Character ch = go.GetComponent<Character>();

            ch.m_baseAI.SetTargetInfo(Player.m_localPlayer.GetZDOID());
            ch.m_baseAI.SetHuntPlayer(true);
            ch.m_level = Random.Range(1, 3);
            Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_Potion_stamina_medium"), position, Quaternion.identity);
            Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_WishbonePing"), position, Quaternion.identity);
        }


        public void SpawnMinions()
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            var min = Main.ElderSpawnMinMinions.Value + (playersCount - 1) * Main.ElderSpawnMinionsMultiplier.Value;
            var max = Main.ElderSpawnMaxMinions.Value + (playersCount - 1) * Main.ElderSpawnMinionsMultiplier.Value;
            int count = Helpers.GetCreaturesCount(character, min, max);
            for (int i = 0; i < count; i++)
            {
                Spawn();
            }
        }

        public int GetMinionsCount()
        {
            int minionsCount = 0;
            foreach (BaseAI baseAI in BaseAI.GetAllInstances())
            {
                if (ElderCreatures.Any(e => baseAI.name.Contains(e)))
                {
                    float num = Utils.DistanceXZ(baseAI.transform.position, character.transform.position);
                    if (num < 25f)
                    {
                        minionsCount++;
                    }
                }
            }

            return minionsCount;
        }

        public int GetMaxMinions()
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            return Main.ElderMaxMinions.Value + (playersCount - 1) * Main.ElderMaxMinionsMultiplier.Value;
        }

        public static void ElderSummon()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("gd_king_summon", "gd_king_rootspawn");
            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = "gd_king_summon";
            PrefabManager.Instance.AddPrefab(gameObject);
        }

        public static void ElderShield()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("gd_king_shield", "gd_king_rootspawn");
            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = "gd_king_shield";
            PrefabManager.Instance.AddPrefab(gameObject);
        }
    }
}
