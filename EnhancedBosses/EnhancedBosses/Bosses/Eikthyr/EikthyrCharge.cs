using Jotunn.Managers;
using System.Threading.Tasks;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class EikthyrCharge : CustomAttack
    {
        public EikthyrCharge()
        {
            name = "Eikthyr_charge";
            bossName = "Eikthyr";
            stopOriginalAttack = false;
        }

        public float teleportDistance = 30f;

        public int Warp_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock", "Water", "character", "character_net", "character_ghost");
        public int ScriptChar_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock", "character", "character_net", "character_ghost");

        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            Character targetCreature = monsterAI.m_targetCreature;

            if (targetCreature == null) return;

            Vector3 position = character.GetEyePoint();

            Heightmap.GetHeight(character.transform.position, out float height1);
            Vector3 target = (!Physics.Raycast(character.GetEyePoint(), character.GetLookDir(), out RaycastHit hitInfo1, float.PositiveInfinity, Warp_Layermask) || !(bool)hitInfo1.collider) ? (position + character.GetLookDir() * 1000f) : hitInfo1.point;
            target.y = height1;

            float warpMagnitude = (teleportDistance * character.GetLookDir()).magnitude;
            Vector3 moveVec = Vector3.MoveTowards(position, target, warpMagnitude);
            character.transform.position = moveVec;

            Vector3 posXZ = new Vector3(targetCreature.transform.position.x, character.transform.position.y, targetCreature.transform.position.z);
            character.transform.LookAt(posXZ);

            Eikthyr.Thunder(character, character.transform.position);

            /*
            if (character.GetHealthPercentage() < 1.5f)
            {
                await Task.Delay(1000);

                Execute_Lightning(character, monsterAI);

                Eikthyr.Thunder(character, character.transform.position);

                await Task.Delay(1000);

                Execute_Lightning(character, monsterAI);

                Eikthyr.Thunder(character, character.transform.position);
            }
            */
        }

        public void Execute_Lightning(Character character, MonsterAI monsterAI)
        {
            Character targetCreature = monsterAI.m_targetCreature;

            if (targetCreature != null)
            {
                Vector3 localPosition = character.transform.localPosition;
                Vector3 localPosition2 = targetCreature.transform.localPosition;
                localPosition.y = 0f;
                localPosition2.y = 0f;
                if (Vector3.Dot(localPosition - localPosition2, targetCreature.transform.forward) >= 0f)
                {
                    character.transform.localPosition = targetCreature.transform.localPosition - targetCreature.transform.forward * 15f;
                    character.transform.localRotation = targetCreature.transform.localRotation;
                }
                else
                {
                    character.transform.localPosition = targetCreature.transform.localPosition + targetCreature.transform.forward * 15f;
                    character.transform.localRotation = targetCreature.transform.localRotation;
                    character.transform.RotateAround(character.transform.localPosition, character.transform.up, 180f);
                }

                Vector3 posXZ = new Vector3(targetCreature.transform.position.x, character.transform.position.y, targetCreature.transform.position.z);
                character.transform.LookAt(posXZ);

                GameObject prefab = PrefabManager.Instance.GetPrefab("fx_eikthyr_forwardshockwave");
                Object.Instantiate(prefab, character.transform.position, Quaternion.LookRotation(character.GetLookDir()));

                HitData hitData = new HitData();
                hitData.m_damage.m_lightning = 20f;
                hitData.SetAttacker(character);

                foreach (RaycastHit raycastHit in Physics.SphereCastAll(character.GetEyePoint(), 5f, character.GetLookDir(), 15f, ScriptChar_Layermask))
                {
                    Character ch = raycastHit.collider?.gameObject?.GetComponent<Character>();
                    if (ch != null && BaseAI.IsEnemy(character, ch))
                    {
                        ch.Damage(hitData);
                    }
                }
            }
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.GetPrefab(name);

            ItemDrop item = gameObject.GetComponent<ItemDrop>();

            return gameObject;
        }
    }
}
