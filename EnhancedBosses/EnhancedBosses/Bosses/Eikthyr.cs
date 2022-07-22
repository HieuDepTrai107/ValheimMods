using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;
using EnhancedBosses.StatusEffects;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class Eikthyr : MonoBehaviour
    {
        public Character character;
        public MonsterAI monsterAI;

        public static List<string> EikthyrCreatures = new List<string>()
        {
            "Boar",
            "Neck",
            "Greyling"
        };

        public void Awake()
        {
            character = gameObject.GetComponent<Character>();
            monsterAI = gameObject.GetComponent<MonsterAI>();
        }

        public bool Process_Attack(Attack attack)
        {
            var weapon = attack.m_weapon;

            if (weapon.m_shared.m_name == "Eikthyr_summon")
            {
                SpawnMinions();
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

            character.transform.position = new Vector3(position.x + Random.Range(-radius, radius), position.y, position.z + Random.Range(-radius, radius));
            Vector3 centerPoint = character.GetCenterPoint();
            centerPoint.Set(centerPoint.x - 0.5f, centerPoint.y - 5f, centerPoint.z + 1f);
            Object.Instantiate(ZNetScene.instance.GetPrefab("fx_eikthyr_forwardshockwave"), centerPoint, Quaternion.Euler(-90f, 0f, 0f));
        }

        public void Stomp()
        {
            Vector3 position = character.transform.position + character.GetLookDir() * 4f;
            foreach (var player in Utils.FindEnemies(character, 10f))
            {
                player.transform.position = position;
                player.GetSEMan().AddStatusEffect("EB_EikthyrStomp", true);
            }
        }

        public void Teleport(float distance = 30f)
        {
            Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_spawn_small"), character.transform.position, Quaternion.identity);

            int Warp_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock", "Water", "character", "character_net", "character_ghost");
            Vector3 position = character.GetEyePoint();

            Heightmap.GetHeight(character.transform.position, out float height);
            Vector3 target = (!Physics.Raycast(character.GetEyePoint(), character.GetLookDir(), out RaycastHit hitInfo, float.PositiveInfinity, Warp_Layermask) || !(bool)hitInfo.collider) ? (position + character.GetLookDir() * 1000f) : hitInfo.point;
            target.y = height;

            float warpMagnitude = (distance * character.GetLookDir()).magnitude;
            Vector3 moveVec = Vector3.MoveTowards(position, target, warpMagnitude);
            character.transform.position = moveVec;

            Vector3 posXZ = new Vector3(Player.m_localPlayer.transform.position.x, character.transform.position.y, Player.m_localPlayer.transform.position.z);
            character.transform.LookAt(posXZ);

            Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_spawn_small"), character.transform.position, Quaternion.identity);
        }

        public void TeleportTo()
        {
            Character targetCreature = monsterAI.m_targetCreature;
            if (!targetCreature)
            {
                return;
            }

            Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_spawn_small"), character.transform.position, Quaternion.identity);

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

            Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_spawn_small"), character.transform.position, Quaternion.identity);
        }

        public void SpawnClone(Vector3 position)
        {
            GameObject prefab = PrefabManager.Instance.GetPrefab("RRRM_EikthyrClone");
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

            Vector3 centerPoint = ch.GetCenterPoint();
            centerPoint.Set(centerPoint.x - 0.5f, centerPoint.y - 5f, centerPoint.z + 1f);
            Object.Instantiate(ZNetScene.instance.GetPrefab("fx_eikthyr_forwardshockwave"), centerPoint, Quaternion.Euler(-90f, 0f, 0f));
        }

        public void SpawnMinions()
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            var min = Main.EikthyrMinMinions.Value + (playersCount - 1) * Main.EikthyrMinionsMultiplier.Value;
            var max = Main.EikthyrMaxMinions.Value + (playersCount - 1) * Main.EikthyrMinionsMultiplier.Value;
            Utils.SpawnCreatures(character, EikthyrCreatures, min, max);
        }

        public static GameObject EikthyrGroundSlam()
        {
            GameObject trollGroundSlam = PrefabManager.Instance.GetPrefab("troll_groundslam");
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("Eikthyr_groundslam", trollGroundSlam);
            ItemDrop.ItemData.SharedData shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;
            shared.m_attack.m_attackAnimation = "attack_stomp";
            shared.m_name = "Eikthyr_groundslam";
            shared.m_aiAttackInterval = Main.EikthyrGroundSlamCooldown.Value;
            shared.m_damages = trollGroundSlam.GetComponent<ItemDrop>().m_itemData.m_shared.m_damages.Clone();
            ItemDrop component = Main.EikthyrStomp.GetComponent<ItemDrop>();
            shared.m_startEffect = component.m_itemData.m_shared.m_startEffect;
            shared.m_attackStatusEffect = PrefabManager.Cache.GetPrefab<SE_Electric>("EB_Electric");
            return gameObject;
        }

        public static GameObject EikthyrSummon()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("Eikthyr_summon", "Eikthyr_charge");
            ItemDrop.ItemData.SharedData shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;
            shared.m_aiAttackInterval = Main.EikthyrSummonCooldown.Value;
            shared.m_name = "Eikthyr_summon";
            shared.m_hitEffect = new EffectList();
            shared.m_triggerEffect = new EffectList();
            shared.m_hitTerrainEffect = new EffectList();
            shared.m_hitEffect = new EffectList();
            shared.m_startEffect = new EffectList();
            shared.m_damages.m_blunt = 0f;
            shared.m_damages.m_chop = 0f;
            shared.m_damages.m_damage = 0f;
            shared.m_damages.m_fire = 0f;
            shared.m_damages.m_frost = 0f;
            shared.m_damages.m_spirit = 0f;
            shared.m_damages.m_slash = 0f;
            shared.m_damages.m_pierce = 0f;
            shared.m_damages.m_pickaxe = 0f;
            shared.m_damages.m_lightning = 0f;
            return gameObject;
        }

        public static GameObject EikthyrClones()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("Eikthyr_clones", "troll_groundslam");
            ItemDrop.ItemData.SharedData shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;
            shared.m_attack.m_attackAnimation = "attack_stomp";
            shared.m_name = "Eikthyr_clones";
            shared.m_aiAttackInterval = Main.EikthyrGroundSlamCooldown.Value;
            return gameObject;
        }
    }
}
