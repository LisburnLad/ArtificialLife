using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
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
  public class NodeBase
  {
    public NodeBase(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs)
    {
      // register this node with the system event broker to allow event publication and subscription
      //ArtificialLifeProperties.SystemEventBroker.Register(this);

      NodeCellType = aCellType;
      itsRow = aRow;
      itsCol = aCol;
      itsInputDirection = DirectionFlag.None;
      itsInputConnections = aNodeInputs;
      itsLastOutputChangeLevel = -1;
    }

    /// <summary>
    /// register this node with the bot's event broker to allow event publication and subscription
    /// </summary>
    /// <param name="aEventBroker"></param>
    public void RegisterEventBroker(EventBroker aEventBroker)
    {
      aEventBroker.Register(this);
    }

    /// <summary>
    /// the directions from which this node is receiving inputs
    /// </summary>
    public DirectionFlag itsInputConnections { get; set; }

    /// <summary>
    /// the 4 possible inputs for a node
    /// </summary>       
    protected bool itsNorthInputChanged = false;
    protected bool itsNorthInput;
    public virtual bool NorthInput 
    {
      get { return itsNorthInput; }
      set 
      {
        itsInputDirection = DirectionFlag.North;

        if( itsNorthInput != value )
        {
          itsNorthInputChanged = true;
        }

        itsNorthInput = value; 
      }
    }

    protected bool itsEastInputChanged = false;
    protected bool itsEastInput;
    public virtual bool EastInput 
    {
      get { return itsEastInput; }
      set 
      {
        itsInputDirection = DirectionFlag.East;

        if (itsEastInput != value)
        {
          itsEastInputChanged = true;
        }

        itsEastInput = value; 
      }
    }

    protected bool itsSouthInputChanged = false;
    protected bool itsSouthInput;
    public virtual bool SouthInput 
    {
      get { return itsSouthInput; }
      set 
      {
        itsInputDirection = DirectionFlag.South;

        if (itsSouthInput != value)
        {
          itsSouthInputChanged = true;
        }

        itsSouthInput = value; 
      }
    }

    protected bool itsWestInputChanged = false;
    protected bool itsWestInput;
    public virtual bool WestInput 
    {
      get { return itsWestInput; }
      set 
      {
        itsInputDirection = DirectionFlag.West;

        if (itsWestInput != value)
        {
          itsWestInputChanged = true;
        }

        itsWestInput = value; 
      }
    }

    /// <summary>
    /// the position of the node in the grid
    /// </summary>
    public int itsRow { get; set; }
    public int itsCol { get; set; }


    /// <summary>
    /// the calculated output of the node
    /// </summary>
    private int itsValue = -1;
    public virtual int Output
    {
      get
      {
        return itsValue;
      }
      set
      {
        int oldValue = itsValue;
        itsValue = value;

        // check if the value has changed
        if (oldValue != itsValue)
        {
          FireOutputChangeEvent();
        }        
      }
    }

    /// <summary>
    /// the last input direction of this node
    /// </summary>
    public DirectionFlag itsInputDirection { get; set; }


    /// <summary>
    /// set true when the internal state has changed
    /// - used by nodes that don't change their output immediately when an input changes
    /// </summary>
    private bool itsInternalStateChanged = false;
    public bool InternalStateChanged 
    { 
      get
      {
        return itsInternalStateChanged;
      }
      set
      {
        itsInternalStateChanged = value;
        
        if (itsInternalStateChanged )
        {
          FireInternalStateChangeEvent();
        }        
      }
    }

    /// <summary>
    /// the output directions of this node
    /// </summary>
    public DirectionFlag itsOutputDirection { get; set; }

    private CellType itsCellType;
    public CellType NodeCellType 
    { 
      get
      {
        return itsCellType;
      }
      set
      {
        // store the cell type
        itsCellType = value;

        // calculate the output directions for this cell type

        // test for a standard node
        if (itsCellType >= CellType.NorthNand && itsCellType < CellType.ConnectionStart)
        {          
          switch ((Direction)((int)itsCellType % 4))
          {
            case Direction.North: itsOutputDirection = DirectionFlag.North; break;
            case Direction.East: itsOutputDirection = DirectionFlag.East; break;
            case Direction.South: itsOutputDirection = DirectionFlag.South; break;
            case Direction.West: itsOutputDirection = DirectionFlag.West; break;
          }
        }
        else
        {
          itsOutputDirection = Common.GetDirectionFlag(itsCellType);
        }
      }
    }




    [EventPublication(ArtificialLifeProperties.NodeInternalStateChangeEvent)]
    public event EventHandler<NodeEventArgs> NodeInternalStateChangeEvent;

    /// <summary>
    /// Fire an event when a node's internal state changes
    /// </summary>
    protected virtual void FireInternalStateChangeEvent()
    {
      // only fire the input state change event if the output change hasn't fired already for this node on this propagation step
      //if (itsOutputChangeFired == false)
      {
        ArtificialLifeProperties.Log("Internal State Change: row = " + itsRow + " col = " + itsCol);

        NodeEventArgs eventArgs = new NodeEventArgs(itsRow, itsCol, PropagationPassLevel);

        // if anyone has subscribed fire an event to show that the options value has been changed
        NodeInternalStateChangeEvent(this, eventArgs);
      }
    }

    [EventPublication(ArtificialLifeProperties.NodeOutputChangeEvent)]
    public event EventHandler<NodeEventArgs> NodeOutputChangeEvent;

    //bool itsOutputChangeFired = false;

    public int itsLastOutputChangeLevel { get; set; }

    /// <summary>
    /// Create the arguments and fire the event when a node output changes
    /// </summary>
    /// <param name="aRow">the row in the grid of the node firing the event</param>
    /// <param name="aCol">the col in the grid of the node firing the event</param>
    /// <param name="aDirection">the direction of the output of the node firing the event</param>
    /// <param name="aValue">the current output value of the node firing the event</param>
    protected virtual void FireOutputChangeEvent()
    {
      DirectionFlag outputDirections = itsOutputDirection & ~itsInputDirection;

      if (outputDirections != DirectionFlag.None)
      {
        ArtificialLifeProperties.Log("Output State Change: row = " + itsRow + " col = " + itsCol + " value = " + Output);

        bool propagateNow = (itsLastOutputChangeLevel < PropagationPassLevel);
        itsLastOutputChangeLevel = PropagationPassLevel;

        // if anyone has subscribed fire an event to show that the options value has been changed
        NodeEventArgs eventArgs = new NodeEventArgs(itsRow, itsCol, outputDirections, (itsValue > 0), false, PropagationPassLevel);
        NodeOutputChangeEvent(this, eventArgs);

        //itsOutputChangeFired = true;
      }
    }


    protected virtual void CalculateOutput()
    {

    }

    //[EventSubscription(ArtificialLifeProperties.PropagateOutputEvent, typeof(OnPublisher))]
    //public void OnPropagateOutputEvent(object sender, EventArgs aArgs)
    //{
    //  //PropagateOutput();
    //}


    public int PropagationPassLevel { get; set; }

    public virtual bool PropagateOutput(int aPropagationPassLevel)
    {
      if (itsInputDirection != DirectionFlag.None || itsInternalStateChanged)
      {
        PropagationPassLevel = aPropagationPassLevel;

        ArtificialLifeProperties.Log("Propagate Node: row = " + itsRow + " col = " + itsCol + " dir = " + itsInputDirection + " state = " + itsInternalStateChanged);

        // reset the nodes input flags and output changed fired
        itsInputDirection = DirectionFlag.None;
        //itsOutputChangeFired = false;

        CalculateOutput();

        return true;
      }

      return false;
    }


    public virtual void TakeInputSnapshot( int aPropagationStep )
    {

    }
  }
}
