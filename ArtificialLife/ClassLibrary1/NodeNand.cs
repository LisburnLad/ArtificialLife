using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class NodeNand : NodeBase
  {
    public NodeNand(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs)
      : base(aCellType, aRow, aCol, aNodeInputs)
    {

    }

    /// <summary>
    /// NAND gate output = 1, unless all 3 inputs are 1
    /// - don't include sides with no input in the calculation    
    /// </summary>
    protected override void CalculateOutput()
    {     
      // if the node has no inputs at all then its output is true
      if (itsInputConnections == DirectionFlag.None)
      {
        Output = 1;
      }
      else
      {
        bool inputSum = true;

        if (itsInputConnections.HasFlag(DirectionFlag.North))
        {
          inputSum &= itsNorthInput;
        }

        if (itsInputConnections.HasFlag(DirectionFlag.East))
        {
          inputSum &= itsEastInput;
        }

        if (itsInputConnections.HasFlag(DirectionFlag.South))
        {
          inputSum &= itsSouthInput;
        }

        if (itsInputConnections.HasFlag(DirectionFlag.West))
        {
          inputSum &= itsWestInput;
        }

        // set the output of the connection
        // - if the input sum is still true then all inputs must have been true so set the output false
        Output = (inputSum?0:1);
      }
    }
  }
}
