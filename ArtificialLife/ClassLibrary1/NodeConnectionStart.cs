using Appccelerate.EventBroker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class NodeConnectionStart : NodeBase
  {
    public NodeConnectionStart(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs, int aTargetEndRow, int aTargetEndCol )
      : base(aCellType, aRow, aCol, aNodeInputs)
    {
      itsTargetRow = aTargetEndRow;
      itsTargetCol = aTargetEndCol;
    }

    public int itsTargetRow { get; set; }
    public int itsTargetCol { get; set; }

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
      Output = (inputSum? 1 : 0);
    }

    [EventPublication(ArtificialLifeProperties.NodeOutputChangeEvent)]
    public event EventHandler<NodeEventArgs> NodeOutputChangeEvent;

    /// <summary>
    /// Create the arguments and fire the event when a node output changes
    /// </summary>
    /// <param name="aRow">the row in the grid of the node firing the event</param>
    /// <param name="aCol">the col in the grid of the node firing the event</param>
    /// <param name="aDirection">the direction of the output of the node firing the event</param>
    /// <param name="aValue">the current output value of the node firing the event</param>
    protected override void FireOutputChangeEvent()
    {
      // if anyone has subscribed fire an event to show that the options value has been changed
      NodeEventArgs eventArgs = new NodeEventArgs(itsTargetRow, itsTargetCol, DirectionFlag.None, (Output>0), true, PropagationPassLevel);
      NodeOutputChangeEvent(this, eventArgs);
    }
  }
}
