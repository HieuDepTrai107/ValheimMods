using Jotunn.Configs;
using Jotunn.Managers;
using Jotunn.Utils;

namespace TradingSkill
{
    public class TradingSkill
    {
        public static Skills.SkillType _TradingSkill;

        public static void AddToSkills()
        {
            _TradingSkill = SkillManager.Instance.AddSkill(new SkillConfig
            {
                Identifier = "blumaye.trading.skill",
                Name = "Торговля",
                Description = "Торговля",
                Icon = AssetUtils.LoadSpriteFromFile("SellThat/Assets/TradingSkill.png"),
                IncreaseStep = 1f
            });
        }

        public static void RaiseSkill(float value)
        {
            Player.m_localPlayer.RaiseSkill(_TradingSkill, value);
        }

        public static Skills.Skill GetTraderSkill()
        {
            return Player.m_localPlayer.m_skills.GetSkill(_TradingSkill);
        }

        // Возвращает уровень навыка торговли (число от 0 до 100)
        public static float GetTraderSkillLevel()
        {
            var skill = GetTraderSkill();
            return skill.m_level + skill.GetLevelPercentage() / 100;
        }

        // Возвращает скидку (число от 0 до 1)
        public static float GetDiscount()
        {
            return (Main.maxDiscount.Value / 100f) * (GetTraderSkillLevel() / 100f);
        }

        // Возвращает новую цену с учётом скидки
        public static int GetNewPrice(int price)
        {
            var newPrice = price * (1 - GetDiscount());
            return (int)(newPrice);
        }

        // Полученный опыт при продаже вещей торговцу
        public static float GetSellXP(ItemDrop.ItemData sellableItem, StoreGui store)
        {
            var amount = sellableItem.m_stack;
            var price = sellableItem.m_shared.m_value;
            return amount * price * Main.sellExpMultiplier.Value;
        }

        // Полученный опыт при покупке вещей у торговца
        public static float GetBuyXP(Trader.TradeItem item)
        {
            var amount = item.m_stack;
            var price = GetNewPrice(item.m_price);
            return amount * price * Main.buyExpMultiplier.Value;
        }
    }
}
