using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class NodeOr : NodeBase
  {
    public NodeOr(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs)
      : base(aCellType, aRow, aCol, aNodeInputs)
    {

    }

    /// <summary>
    /// OR gate output = 1, if any of the inputs are 1
    /// - don't include sides with no input in the calculation    
    /// </summary>
    protected override void CalculateOutput()
    {     
      // if the node has no inputs at all then its output is false
      if (itsInputConnections == DirectionFlag.None)
      {
        Output = 0;
      }
      else
      {
        bool inputSum = false;

        if (itsInputConnections.HasFlag(DirectionFlag.North))
        {
          inputSum |= itsNorthInput;
        }

        if (itsInputConnections.HasFlag(DirectionFlag.East))
        {
          inputSum |= itsEastInput;
        }

        if (itsInputConnections.HasFlag(DirectionFlag.South))
        {
          inputSum |= itsSouthInput;
        }

        if (itsInputConnections.HasFlag(DirectionFlag.West))
        {
          inputSum |= itsWestInput;
        }

        // set the output of the gate        
        Output = (inputSum ? 1 : 0);
      }
    }
  }
}
