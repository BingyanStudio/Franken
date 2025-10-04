using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Franken;

public class AssignInitalPtCommand : ICommand
{
    void ICommand.Execute(BattleStats target) => target.AssignInitialPt();
}
