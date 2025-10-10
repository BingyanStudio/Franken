using System.Collections.Generic;
using System.Linq;

namespace Franken;

public static class MergeUtil
{
    public static CSV.ActorBodyPart.Component EssentialComps =>
        CSV.ActorBodyPart.Component.Head |
        CSV.ActorBodyPart.Component.Heart |
        CSV.ActorBodyPart.Component.Torso |
        CSV.ActorBodyPart.Component.Limb;

    public static bool CanMerge(IEnumerable<CSV.ActorBodyPart> parts)
    {
        CSV.ActorBodyPart.Component comps = CSV.ActorBodyPart.Component.None;
        parts.ForEach(part => comps |= part.Comp);
        return comps.HasFlag(EssentialComps);
    }

    /// <summary>
    /// 将<see cref="CSV.ActorBodyPart"/>融合成<see cref="ActorBody"/>
    /// <br/>融合前记得判断<see cref="CanMerge(IEnumerable{ActorBodyPart})"/>
    /// </summary>
    public static ActorBodyData Merge(IEnumerable<string> parts, string name) => new()
    {
        Name = name,
        Parts = parts.ToList()
    };

    /// <summary>
    /// 统计所有加算乘算属性
    /// </summary>
    public static ActorStats CalculateStats(IEnumerable<ActorBodyPartStats> stats)
    {
        int addHp = 0, addSan = 0, addCmp = 0, addPti = 0, addPth = 0, addAtk = 0, addDef = 0, addAgi = 0;
        float mulHp = 0, mulSan = 0, mulCmp = 0, mulPti = 0, mulPth = 0, mulAtk = 0, mulDef = 0, mulAgi = 0;

        stats.ForEach(s =>
        {
            switch (s.Hp.Type)
            {
                case Number.ValueType.Int: addHp += s.Hp; break;
                case Number.ValueType.Float: mulHp += s.Hp; break;
            }
            switch (s.San.Type)
            {
                case Number.ValueType.Int: addSan += s.San; break;
                case Number.ValueType.Float: mulSan += s.San; break;
            }
            switch (s.Cmp.Type)
            {
                case Number.ValueType.Int: addCmp += s.Cmp; break;
                case Number.ValueType.Float: addCmp += s.Cmp; break;
            }
            switch (s.Pti.Type)
            {
                case Number.ValueType.Int: addPti += s.Pti; break;
                case Number.ValueType.Float: mulPti += s.Pti; break;
            }
            switch (s.Pth.Type)
            {
                case Number.ValueType.Int: addPth += s.Pth; break;
                case Number.ValueType.Float: mulPth += s.Pth; break;
            }
            switch (s.Def.Type)
            {
                case Number.ValueType.Int: addDef += s.Def; break;
                case Number.ValueType.Float: mulDef += s.Def; break;
            }
            switch (s.Agi.Type)
            {
                case Number.ValueType.Int: addAgi += s.Agi; break;
                case Number.ValueType.Float: mulAgi += s.Agi; break;
            }
        });

        return new()
        {
            //初始Hp=MaxHp
            MaxHp = (int)(addHp * (mulHp + 1)),
            Hp = (int)(addHp * (mulHp + 1)),

            San = (int)(addSan * (mulSan + 1)),
            Cmp = (int)(addCmp * (mulCmp + 1)),

            //初始Pt可以随便设置，但是先设置为Pti好
            Pt = (int)(addPti * (mulPti + 1)),
            Pti = (int)(addPti * (mulPti + 1)),
            Pth = (int)(addPth * (mulPth + 1)),

            Atk = (int)(addAtk * (mulAtk + 1)),
            Def = (int)(addDef * (mulDef + 1)),
            Agi = (int)(addAgi * (mulAgi + 1)),
        };
    }
}