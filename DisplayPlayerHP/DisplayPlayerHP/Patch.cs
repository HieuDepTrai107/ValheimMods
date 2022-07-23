using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Log = Jotunn.Logger;
using Object = UnityEngine.Object;

namespace DisplayPlayerHP
{
    public class EnemyHudPatch
    {
        public static int _playerHpFontSize = 14;
        public static string _playerHpPrefix = "playerHPText";
        public static string _playerStaminaPrefix = "playerStaminaBar";

        public static ConditionalWeakTable<EnemyHud.HudData, Text> _hpTextCache = new();
        public static ConditionalWeakTable<EnemyHud.HudData, Text> _staminaTextCache = new();

        public static void CreatePlayerHealthBar(EnemyHud.HudData hudData)
        {
            var hpRoot = (hudData.m_gui.transform.Find("Health") as RectTransform);
            hpRoot.sizeDelta = new Vector2(hpRoot.sizeDelta.x, hpRoot.sizeDelta.y * 3f);
            hpRoot.anchoredPosition = new Vector2(hpRoot.anchoredPosition.x, hpRoot.anchoredPosition.y + 25f);

            hudData.m_healthFast.m_bar.sizeDelta = new Vector2(hudData.m_healthFast.m_width, hpRoot.sizeDelta.y);
            hudData.m_healthSlow.m_bar.sizeDelta = new Vector2(hudData.m_healthSlow.m_width, hpRoot.sizeDelta.y);

            var hpText = Object.Instantiate(hudData.m_name, hudData.m_name.transform.parent);
            hpText.name = _playerHpPrefix;
            hpText.rectTransform.anchoredPosition = new Vector2(hpText.rectTransform.anchoredPosition.x, 3.5f + 25f);
            hpText.text = $"<size={_playerHpFontSize}>{hudData.m_character.GetHealth():0}/{hudData.m_character.GetMaxHealth():0}</size>";
            hpText.color = Color.white;
            Object.Destroy(hpText.GetComponent<Outline>());
            _hpTextCache.Add(hudData, hpText);
            var nameText = hudData.m_gui.transform.Find("Name") as RectTransform;
            nameText.anchoredPosition = new Vector2(nameText.anchoredPosition.x, nameText.anchoredPosition.y + 25f);
        }

        public static void UpdatePlayerHealthBar(EnemyHud.HudData hudData)
        {
            if (_hpTextCache.TryGetValue(hudData, out Text hpText))
            {
                hpText.text = $"<size={_playerHpFontSize}>{hudData.m_character.GetHealth():0}/{hudData.m_character.GetMaxHealth():0}</size>";
            }
        }

        public static void CreatePlayerStaminaBar(EnemyHud.HudData hudData)
        {
            GuiBar staminaBar = Object.Instantiate(hudData.m_healthFast, hudData.m_name.transform.parent);
            staminaBar.name = _playerStaminaPrefix;
            staminaBar.SetColor(Color.yellow);
            staminaBar.m_bar.sizeDelta = new Vector2(100f, 15f);
            staminaBar.m_bar.anchoredPosition = new Vector2(150f, -10f);
            staminaBar.gameObject.layer = 1;

            var bkg = hudData.m_gui.transform.Find("Health").Find("darken");
            var test = Object.Instantiate(bkg, staminaBar.transform) as RectTransform;
            test.sizeDelta = new Vector2(-292f, -20f);
            test.anchoredPosition = new Vector2(0, -10f);
            test.gameObject.layer = 10;

            Text staminaText = Object.Instantiate(hudData.m_name, staminaBar.transform.parent);
            staminaText.rectTransform.anchoredPosition = new Vector2(0f, 8f);
            staminaText.color = Color.white;
            Object.Destroy(staminaText.GetComponent<Outline>());

            _staminaTextCache.Add(hudData, staminaText);
        }

        public static void UpdatePlayerStaminaBar(EnemyHud.HudData hudData)
        {
            var maxStamina = hudData.m_character.m_nview.GetZDO().GetFloat("maxStamina");
            var currentStamina = hudData.m_character.m_nview.GetZDO().GetFloat("stamina");
            var staminaPercentage = currentStamina / maxStamina;

            GuiBar staminaBar = Utils.FindChild(hudData.m_name.transform.parent, _playerStaminaPrefix).GetComponent<GuiBar>();
            staminaBar.m_bar.sizeDelta = new Vector2(100 * staminaPercentage, staminaBar.m_bar.sizeDelta.y);
            staminaBar.gameObject.layer = 10;

            if (_staminaTextCache.TryGetValue(hudData, out Text staminaText))
            {
                staminaText.text = $"<size={_playerHpFontSize}>{Mathf.Min(currentStamina, maxStamina):0}/{maxStamina:0}</size>";
            }
        }
        

        [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.ShowHud))]
        public static class EnemyHud_ShowHud
        {
            public static void Prefix(ref EnemyHud __instance, ref Character c, ref bool __state)
            {
                __state = __instance.m_huds.ContainsKey(c);
            }

            public static void Postfix(ref EnemyHud __instance, ref Character c, ref bool __state)
            {
                if (__state || !__instance.m_huds.TryGetValue(c, out EnemyHud.HudData hudData))
                {
                    return;
                }

                if (c.IsPlayer())
                {
                    CreatePlayerHealthBar(hudData);
                    CreatePlayerStaminaBar(hudData);
                }
            }
        }

        [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.UpdateHuds))]
        public static class ShowHudUpdateHudsPatch
        {
            public static void Postfix(ref EnemyHud __instance)
            {
                Character character = null;

                foreach (KeyValuePair<Character, EnemyHud.HudData> keyValuePair in __instance.m_huds)
                {
                    EnemyHud.HudData value = keyValuePair.Value;

                    if (!value.m_character || !__instance.TestShow(value.m_character))
                    {
                        if (character == null)
                        {
                            character = value.m_character;
                            Object.Destroy(value.m_gui);
                        }
                    }
                    else
                    {
                        if (value.m_character.IsPlayer())
                        {
                            UpdatePlayerHealthBar(value);
                            UpdatePlayerStaminaBar(value);
                        }
                    }
                }

                if (character != null)
                {
                    __instance.m_huds.Remove(character);
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.SetMaxStamina))]
        public static class Player_Awake_Postfix
        {
            public static void Postfix(Player __instance)
            {
                __instance.m_nview.GetZDO()?.Set("maxStamina", __instance.m_maxStamina);
            }
        }

        [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.LateUpdate))]
        public static class EnemyHud_LateUpdate_Transpiler
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    .MatchForward(
                        useEnd: false,
                        new CodeMatch(OpCodes.Stloc_3),
                        new CodeMatch(OpCodes.Ldloc_3),
                        new CodeMatch(OpCodes.Ldloc_1),
                        new CodeMatch(OpCodes.Call))
                    .Advance(offset: 3)
                    .SetInstructionAndAdvance(Transpilers.EmitDelegate<Func<Character, Player, bool>>(CharacterLocalPlayerEqualityDelegate))
                    .InstructionEnumeration();
            }  
        }
 

        public static bool CharacterLocalPlayerEqualityDelegate(Character character, Player player)
        {
            return false;
        }
    }
}
