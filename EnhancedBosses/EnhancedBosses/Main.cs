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

namespace EnhancedBosses
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
	[BepInDependency("castix_ValheimFPSBoost", BepInDependency.DependencyFlags.SoftDependency)]
	public class Main : BaseUnityPlugin
    {
        #region Declarations
        public const string PluginGUID = "blumaye.enhancedbosses";
        public const string PluginName = "EnhancedBosses";
        public const string PluginVersion = "1.0.0";

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

		public static Harmony harmony = new Harmony(PluginGUID);

		public static List<Boss> bosses = new();

		public static ConfigEntry<bool> ModEnabled;

		public static ConfigEntry<int> EikthyrAntlerCooldown;
		public static ConfigEntry<int> EikthyrChargeCooldown;
		public static ConfigEntry<int> EikthyrStompCooldown;
		public static ConfigEntry<int> EikthyrGroundSlamCooldown;
		public static ConfigEntry<int> EikthyrClonesCooldown;
		public static ConfigEntry<int> EikthyrSummonCooldown;
		public static ConfigEntry<int> EikthyrMinMinions;
		public static ConfigEntry<int> EikthyrMaxMinions;
		public static ConfigEntry<int> EikthyrMinionsMultiplier;

		public static ConfigEntry<int> ElderRootsCooldown;
		public static ConfigEntry<int> ElderStompCooldown;
		public static ConfigEntry<int> ElderScreamCooldown;
		public static ConfigEntry<int> ElderShootCooldown;
		public static ConfigEntry<int> ElderShieldCooldown;
		public static ConfigEntry<int> ElderSummonCooldown;
		public static ConfigEntry<int> ElderHealCooldown;
		public static ConfigEntry<int> ElderTeleportCooldown;
		public static ConfigEntry<int> ElderMinMinions;
		public static ConfigEntry<int> ElderMaxMinions;
		public static ConfigEntry<int> ElderMinionsMultiplier;
		public static ConfigEntry<int> ElderShieldDuration;
		public static ConfigEntry<int> ElderShieldHP;
		public static ConfigEntry<int> ElderHeal;
		public static ConfigEntry<int> ElderHealRadius;
		public static ConfigEntry<int> ElderTeleportationDistance;

		public static ConfigEntry<int> BonemassAOECooldown;
		public static ConfigEntry<bool> BonemassTripEffect;
		public static ConfigEntry<int> BonemassPunchCooldown;
		public static ConfigEntry<int> BonemassThrowCooldown;
		public static ConfigEntry<int> BonemassSummonCooldown;
		public static ConfigEntry<int> BonemassMinMinions;
		public static ConfigEntry<int> BonemassMaxMinions;
		public static ConfigEntry<int> BonemassMinionsMultiplier;

		public static ConfigEntry<int> ModerTauntCooldown;
		public static ConfigEntry<int> ModerBiteCooldown;
		public static ConfigEntry<int> ModerClawLeftCooldown;
		public static ConfigEntry<int> ModerClawRightCooldown;
		public static ConfigEntry<int> ModerSpitShotgunCooldown;
		public static ConfigEntry<int> ModerColdbreathCooldown;
		public static ConfigEntry<int> ModerMinMinions;
		public static ConfigEntry<int> ModerMaxMinions;
		public static ConfigEntry<int> ModerMinionsMultiplier;

		public static ConfigEntry<int> YagluthBeamCooldown;
		public static ConfigEntry<int> YagluthMeteorsCooldown;
		public static ConfigEntry<int> YagluthNovaCooldown;
		public static ConfigEntry<int> YagluthTauntCooldown;
		public static ConfigEntry<int> YagluthMinMinions;
		public static ConfigEntry<int> YagluthMaxMinions;
		public static ConfigEntry<int> YagluthMinionsMultiplier;
        #endregion

        public void Awake()
		{
			CreateConfigValues();
			if (!ModEnabled.Value) return;

			if (Chainloader.PluginInfos.ContainsKey("castix_ValheimFPSBoost"))
            {
				BonemassTripEffect.Value = false;
            }

			PrefabManager.OnPrefabsRegistered += LoadBosses;
			PrefabManager.OnVanillaPrefabsAvailable += SetupBosses;
			SetupStatusEffects();
			harmony?.PatchAll();
		}

		public void CreateConfigValues()
        {
			Config.SaveOnConfigSet = true;

			ModEnabled = Config.Bind("Мод", "Включить мод", true);

			EikthyrAntlerCooldown = Config.Bind("Eikthyr", "EikthyrAntlerCooldown", 5);
			EikthyrChargeCooldown = Config.Bind("Eikthyr", "EikthyrChargeCooldown", 25);
			EikthyrStompCooldown = Config.Bind("Eikthyr", "EikthyrStompCooldown", 40);
			EikthyrGroundSlamCooldown = Config.Bind("Eikthyr", "EikthyrGroundSlamCooldown", 40);
			EikthyrSummonCooldown = Config.Bind("Eikthyr", "EikthyrSummonCooldown", 40);
			EikthyrClonesCooldown = Config.Bind("Eikthyr", "EikthyrClonesCooldown", 60);
			EikthyrMinMinions = Config.Bind("Eikthyr", "EikthyrMinMinions", 2);
			EikthyrMaxMinions = Config.Bind("Eikthyr", "EikthyrMaxMinions", 4);
			EikthyrMinionsMultiplier = Config.Bind("Eikthyr", "EikthyrMinionsMultiplier", 2);

			ElderRootsCooldown = Config.Bind("Elder", "ElderRootsCooldown", 25);
			ElderStompCooldown = Config.Bind("Elder", "ElderStompCooldown", 5);
			ElderScreamCooldown = Config.Bind("Elder", "ElderScreamCooldown", 30);
			ElderShootCooldown = Config.Bind("Elder", "ElderShootCooldown", 6);
			ElderSummonCooldown = Config.Bind("Elder", "ElderSummonCooldown", 90);
			ElderShieldCooldown = Config.Bind("Elder", "ElderShieldCooldown", 60);
			ElderHealCooldown = Config.Bind("Elder", "ElderHealCooldown", 60);
			ElderTeleportCooldown = Config.Bind("Elder", "ElderTeleportCooldown", 60);
			ElderMinMinions = Config.Bind("Elder", "ElderMinMinions", 2);
			ElderMaxMinions = Config.Bind("Elder", "ElderMaxMinions", 4);
			ElderMinionsMultiplier = Config.Bind("Elder", "ElderMinionsMultiplier", 2);
			ElderShieldDuration = Config.Bind("Elder", "ElderShieldDuration", 20);
			ElderShieldHP = Config.Bind("Elder", "ElderShieldHP", 400);
			ElderHeal = Config.Bind("Elder", "ElderHeal", 500);
			ElderHealRadius = Config.Bind("Elder", "ElderHealRadius", 20);
			ElderTeleportationDistance = Config.Bind("Elder", "ElderTeleportationDistance", 20);

			BonemassAOECooldown = Config.Bind("Bonemass", "BonemassAOECooldown", 30);
			BonemassTripEffect = Config.Bind("Bonemass", "BonemasTripEffect", true);
			BonemassPunchCooldown = Config.Bind("Bonemass", "BonemassPunchCooldown", 7);
			BonemassThrowCooldown = Config.Bind("Bonemass", "BonemassThrowCooldown", 50);
			BonemassSummonCooldown = Config.Bind("Bonemass", "BonemassSummonCooldown", 90);
			BonemassMinMinions = Config.Bind("Bonemass", "BonemassMinMinions", 2);
			BonemassMaxMinions = Config.Bind("Bonemass", "BonemassMaxMinions", 3);
			BonemassMinionsMultiplier = Config.Bind("Bonemass", "BonemassMinionsMultiplier", 2);

			ModerTauntCooldown = Config.Bind("Moder", "ModerTauntCooldown", 30);
			ModerBiteCooldown = Config.Bind("Moder", "ModerBiteCooldown", 30);
			ModerClawLeftCooldown = Config.Bind("Moder", "ModerClawLeftCooldown", 30);
			ModerClawRightCooldown = Config.Bind("Moder", "ModerClawRightCooldown", 30);
			ModerSpitShotgunCooldown = Config.Bind("Moder", "ModerSpitShotgun", 8);
			ModerColdbreathCooldown = Config.Bind("Moder", "ModerColdbreathCooldown", 8);
			ModerMinMinions = Config.Bind("Moder", "ModerMinMinions", 2);
			ModerMaxMinions = Config.Bind("Moder", "ModerMaxMinions", 3);
			ModerMinionsMultiplier = Config.Bind("Moder", "ModerMinionsMultiplier", 2);

			YagluthBeamCooldown = Config.Bind("Yagluth", "YagluthBeamCooldown", 15);
			YagluthMeteorsCooldown = Config.Bind("Yagluth", "YagluthMeteorsCooldown", 25);
			YagluthNovaCooldown = Config.Bind("Yagluth", "YagluthNovaCooldown", 20);
			YagluthTauntCooldown = Config.Bind("Yagluth", "YagluthTauntCooldown", 60);
			YagluthMinMinions = Config.Bind("Yagluth", "YagluthMinMinions", 1);
			YagluthMaxMinions = Config.Bind("Yagluth", "YagluthMaxMinions", 2);
			YagluthMinionsMultiplier = Config.Bind("Yagluth", "YagluthMinionsMultiplier", 2);
		}

		public void LoadBosses()
		{
			LoadPrefabs();
			LoadEikthyr();
			LoadElder();
			LoadBonemass();
			LoadModer();
			LoadYagluth();

			PrefabManager.OnPrefabsRegistered -= LoadBosses;
		}

		public void LoadPrefabs()
		{
			EikthyrAntler = PrefabManager.Instance.GetPrefab("Eikthyr_antler");
			Utils.SetCooldown(EikthyrAntler, EikthyrAntlerCooldown.Value);
			EikthyrCharge = PrefabManager.Instance.GetPrefab("Eikthyr_charge");
			Utils.SetCooldown(EikthyrAntler, EikthyrChargeCooldown.Value);
			EikthyrStomp = PrefabManager.Instance.GetPrefab("Eikthyr_stomp");
			Utils.SetCooldown(EikthyrStomp, EikthyrStompCooldown.Value);

			ElderRoots = PrefabManager.Instance.GetPrefab("gd_king_rootspawn");
			Utils.SetCooldown(ElderRoots, ElderRootsCooldown.Value);
			ElderStomp = PrefabManager.Instance.GetPrefab("gd_king_stomp");
			Utils.SetCooldown(ElderStomp, ElderStompCooldown.Value);
			ElderScream = PrefabManager.Instance.GetPrefab("gd_king_scream");
			Utils.SetCooldown(ElderScream, ElderScreamCooldown.Value);
			ElderShoot = PrefabManager.Instance.GetPrefab("gd_king_shoot");
			Utils.SetCooldown(ElderShoot, ElderShootCooldown.Value);

			BonemassAOE = PrefabManager.Instance.GetPrefab("bonemass_attack_aoe");
			Utils.SetCooldown(BonemassAOE, BonemassAOECooldown.Value);
			BonemassPunch = PrefabManager.Instance.GetPrefab("bonemass_attack_punch");
			Utils.SetCooldown(BonemassPunch, BonemassPunchCooldown.Value);
			BonemassThrow = PrefabManager.Instance.GetPrefab("bonemass_attack_throw");
			Utils.SetCooldown(BonemassThrow, BonemassThrowCooldown.Value);

			ModerTaunt = PrefabManager.Instance.GetPrefab("dragon_taunt");
			Utils.SetCooldown(ModerTaunt, ModerTauntCooldown.Value);
			ModerBite = PrefabManager.Instance.GetPrefab("dragon_bite");
			Utils.SetCooldown(ModerBite, ModerBiteCooldown.Value);
			ModerClawLeft = PrefabManager.Instance.GetPrefab("dragon_claw_left");
			Utils.SetCooldown(ModerClawLeft, ModerClawLeftCooldown.Value);
			ModerClawRight = PrefabManager.Instance.GetPrefab("dragon_claw_right");
			Utils.SetCooldown(ModerClawRight, ModerClawRightCooldown.Value);
			ModerSpitShotgun = PrefabManager.Instance.GetPrefab("dragon_spit_shotgun");
			Utils.SetCooldown(ModerSpitShotgun, ModerSpitShotgunCooldown.Value);
			ModerColdbreath = PrefabManager.Instance.GetPrefab("dragon_coldbreath");
			Utils.SetCooldown(ModerColdbreath, ModerColdbreathCooldown.Value);

			YagluthBeam = PrefabManager.Instance.GetPrefab("GoblinKing_Beam");
			YagluthMeteors = PrefabManager.Instance.GetPrefab("GoblinKing_Meteors");
			YagluthNova = PrefabManager.Instance.GetPrefab("GoblinKing_Nova");
			YagluthTaunt = PrefabManager.Instance.GetPrefab("GoblinKing_Taunt");

			GreydwarfShamanAttack = PrefabManager.Instance.GetPrefab("Greydwarf_shaman_attack");
			GreydwarfAttack = PrefabManager.Instance.GetPrefab("Greydwarf_attack");
			GreydwarfShamanHeal = PrefabManager.Instance.GetPrefab("Greydwarf_shaman_heal");
		}

		public void SetupStatusEffects()
        {
			ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<SE_EikthyrStomp>(), fixReference: true));
			ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<SE_Electric>(), fixReference: true));
			ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<SE_Slow>(), fixReference: true));
			ItemManager.Instance.AddStatusEffect(new CustomStatusEffect(ScriptableObject.CreateInstance<SE_Trip>(), fixReference: true));
		}

		public void LoadEikthyr()
        {
			GameObject prefab = PrefabManager.Cache.GetPrefab<GameObject>("Eikthyr");
			prefab.AddComponent<Eikthyr>();
			Humanoid humanoid = prefab.GetComponent<Humanoid>();
			humanoid.m_defaultItems = new GameObject[]
			{
				EikthyrAntler,
				EikthyrCharge,
				EikthyrStomp,
				Eikthyr.EikthyrGroundSlam(),
				Eikthyr.EikthyrSummon(),
				Eikthyr.EikthyrClones()
			};
		}

		public void LoadElder()
        {
			GameObject prefab = PrefabManager.Cache.GetPrefab<GameObject>("gd_king");
			prefab.AddComponent<Elder>();
			Humanoid humanoid = prefab.GetComponent<Humanoid>();
			humanoid.m_defaultItems = new GameObject[]
			{
				ElderRoots,
				ElderStomp,
				ElderScream,
				ElderShoot,
				Elder.ElderSummon(),
				Elder.ElderShield()
			};
		}

		public void LoadBonemass()
		{
			GameObject prefab = PrefabManager.Cache.GetPrefab<GameObject>("Bonemass");
			prefab.AddComponent<Bonemass>();
			Humanoid humanoid = prefab.GetComponent<Humanoid>();
			humanoid.m_defaultItems = new GameObject[]
			{
				BonemassAOE,
				BonemassPunch,
				BonemassThrow,
				Bonemass.BonemassSummon()
			};
		}

		public void LoadModer()
        {
			GameObject prefab = PrefabManager.Cache.GetPrefab<GameObject>("Dragon");
			prefab.AddComponent<Moder>();
			Humanoid humanoid = prefab.GetComponent<Humanoid>();
			humanoid.m_defaultItems = new GameObject[]
			{
				ModerTaunt,
				ModerBite,
				ModerClawLeft,
				ModerClawRight,
				ModerSpitShotgun,
				ModerColdbreath,
				Moder.ModerSummon()
			};
		}

		public void LoadYagluth()
		{
			GameObject prefab = PrefabManager.Cache.GetPrefab<GameObject>("GoblinKing");
			prefab.AddComponent<Yagluth>();
			Humanoid humanoid = prefab.GetComponent<Humanoid>();
			humanoid.m_defaultItems = new GameObject[]
			{
				YagluthBeam,
				YagluthMeteors,
				YagluthNova,
				YagluthTaunt,
				Yagluth.YagluthSummon()
			};
		}

		public void LoadGreydwarfShaman()
        {
			GameObject prefab = PrefabManager.Cache.GetPrefab<GameObject>("Greydwarf_Shaman");
			Character character = prefab.GetComponent<Character>();
			Humanoid humanoid = (Humanoid)character;
			humanoid.m_defaultItems = new GameObject[]
			{
				GreydwarfShamanAttack,
				GreydwarfAttack,
				GreydwarfShamanHeal
			};
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