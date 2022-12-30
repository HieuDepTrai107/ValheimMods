using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static UnityEngine.RectTransform;
using Object = UnityEngine.Object;
using Log = Jotunn.Logger;

namespace TradingSkill
{
	public class StoreGuiPatch
	{
		[HarmonyPatch(typeof(StoreGui), nameof(StoreGui.Awake))]
		public static class StoreGui_Awake_Postfix
		{
			public static void Postfix(StoreGui __instance)
			{
				Log.LogWarning("Awake");

				RectTransform transform = __instance.m_rootPanel.transform.Find("SellPanel") as RectTransform;
				transform.anchoredPosition = new Vector2(780f, 0f);
				transform.SetAsFirstSibling();
				transform.gameObject.SetActive(false);

				SellingPanel.SetupSellingPanel(__instance);
			}
		}

		[HarmonyPatch(typeof(StoreGui), nameof(StoreGui.Update))]
		public static class StoreGui_Update_Postfix
		{
			public static void Postfix(StoreGui __instance)
			{
				if (__instance.m_rootPanel.activeSelf)
                {
					SellingPanel.UpdateSellingPanel(__instance);
				}
			}
		}

		[HarmonyPatch(typeof(StoreGui), nameof(StoreGui.Show))]
		public static class StoreGui_Show_Postfix
		{
			public static void Postfix(StoreGui __instance, Trader trader)
			{
				__instance.m_rootPanel.transform.parent.Find("SellingPanel").gameObject.SetActive(true);
			}
		}

		[HarmonyPatch(typeof(StoreGui), nameof(StoreGui.Hide))]
		public static class StoreGui_Hide_Postfix
		{
			public static void Postfix(StoreGui __instance)
			{
				__instance.m_rootPanel.transform.parent.Find("SellingPanel").gameObject.SetActive(false);
			}
		}

		[HarmonyPatch(typeof(StoreGui), nameof(StoreGui.FillList))]
        public static class StoreGui_FillList_Prefix
        {
            public static bool Prefix(StoreGui __instance)
            {
				FillBuyableList(__instance);
				FillSellableList(__instance);

				return false;
			}
        }

		public static void FillSellableList(StoreGui __instance)
		{
			Log.LogWarning("FillSellableList");
			int playerCoins = __instance.GetPlayerCoins();
			int num = __instance.GetSelectedItemIndex();

			// float num2 = Buying.Count * __instance.m_itemSpacing;
			RectTransform rectTransform = __instance.m_rootPanel.transform.parent.Find("SellingPanel").Find("SellingItemListRoot") as RectTransform;
			// rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, num2);


			for (int i = 0; i < __instance.m_trader.m_items.Count; i++)
			{
				ItemDrop.ItemData item = __instance.m_trader.m_items[i].m_prefab.GetComponent<ItemDrop>().m_itemData;

				GameObject element = Object.Instantiate(__instance.m_listElement, rectTransform);
				element.SetActive(true);

				RectTransform transform = element.transform as RectTransform;
				transform.anchoredPosition = new Vector2(0f, i * (0f - __instance.m_itemSpacing));

				Image component = element.transform.Find("icon").GetComponent<Image>();
				component.sprite = item.m_shared.m_icons[0];
				component.color = Color.white;

				string text = Localization.instance.Localize(item.m_shared.m_name);
				if (item.m_stack > 1)
				{
					text = text + " x" + item.m_stack;
				}

				Text component2 = element.transform.Find("name").GetComponent<Text>();
				component2.text = text;
				component2.color = Color.white;

				UITooltip component3 = element.GetComponent<UITooltip>();
				component3.m_topic = item.m_shared.m_name;
				component3.m_text = item.GetTooltip();

				Text component4 = Utils.FindChild(element.transform, "price").GetComponent<Text>();
				component4.text = TradingSkill.GetNewPrice(100).ToString();
			}
		}
		public static void FillBuyableList(StoreGui __instance)
        {
			int playerCoins = __instance.GetPlayerCoins();
			int num = __instance.GetSelectedItemIndex();

			List<Trader.TradeItem> availableItems = __instance.m_trader.GetAvailableItems();
			foreach (GameObject item in __instance.m_itemList)
			{
				Object.Destroy(item);
			}

			__instance.m_itemList.Clear();
			float num2 = availableItems.Count * __instance.m_itemSpacing;
			num2 = Mathf.Max(__instance.m_itemlistBaseSize, num2);
			__instance.m_listRoot.SetSizeWithCurrentAnchors(Axis.Vertical, num2);

			for (int i = 0; i < availableItems.Count; i++)
			{
				Trader.TradeItem tradeItem = availableItems[i];

				GameObject element = Object.Instantiate(__instance.m_listElement, __instance.m_listRoot);
				element.SetActive(true);

				RectTransform transform = element.transform as RectTransform;
				transform.anchoredPosition = new Vector2(0f, i * (0f - __instance.m_itemSpacing));

				bool canOfford = TradingSkill.GetNewPrice(tradeItem.m_price) <= playerCoins;
				bool hasRequiredGlobalKey = HasRequiredGlobalKey(tradeItem);
				bool hasEnoughLevel = HasEnoughLevel(tradeItem);

				Image component = element.transform.Find("icon").GetComponent<Image>();
				component.sprite = tradeItem.m_prefab.m_itemData.m_shared.m_icons[0];
				component.color = canOfford ? Color.white : new Color(1f, 0f, 1f, 0f);
				if (!canOfford || !hasRequiredGlobalKey || !hasEnoughLevel)
				{
					component.color = new Color(1f, 0f, 1f, 0f);
				}

				string text = Localization.instance.Localize(tradeItem.m_prefab.m_itemData.m_shared.m_name);
				if (tradeItem.m_stack > 1)
				{
					text = text + " x" + tradeItem.m_stack;
				}

				Text component2 = element.transform.Find("name").GetComponent<Text>();
				component2.text = text;
				component2.color = Color.white;
				if (!canOfford || !hasRequiredGlobalKey || !hasEnoughLevel)
				{
					component2.color = Color.grey;
				}

				UITooltip component3 = element.GetComponent<UITooltip>();
				component3.m_topic = tradeItem.m_prefab.m_itemData.m_shared.m_name;
				component3.m_text = tradeItem.m_prefab.m_itemData.GetTooltip();
				component3.m_text += $"\n<color=orange>GlobalKey: {tradeItem.m_requiredGlobalKey}</color>";
				component3.m_text += $"\n<color=orange>Скидка: {Math.Round(TradingSkill.GetDiscount() * 100f, 2)}%</color>";


				Text component4 = Utils.FindChild(element.transform, "price").GetComponent<Text>();
				component4.text = TradingSkill.GetNewPrice(tradeItem.m_price).ToString();
				if (!canOfford || !hasRequiredGlobalKey || !hasEnoughLevel)
				{
					component4.color = Color.grey;
				}

				element.GetComponent<Button>().onClick.AddListener(delegate
				{
					__instance.OnSelectedItem(element);
				});

				__instance.m_itemList.Add(element);
			}

			if (num < 0)
			{
				num = 0;
			}

			__instance.SelectItem(num, center: false);
		}


