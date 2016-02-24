using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class NodeXor : NodeBase
  {
    public NodeXor(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs)
      : base(aCellType, aRow, aCol, aNodeInputs)
    {

    }

    /// <summary>
    /// XOR gate output = 0, if all of the inputs are the same (either 0 or 1)
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
        

        int numberOfInputs = 0;
        bool[] inputs = new bool[4];

        if (itsInputConnections.HasFlag(DirectionFlag.North))
        {
          inputs[numberOfInputs++] = itsNorthInput;
        }

        if (itsInputConnections.HasFlag(DirectionFlag.East))
        {
          inputs[numberOfInputs++] = itsEastInput;
        }

        if (itsInputConnections.HasFlag(DirectionFlag.South))
        {
          inputs[numberOfInputs++] = itsSouthInput;
        }

        if (itsInputConnections.HasFlag(DirectionFlag.West))
        {
          inputs[numberOfInputs++] = itsWestInput;
        }


        // if any of the inputs are different then the output is true
        bool inputSum = false;
        for (int index = 1; index < numberOfInputs; index++ )
        {
          if( inputs[index] != inputs[index-1] )
          {
            inputSum = true;
            break;
          }
        }

        // set the output of the gate        
        Output = (inputSum ? 1 : 0);
      }
    }
  }
}
