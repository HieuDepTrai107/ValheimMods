using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using EnhancedBosses.StatusEffects;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class Eikthyr : MonoBehaviour
    {
        public Character character;
        public Humanoid humanoid;
        public MonsterAI monsterAI;
        public ZNetView zNetView;

        public static List<string> EikthyrCreatures = new List<string>()
        {
            "Boar",
            "Neck",
            "Greyling"
        };

        public void Awake()
        {
            character = base.GetComponent<Character>();
            zNetView = base.GetComponent<ZNetView>();
            monsterAI = base.GetComponent<MonsterAI>();
            humanoid = base.GetComponent<Humanoid>();
            zNetView.GetZDO().Set("specialAbility", true);
        }

        public void Update()
        {
            if (character.GetHealthPercentage() < 0.5f)
            {
                if (zNetView.GetZDO().GetBool("specialAbility"))
                {
                    SpawnClones();
                    zNetView.GetZDO().Set("specialAbility", false);
                }
            }
        }

        public bool Process_Attack(Attack attack)
        {
            var weapon = attack.m_weapon;

            if (weapon.m_shared.m_name == "Eikthyr_summon")
            {
                SpawnMinions();
                return false;
            }

            if (weapon.m_shared.m_name == "Eikthyr_vortex")
            {
                Vortex();
                return false;
            }

            if (character.GetHealthPercentage() < 0.5f)
            {
                if (weapon.m_shared.m_name == "Eikthyr_clones")
                {
                    SpawnClones();
                    return false;
                }
                else if (weapon.m_dropPrefab == Main.EikthyrAntler)
                {
                    weapon.m_shared.m_attackStatusEffect = ScriptableObject.CreateInstance<SE_Electric>();
                }
                else if (weapon.m_shared.m_name == "Eikthyr_groundslam")
                {
                    Storm();
                }
                else if (weapon.m_dropPrefab == Main.EikthyrStomp)
                {
                    Stomp();
                }
                else if (weapon.m_dropPrefab == Main.EikthyrCharge)
                {
                    weapon.m_shared.m_attackStatusEffect = ScriptableObject.CreateInstance<SE_Electric>();
                    Teleport();
                }
            }

            return true;
        }

        public void SpawnClones(int count = 3, int radius = 10)
        {
            Vector3 position = character.transform.position;

            for (int i = 0; i < count; i++)
            {
                Vector3 vector = new Vector3(position.x + Random.Range(-radius, radius), position.y, position.z + Random.Range(-radius, radius));
                SpawnClone(vector);
            }

            Vector3 newPosition = new Vector3(position.x + Random.Range(-radius, radius), position.y, position.z + Random.Range(-radius, radius));
            character.transform.position = newPosition;

            Thunder(newPosition);
 
            character.Heal(Main.EikthyrHeal.Value);
        }
        public void Storm(int count = 30, int radius = 20)
        {
            Vector3 position = character.transform.position;
            
            for (int i = 0; i < count; i++)
            {
                Vector3 vector = new Vector3(Random.Range(position.x - radius, position.x + radius), position.y, Random.Range(position.z - radius, position.z + radius));
                Thunder(vector);
            }
        }

        public void Stomp()
        {
            Vector3 position = character.transform.position + character.GetLookDir() * 4f;
            foreach (var player in Helpers.FindEnemies(character, 10f))
            {
                player.transform.position = position;
                player.GetSEMan().AddStatusEffect(ScriptableObject.CreateInstance<SE_Electric>());
            }
        }

        public void Teleport(float distance = 30f)
        {
            Character targetCreature = monsterAI.m_targetCreature;

            int Warp_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock", "Water", "character", "character_net", "character_ghost");
            Vector3 position = character.GetEyePoint();

            Heightmap.GetHeight(character.transform.position, out float height);
            Vector3 target = (!Physics.Raycast(character.GetEyePoint(), character.GetLookDir(), out RaycastHit hitInfo, float.PositiveInfinity, Warp_Layermask) || !(bool)hitInfo.collider) ? (position + character.GetLookDir() * 1000f) : hitInfo.point;
            target.y = height;

            float warpMagnitude = (distance * character.GetLookDir()).magnitude;
            Vector3 moveVec = Vector3.MoveTowards(position, target, warpMagnitude);
            character.transform.position = moveVec;

            Vector3 posXZ = new Vector3(targetCreature.transform.position.x, character.transform.position.y, targetCreature.transform.position.z);
            character.transform.LookAt(posXZ);

            Thunder(character.transform.position);
        }

        public void TeleportTo()
        {
            Character targetCreature = monsterAI.m_targetCreature;
            if (!targetCreature)
            {
                return;
            }

            Vector3 localPosition = character.transform.localPosition;
            Vector3 localPosition2 = targetCreature.transform.localPosition;
            localPosition.y = 0f;
            localPosition2.y = 0f;
            if (Vector3.Dot(localPosition - localPosition2, targetCreature.transform.forward) >= 0f)
            {
                character.transform.localPosition = targetCreature.transform.localPosition - targetCreature.transform.forward * 2f;
                character.transform.localRotation = targetCreature.transform.localRotation;
            }
            else
            {
                character.transform.localPosition = targetCreature.transform.localPosition + targetCreature.transform.forward * 2f;
                character.transform.localRotation = targetCreature.transform.localRotation;
                character.transform.RotateAround(character.transform.localPosition, character.transform.up, 180f);
            }

            Thunder(character.transform.position);
        }

        public void SpawnClone(Vector3 position)
        {
            GameObject prefab = PrefabManager.Instance.GetPrefab("RRRM_EikthyrClone");
            prefab.GetComponent<Eikthyr>().enabled = false;
            GameObject gameobject = Object.Instantiate(prefab, position, Quaternion.identity);
            Character ch = gameobject.GetComponent<Character>();
            ch.m_faction = Character.Faction.Boss;

            var td = gameobject.AddComponent<CharacterTimedDestruction>();
            if (td != null)
            {
                td.m_timeoutMin = 8f;
                td.m_timeoutMax = 8f; 
                td.Trigger();
            }

            ch.m_baseAI.SetHuntPlayer(true);

            Thunder(position);
        }

        public void Spawn(float radius = 10f)
        {
            Vector3 position = character.transform.position;
            Vector3 vector = new Vector3(position.x + Random.Range(-radius, radius), position.y, position.z + Random.Range(-radius, radius));

            GameObject prefab = Helpers.GetRandomCreature(EikthyrCreatures);
            GameObject go = Object.Instantiate(prefab, position, Quaternion.identity);
            Character ch = go.GetComponent<Character>();
            
            ch.m_baseAI.SetTargetInfo(Player.m_localPlayer.GetZDOID());
            ch.m_baseAI.SetHuntPlayer(true);
            ch.m_level = Random.Range(1, 3);
            Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_Potion_stamina_medium"), position, Quaternion.identity);
            Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_WishbonePing"), position, Quaternion.identity);

            Thunder(position);
        }

        public void Vortex()
        {
            Vector3 vector = character.transform.position + character.transform.forward * 4f + Vector3.up;
            GameObject prefab = PrefabManager.Instance.GetPrefab("ArcaneOrbitSphere");

            TimedDestruction td = prefab.GetComponent<TimedDestruction>();
            td.m_timeout = 3f;

            GameObject gameObject = Object.Instantiate(prefab, vector, Quaternion.identity);

            Vortex vortex = gameObject.GetComponent<Vortex>();
            vortex.character = character;
        }

        public List<Character> GetCharacters(Vector3 position, float range)
        {
            List<Character> list = new();

            List<Character> allCharacters = new List<Character>();
            Character.GetCharactersInRange(position, range, allCharacters);
            foreach (Character ch in allCharacters)
            {
                if (BaseAI.IsEnemy(character, ch))
                {
                    list.Add(ch);
                }
            }

            return list;
        }


        public void Thunder(Vector3 position)
        {
            Heightmap.GetHeight(position, out float height);

            Vector3 vector1 = new Vector3(position.x - 0.5f, height - 5f, position.z + 1f);
            Object.Instantiate(ZNetScene.instance.GetPrefab("fx_eikthyr_forwardshockwave"), vector1, Quaternion.Euler(-90f, 0f, 0f));
            
            Vector3 vector2 = new Vector3(position.x, height, position.z);
            foreach (var character in GetCharacters(vector2, 2f))
            {
                HitData hitData = new HitData();
                hitData.m_damage.m_lightning = Random.Range(15f, 25f);
                character.ApplyDamage(hitData, true, true);
                character.GetSEMan().AddStatusEffect(ScriptableObject.CreateInstance<SE_Electric>());
            }
        }

        public void SpawnMinions()
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            var min = Main.EikthyrSpawnMinMinions.Value + (playersCount - 1) * Main.EikthyrSpawnMinionsMultiplier.Value;
            var max = Main.EikthyrSpawnMaxMinions.Value + (playersCount - 1) * Main.EikthyrSpawnMinionsMultiplier.Value;
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
                if (EikthyrCreatures.Any(e => baseAI.name.Contains(e)))
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
            return Main.EikthyrMaxMinions.Value + (playersCount - 1) * Main.EikthyrMaxMinionsMultiplier.Value;
        }


        public static void EikthyrGroundSlam()
        {
            GameObject trollGroundSlam = PrefabManager.Instance.GetPrefab("troll_groundslam");
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("Eikthyr_groundslam", trollGroundSlam);

            ItemDrop.ItemData.SharedData shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;
            shared.m_attack.m_attackAnimation = "attack_stomp";
            shared.m_name = "Eikthyr_groundslam";
            shared.m_damages = trollGroundSlam.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.Clone();

            ItemDrop component = Main.EikthyrStomp.GetComponent<ItemDrop>();
            shared.m_startEffect = component.m_itemData.m_shared.m_startEffect;
            shared.m_attackStatusEffect = PrefabManager.Cache.GetPrefab<SE_Electric>("EB_Electric");
            PrefabManager.Instance.AddPrefab(gameObject);
        }

        public static void EikthyrSummon()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("Eikthyr_summon", "Eikthyr_charge");
            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = "Eikthyr_summon";
            PrefabManager.Instance.AddPrefab(gameObject);
        }

        public static void EikthyrClones()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("Eikthyr_clones", "Eikthyr_stomp");
            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = "Eikthyr_clones";
            PrefabManager.Instance.AddPrefab(gameObject);
        }

        public static void EikthyrVortex()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("Eikthyr_vortex", "Eikthyr_charge");
            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = "Eikthyr_vortex";
            PrefabManager.Instance.AddPrefab(gameObject);
        }
    }
}
