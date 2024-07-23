using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.Json.Nodes;
using static game_tools.Helpers.DataInitializer;

namespace game_tools.Helpers
{
    /// <summary>
    /// 初始化必要数据
    /// </summary>
    public static class DataInitializer
    {
        public class ScreenLocations
        {
            public string? Name { get; set; } //标记这些参数适应哪一个分辨率
            public List<Detail>? Details { get; set; }
        }

        public class Detail
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int StratX { get; set; }
            public int EndX { get; set; }
        }


        /// <summary>
        /// 初始化词条数据
        /// </summary>
        public static List<EquipmentItems> initEquitpmentItems()
        {
            List<EquipmentItems> equipmentItems = new List<EquipmentItems>();
            equipmentItems.Add(new EquipmentItems() { subString = "物品强度", Name = "Strength" });
            equipmentItems.Add(new EquipmentItems() { subString = "护甲值", Name = "Armor" });
            equipmentItems.Add(new EquipmentItems() { subString = "生命上限", Name = "MaximumHealth" });
            equipmentItems.Add(new EquipmentItems() { subString = "暴击几率", Name = "CriticalChance" });
            equipmentItems.Add(new EquipmentItems() { subString = "暴击伤害", Name = "CriticalDamage" });
            equipmentItems.Add(new EquipmentItems() { subString = "全元素抗性", Name = "AllElementResistance" });
            equipmentItems.Add(new EquipmentItems() { subString = "攻击速度", Name = "AttackSpeed" });
            equipmentItems.Add(new EquipmentItems() { subString = "资源消耗减免", Name = "resourceReduction" });
            equipmentItems.Add(new EquipmentItems() { subString = "点智力", Name = "Intelligence" });
            equipmentItems.Add(new EquipmentItems() { subString = "幸运一击几率", Name = "CriticalHitChance" });
            equipmentItems.Add(new EquipmentItems() { subString = "易伤伤害", Name = "IncreasedDamageTaken" });
            equipmentItems.Add(new EquipmentItems() { subString = "压制伤害", Name = "DamageSuppression" });
            equipmentItems.Add(new EquipmentItems() { subString = "持续性伤害", Name = "DamageOverTime" });
            equipmentItems.Add(new EquipmentItems() { subString = "冷却时间缩减", Name = "CDR" });
            equipmentItems.Add(new EquipmentItems() { subString = "移动速度", Name = "MovementSpeed" });

            //equipmentItems.Add(new EquipmentItems() { subString = "伤害", Name = "Damage" });

            return equipmentItems;
        }

        public static void InitScreenPotions()
        {
            Console.WriteLine("初始化自动化数据");

            //1269 723
            //11
            //3 
            //1874 964
            int startX = 1269;
            int startY = 723;
            int endX = 1874;
            int endY = 964;
            int width = (endX - startX) / 11;
            int height = (endY - startY) / 3;
            List<Detail> details = new List<Detail>();
            int widthEquipment = 1240 - 871;
            int widthOffset = 52;
            int startWidth = 0;
            int startHeight = 0;
            int countX = 0;
            int countY = 0;
            while (countY < 3)
            {
                startHeight += height / 2;
                while (countX < 11)
                {
                    startWidth += width / 2;

                    details.Add(new Detail()
                    {
                        X = startX + startWidth,
                        Y = startY + startHeight,
                        StratX = startX + startWidth - widthEquipment - widthOffset - 12,
                        EndX = startX + startWidth - widthOffset + 4
                    });
                    countX++;
                    startWidth += width / 2;
                }
                startHeight += height / 2;
                countY++;
                countX = 0;
                startWidth = 0;
            }

            JObject jsonObject = new JObject();
            // 将列表对象转换为 JArray
            JArray jsonArray = JArray.FromObject(details);
            jsonObject["1080"] = jsonArray;
            // 将 JSON 字符串写入本地文件
            File.WriteAllText("screenPotions.json", jsonObject.ToString());
        }

        /// <summary>
        /// 加载json数据
        /// </summary>
        /// <returns></returns>
        public static JObject GetScreenPotions()
        {
            if (File.Exists("screenPotions.json"))
            {
                // 读取 JSON 文件内容
                string json = File.ReadAllText("screenPotions.json");

                JObject obj = JObject.Parse(json);

                return obj;
            }
            else
            {
                // 如果文件不存在，返回默认值或者空对象
                return new JObject();
            }
        }
    }
}
