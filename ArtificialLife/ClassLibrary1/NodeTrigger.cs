using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class NodeTrigger : NodeBase
  {
    public NodeTrigger(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs)
      : base(aCellType, aRow, aCol, aNodeInputs)
    {

    }

    /// <summary>
    /// toggle the output each time an input state changes from 0->1
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
        bool currentOutput = (Output > 0);

        if( Output == -1)
        {
          // if the output is currently uninitialized set the output to be the value of the central input
          switch (itsOutputDirection)
          {
            case DirectionFlag.North: currentOutput = itsSouthInput; break;
            case DirectionFlag.East: currentOutput = itsWestInput; break;
            case DirectionFlag.South: currentOutput = itsNorthInput; break;
            case DirectionFlag.West: currentOutput = itsEastInput; break;
          }
        }

        bool inputChanged = false;

        // test if the node has a connection from the north
        if (itsInputConnections.HasFlag(DirectionFlag.North) )
        {          
          // has this input changed
          if(itsNorthInputChanged)
          {
            itsNorthInputChanged = false;

            // only inputs to the side of the node control if the output of the trigger should change
            // - the actual output value of the node is given by the central input
            if (itsOutputDirection != DirectionFlag.South)
            {
              inputChanged = true;
            }
          }

          // for a node with a southerly output its actual value is given by the north input
          if (itsOutputDirection == DirectionFlag.South && itsNorthInput)
          {
            currentOutput = true;
          }
        }

        if (itsInputConnections.HasFlag(DirectionFlag.East))
        {
          if(itsEastInputChanged )
          {
            itsEastInputChanged = false;

            if (itsOutputDirection != DirectionFlag.West)
            {
              inputChanged = true;
            }
          }

          if (itsOutputDirection == DirectionFlag.West && itsEastInput)
          {
            currentOutput = true;
          }
        }

        if (itsInputConnections.HasFlag(DirectionFlag.South))
        {
          if (itsSouthInputChanged)
          {
            itsSouthInputChanged = false;

            if (itsOutputDirection != DirectionFlag.North)
            {
              inputChanged = true;
            }
          }

          if (itsOutputDirection == DirectionFlag.North && itsSouthInput)
          {
            currentOutput = true;
          }
        }

        if (itsInputConnections.HasFlag(DirectionFlag.West) )
        {
          if(itsWestInputChanged)
          {
            itsWestInputChanged = false;

            if (itsOutputDirection != DirectionFlag.East)
            {
              inputChanged = true;
            }
          }

          if (itsOutputDirection == DirectionFlag.East && itsWestInput)
          {
            currentOutput = true;
          }
        }


        if( inputChanged )
        {
          switch( itsOutputDirection)
          {
            case DirectionFlag.North: currentOutput = itsSouthInput; break;
            case DirectionFlag.East: currentOutput = itsWestInput; break;
            case DirectionFlag.South: currentOutput = itsNorthInput; break;
            case DirectionFlag.West: currentOutput = itsEastInput; break;
          }
        }

        // set the output of the gate        
        Output = (currentOutput ? 1 : 0);
      }
    }
  }
}
