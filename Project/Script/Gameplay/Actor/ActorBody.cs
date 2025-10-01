using System.Collections.Generic;
using System.Linq;

namespace Franken;

public class ActorBody
{

    public ActorStats Stats { get; set; }

    public CSV.ActorBodyPart[] Parts { get; set; }

    // 感觉这里性能并不吃紧，所以直接查好了
    public CSV.ActorBodyPart Head => Parts.First(part => part.Comp == CSV.ActorBodyPart.Component.Head);
    public CSV.ActorBodyPart Heart => Parts.First(part => part.Comp == CSV.ActorBodyPart.Component.Heart);
    public CSV.ActorBodyPart Torso => Parts.First(part => part.Comp == CSV.ActorBodyPart.Component.Torso);
    public IEnumerable<CSV.ActorBodyPart> Limbs => Parts.Where(part => part.Comp == CSV.ActorBodyPart.Component.Limb);
}