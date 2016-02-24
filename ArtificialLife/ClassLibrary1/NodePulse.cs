using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class NodePulse : NodeBase
  {
    public NodePulse(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs)
      : base(aCellType, aRow, aCol, aNodeInputs)
    {

    }

    /// <summary>
    /// Pulse Node Output - when the input changes 0->1 the output will fire for a single propagation pass    
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
        bool currentOutput = (Output > 0);
        bool inputSum = false;

        // test if the node has a connection from the north
        if (itsInputConnections.HasFlag(DirectionFlag.North))
        {
          // has this input changed and become true
          if (itsNorthInputChanged && itsNorthInput)
          {
            itsNorthInputChanged = false;
            inputSum |= itsNorthInput;
          }
        }

        if (itsInputConnections.HasFlag(DirectionFlag.East))
        {
          if (itsEastInputChanged && itsEastInput)
          {
            itsEastInputChanged = false;
            inputSum |= itsEastInput;
          }
        }

        if (itsInputConnections.HasFlag(DirectionFlag.South))
        {
          if (itsSouthInputChanged && itsSouthInput)
          {
            itsSouthInputChanged = false;
            inputSum |= itsSouthInput;
          }
        }

        if (itsInputConnections.HasFlag(DirectionFlag.West))
        {
          if (itsWestInputChanged && itsWestInput)
          {
            itsWestInputChanged = false;
            inputSum |= itsWestInput;
          }
        }

        // set the output of the gate        
        if (currentOutput)
        {
          // if the output is currently 1 reset to 0
          Output = 0;
        }
        else
        {
          // otherwise make the output 1 if any of the inputs have gone from 0 -> 1
          Output = (inputSum ? 1 : 0);
        }

        // if the output has changed set the flag to indicate that the internal state has changed
        // - this then adds this node into the list of nodes to propagate on the next step and 
        // thereby ensures that the pulse only lasts for one step
        if( (Output>0) != currentOutput )
        {
          InternalStateChanged = true;
        }
      }
    }

  }
}
