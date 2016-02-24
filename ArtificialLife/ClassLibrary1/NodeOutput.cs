using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  /// <summary>
  /// Base class for a bot node
  /// </summary>
  public class NodeOutput : NodeBase
  {
    public NodeOutput(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs)
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
      inputSum |= itsNorthInput;
      inputSum |= itsEastInput;
      inputSum |= itsSouthInput;
      inputSum |= itsWestInput;

      // set the output of the connection
      Output = (inputSum ? 1 : 0); 
    }
  }
}
