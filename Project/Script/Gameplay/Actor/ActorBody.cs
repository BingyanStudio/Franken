using System.Collections.Generic;
using System.Linq;

namespace Franken;

public class ActorBody
{
    public ActorBodyData Data;

    private ActorStats stats;
    public ActorStats Stats => stats ??= MergeUtil.CalculateStats(Data.Parts.Select(ActorBodyPartStats.FromCSV));

    // 感觉这里性能并不吃紧，所以直接查好了
    public CSV.ActorBodyPart Head => Data.Parts.Select(CSV.ActorBodyPart.Get).First(part => part.Comp == CSV.ActorBodyPart.Component.Head);
    public CSV.ActorBodyPart Heart => Data.Parts.Select(CSV.ActorBodyPart.Get).First(part => part.Comp == CSV.ActorBodyPart.Component.Heart);
    public CSV.ActorBodyPart Torso => Data.Parts.Select(CSV.ActorBodyPart.Get).First(part => part.Comp == CSV.ActorBodyPart.Component.Torso);
    public IEnumerable<CSV.ActorBodyPart> Limbs => Data.Parts.Select(CSV.ActorBodyPart.Get).Where(part => part.Comp == CSV.ActorBodyPart.Component.Limb);
}