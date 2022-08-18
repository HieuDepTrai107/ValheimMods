using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using EnhancedBosses.Bosses;
using EnhancedBosses.StatusEffects;
using System.Collections.Generic;
using BepInEx.Bootstrap;
using System.IO;
using System.Reflection;
using Jotunn.Utils;

namespace EnhancedBosses
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
	[BepInDependency("castix_ValheimFPSBoost", BepInDependency.DependencyFlags.SoftDependency)]
	public class Main : BaseUnityPlugin
    {        
		public const string PluginGUID = "blumaye.enhancedbosses";
        public const string PluginName = "EnhancedBosses";
        public const string PluginVersion = "1.0.0";

		public static Harmony harmony = new Harmony(PluginGUID);

		public static readonly string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public static Dictionary<string, Dictionary<string, ItemInfo>> data = Helpers.DeserializeFile("settings.json");

		public static List<Boss> bosses = new();

		#region Declarations
		public static GameObject EikthyrAntler;
		public static GameObject EikthyrCharge;
		public static GameObject EikthyrStomp;

		public static GameObject ElderRoots;
		public static GameObject ElderStomp;
		public static GameObject ElderScream;
		public static GameObject ElderShoot;

		public static GameObject BonemassAOE;
		public static GameObject BonemassPunch;
		public static GameObject BonemassThrow;

		public static GameObject ModerTaunt;
		public static GameObject ModerBite;
		public static GameObject ModerClawLeft;
		public static GameObject ModerClawRight;
		public static GameObject ModerSpitShotgun;
		public static GameObject ModerColdbreath;

		public static GameObject YagluthBeam;
		public static GameObject YagluthMeteors;
		public static GameObject YagluthNova;
		public static GameObject YagluthTaunt;

		public static GameObject GreydwarfShamanAttack;
		public static GameObject GreydwarfAttack;
		public static GameObject GreydwarfShamanHeal;

		public static ConfigEntry<bool> ModEnabled;

		// Eikthyr
		public static ConfigEntry<int> EikthyrHeal;
		public static ConfigEntry<int> EikthyrSummonCooldown;
		public static ConfigEntry<int> EikthyrSpawnMinMinions;
		public static ConfigEntry<int> EikthyrSpawnMaxMinions;
		public static ConfigEntry<int> EikthyrSpawnMinionsMultiplier;
		public static ConfigEntry<int> EikthyrMaxMinions;
		public static ConfigEntry<int> EikthyrMaxMinionsMultiplier;

		// Elder
		public static ConfigEntry<int> ElderHealCooldown;
		public static ConfigEntry<int> ElderShieldDuration;
		public static ConfigEntry<int> ElderShieldHP;
		public static ConfigEntry<int> ElderHeal;
		public static ConfigEntry<int> ElderHealRadius;
		public static ConfigEntry<int> ElderTeleportCooldown;
		public static ConfigEntry<int> ElderTeleportationDistance;
		public static ConfigEntry<int> ElderSpawnMinMinions;
		public static ConfigEntry<int> ElderSpawnMaxMinions;
		public static ConfigEntry<int> ElderMaxMinions;
		public static ConfigEntry<int> ElderSpawnMinionsMultiplier;
		public static ConfigEntry<int> ElderMaxMinionsMultiplier;

		// Bonemass

		public static ConfigEntry<bool> BonemassTripEffect;
		public static ConfigEntry<int> BonemassMinMinions;
		public static ConfigEntry<int> BonemassMaxMinions;
		public static ConfigEntry<int> BonemassMinionsMultiplier;

		// Moder
		public static ConfigEntry<int> ModerMinMinions;
		public static ConfigEntry<int> ModerMaxMinions;
		public static ConfigEntry<int> ModerMinionsMultiplier;

		// Yagluth
		public static ConfigEntry<int> YagluthMinMinions;
		public static ConfigEntry<int> YagluthMaxMinions;
		public static ConfigEntry<int> YagluthMinionsMultiplier;
		#endregion

		public class ItemInfo
		{
			public bool Enabled { get; set; }

			public int Cooldown { get; set; }
		}

		public void Awake()
		{
			CreateConfigValues();
			if (!ModEnabled.Value) return;

			if (Chainloader.PluginInfos.ContainsKey("castix_ValheimFPSBoost"))
            {
				BonemassTripEffect.Value = false;
            }

			CreateVortex();

			PrefabManager.OnPrefabsRegistered += LoadBosses;
			PrefabManager.OnVanillaPrefabsAvailable += SetupBosses;
			SetupStatusEffects();
			harmony?.PatchAll();
		}

		public void CreateConfigValues()
		{
			Config.SaveOnConfigSet = true;

			ModEnabled = Config.Bind("Мод", "Включить мод", true);

			// Eikthyr
			EikthyrHeal = Config.Bind("Eikthyr", "EikthyrHeal", 100);
			EikthyrSpawnMinMinions = Config.Bind("Eikthyr", "EikthyrSpawnMinMinions", 2);
			EikthyrSpawnMaxMinions = Config.Bind("Eikthyr", "EikthyrSpawnMaxMinions", 3);
			EikthyrSpawnMinionsMultiplier = Config.Bind("Eikthyr", "EikthyrSpawnMinionsMultiplier", 1);
			EikthyrMaxMinions = Config.Bind("Eikthyr", "EikthyrMaxMinions", 4);
			EikthyrMaxMinionsMultiplier = Config.Bind("Eikthyr", "EikthyrMaxMinionsMultiplier", 1);

			// Elder
			ElderShieldDuration = Config.Bind("Elder", "ElderShieldDuration", 20);
			ElderShieldHP = Config.Bind("Elder", "ElderShieldHP", 200);
			ElderHeal = Config.Bind("Elder", "ElderHeal", 200);
			ElderHealRadius = Config.Bind("Elder", "ElderHealRadius", 20);
			ElderHealCooldown = Config.Bind("Elder", "ElderHealCooldown", 60);
			ElderTeleportCooldown = Config.Bind("Elder", "ElderTeleportCooldown", 60);
			ElderTeleportationDistance = Config.Bind("Elder", "ElderTeleportationDistance", 20);
			ElderSpawnMinMinions = Config.Bind("Elder", "ElderSpawnMinMinions", 2);
			ElderSpawnMaxMinions = Config.Bind("Elder", "ElderSpawnMaxMinions", 3);
			ElderSpawnMinionsMultiplier = Config.Bind("Elder", "ElderSpawnMinionsMultiplier", 2);
			ElderMaxMinions = Config.Bind("Elder", "ElderMaxMinions", 4);
			ElderMaxMinionsMultiplier = Config.Bind("Elder", "ElderMaxMinionsMultiplier", 2);

			// Bonemass
			BonemassTripEffect = Config.Bind("Bonemass", "BonemasTripEffect", true);
			BonemassMinMinions = Config.Bind("Bonemass", "BonemassMinMinions", 2);
			BonemassMaxMinions = Config.Bind("Bonemass", "BonemassMaxMinions", 3);
			BonemassMinionsMultiplier = Config.Bind("Bonemass", "BonemassMinionsMultiplier", 2);

			// Moder
			ModerMinMinions = Config.Bind("Moder", "ModerMinMinions", 2);
			ModerMaxMinions = Config.Bind("Moder", "ModerMaxMinions", 3);
			ModerMinionsMultiplier = Config.Bind("Moder", "ModerMinionsMultiplier", 2);

			// Yagluth
			YagluthMinMinions = Config.Bind("Yagluth", "YagluthMinMinions", 1);
			YagluthMaxMinions = Config.Bind("Yagluth", "YagluthMaxMinions", 2);
			YagluthMinionsMultiplier = Config.Bind("Yagluth", "YagluthMinionsMultiplier", 2);
		}

		public void CreateVortex()
		{
			AssetBundle assetBundle = AssetUtils.LoadAssetBundleFromResources("eb_assetbundle");

			GameObject gameObject4 = assetBundle.LoadAsset<GameObject>("ArcaneOrbitSphere.prefab");
			gameObject4.AddComponent<ZNetView>();
			gameObject4.AddComponent<ZSyncTransform>();

			TimedDestruction td = gameObject4.AddComponent<TimedDestruction>();
			td.m_triggerOnAwake = true;

			gameObject4.AddComponent<Vortex>();
			PrefabManager.Instance.AddPrefab(gameObject4);
		}

		public void SetupStatusEffects()
		{
			ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<SE_Electric>(), fixReference: true));
			ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<SE_Slow>(), fixReference: true));
			ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<SE_Trip>(), fixReference: true));
		}

		public void LoadBosses()
		{
			LoadEikthyr();
			LoadElder();
			LoadBonemass();
			LoadModer();
			LoadYagluth();

			foreach (var kvp1 in data)
			{
				var bossName = kvp1.Key;
				var items = kvp1.Value;
				var humanoid = PrefabManager.Cache.GetPrefab<Humanoid>(bossName);

				List<GameObject> list = new List<GameObject>();

				foreach (var kvp2 in items)
				{
					string itemName = kvp2.Key;
					ItemInfo itemInfo = kvp2.Value;

					if (itemInfo.Enabled)
					{
						GameObject gameObject = PrefabManager.Instance.GetPrefab(itemName);
						ItemDrop item = gameObject.GetComponent<ItemDrop>();
						item.m_itemData.m_shared.m_aiAttackInterval = itemInfo.Cooldown;
						list.Add(gameObject);
					}
				};

				humanoid.m_defaultItems = list.ToArray();
			}

			PrefabManager.OnPrefabsRegistered -= LoadBosses;
		}

		public void LoadEikthyr()
		{
			EikthyrAntler = PrefabManager.Instance.GetPrefab("Eikthyr_antler");
			EikthyrCharge = PrefabManager.Instance.GetPrefab("Eikthyr_charge");
			EikthyrStomp = PrefabManager.Instance.GetPrefab("Eikthyr_stomp");

			Eikthyr.EikthyrGroundSlam();
			Eikthyr.EikthyrSummon();
			Eikthyr.EikthyrClones();
			Eikthyr.EikthyrVortex();

			GameObject prefab = PrefabManager.Instance.GetPrefab("Eikthyr");
			prefab.AddComponent<Eikthyr>();
		}

		public void LoadElder()
		{
			ElderRoots = PrefabManager.Instance.GetPrefab("gd_king_rootspawn");
			ElderStomp = PrefabManager.Instance.GetPrefab("gd_king_stomp");
			ElderScream = PrefabManager.Instance.GetPrefab("gd_king_scream");
			ElderShoot = PrefabManager.Instance.GetPrefab("gd_king_shoot");

			Elder.ElderSummon();
			Elder.ElderShield();

			GameObject prefab = PrefabManager.Instance.GetPrefab("gd_king");
			prefab.AddComponent<Elder>();
		}

		public void LoadBonemass()
		{
			BonemassAOE = PrefabManager.Instance.GetPrefab("bonemass_attack_aoe");
			BonemassPunch = PrefabManager.Instance.GetPrefab("bonemass_attack_punch");
			BonemassThrow = PrefabManager.Instance.GetPrefab("bonemass_attack_throw");

			Bonemass.BonemassSummon();

			GameObject prefab = PrefabManager.Instance.GetPrefab("Bonemass");
			prefab.AddComponent<Bonemass>();
		}

		public void LoadModer()
		{
			ModerTaunt = PrefabManager.Instance.GetPrefab("dragon_taunt");
			ModerBite = PrefabManager.Instance.GetPrefab("dragon_bite");
			ModerClawLeft = PrefabManager.Instance.GetPrefab("dragon_claw_left");
			ModerClawRight = PrefabManager.Instance.GetPrefab("dragon_claw_right");
			ModerSpitShotgun = PrefabManager.Instance.GetPrefab("dragon_spit_shotgun");
			ModerColdbreath = PrefabManager.Instance.GetPrefab("dragon_coldbreath");

			Moder.ModerSummon();

			GameObject prefab = PrefabManager.Instance.GetPrefab("Dragon");
			prefab.AddComponent<Moder>();
		}

		public void LoadYagluth()
		{
			YagluthBeam = PrefabManager.Instance.GetPrefab("GoblinKing_Beam");
			YagluthMeteors = PrefabManager.Instance.GetPrefab("GoblinKing_Meteors");
			YagluthNova = PrefabManager.Instance.GetPrefab("GoblinKing_Nova");
			YagluthTaunt = PrefabManager.Instance.GetPrefab("GoblinKing_Taunt");

			Yagluth.YagluthSummon();

			GameObject prefab = PrefabManager.Instance.GetPrefab("GoblinKing");
			prefab.AddComponent<Yagluth>();
		}

		public void SetupBosses()
		{
			foreach (var character in Resources.FindObjectsOfTypeAll<Character>())
			{
				if (character.IsBoss())
				{
					character.gameObject.AddComponent<Boss>();
				}
			}

			PrefabManager.OnVanillaPrefabsAvailable -= SetupBosses;
		}
	}
}