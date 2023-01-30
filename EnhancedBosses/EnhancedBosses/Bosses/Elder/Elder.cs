using UnityEngine;
using Jotunn.Managers;
using System.Collections.Generic;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class Elder : Boss
    {
        public Elder()
        {
            bossName = "gd_king";
            customAttacks = new()
            {
                new ElderStomp(),
                new ElderScream(),
                new ElderRootSpawn(),
                new ElderShoot(),
                new ElderSummon(),
                new ElderShield(),
                new ElderTeleport(),
                new ElderEatTree(),
                new ElderDemolishTrees()
            };
        }

        public override void SetupCharacter()
        {
            PrefabManager.Instance.GetPrefab(bossName).AddComponent<Elder>();
        }


        public static bool IsTreeExistInRange(Vector3 position, float radius)
        {
            var colliders = Physics.OverlapSphere(position, radius);
            foreach (var collider in colliders)
            {
                if (collider?.gameObject?.GetComponentInParent<TreeBase>() is TreeBase tree)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsLogExistInRange(Vector3 position, float radius)
        {
            var colliders = Physics.OverlapSphere(position, radius);
            foreach (var collider in colliders)
            {
                if (collider?.gameObject?.GetComponentInParent<TreeLog>() is TreeLog log)
                {
                    return true;
                }
            }

            return false;
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

        public static TreeLog FindNearLog(Vector3 position, float radius)
        {
            TreeLog nearLog = null;
            float minDistance = 10000;
            var colliders = Physics.OverlapSphere(position, radius);
            foreach (var collider in colliders)
            {
                if (collider?.gameObject?.GetComponentInParent<TreeLog>() is TreeLog log)
                {
                    float distance = Vector3.Distance(log.transform.position, position);
                    if (log != nearLog && distance < minDistance)
                    {
                        nearLog = log;
                        minDistance = distance;
                    }
                }
            }

            return nearLog;
        }

        public static List<TreeBase> FindNearTrees(Vector3 position, float radius)
        {
            List<TreeBase> list = new List<TreeBase>();

            var colliders = Physics.OverlapSphere(position, radius);
            foreach (var collider in colliders)
            {
                if (collider?.gameObject?.GetComponentInParent<TreeBase>() is TreeBase tree)
                {
                    if (!list.Contains(tree))
                    {
                        list.Add(tree);
                    }
                }
            }

            return list;
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

        public void DemolishTrees()
        {
            var trees = FindNearTrees(monsterAI.GetTargetCreature().transform.position, 10f);
            foreach (var tree in trees)
            {
                DemolishTree(tree);
            }
        }

        public void DemolishTree(TreeBase tree)
        {
            tree.m_destroyedEffect.Create(tree.transform.position, tree.transform.rotation, tree.transform);

            var direcion = monsterAI.GetTargetCreature().transform.position - tree.transform.position;
            tree.SpawnLog(direcion);

            List<GameObject> dropList = tree.m_dropWhenDestroyed.GetDropList();
            for (int i = 0; i < dropList.Count; i++)
            {
                Vector2 vector = Random.insideUnitCircle * 0.5f;
                Vector3 position = tree.transform.position + Vector3.up * tree.m_spawnYOffset + new Vector3(vector.x, tree.m_spawnYStep * i, vector.y);
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
                Object.Instantiate(dropList[i], position, rotation);
            }
            tree.gameObject.SetActive(value: false);
            tree.m_nview.Destroy();
        }

    }
}
