using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;
using Object = UnityEngine.Object;
using Log = Jotunn.Logger;

namespace TradingSkill
{
	public class SellingPanel
    {
		public static Transform panel;

		public static void SetupSellingPanel(StoreGui __instance)
		{
			Log.LogWarning("SetupSellingPanel");

			panel = CreatePanel(__instance);
			CreateTopic(__instance);
			CreateButton(__instance);
			CreateItemList(__instance);
		}
		
		public static void UpdateSellingPanel(StoreGui __instance)
		{
			// RectTransform rectTransform1 = __instance.m_rootPanel.transform.Find("SellingPanel") as RectTransform;
			// rectTransform1.anchoredPosition = new Vector2(SellThatPlugin.a.Value, SellThatPlugin.b.Value);
			// rectTransform1.SetSizeWithCurrentAnchors(Axis.Horizontal, SellThatPlugin.c.Value);
			// rectTransform1.SetSizeWithCurrentAnchors(Axis.Vertical, SellThatPlugin.d.Value);

			// RectTransform rectTransform2 = rectTransform1.Find("SellingPanelTopic") as RectTransform;
			// rectTransform2.anchoredPosition = new Vector2(SellThatPlugin.e.Value, SellThatPlugin.f.Value);

			RectTransform rectTransform = __instance.m_rootPanel.transform.Find("SellingPanel").Find("SellingItemListRoot") as RectTransform;
			rectTransform.anchoredPosition = new Vector2(Main.a.Value, Main.b.Value);
			rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, Main.c.Value);
			rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, Main.d.Value);
		}

		public static Transform CreatePanel(StoreGui __instance)
        {
			Transform transform = Object.Instantiate(InventoryGui.instance.m_player.transform.Find("Bkg"), __instance.m_rootPanel.transform);
			transform.SetAsFirstSibling();
			transform.name = "SellingPanel";

			RectTransform rectTransform = transform as RectTransform;
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(0f, 1f);
			rectTransform.anchoredPosition = new Vector2(550f, -450f);
			rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, 300f);
			rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, 300f);

			return transform;
		}

		public static void CreateTopic(StoreGui __instance)
        {
			Transform transform = Object.Instantiate(__instance.m_rootPanel.transform.Find("topic"), panel);
			transform.name = "SellingPanelTopic";

			Text text = transform.GetComponent<Text>();
			text.text = "Selling";

			RectTransform rectTransform = transform as RectTransform;
			rectTransform.anchoredPosition = new Vector2(0f, -30f);
		}

		public static void CreateButton(StoreGui __instance)
        {
			Transform transform = Object.Instantiate(__instance.m_rootPanel.transform.Find("BuyButton"), panel);
			transform.name = "SellButton";
			
			Button button = transform.GetComponent<Button>();

			Text text = transform.GetComponentInChildren<Text>();
			text.text = "Sell";

			RectTransform rectTransform = transform as RectTransform;
			rectTransform.anchoredPosition = new Vector2(0f, 40f);
		}

		public static void CreateItemList(StoreGui __instance)
        {
			GameObject listRoot = new GameObject("SellingItemListRoot", typeof(RectTransform));
			listRoot.transform.SetParent(panel, false);

			RectTransform rectTransform = listRoot.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(-110f, 90f);
			rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, 200f);
		}

		public static void FillSellableList(StoreGui __instance)
		{
			Log.LogWarning("FillSellableList");
			int playerCoins = __instance.GetPlayerCoins();
			int num = __instance.GetSelectedItemIndex();

			float num2 = __instance.m_tempItems.Count * __instance.m_itemSpacing;
			num2 = Mathf.Max(200f, num2);
			RectTransform rectTransform = __instance.m_rootPanel.transform.Find("SellingPanel").Find("SellingItemListRoot") as RectTransform;
			rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, num2);


			foreach (var item in __instance.m_tempItems)
			{
				Log.LogWarning(item.m_dropPrefab.name);
			}


			for (int i = 0; i < __instance.m_tempItems.Count; i++)
			{
				ItemDrop.ItemData item = __instance.m_tempItems[i];

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
	}
}
