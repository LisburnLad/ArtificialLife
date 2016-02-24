using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class NodeConnection : NodeBase
  {
    public NodeConnection(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs)
      : base(aCellType, aRow, aCol, aNodeInputs)
    {

    }

    public override bool NorthInput
    {
      get { return itsNorthInput; }
      set 
      {
        itsInputDirection = DirectionFlag.North;
        itsNorthInput = value;
        ChangeOutput();
      }
    }
    
    public override bool EastInput
    {
      get { return itsEastInput; }
      set 
      {
        itsInputDirection = DirectionFlag.East;
        itsEastInput = value;
        ChangeOutput();
      }
    }

    public override bool SouthInput
    {
      get { return itsSouthInput; }
      set 
      {
        itsInputDirection = DirectionFlag.South;
        itsSouthInput = value;
        ChangeOutput();
      }
    }

    public override bool WestInput
    {
      get { return itsWestInput; }
      set 
      {
        itsInputDirection = DirectionFlag.West;
        itsWestInput = value;
        ChangeOutput();
      }
    }


    /// <summary>
    /// change the connection value if any of the inputs have changed
    /// </summary>
    private void ChangeOutput()
    {
      bool inputSum = false;
      
      // for a connection the output is set if any of the inputs are set
      // - additionally the output directions correspond to the input directions

      if( itsOutputDirection.HasFlag( DirectionFlag.North ))
      {
        inputSum |= itsNorthInput;
      }

      if (itsOutputDirection.HasFlag(DirectionFlag.East))
      {
        inputSum |= itsEastInput;
      }

      if (itsOutputDirection.HasFlag(DirectionFlag.South))
      {
        inputSum |= itsSouthInput;
      }

      if (itsOutputDirection.HasFlag(DirectionFlag.West))
      {
        inputSum |= itsWestInput;
      }

      // set the output of the connection
      Output = (inputSum? 1 : 0);
    }
  }
}
