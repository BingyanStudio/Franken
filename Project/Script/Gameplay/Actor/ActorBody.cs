using System.Collections.Generic;
using System.Linq;

namespace Franken;

public class ActorBody
{
    public ActorStats Stats { get; set; }

    public ActorBodyPart[] Parts { get; set; }

    // 感觉这里性能并不吃紧，所以直接查好了
    public ActorBodyPart Head => Parts.First(part => part.Comp == ActorBodyPart.Component.Head);
    public ActorBodyPart Heart => Parts.First(part => part.Comp == ActorBodyPart.Component.Heart);
    public ActorBodyPart Torso => Parts.First(part => part.Comp == ActorBodyPart.Component.Torso);
    public IEnumerable<ActorBodyPart> Limbs => Parts.Where(part => part.Comp == ActorBodyPart.Component.Limb);
}