using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class NodeConnectionEnd : NodeBase
  {
    public NodeConnectionEnd(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs, Direction aOutputDirection)
      : base(aCellType, aRow, aCol, aNodeInputs)
    {
      switch(aOutputDirection)
      {
        case Direction.North: itsOutputDirection = DirectionFlag.North; break;
        case Direction.East: itsOutputDirection = DirectionFlag.East; break;
        case Direction.South: itsOutputDirection = DirectionFlag.South; break;
        case Direction.West: itsOutputDirection = DirectionFlag.West; break;
      }
    }
  }
}
