using HarmonyLib;
using Jotunn.Managers;
using EnhancedBosses.Bosses;
using EnhancedBosses.StatusEffects;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace EnhancedBosses
{
    class Patches
    {
        /*
        [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.TestShow))]
        public static class EnemyHud_TestShow_Postfix
        {
            public static void Postfix(ref Character c, ref bool __result)
            {
                if (c.name.Contains("RRRM_EikthyrClone"))
                {
                    __result = false;
                }
            }
        }
        */

        [HarmonyPatch(typeof(Minimap), nameof(Minimap.UpdateEventPin))]
        public static class UpdateEventMobMinimapPinsPatch
        {
            public static void Postfix()
            {
                PinManager.CheckBossPins();
            }
        }

        [HarmonyPatch(typeof(BaseAI), nameof(BaseAI.CanUseAttack))]
        public static class BaseAI_CanUseAttack
        {
            public static void Postfix(BaseAI __instance, ItemDrop.ItemData item, ref bool __result)
            {
                if (__instance.name.Contains("Eikthyr"))
                {
                    Eikthyr eikthyr = __instance.gameObject.GetComponent<Eikthyr>();
                    if (item.m_shared.m_name == "Eikthyr_summon")
                    {
                        if (eikthyr.GetMinionsCount() > eikthyr.GetMaxMinions())
                        {
                            __result = false;
                        }
                    }
                }
                else if (__instance.name.Contains("gd_king"))
                {
                    Elder elder = __instance.gameObject.GetComponent<Elder>();
                    if (item.m_shared.m_name == "gd_king_summon")
                    {
                        if (elder.GetMinionsCount() > elder.GetMaxMinions())
                        {
                            __result = false;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.DoAttack))]
        public static class MonsterAI_DoAttack_Postfix
        {
            public static bool Prefix(MonsterAI __instance)
            {
                if (__instance.name.Contains("gd_king"))
                {
                    Elder elder = __instance.gameObject.GetComponent<Elder>();
                    if (elder.isHealing || elder.isTeleporting)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(Attack), nameof(Attack.OnAttackTrigger))]
        public static class Attack_OnAttackDone_Prefix
        {
            public static bool Prefix(Attack __instance)
            {
                var character = __instance.m_character as Character;
                var weapon = __instance.m_weapon;

                if (Helpers.isEikthyr(character))
                {
                    var eikthyr = character.gameObject.GetComponent<Eikthyr>();
                    return eikthyr.Process_Attack(__instance);
                }
                else if (character.name.Contains("RRRM_EikthyrClone"))
                {
                    weapon.m_shared.m_attackStatusEffect = PrefabManager.Cache.GetPrefab<SE_Electric>("EB_Electric");
                }
                else if (Helpers.isElder(character))
                {
                    var elder = character.gameObject.GetComponent<Elder>();
                    return elder.Process_Attack(__instance);
                }
                else if (Helpers.isBonemass(character))
                {
                    var bonemass = character.gameObject.GetComponent<Bonemass>();
                    return bonemass.Process_Attack(__instance);
                }
                else if (Helpers.isModer(character))
                {
                    var moder = character.gameObject.GetComponent<Moder>();
                    return moder.Process_Attack(__instance);
                }
                else if (Helpers.isYagluth(character))
                {
                    var yagluth = character.gameObject.GetComponent<Yagluth>();
                    return yagluth.Process_Attack(__instance);
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.FixedUpdate))]
		public static class Player_FixedUpdate_Postfix
        {
            public static float Ticks = 100;

            public static float OldVignetteIntensity = 0.2f;
            public static float OldChromaticAberrationIntensity = 20f;
            public static float OldColorGradingSaturation = 2f;

            public static float NewVignetteIntensity = 0.45f;
            public static float NewChromaticAberrationIntensity = 0.15f;
            public static float NewColorGradingSaturation = 1f;
            public static float NewVignetteColorR = 0f;
            public static float NewVignetteColorG = 0f;
            public static float NewVignetteColorB = 0f;

            public static float DeltaVignetteIntensity = Mathf.Abs(OldVignetteIntensity - NewVignetteIntensity) / Ticks;
            public static float DeltaChromaticAberrationIntensity = Mathf.Abs(OldChromaticAberrationIntensity - NewChromaticAberrationIntensity) / Ticks;
            public static float DeltaColorGradingSaturation = Mathf.Abs(OldColorGradingSaturation - NewColorGradingSaturation) / Ticks;


            public static void Postfix(ref Player __instance)
			{
                if (!Main.BonemassTripEffect.Value) return;
                if (!__instance.m_nview.IsValid()) return;
                if (!__instance.m_nview.IsOwner()) return;
                if (__instance.GetPlayerID() != Player.m_localPlayer.GetPlayerID()) return;

                if (!__instance.GetSEMan().HaveStatusEffect("EB_Trip"))
                {
                    var component = GameCamera.instance.gameObject.GetComponent<PostProcessingBehaviour>();

                    var ratio = Helpers.Lerp(ref component.m_Vignette.model.m_Settings.intensity, NewVignetteIntensity, DeltaVignetteIntensity);
                    Helpers.Lerp(ref component.m_ChromaticAberration.model.m_Settings.intensity, NewChromaticAberrationIntensity, DeltaChromaticAberrationIntensity);
                    Helpers.Lerp(ref component.m_ColorGrading.model.m_Settings.basic.saturation, NewColorGradingSaturation, DeltaColorGradingSaturation);

                    if (ratio == 1)
                    {
                        component.m_Vignette.model.enabled = false;
                        component.m_ColorGrading.model.isDirty = false;
                    }

                    component.m_Vignette.model.m_Settings.color.r *= 1 - ratio;
                    component.m_Vignette.model.m_Settings.color.g *= 1 - ratio;
                    component.m_Vignette.model.m_Settings.color.b *= 1 - ratio;
                }
            }
		}
	}
}
