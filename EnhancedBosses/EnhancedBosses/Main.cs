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
	[BepInDependency("MonsterLabZ", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.alexanderstrada.rrrcore", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.alexanderstrada.rrrmonsters", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("castix_ValheimFPSBoost", BepInDependency.DependencyFlags.SoftDependency)]
	public class Main : BaseUnityPlugin
    {        
		public const string PluginGUID = "blumaye.enhancedbosses";
        public const string PluginName = "EnhancedBosses";
        public const string PluginVersion = "2.0.0";

		public AssetBundle assetBundle;

		public static Harmony harmony = new Harmony(PluginGUID);

		public static readonly string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public static Dictionary<string, Dictionary<string, ItemInfo>> cfg = Helpers.DeserializeFile("settings.json");

		public static ConfigEntry<bool> ModEnabled;
		public static ConfigEntry<bool> BonemassTripEffect;
		public static ConfigEntry<float> ModerHealthThreshold;

		public static List<Boss> bossList = new()
		{
			new Eikthyr(),
			new Elder(),
			new Bonemass(),
			new Moder(),
			new Yagluth(),
			new Queen()
		};

		public static List<Boss> pinsList = new();

		public class ItemInfo
		{
			public bool Enabled { get; set; }

			public int Cooldown { get; set; }

			public List<string> Creatures { get; set; }

			public int maxMinionsCount { get; set; }

			public int maxMinionsCountPerPlayer { get; set; }

			public int spawnMinionsCount { get; set; }

			public int spawnMinionsCountPerPlayer { get; set; }

			public float Heal { get; set; }

			public float HpThreshold { get; set; }

			public float Distance { get; set; }

			public float ShieldHp { get; set; }

			public float ShieldDuration { get; set; }

		}

		public void Awake()
		{
			CreateConfigValues();
			if (!ModEnabled.Value) return;

			if (Chainloader.PluginInfos.ContainsKey("castix_ValheimFPSBoost"))
            {
				BonemassTripEffect.Value = false;
            }

			LoadAssets();
			CreateVortex();

			PrefabManager.OnPrefabsRegistered += SetupBosses;
			SetupStatusEffects();
			harmony?.PatchAll();
		}

		public void OnDestroy()
        {
			harmony?.UnpatchSelf();
        }

		public void LoadAssets()
		{
			assetBundle = AssetUtils.LoadAssetBundleFromResources("eb_assetbundle");
		}

		public void CreateConfigValues()
		{
			Config.SaveOnConfigSet = true;

			ModEnabled = Config.Bind("General", "Enabled Mod", true);
			BonemassTripEffect = Config.Bind("Bonemass", "Bonemass hallucinations", true);
			ModerHealthThreshold = Config.Bind("Moder", "Moder land hp threshold", 0.75f, "value beetwen 0 and 1");
		}

		public void CreateVortex()
		{
			GameObject gameObject = assetBundle.LoadAsset<GameObject>("ArcaneOrbitSphere.prefab");
			gameObject.AddComponent<ZNetView>();
			gameObject.AddComponent<ZSyncTransform>();
			gameObject.AddComponent<Vortex>();

			TimedDestruction td = gameObject.AddComponent<TimedDestruction>();
			td.m_triggerOnAwake = true;

			PrefabManager.Instance.AddPrefab(gameObject);
		}

		public void SetupStatusEffects()
		{
			ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<SE_BaseShield>(), fixReference: true));
			ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<SE_Slow>(), fixReference: true));
			ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<SE_Trip>(), fixReference: true));
		}

		public void SetupBosses()
		{
			foreach (Boss boss in bossList)
			{
				boss.SetupCharacter();
				boss.SetupCustomAttacks();
			}

			PrefabManager.OnPrefabsRegistered -= SetupBosses;
		}
	}
}