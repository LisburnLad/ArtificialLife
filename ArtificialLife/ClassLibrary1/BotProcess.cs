using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class PropagationEventArgs : EventArgs
  {
    public PropagationEventArgs(int aPropagationStep)
    {
      StepNumber = aPropagationStep;
    }

    public int StepNumber { get; set; }
  }

  class BotProcess
  {
    BotStructure itsStructure;

    public int itsSideLength 
    { 
      get
      {
        return itsStructure.kSideLength;
      }
    }

    public  CellType[,] itsGrid
    {
      get
      {
        return itsStructure.itsGrid;
      }
    }

    /// <summary>
    /// the current state of each cell in the bot
    /// </summary>
    char[,] itsCellState;
    public char[,] CellState 
    {
      get
      {
        return itsCellState;
      }
    }

    /// <summary>
    /// When set true each step of propagation will be written to the output if a test file name is specified
    /// </summary>
    public bool ShowAllPropagationSteps { get; set; }

    /// <summary>
    /// the actual nodes that form the bot structure
    /// </summary>
    NodeBase[,] itsGridNodes;


    EventBroker itsEventBroker = null;

    public BotProcess(BotStructure aStructure, EventBroker aEventBroker)
    {
      // register this node with the system event broker to allow event publication and subscription
      itsEventBroker = aEventBroker;
      itsEventBroker.Register(this);

      itsStructure = aStructure;

      InitializeOutput();

      //Evaluate();
    }

    /// <summary>
    /// the current propagation step
    /// </summary>
    private int itsPropagationPass = 0;

    /// <summary>
    /// Evaluate the bot starting at a propagation step of zero
    /// </summary>
    /// <param name="aNorthInput"></param>
    /// <param name="aEastInput"></param>
    /// <param name="aSouthInput"></param>
    /// <param name="aWestInput"></param>
    /// <param name="aFileName"></param>
    /// <param name="aResetPropagationStep"></param>    
    public void Evaluate(bool aNorthInput, bool aEastInput, bool aSouthInput, bool aWestInput, string aFileName, bool aResetPropagationStep )
    {
      ArtificialLifeProperties.Log("\n\nSTARTING EVALUATION\n");

      // reset the propagation pass number
      if (aResetPropagationStep)
      {
        itsPropagationPass = 0;
      }

      // set the current state of the grid input cells
      SetGridInputCells(aNorthInput, aEastInput, aSouthInput, aWestInput);

      if( ShowAllPropagationSteps )
      {
        WriteOutputToFile( aFileName + itsPropagationPass.ToString() + ".txt" );
      }

      // propagate the output of any nodes whose output or internal state has changed
      do
      {
        itsPropagationPass++;

        ArtificialLifeProperties.Log("\nPropagating Output: pass = " + itsPropagationPass + "\n");

        PropagateCells(aFileName);    

        // fire an event to indicate the end of a propagation cycle
        // - this is the trigger for the bot position to be updated
        PropagationEnd();

        if( ShowAllPropagationSteps && string.IsNullOrEmpty( aFileName ) == false )
        {
          WriteOutputToFile( aFileName + itsPropagationPass.ToString() + ".txt" );
        }
      }
      while (itsCellsToPropagate.Count > 0 && itsPropagationPass < 10);
    }

    [EventPublication(ArtificialLifeProperties.PropagationEndEvent)]
    public event EventHandler<PropagationEventArgs> PropagationEndEvent;

    public void PropagationEnd()
    {
      PropagationEventArgs eventArgs = new PropagationEventArgs(itsPropagationPass);
      PropagationEndEvent(this, eventArgs);
    }


    [EventPublication(ArtificialLifeProperties.PropagateOutputEvent)]
    public event EventHandler PropagateOutputEvent;

    /// <summary>
    /// flag to detect if the node output has changed during the current propagation
    /// </summary>
    private bool itsNodeOutputChanged = false;

    /// <summary>
    /// Propagate the output of any nodes whose output has changed
    /// </summary>
    private void PropagateNodeOutput()
    {
      // reset the flag to indicate if this propagation causes a change
      itsNodeOutputChanged = false;

      // pass on the output of any nodes whose state has changed
      PropagateOutputEvent(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Set the current state of the grid input cells
    /// </summary>
    public void SetGridInputCells(bool aNorthInput, bool aEastInput, bool aSouthInput, bool aWestInput)
    {
      int middleCell = (itsSideLength / 2);
      int lastCell = (itsSideLength - 1);

      // top row input cell
      SetNodeOutput(0, middleCell, Direction.South, aNorthInput);

      // bottom row input cell
      SetNodeOutput(lastCell, middleCell, Direction.North, aSouthInput);

      // left col input cell
      SetNodeOutput(middleCell, 0, Direction.East, aWestInput);

      // right col input cell
      SetNodeOutput(middleCell, lastCell, Direction.West, aEastInput);
    }

    private void SetNodeOutput(int aRow, int aCol, Direction aDirection, bool aValue)
    {
      if( itsGridNodes[aRow, aCol] != null )
      {
        itsGridNodes[aRow, aCol].Output = (aValue?1:0);
      }      
    }


    private NodeBase GetGridNode(int aRow, int aCol)
    {
      if( aRow > 0 && aRow < (itsSideLength-1) && aCol > 0 && aCol < (itsSideLength-1))
      {
        return itsGridNodes[aRow, aCol];
      }

      return null;
    }


    #region Event Handlers

    /// <summary>
    /// the list of nodes to propagate
    /// </summary>    
    List<ArtificialLife.BotCellToProcess> itsCellsToPropagate = new List<BotCellToProcess>();

    /// <summary>
    /// propagation step used in debugging when monitoring how cell output is changing at each step of the pass
    /// </summary>
    static int PropagationStep = 0;

    public void PropagateCells(string aFileName, int aPropagationStep)
    {
      // reset the propagation step at the start of each pass
      PropagationStep = 0;

      itsPropagationPass = aPropagationStep;
      PropagateCells(aFileName);
    }

    /// <summary>
    /// Process all the cells in the list
    /// - new cells can be added to this list during processing
    /// </summary>
    public void PropagateCells(string aFileName)
    {
      // set the current wave of cell processing to be the level of the first node
      int lastCellLevel = ((itsCellsToPropagate.Count > 0) ? itsCellsToPropagate.ElementAt(0).itsLevel : 1);

      while (itsCellsToPropagate.Count > 0 && lastCellLevel <= itsPropagationPass)
      {        
        // get and process the first cell in the list
        var cell = itsCellsToPropagate.ElementAt(0);

        if (cell.itsLevel <= itsPropagationPass)
        {
          if (itsGridNodes[cell.itsRow, cell.itsCol].PropagateOutput(itsPropagationPass + 1))
          {            
            //WriteOutputToFile(aFileName + (itsPropagationPass-1).ToString() + "_" + PropagationStep + ".txt");
            //PropagationStep++;
          }

          // remove this cell from the list of cells to process
          itsCellsToPropagate.RemoveAt(0);
        }

        // keep track of the current level of cell processing
        lastCellLevel = cell.itsLevel;        
      }
    }


    private void AddCellToPropagate( int aRow, int aCol, string aReason )
    {
      int nextStep = itsPropagationPass + 1;

      itsGridNodes[aRow, aCol].TakeInputSnapshot(nextStep);

      // test that the supplied cell isn't already in the list of cells to propagate
      foreach( BotCellToProcess cellToProcess in itsCellsToPropagate)
      {
        if( cellToProcess.itsRow == aRow && cellToProcess.itsCol == aCol && cellToProcess.itsLevel == nextStep)
        {
          return;
        }
      }
      
      ArtificialLifeProperties.Log("Adding Cell For Propagation: row = " + aRow + " col = " + aCol + " step = " + nextStep + " - " + aReason + "\n");

      // add the cell to be propagated to the list of cells still to process
      itsCellsToPropagate.Add(new BotCellToProcess(aRow, aCol, nextStep));
    }


    /// <summary>
    /// Handle the node internal state change event on the same thread as the publisher
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    [EventSubscription(ArtificialLifeProperties.NodeInternalStateChangeEvent, typeof(OnPublisher))]
    public void OnNodeInternalStateChangeEvent(object sender, NodeEventArgs aArgs)
    {
      AddCellToPropagate(aArgs.itsRow, aArgs.itsCol, "internal state change");

      // set the flag to show that at least one node has changed its internal state
      // during this propagation cycle
      // - by setting this flag this will keep processing of the bot going
      itsNodeOutputChanged = true;
    }

    /// <summary>
    /// Handle the node output change event on the same thread as the publisher
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    [EventSubscription(ArtificialLifeProperties.NodeOutputChangeEvent, typeof(OnPublisher))]
    public void OnNodeOutputChangeEvent(object sender, NodeEventArgs aArgs)
    {
      // set the flag to show that at least one node output has changed during this propagation cycle
      itsNodeOutputChanged = true;
      
      NodeBase gridNode = null;

      // store the new state into the grid that monitors this passes state changes
      itsCellState[aArgs.itsRow, aArgs.itsCol] = (aArgs.itsValue ? '1' : '0');

      // test if the event specifies the values for a target node (end of a target connection)
      if (aArgs.itsTargetNode)
      {
        // get the target node and check that its a connection end
        gridNode = GetGridNode(aArgs.itsRow, aArgs.itsCol);
        if (gridNode != null && gridNode.NodeCellType == CellType.ConnectionEnd)
        {
          gridNode.Output = (aArgs.itsValue?1:0);

          //AddCellToPropagate(gridNode.itsRow, gridNode.itsCol, "output change target node");
        }
      }
      else
      {
        //
        // the event specifies the change to the current node
        //

        if (aArgs.itsDirection.HasFlag(DirectionFlag.North))
        {
          gridNode = GetGridNode(aArgs.itsRow - 1, aArgs.itsCol);
          if (gridNode != null)
          {
            gridNode.SouthInput = aArgs.itsValue;
            AddCellToPropagate(gridNode);
          }
        }

        if (aArgs.itsDirection.HasFlag(DirectionFlag.East))
        {
          gridNode = GetGridNode(aArgs.itsRow, aArgs.itsCol + 1);
          if (gridNode != null)
          {
            gridNode.WestInput = aArgs.itsValue;
            AddCellToPropagate(gridNode);
          }
        }

        if (aArgs.itsDirection.HasFlag(DirectionFlag.South))
        {
          gridNode = GetGridNode(aArgs.itsRow + 1, aArgs.itsCol);
          if (gridNode != null)
          {
            gridNode.NorthInput = aArgs.itsValue;
            AddCellToPropagate(gridNode);
          }
        }

        if (aArgs.itsDirection.HasFlag(DirectionFlag.West))
        {
          gridNode = GetGridNode(aArgs.itsRow, aArgs.itsCol - 1);
          if (gridNode != null)
          {
            gridNode.EastInput = aArgs.itsValue;
            AddCellToPropagate(gridNode);
          }
        }        
      }
    }

    private void AddCellToPropagate(NodeBase gridNode)
    {
      if (gridNode != null && gridNode.NodeCellType >= CellType.NorthNand && gridNode.NodeCellType < CellType.ConnectionStart)
      {
        AddCellToPropagate(gridNode.itsRow, gridNode.itsCol, "output change current node");
      }
    }

    #endregion Event Handlers
    

    /// <summary>
    /// allocate and initialize the output array
    /// </summary>
    public void InitializeOutput()
    {
      // create the array of grid cell nodes
      itsGridNodes = new NodeBase[itsSideLength, itsSideLength];

      // create the array of grid cell values
      itsCellState = new char[itsSideLength, itsSideLength];

      // initially set all connections to zero and all other cells blank
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          if (itsGrid[row, col] == CellType.OutOfBounds)
          {
            itsCellState[row, col] = '-';
          }
          else if (itsGrid[row, col] > CellType.EmptyCell )
          {
            itsCellState[row, col] = '0';

            // create the appropriate node to match the cell type
            CreateGridNodes(row, col);
          }          
          else
          {
            itsCellState[row, col] = '-';            
          }
        }
      }
    }

    /// <summary>
    /// Create a specific node type in the supplied grid location to match the specified cell type at that point
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void CreateGridNodes(int row, int col)
    {
      AssignGridNodes(row, col, itsGrid[row, col], ref itsGridNodes[row, col]);
    }


    #region Test For Node Inputs    

    /// <summary>
    /// Test if the specified node has an input from the North
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <param name="aCellType"></param>
    /// <returns></returns>
    private bool TestForNorthInput(int aRow, int aCol, CellType aCellType)
    {
      if(aRow > 0)
      {
        // test that the current node isn't pointing in the direction of the input
        if ((Direction)((int)aCellType % 4) != Direction.North)
        {
          CellType inputCell = itsGrid[aRow - 1, aCol];

          // test if the cell above contains a node
          if( inputCell >= CellType.NorthNand && inputCell < CellType.ConnectionStart )
          {
            // test if the node has a south output
            return (Direction)((int)inputCell % 4) == Direction.South;
          }
          else if( inputCell == CellType.ConnectionEnd)
          {
            // test if the end of the targeted connection points into this cell
            for (int index = 0; index < itsStructure.itsDirectedConnections.Count; index++)
            {
              if (itsStructure.itsDirectedConnections[index].itsDirection == Direction.South
                && itsStructure.itsDirectedConnections[index].itsEnd.X == (aRow-1)
                && itsStructure.itsDirectedConnections[index].itsEnd.Y == aCol)
              {
                return true;
              }
            }
          }
          else
          {
            // test if the node above has a south connection
            return inputCell == CellType.NorthSouth
                || inputCell == CellType.EastSouthWest
                || inputCell == CellType.NorthEastSouth
                || inputCell == CellType.SouthWestNorth
                || inputCell == CellType.EastSouth
                || inputCell == CellType.SouthWest
                || inputCell == CellType.NorthEastSouthWest
                || inputCell == CellType.InputCell;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Test if the specified node has an input from the South
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <param name="aCellType"></param>
    /// <returns></returns>
    private bool TestForSouthInput(int aRow, int aCol, CellType aCellType)
    {
      if (aRow < (itsSideLength-1))
      {
        // test that the current node isn't pointing in the direction of the input
        if ((Direction)((int)aCellType % 4) != Direction.South)
        {
          CellType inputCell = itsGrid[aRow + 1, aCol];

          // test if the cell below contains a node
          if (inputCell >= CellType.NorthNand && inputCell < CellType.ConnectionStart)
          {
            // test if the node has a north output
            return (Direction)((int)inputCell % 4) == Direction.North;
          }
          else if (inputCell == CellType.ConnectionEnd)
          {
            // test if the end of the targeted connection points into this cell
            for (int index = 0; index < itsStructure.itsDirectedConnections.Count; index++)
            {
              if (itsStructure.itsDirectedConnections[index].itsDirection == Direction.North
                && itsStructure.itsDirectedConnections[index].itsEnd.X == (aRow + 1)
                && itsStructure.itsDirectedConnections[index].itsEnd.Y == aCol)
              {
                return true;
              }
            }
          }
          else
          {
            // test if the node below has a north connection
            return inputCell == CellType.NorthSouth
                || inputCell == CellType.WestNorthEast
                || inputCell == CellType.NorthEastSouth
                || inputCell == CellType.SouthWestNorth
                || inputCell == CellType.NorthEast
                || inputCell == CellType.WestNorth
                || inputCell == CellType.NorthEastSouthWest
                || inputCell == CellType.InputCell;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Test if the specified node has an input from the West
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <param name="aCellType"></param>
    /// <returns></returns>
    private bool TestForWestInput(int aRow, int aCol, CellType aCellType)
    {
      if (aCol > 0)
      {
        // test that the current node isn't pointing in the direction of the input
        if ((Direction)((int)aCellType % 4) != Direction.West)
        {
          CellType inputCell = itsGrid[aRow, aCol-1];

          // test if the cell above contains a node
          if (inputCell >= CellType.NorthNand && inputCell < CellType.ConnectionStart)
          {
            // test if the test node has a east output
            return (Direction)((int)inputCell % 4) == Direction.East;
          }
          else if (inputCell == CellType.ConnectionEnd)
          {
            // test if the end of the targeted connection points into this cell
            for (int index = 0; index < itsStructure.itsDirectedConnections.Count; index++)
            {
              if (itsStructure.itsDirectedConnections[index].itsDirection == Direction.East
                && itsStructure.itsDirectedConnections[index].itsEnd.X == aRow
                && itsStructure.itsDirectedConnections[index].itsEnd.Y == aCol-1)
              {
                return true;
              }
            }
          }
          else
          {
            // test if the node to the left has an east connection
            return inputCell == CellType.WestEast
                || inputCell == CellType.WestNorthEast
                || inputCell == CellType.EastSouthWest
                || inputCell == CellType.NorthEastSouth
                || inputCell == CellType.NorthEast
                || inputCell == CellType.EastSouth
                || inputCell == CellType.NorthEastSouthWest
                || inputCell == CellType.InputCell;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Test if the specified node has an input from the East
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <param name="aCellType"></param>
    /// <returns></returns>
    private bool TestForEastInput(int aRow, int aCol, CellType aCellType)
    {
      if (aCol < (itsSideLength-1))
      {
        // test that the current node isn't pointing in the direction of the input
        if ((Direction)((int)aCellType % 4) != Direction.East)
        {
          CellType inputCell = itsGrid[aRow, aCol + 1];

          // test if the cell above contains a node
          if (inputCell >= CellType.NorthNand && inputCell < CellType.ConnectionStart)
          {
            // test if the test node has a east output
            return (Direction)((int)inputCell % 4) == Direction.West;
          }
          else if (inputCell == CellType.ConnectionEnd)
          {
            // test if the end of the targeted connection points into this cell
            for (int index = 0; index < itsStructure.itsDirectedConnections.Count; index++)
            {
              if (itsStructure.itsDirectedConnections[index].itsDirection == Direction.West
                && itsStructure.itsDirectedConnections[index].itsEnd.X == aRow
                && itsStructure.itsDirectedConnections[index].itsEnd.Y == aCol + 1)
              {
                return true;
              }
            }
          }
          else
          {
            // test if the node to the left has a west connection
            return inputCell == CellType.WestEast
                || inputCell == CellType.WestNorthEast
                || inputCell == CellType.EastSouthWest
                || inputCell == CellType.SouthWestNorth
                || inputCell == CellType.SouthWest
                || inputCell == CellType.WestNorth
                || inputCell == CellType.NorthEastSouthWest
                || inputCell == CellType.InputCell;
          }
        }
      }

      return false;
    }

    #endregion Test For Node Inputs


    private void AssignGridNodes(int row, int col, CellType cellType, ref NodeBase gridNode)
    {
      // calculate the inputs to the specified cell
      DirectionFlag nodeInputs = DirectionFlag.None;

      // calculate the inputs for nodes
      // - input cells are set directly therefore don't receive their input from any other nodes
      if (cellType != CellType.InputCell && cellType > CellType.EmptyCell)
      {
        if(TestForNorthInput(row,col,cellType))
        {
          nodeInputs |= DirectionFlag.North;
        }
        
        if( TestForSouthInput(row,col,cellType) )
        {
          nodeInputs |= DirectionFlag.South;
        }

        if (TestForEastInput(row, col, cellType))
        {
          nodeInputs |= DirectionFlag.East;
        }

        if (TestForWestInput(row, col, cellType))
        {
          nodeInputs |= DirectionFlag.West;
        }
      }

      // create the appropriate node given the cell type
      if (cellType >= CellType.NorthNand && cellType <= CellType.WestNand)
      {
        gridNode = new NodeNand(cellType, row, col, nodeInputs);
      }
      else if (cellType >= CellType.NorthDelay && cellType <= CellType.WestDelay)
      {
        gridNode = new NodeDelay(cellType, row, col, nodeInputs);
      }
      else if (cellType >= CellType.NorthXor && cellType <= CellType.WestXor)
      {
        gridNode = new NodeXor(cellType, row, col, nodeInputs);
      }
      else if (cellType >= CellType.NorthTrigger && cellType <= CellType.WestTrigger)
      {
        gridNode = new NodeTrigger(cellType, row, col, nodeInputs);
      }
      else if (cellType >= CellType.NorthOr && cellType <= CellType.WestOr)
      {
        gridNode = new NodeOr(cellType, row, col, nodeInputs);
      }
      else if (cellType >= CellType.NorthPulse && cellType <= CellType.WestPulse)
      {
        gridNode = new NodePulse(cellType, row, col, nodeInputs);
      }
      else if (cellType == CellType.InputCell)
      {
        gridNode = new NodeInput(cellType, row, col, nodeInputs);

        // if this is an input cell explicitly set its output direction
        int middleCell = (itsSideLength / 2);
        int lastCell = (itsSideLength - 1);
        if (row == 0 && col == middleCell)
        {
          // top row input cell
          gridNode.itsOutputDirection = DirectionFlag.South;
        }
        else if (row == lastCell && col == middleCell)
        {
          // bottom row input cell
          gridNode.itsOutputDirection = DirectionFlag.North;
        }
        else if (row == middleCell && col == 0)
        {
          // left col input cell
          gridNode.itsOutputDirection = DirectionFlag.East;
        }
        else if (row == middleCell && col == lastCell)
        {
          // bottom row input cell
          gridNode.itsOutputDirection = DirectionFlag.West;
        }
      }
      else if (cellType == CellType.OutputCell)
      {
        gridNode = new NodeOutput(cellType, row, col, nodeInputs);
      }
      else if (cellType == CellType.ConnectionStart)
      {
        // search the list of targeted connections for this start point
        int index = 0;
        for (; index < itsStructure.itsDirectedConnections.Count; index++)
        {
          if (itsStructure.itsDirectedConnections[index].itsStart.X == row
           && itsStructure.itsDirectedConnections[index].itsStart.Y == col)
          {
            break;
          }
        }

        if (index < itsStructure.itsDirectedConnections.Count)
        {
          // a targeted connection start
          gridNode = new NodeConnectionStart(cellType, row, col, nodeInputs,
                                             itsStructure.itsDirectedConnections[index].itsEnd.X,
                                             itsStructure.itsDirectedConnections[index].itsEnd.Y);
        }
        else
        {
          throw new Exception("targeted connection not found");
        }
      }
      else if (cellType == CellType.ConnectionEnd)
      {
        // search the list of targeted connections for this end point
        int index = 0;
        for (; index < itsStructure.itsDirectedConnections.Count; index++)
        {
          if (itsStructure.itsDirectedConnections[index].itsEnd.X == row
           && itsStructure.itsDirectedConnections[index].itsEnd.Y == col)
          {
            break;
          }
        }

        if (index < itsStructure.itsDirectedConnections.Count)
        {
          // a targeted connection end
          gridNode = new NodeConnectionEnd(cellType, row, col, nodeInputs, itsStructure.itsDirectedConnections[index].itsDirection);
        }
        else
        {
          throw new Exception("targeted connection not found");
        }
      }
      else if (cellType > CellType.EmptyCell && cellType < CellType.NorthNand)
      {
        // a connection
        gridNode = new NodeConnection(cellType, row, col, nodeInputs);
      }
      else if (cellType != CellType.EmptyCell && cellType != CellType.OutOfBounds)
      {
        gridNode = new NodeBase(cellType, row, col, nodeInputs);
      }

      gridNode.RegisterEventBroker(itsEventBroker);
    }


    /// <summary>
    /// write the current state of each cell to the specified file
    /// </summary>
    /// <param name="aFileName"></param>
    public void WriteGridOutputToFile( string aFileName )
    {
      using (TextWriter writer = new StreamWriter(aFileName))
      {
        for (int row = 0; row < itsSideLength; row++)
        {
          for (int col = 0; col < itsSideLength; col++)
          {
            if (col > 0)
            {
              writer.Write(" ");
            }

            writer.Write("{0}", itsCellState[row, col]);
          }

          writer.WriteLine();
        }
      }
    }

    /// <summary>
    /// write the current state of each cell to the specified file
    /// </summary>
    /// <param name="aFileName"></param>
    public void WriteOutputToFile(string aFileName)
    {
      using (TextWriter writer = new StreamWriter(aFileName))
      {
        for (int row = 0; row < itsSideLength; row++)
        {
          for (int col = 0; col < itsSideLength; col++)
          {
            if (col > 0)
            {
              writer.Write(" ");
            }

            if (itsGridNodes[row, col] != null)
            {
              // if the output is current -1 it hasn't yet been set
              if (itsGridNodes[row, col].Output == -1)
              {
                writer.Write("X");
              } 
              else
              {
                writer.Write("{0}", itsGridNodes[row, col].Output);
              }
            }
            else
            {
              writer.Write(" ");
            }
          }

          writer.WriteLine();
        }
      }
    }

    /// <summary>
    /// get the values of the output nodes
    /// </summary>
    /// <param name="aNorthOutput"></param>
    /// <param name="aEastOutput"></param>
    /// <param name="aSouthOutput"></param>
    /// <param name="aWestOutput"></param>
    public void GetOutputs(out bool aNorthOutput, out bool aEastOutput, out bool aSouthOutput, out bool aWestOutput)
    {
      int middleCell = (itsSideLength / 2);
    
      int row = middleCell - 1;
      int col = middleCell;
      if (itsGrid[row, col] == CellType.OutputCell)
      {
        aNorthOutput = (itsGridNodes[row, col].Output > 0);
      }
      else aNorthOutput = false;

      row = middleCell + 1;
      col = middleCell;
      if (itsGrid[row, col] == CellType.OutputCell)
      {
        aSouthOutput = (itsGridNodes[row, col].Output > 0);
      }
      else aSouthOutput = false;

      row = middleCell;
      col = middleCell - 1;
      if (itsGrid[row, col] == CellType.OutputCell)
      {
        aWestOutput = (itsGridNodes[row, col].Output > 0);
      }
      else aWestOutput = false;

      row = middleCell;
      col = middleCell + 1;
      if (itsGrid[row, col] == CellType.OutputCell)
      {
        aEastOutput = (itsGridNodes[row, col].Output > 0);
      }
      else aEastOutput = false;
    }

  }
}