		[HarmonyPatch(typeof(StoreGui), nameof(StoreGui.UpdateBuyButton))]
		public static class StoreGui_UpdateBuyButton_Prefix
		{
			public static bool Prefix(StoreGui __instance)
			{
				UITooltip component = __instance.m_buyButton.GetComponent<UITooltip>();
				if (__instance.m_selectedItem != null)
				{
					bool canOfford = TradingSkill.GetNewPrice(__instance.m_selectedItem.m_price) <= __instance.GetPlayerCoins();
					bool hasRequiredGlobalKey = HasRequiredGlobalKey(__instance.m_selectedItem);
					bool hasEnoughLevel = HasEnoughLevel(__instance.m_selectedItem);
					bool hasEmptySlot = Player.m_localPlayer.GetInventory().HaveEmptySlot();

					__instance.m_buyButton.interactable = canOfford && hasRequiredGlobalKey && hasEnoughLevel && hasEmptySlot;
					
					if (!canOfford)
					{
						component.m_text = Localization.instance.Localize("$msg_missingrequirement");
					}
					else if (!hasRequiredGlobalKey)
                    {
						component.m_text = Localization.instance.Localize("hasRequiredGlobalKey");
					}
					else if (!hasEnoughLevel)
					{
						component.m_text = Localization.instance.Localize("Недостаточный уровень торговли");
					}
					else if (!hasEmptySlot)
					{
						component.m_text = Localization.instance.Localize("$inventory_full");
					}
					else
					{
						component.m_text = "";
					}
				}
				else
				{
					__instance.m_buyButton.interactable = false;
					component.m_text = "";
				}

				return false;
			}
		}


		public static bool HasRequiredGlobalKey(Trader.TradeItem item)
		{
			if (string.IsNullOrEmpty(item.m_requiredGlobalKey)) return true;

			string requiredGlobalKey = item.m_requiredGlobalKey.Contains(",") ? item.m_requiredGlobalKey.Split(',')[0] : item.m_requiredGlobalKey;
			return ZoneSystem.instance.GetGlobalKey(requiredGlobalKey);
		}

		public static bool HasEnoughLevel(Trader.TradeItem item)
		{
			if (string.IsNullOrEmpty(item.m_requiredGlobalKey)) return true;

			int requiredSkillLevel = item.m_requiredGlobalKey.Contains(",") ? Convert.ToInt32(item.m_requiredGlobalKey.Split(',')[1]) : 0;
			return TradingSkill.GetTraderSkillLevel() >= requiredSkillLevel;
		}
	}
}
