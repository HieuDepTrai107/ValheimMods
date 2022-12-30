using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;
using Object = UnityEngine.Object;
using Log = Jotunn.Logger;
using Jotunn.Managers;

namespace TradingSkill
{
	public class SellingPanel
    {
		public static Transform panel;

		public static void SetupSellingPanel(StoreGui __instance)
		{
			Log.LogWarning("SetupSellingPanel");

			CreatePanel(__instance);
			CreateItemList(__instance);
			CreateScrollBar(__instance);
		}
		
		public static void UpdateSellingPanel(StoreGui __instance)
		{
			// RectTransform rectTransform1 = __instance.m_rootPanel.transform.Find("SellingPanel") as RectTransform;
			// rectTransform1.anchoredPosition = new Vector2(SellThatPlugin.a.Value, SellThatPlugin.b.Value);
			// rectTransform1.SetSizeWithCurrentAnchors(Axis.Horizontal, SellThatPlugin.c.Value);
			// rectTransform1.SetSizeWithCurrentAnchors(Axis.Vertical, SellThatPlugin.d.Value);

			RectTransform rectTransform1 = __instance.m_rootPanel.transform.parent.Find("SellingPanel") as RectTransform;
			rectTransform1.anchoredPosition = new Vector2(Main.a.Value, Main.b.Value);

			RectTransform rectTransform2 = __instance.m_rootPanel.transform.parent.Find("SellingPanel").Find("SellingItemListRoot") as RectTransform;
			rectTransform2.anchoredPosition = new Vector2(Main.c.Value, Main.d.Value);
			// rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, Main.c.Value);
			// rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, Main.d.Value);
		}

		public static void CreatePanel(StoreGui __instance)
		{
			GameObject gameObject = Object.Instantiate(__instance.m_rootPanel.gameObject, __instance.m_rootPanel.transform.parent);
			gameObject.transform.name = "SellingPanel";

			Object.DestroyImmediate(gameObject.transform.Find("ItemList").gameObject);

			RectTransform rectTransform = gameObject.transform as RectTransform;
			rectTransform.anchoredPosition = new Vector2(500f, -180f);
		}

		public static void CreateItemList(StoreGui __instance)
		{
			GameObject listRoot = new GameObject("SellingItemListRoot", typeof(RectTransform));
			listRoot.transform.SetParent(__instance.m_rootPanel.transform.parent.Find("SellingPanel"), false);

			RectTransform rectTransform = listRoot.GetComponent<RectTransform>();
			rectTransform.anchoredPosition = new Vector2(-110f, 90f);
			rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, 200f);
			rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, 300f);
		}

		public static void CreateScrollBar(StoreGui __instance)
		{
			// GameObject gameObject = GUIManager.Instance.CreateScrollView(__instance.m_rootPanel.transform.parent.Find("SellingPanel"), false, true, 50f, 5f, ColorBlock.defaultColorBlock, Color.green, 10f, 100f);

			Transform parentTransform = __instance.m_rootPanel.transform.parent.Find("SellingPanel").Find("SellingItemListRoot");


			DefaultControls.Resources resources = new()
			{
				standard = GUIManager.Instance.GetSprite("UISprite")
			};

			Scrollbar scrollbar = DefaultControls.CreateScrollbar(resources).GetComponent<Scrollbar>();
			scrollbar.transform.SetParent(parentTransform, worldPositionStays: false);

			scrollbar.GetComponent<RectTransform>()
				.SetAnchorMin(Vector2.right)
				.SetAnchorMax(Vector2.one)
				.SetPivot(Vector2.one)
				.SetPosition(Vector2.zero)
				.SetSizeDelta(new(10f, 0f));

			scrollbar.direction = Scrollbar.Direction.BottomToTop;
			scrollbar.GetComponent<Image>().SetColor(new(0f, 0f, 0f, 0.6f));
			scrollbar.handleRect.GetComponent<Image>().SetColor(new(1f, 1f, 1f, 0.9f));

			parentTransform.GetComponent<RectTransform>().OffsetSizeDelta(new(10f, 0f));

			ScrollRect scrollRect = parentTransform.gameObject.AddComponent<ScrollRect>();
			scrollRect.content = parentTransform as RectTransform;
			scrollRect.viewport = parentTransform.GetComponent<RectTransform>();
			scrollRect.verticalScrollbar = scrollbar;
			scrollRect.movementType = ScrollRect.MovementType.Clamped;
			scrollRect.inertia = false;
			scrollRect.scrollSensitivity = 1f;
			scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
		}
	}
}
