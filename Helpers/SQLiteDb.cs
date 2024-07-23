using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;

namespace game_tools.Helpers
{
    public class SQLiteDb : DbContext
    {
        public SQLiteDb(DbContextOptions<SQLiteDb> options) : base(options) { }

        public DbSet<Equipments> Equipments { get; set; }

        //public DbSet<EquipmentItem> EquipmentItem { get; set; }
    }

    /// <summary>
    /// 装备信息表
    /// </summary>
    public class Equipments
    {
        public int Id { get; set; }
        public string? Name { get; set; }//装备名字
        public string? Place { get; set; }//部位
        public string? Strength { get; set; }//物品强度
        public string? Armor { get; set; }//护甲值
        public string? MaximumHealth { get; set; }//生命上限
        public string? CriticalChance { get; set; }//暴击率
        public string? CriticalDamage { get; set; }//暴击伤害
        public string? SpecialEffectName { get; set; }//特效名字
        public string? SpecialEffectDescription { get; set; }//特效描述
        public string? AmazingPower { get; set; } //威能
        public string? Needlevel { get; set; } //需要等级
        public string? Traded { get; set; } //账号绑定
        public string ImagePath { get; set; } //图片路径
        public Int16? ItemIndex { get; set; }//物品在背包里的位置从1-33

        public string? AllElementResistance { get; set; } //全元素抗性

        public string? AttackSpeed { get; set; }  // 攻击速度

        public string? ResourceReduction { get; set; }  // 资源消耗减免

        public string? Intelligence { get; set; }  // 点智力

        public string? CriticalHitChance { get; set; }  // 幸运一击几率

        public string? IncreasedDamageTaken { get; set; }  // 易伤伤害


        public string? DamageSuppression { get; set; }  // 压制伤害


        public string? DamageOverTime { get; set; }  // 持续性伤害


        public string? CDR { get; set; }  // 冷却时间缩减


        public string? Damage { get; set; }  // 伤害

        public string? MovementSpeed { get; set; }  // 移动速度

        public int? TaskBatch { get; set; }  // 任务批次

        public DateTime CreatedAt { get; set; } = DateTime.Now; //当前时间

    }

    /// <summary>
    /// 词条信息表
    /// </summary>
    public class EquipmentItems
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string subString { get; set; }
    }

}
