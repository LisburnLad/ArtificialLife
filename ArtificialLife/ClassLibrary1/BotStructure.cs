//#define DRAW_GRID_STAGES


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using System.Drawing;
using System.IO;



namespace ArtificialLife
{
  /// <summary>
  /// Information about a cell whose outputs are still to be expanded
  /// </summary>
  class BotCellToProcess
  {
    public int itsRow { get; set; }
    public int itsCol { get; set; }
    public Direction itsDirection { get; set; }
    public CellInformation itsCellInfo { get; set; }
    public int itsLevel { get; set; }

    public BotCellToProcess(int aRow, int aCol, Direction aDirection, CellInformation aCellInformation,int aLevel)
    {
      itsRow = aRow;
      itsCol = aCol;
      itsDirection = aDirection;
      itsCellInfo = aCellInformation;
      itsLevel = aLevel;
    }

    public BotCellToProcess(int aRow, int aCol, int aLevel)
    {
      itsRow = aRow;
      itsCol = aCol;
      itsLevel = aLevel;
    }
  };

  /// <summary>
  /// Information about a connection that still needs to be made
  /// </summary>
  class ConnectionsToProcess
  {
    public int itsLevel { get; set; }
    public Point itsConnectionStart { get; set; }
    public Direction itsDirection { get; set; }
    public CellInformation itsCellInfo { get; set; }
    
    public ConnectionsToProcess(int aNodeLevel, Point aConnectionStart, Direction aDirection, CellInformation aStartCellInformation)
    {
      itsLevel = aNodeLevel;
      itsConnectionStart = aConnectionStart;
      itsDirection = aDirection;
      itsCellInfo = aStartCellInformation;
    }
  };

  /// <summary>
  /// Definition of a cells position and type
  /// </summary>
  public class CellDefinition
  {
    public Point itsPosition { get; set; }
    public CellType itsType { get; set; }

    public CellDefinition( int aRow, int aCol, CellType aType)
    {
      itsPosition = new Point( aRow, aCol );
      itsType = aType;
    }
  }

  // use a structure rather than a class to avoid having to allocate each object in array
  struct CellInformation
  {
    public int itsID { get; set; }
    public CellType itsType { get; set; }   

    public void Set(int aID, CellType aType)
    {
      itsID = aID;
      itsType = aType;
    }
  };

  public class DirectedConnection
  {
    public Point itsStart { get; set; }
    public Point itsEnd { get; set; }

    // the direction of the connection end - this shows which cell is being connected to
    public Direction itsDirection { get; set; }
  };

  //class BotStructure
  public partial class BotStructure
  {
    /// <summary>
    /// the length of side of the square grid
    /// </summary>
    public int kSideLength = 15;

    
    /// <summary>
    /// the grid representing the structure of the bot
    /// </summary>
    public CellType[,] itsGrid;


    /// <summary>
    /// the list of grid cells that still need to be expanded
    /// </summary>    
    List<ArtificialLife.BotCellToProcess> itsCellsToProcess;

    /// <summary>
    /// the list of targeted connections that need to be expanded at the end of a expansion level
    /// </summary>
    List<ArtificialLife.ConnectionsToProcess> itsConnectionsToProcess;

    /// <summary>
    /// the list of targeted connections in the grid
    /// </summary>
    public List<DirectedConnection> itsDirectedConnections;
    
    ChromosomeDecode itsChromosomeDecode = null;

    public BotStructure(int aSideLength)
    {
      kSideLength = aSideLength;
    }

    /// <summary>
    /// Create the bot structure from a bot descriptor file
    /// </summary>
    /// <param name="aFileName"></param>
    public BotStructure(string aFileName)
    {
      LoadGridFromFile(aFileName);
    }

    /// <summary>
    ///  generate the grid from the supplied chromosome
    /// </summary>
    /// <param name="aChromosome"></param>
    public void Generate(Chromosome aChromosome, bool aShowGrid, Pruning aPruneGrid)
    {
      itsChromosomeDecode = new ChromosomeDecode(aChromosome);
      itsChromosomeDecode.Run(aShowGrid);

      // generate the blank grid structure
      CreateGrid();

      // start growing the grid nodes
      GrowGrid(aShowGrid, aPruneGrid);
    }

    /// <summary>
    /// generate the grid from the supplied chromosome but add in the supplied target nodes before growing the cells
    /// - this is used as a debug function to test targeted connections
    /// </summary>
    /// <param name="aChromosome"></param>
    /// <param name="aPlacedTargets"></param>
    public void Generate(Chromosome aChromosome, CellDefinition[] aPlacedTargets, bool aShowGrid, Pruning aPruneGrid)
    {
      itsChromosomeDecode = new ChromosomeDecode(aChromosome);
      itsChromosomeDecode.Run(aShowGrid);

      // generate the blank grid structure
      CreateGrid();

      // add the supplied target nodes before growing the cells
      AddPlacedTargets(aPlacedTargets);

      // start growing the grid nodes
      GrowGrid(aShowGrid, aPruneGrid);      
    }


    /// <summary>
    /// put the supplied nodes into the grid at their given positions
    /// </summary>
    /// <param name="aPlacedTargets"></param>
    private void AddPlacedTargets(CellDefinition[] aPlacedTargets)
    {
      foreach( CellDefinition target in aPlacedTargets)
      {
        // make sure there's nothing already in that grid position
        if (itsGrid[target.itsPosition.X, target.itsPosition.Y] == CellType.EmptyCell)
        {
          itsGrid[target.itsPosition.X, target.itsPosition.Y] = target.itsType;
        }
      }
    }

    /// <summary>
    /// save the grid image 
    /// </summary>
    /// <param name="?"></param>
    public void SaveGrid( string aImageName)
    {
      BotDraw botDraw = new BotDraw(this);
      botDraw.ShowGrid();
      botDraw.CreateGridImage(aImageName);
    }

    /// <summary>
    /// add the basic structural cells to the initial grid
    /// </summary>
    private void AddInitialGridCells()
    {
      int centerRow = kSideLength / 2;
      int centerCol = kSideLength / 2;

      for(int row = 0; row < kSideLength; row++)
      {
        for(int col = 0; col < kSideLength; col++)
        {
          // set the top and bottom grid edges and input cells
          if( row == 0 || row == (kSideLength-1) )
          {
            if (col == centerCol)
            {
              itsGrid[row, col] = CellType.InputCell;
            }
            else
            {
              itsGrid[row, col] = CellType.OutOfBounds;
            }
          }

          // set the left and right grid edges and input cells
          if (col == 0 || col == (kSideLength - 1))
          {
            if (row == centerRow)
            {
              itsGrid[row, col] = CellType.InputCell;
            }
            else
            {
              itsGrid[row, col] = CellType.OutOfBounds;
            }
          }

          // set the output cells
          if ((row == centerRow && (col == (centerCol - 1) || col == (centerCol + 1)))
           || (col == centerCol && (row == (centerRow - 1) || row == (centerRow + 1))))
          {
            itsGrid[row, col] = CellType.OutputCell;
          }
          else if( row == centerRow && col == centerCol )
          {
            itsGrid[row, col] = CellType.OutOfBounds;
          }          
        }
      }
    }

    /// <summary>
    /// write the grid cell types to a file
    /// </summary>
    /// <param name="aFileName"></param>
    public void WriteGridToFile( string aFileName )
    {
      using (TextWriter writer = new StreamWriter(aFileName))
      {
        for (int row = 0; row < kSideLength; row++)
        {
          for (int col = 0; col < kSideLength; col++)
          {
            if( col > 0 )
            {
              writer.Write(",");
            }
            writer.Write("{0:X2}", (int)itsGrid[row, col]);
          }

          writer.WriteLine();
        }
      }
    }

    /// <summary>
    /// Create the grid from a bot descriptor file
    /// </summary>
    /// <param name="aFileName"></param>
    public void LoadGridFromFile( string aFileName )
    {
      List<string> gridLines = new List<string>();
      using (TextReader reader = new StreamReader(aFileName))
      {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
          gridLines.Add(line);           
        }
      }

      if( gridLines.Count > 0 )
      {        
        // set the side length to the number of rows
        // (the number of cols is also expected to match this)
        kSideLength = gridLines.Count;
        
        // allocate the grid cells
        itsGrid = new CellType[kSideLength, kSideLength];

        // set the input, output and out of bounds grid squares
        AddInitialGridCells();

        for (int row = 0; row < kSideLength; row++)
        {
          string[] cells = gridLines[row].Split(',');

          for (int col = 0; col < kSideLength; col++)
          {
            CellType type = (CellType)int.Parse(cells[col], System.Globalization.NumberStyles.HexNumber);
            itsGrid[row, col] = type;
          }
        }
      }
    }

    /// <summary>
    /// create and initialize a blank grid and its required structures
    /// </summary>
    private void CreateGrid()
    {
      // allocate the grid cells
      itsGrid = new CellType[kSideLength, kSideLength];

      // set the input, output and out of bounds grid squares
      AddInitialGridCells();

      // allocate the list of grid cells that still need to be expanded
      itsCellsToProcess = new List<BotCellToProcess>();

      // allocate the list of targeted connections to process at the end of each cell expansion level
      itsConnectionsToProcess = new List<ConnectionsToProcess>();

      // allocate the list of targeted connections
      itsDirectedConnections = new List<DirectedConnection>();
    }


    /// <summary>
    /// start growing the grid from the input cells
    /// </summary>
    private void GrowGrid(bool aShowGrid, Pruning aPruneGrid = Pruning.All)
    {
      // put the initial cells into the grid
      SetInputCells();

      // start growing the grid
      ExpandCells();

      // save an image of the grid
      //SaveGrid("c:\\grid.bmp");

      // remove unused nodes and connections from the grid
      PruneGrid(aShowGrid, aPruneGrid);

      //Console.WriteLine("Chromosome: " + itsChromosomeDecode.itsChromosome.ToBinaryString());
    }

    /// <summary>
    /// remove unused nodes and connections from the grid
    /// </summary>
    private void PruneGrid(bool aShowGrid, Pruning aPruneGrid)
    {
      //Console.WriteLine("Chromosome: " + itsChromosomeDecode.itsChromosome.ToBinaryString());

      if (aShowGrid)
      {
        SaveGrid("beforepruning.bmp");
      }

      bool nodesRemoved = false;

      int step = 1;
      do
      {
        nodesRemoved = PerformPruningPass(nodesRemoved, aPruneGrid);

        //if (aShowGrid)
        //{
        //  SaveGrid("c:\\pruning_step_" + step + ".bmp");
        //  step++;
        //}
      }
      while (nodesRemoved);

      if (aShowGrid)
      {
        SaveGrid("afterpruning.bmp");
      }
    }

    /// <summary>
    /// Removed unused nodes and connections
    /// </summary>
    /// <param name="nodesRemoved"></param>
    /// <param name="aPruneGrid">this flag can be used to stop parts of pruning - used in tests</param>
    /// <returns></returns>
    private bool PerformPruningPass(bool nodesRemoved, Pruning aPruneGrid)
    {
      nodesRemoved = false;

      int centerRow = kSideLength / 2;
      int centerCol = kSideLength / 2;

      int lastRow = (kSideLength - 2);
      int lastCol = (kSideLength - 2);

      for (int row = 1; row < kSideLength - 1; row++)
      {
        for (int col = 1; col < kSideLength - 1; col++)
        {
          // remove unnecessary edge connections and nodes
          if (aPruneGrid.HasFlag( Pruning.EdgeConnections ))
          {
            nodesRemoved = RemoveEdgeConnectionsAndNodes(nodesRemoved, centerRow, centerCol, lastRow, lastCol, row, col);
          }

          // remove any nodes whose output isn't connected to anything
          if (aPruneGrid.HasFlag( Pruning.NoOutput))
          {
            nodesRemoved = RemoveNodesWithNoOutput(nodesRemoved, row, col);
          }

          // remove any targeted connections that don't join to anything
          if (aPruneGrid.HasFlag(Pruning.UnjoinedConnections))
          {
            nodesRemoved = RemoveUnjoinedTargetedConnections(nodesRemoved, row, col);
          }

          // remove any unjoined connections 
          if (itsGrid[row, col] > CellType.EmptyCell && itsGrid[row, col] < CellType.NorthNand)
          {            
            nodesRemoved = RemoveConnectionPartsToEmptyCells(nodesRemoved, row, col, row - 1, col, DirectionFlag.North);
            nodesRemoved = RemoveConnectionPartsToEmptyCells(nodesRemoved, row, col, row + 1, col, DirectionFlag.South);
            nodesRemoved = RemoveConnectionPartsToEmptyCells(nodesRemoved, row, col, row, col+1, DirectionFlag.East);
            nodesRemoved = RemoveConnectionPartsToEmptyCells(nodesRemoved, row, col, row, col-1, DirectionFlag.West);
          }
        }
      }

      return nodesRemoved;
    }

    /// <summary>
    /// Remove any targeted connections that don't join onto anything
    /// </summary>
    /// <param name="aNodesRemoved"></param>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool RemoveUnjoinedTargetedConnections(bool aNodesRemoved, int aRow, int aCol)
    {
      if( itsGrid[aRow,aCol] == CellType.ConnectionEnd )
      {
        // search the list of targeted connections for this end point
        int index = 0;
        for (; index < itsDirectedConnections.Count; index++)
        {
          if (itsDirectedConnections[index].itsEnd.X == aRow && itsDirectedConnections[index].itsEnd.Y == aCol)
          {
            break;
          }
        }

        if (index < itsDirectedConnections.Count)
        {
          // get the grid position of the cell connecting to the end of the directed connection
          int row = aRow;
          int col = aCol;

          // test the direction of the connection end
          switch(itsDirectedConnections[index].itsDirection)
          {
            case Direction.North:
              row = aRow - 1;
              break;
            case Direction.South:
              row = aRow + 1;
              break;
            case Direction.East:
              col = aCol + 1;
              break;
            case Direction.West:
              col = aCol - 1;
              break;
          }

          // if the connection end points off the grid or to an empty cell then remove the connection
          if( (row == 0 || row == (kSideLength-1)) || (col == 0 || col == (kSideLength-1)) ||  itsGrid[row,col] == CellType.EmptyCell )
          {
            // remove the connection end
            itsGrid[aRow,aCol] = CellType.EmptyCell;

            // remove the connection start
            itsGrid[itsDirectedConnections[index].itsStart.X,itsDirectedConnections[index].itsStart.Y] = CellType.EmptyCell;

            // remove the connection from the list of directed connections
            itsDirectedConnections.RemoveAt(index);

            // set the flag to show that at least one node has been removed
            aNodesRemoved = true;
          }
        }
        else
        {
          throw new Exception("targeted connection end not found");
        }



      }
      return aNodesRemoved;
    }

    /// <summary>
    /// If the joining cell is empty, remove the part of connection in the target cell that points into this cell
    /// </summary>
    /// <param name="nodesRemoved"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="aJoinRow"></param>
    /// <param name="aJoinCol"></param>
    /// <param name="aConnectionDirection"></param>
    /// <returns></returns>
    private bool RemoveConnectionPartsToEmptyCells(bool nodesRemoved, int row, int col, int aJoinRow, int aJoinCol, DirectionFlag aConnectionDirection)
    {
      DirectionFlag directionFlag = Common.GetDirectionFlag(itsGrid[row, col]);
      if ((directionFlag & aConnectionDirection) != DirectionFlag.None)
      {
        // test if the connection is into an empty cell
        if (itsGrid[aJoinRow, aJoinCol] == CellType.EmptyCell)
        {
          // remove this part of the connection
          directionFlag &= ~aConnectionDirection;
          itsGrid[row, col] = Common.GetConnectionFromDirectionFlag(directionFlag);

          nodesRemoved = true;
        }
        else if (itsGrid[aJoinRow, aJoinCol] > CellType.EmptyCell && itsGrid[aJoinRow, aJoinCol] < CellType.NorthNand)
        {
          // the connection is into another connection - make sure it joins
          DirectionFlag joinDirectionFlag = Common.GetDirectionFlag(itsGrid[aJoinRow, aJoinCol]);

          if ((aConnectionDirection & DirectionFlag.North) != DirectionFlag.None && (joinDirectionFlag & DirectionFlag.South) == DirectionFlag.None)
          {
            // remove this part of the connection
            directionFlag &= ~aConnectionDirection;
            itsGrid[row, col] = Common.GetConnectionFromDirectionFlag(directionFlag);

            nodesRemoved = true;
          }

          if ((aConnectionDirection & DirectionFlag.South) != DirectionFlag.None && (joinDirectionFlag & DirectionFlag.North) == DirectionFlag.None)
          {
            // remove this part of the connection
            directionFlag &= ~aConnectionDirection;
            itsGrid[row, col] = Common.GetConnectionFromDirectionFlag(directionFlag);

            nodesRemoved = true;
          }

          if ((aConnectionDirection & DirectionFlag.East) != DirectionFlag.None && (joinDirectionFlag & DirectionFlag.West) == DirectionFlag.None)
          {
            // remove this part of the connection
            directionFlag &= ~aConnectionDirection;
            itsGrid[row, col] = Common.GetConnectionFromDirectionFlag(directionFlag);

            nodesRemoved = true;
          }

          if ((aConnectionDirection & DirectionFlag.West) != DirectionFlag.None && (joinDirectionFlag & DirectionFlag.East) == DirectionFlag.None)
          {
            // remove this part of the connection
            directionFlag &= ~aConnectionDirection;
            itsGrid[row, col] = Common.GetConnectionFromDirectionFlag(directionFlag);

            nodesRemoved = true;
          }
        }
      }

      return nodesRemoved;
    }


    /// <summary>
    /// Remove any nodes whose output isn't connected to anything
    /// </summary>
    /// <param name="nodesRemoved"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool RemoveNodesWithNoOutput(bool nodesRemoved, int row, int col)
    {
      // test if the cell contains a node
      if (itsGrid[row, col] >= CellType.NorthNand && itsGrid[row, col] < CellType.ConnectionStart)
      {
        CellType gridCell = itsGrid[row, col];
        Direction direction = GetNodeDirection(gridCell);

        ConnectionPasses = 0;

        switch (direction)
        {
          case Direction.North:

            // test if the output cell is empty
            if (itsGrid[row - 1, col] == CellType.EmptyCell)
            {
              nodesRemoved = ClearCell(nodesRemoved, row, col);
            }
            // test if the output cell contains another node output
            else if (itsGrid[row - 1, col] >= CellType.NorthNand && itsGrid[row - 1, col] < CellType.ConnectionStart && GetNodeDirection(itsGrid[row - 1, col]) == Direction.South)
            {
              nodesRemoved = ClearCell(nodesRemoved, row, col);
            }
            // test if the output cell contains a connection
            else if (itsGrid[row - 1, col] > CellType.EmptyCell && itsGrid[row - 1, col] < CellType.NorthNand)
            {
              // if the output cell contains a connection make sure it joins with the nodes output
              DirectionFlag directionFlag = Common.GetDirectionFlag(itsGrid[row - 1, col]);
              if ((directionFlag & DirectionFlag.South) == DirectionFlag.None)
              {
                nodesRemoved = ClearCell(nodesRemoved, row, col);
              }
              else
              {
                // there is a connection - check that this connection terminates somewhere useful
                if (TestConnectionResultsInNodeInput(row - 1, col, Direction.North) == false)
                {
                  // the output connection of this node didn't go anywhere useful
                  nodesRemoved = ClearCell(nodesRemoved, row, col);
                }
              }
            }
            break;

          case Direction.South:

            // test if the output cell is empty
            if (itsGrid[row + 1, col] == CellType.EmptyCell)
            {
              nodesRemoved = ClearCell(nodesRemoved, row, col);
            }
            // test if the output cell contains another node output
            else if (itsGrid[row + 1, col] >= CellType.NorthNand && itsGrid[row + 1, col] < CellType.ConnectionStart && GetNodeDirection(itsGrid[row + 1, col]) == Direction.North)
            {
              nodesRemoved = ClearCell(nodesRemoved, row, col);
            }
            // test if the output cell contains a connection
            else if (itsGrid[row + 1, col] > CellType.EmptyCell && itsGrid[row + 1, col] < CellType.NorthNand)
            {
              // if the output cell contains a connection make sure it joins with the nodes output
              DirectionFlag directionFlag = Common.GetDirectionFlag(itsGrid[row + 1, col]);
              if ((directionFlag & DirectionFlag.North) == DirectionFlag.None)
              {
                nodesRemoved = ClearCell(nodesRemoved, row, col);
              }
              else
              {
                // there is a connection - check that this connection terminates somewhere useful
                if (TestConnectionResultsInNodeInput(row + 1, col, Direction.South) == false)
                {
                  // the output connection of this node didn't go anywhere useful
                  nodesRemoved = ClearCell(nodesRemoved, row, col);
                }
              }
            }
            break;

          case Direction.East:

            // test if the output cell is empty
            if (itsGrid[row, col + 1] == CellType.EmptyCell)
            {
              nodesRemoved = ClearCell(nodesRemoved, row, col);
            }
            // test if the output cell contains another node output
            else if (itsGrid[row, col + 1] >= CellType.NorthNand && itsGrid[row, col + 1] < CellType.ConnectionStart && GetNodeDirection(itsGrid[row, col + 1]) == Direction.West)
            {
              nodesRemoved = ClearCell(nodesRemoved, row, col);
            }
            // test if the output cell contains a connection
            else if (itsGrid[row, col + 1] > CellType.EmptyCell && itsGrid[row, col + 1] < CellType.NorthNand)
            {
              // if the output cell contains a connection make sure it joins with the nodes output
              DirectionFlag directionFlag = Common.GetDirectionFlag(itsGrid[row, col + 1]);
              if ((directionFlag & DirectionFlag.West) == DirectionFlag.None)
              {
                nodesRemoved = ClearCell(nodesRemoved, row, col);
              }
              else
              {
                // there is a connection - check that this connection terminates somewhere useful
                if (TestConnectionResultsInNodeInput(row, col + 1, Direction.East) == false)
                {
                  // the output connection of this node didn't go anywhere useful
                  nodesRemoved = ClearCell(nodesRemoved, row, col);
                }
              }
            }
            break;

          case Direction.West:

            // test if the output cell is empty
            if (itsGrid[row, col - 1] == CellType.EmptyCell)
            {
              nodesRemoved = ClearCell(nodesRemoved, row, col);
            }
            // test if the output cell contains another node output
            else if (itsGrid[row, col - 1] >= CellType.NorthNand && itsGrid[row, col - 1] < CellType.ConnectionStart && GetNodeDirection(itsGrid[row, col - 1]) == Direction.East)
            {
              nodesRemoved = ClearCell(nodesRemoved, row, col);
            }
            // test if the output cell contains a connection
            else if (itsGrid[row, col - 1] > CellType.EmptyCell && itsGrid[row, col - 1] < CellType.NorthNand)
            {
              // if the output cell contains a connection make sure it joins with the nodes output
              DirectionFlag directionFlag = Common.GetDirectionFlag(itsGrid[row, col - 1]);
              if ((directionFlag & DirectionFlag.East) == DirectionFlag.None)
              {
                nodesRemoved = ClearCell(nodesRemoved, row, col);
              }
              else
              {
                // there is a connection - check that this connection terminates somewhere useful
                if (TestConnectionResultsInNodeInput(row, col - 1, Direction.West ) == false)
                {
                  // the output connection of this node didn't go anywhere useful
                  nodesRemoved = ClearCell(nodesRemoved, row, col);
                }
              }
            }
            break;
        }
      }
      return nodesRemoved;
    }


    public int ConnectionPasses { get; set; }

    /// <summary>
    /// Test that this connection or its branches end up as either inputs to a node or connecting to the bot ouput
    /// </summary>
    /// <param name="p"></param>
    /// <param name="col"></param>
    /// <param name="directionFlag"></param>
    /// <returns></returns>
    private bool TestConnectionResultsInNodeInput(int row, int col, Direction aOutputDirection)
    {
      return true;

      try
      {
        ConnectionPasses++;

        if( ConnectionPasses > 10 )
        {
          Console.WriteLine("Chromosome: " + itsChromosomeDecode.itsChromosome.ToBinaryString());
          return false;
        }


        if (row >= 1 && row < (kSideLength - 1) && col >= 1 && col < (kSideLength - 1))
        {
          // find the directions of this connection
          DirectionFlag directionFlag = Common.GetDirectionFlag(itsGrid[row, col]);

          // dont examine the connection in the direction from which we've come
          if (aOutputDirection != Direction.North && (directionFlag & DirectionFlag.North) != DirectionFlag.None)
          {
            // examine the cell above
            if (TestForInputToNodeOrBotOutput(row - 1, col, Direction.South))
            {
              return true;
            }
          }

          // dont examine the connection in the direction from which we've come
          if (aOutputDirection != Direction.South && (directionFlag & DirectionFlag.South) != DirectionFlag.None)
          {
            // examine the cell below
            if (TestForInputToNodeOrBotOutput(row + 1, col, Direction.North))
            {
              return true;
            }
          }

          // dont examine the connection in the direction from which we've come
          if (aOutputDirection != Direction.East && (directionFlag & DirectionFlag.East) != DirectionFlag.None)
          {
            // examine the cell to the right
            if (TestForInputToNodeOrBotOutput(row, col + 1, Direction.West))
            {
              return true;
            }
          }

          // dont examine the connection in the direction from which we've come
          if (aOutputDirection != Direction.West && (directionFlag & DirectionFlag.West) != DirectionFlag.None)
          {
            // examine the cell to the left
            if (TestForInputToNodeOrBotOutput(row, col - 1, Direction.East))
            {
              return true;
            }
          }
        }
      }
      catch
      {
        Console.WriteLine("Chromosome: " + itsChromosomeDecode.itsChromosome.ToBinaryString());
      }

      return false;
    }

    /// <summary>
    /// Test if the supplied target cell contains a node and this node doesn't have an output in the specified direction. 
    /// Or the target cell contains a bot output cell.
    /// If the target cell contains a connection, follow this connection to see if it forms a useful connection.
    /// </summary>
    /// <param name="targetRow"></param>
    /// <param name="targetCol"></param>
    /// <param name="aOutputDirection"></param>
    /// <returns></returns>
    private bool TestForInputToNodeOrBotOutput(int targetRow, int targetCol, Direction aOutputDirection)
    {
      if (targetRow >= 1 && targetRow <= (kSideLength - 1) && targetCol >= 1 && targetCol <= (kSideLength - 1))
      {
        // test if the cell contains a node
        if (itsGrid[targetRow, targetCol] == CellType.OutputCell || itsGrid[targetRow, targetCol] == CellType.ConnectionStart || itsGrid[targetRow, targetCol] == CellType.ConnectionEnd)
        {
          // the connection is into an output cell or a targetted connection
          return true;
        }
        else if (itsGrid[targetRow, targetCol] >= CellType.NorthNand && itsGrid[targetRow, targetCol] < CellType.ConnectionStart)
        {
          // make sure the connection isn't to the output of the attached node
          Direction direction = GetNodeDirection(itsGrid[targetRow, targetCol]);
          if (direction != aOutputDirection)
          {
            // the connection is into a node
            return true;
          }
        }
        else if (itsGrid[targetRow, targetCol] > CellType.EmptyCell && itsGrid[targetRow, targetCol] < CellType.NorthNand)
        {
          try
          {
            // the connection is into another connection
            if (TestConnectionResultsInNodeInput(targetRow, targetCol, aOutputDirection))
            {
              return true;
            }
          }
          catch
          {
            Console.WriteLine("Chromosome: " + itsChromosomeDecode.itsChromosome.ToBinaryString());
          }
        }
      }

      return false;
    }

    /// <summary>
    /// remove unnecessary edge connections and nodes
    /// </summary>
    /// <param name="nodesRemoved"></param>
    /// <param name="centerRow"></param>
    /// <param name="centerCol"></param>
    /// <param name="lastRow"></param>
    /// <param name="lastCol"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool RemoveEdgeConnectionsAndNodes(bool nodesRemoved, int centerRow, int centerCol, int lastRow, int lastCol, int row, int col)
    {
      if (row == 1 || row == (kSideLength - 2) || col == 1 || col == (kSideLength - 2))
      {
        // test if the cell contains a connection
        if (itsGrid[row, col] > CellType.EmptyCell && itsGrid[row, col] < CellType.NorthNand)
        {
          // make sure the connection isn't at one of the input cells
          if (((row == 1 && col == centerCol) || (row == lastRow && col == centerCol) || (col == 1 && row == centerRow) || (col == lastCol && row == centerRow)) == false)
          {
            // remove a single part of a connection if it points off grid
            if( PruneConnectionsOutOfGrid(lastRow, row, col) )
            {
              nodesRemoved = true;
            }

            // remove any connections that only point out of the grid
            nodesRemoved = RemoveConnectionsOutOfGrid(nodesRemoved, lastRow, row, col);
          }
        }

        // test if the cell contains a node
        if (itsGrid[row, col] >= CellType.NorthNand && itsGrid[row, col] < CellType.ConnectionStart)
        {
          // test node outputs

          CellType gridCell = itsGrid[row, col];
          Direction direction = GetNodeDirection(gridCell);

          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, direction);
        }
      }
      return nodesRemoved;
    }

    /// <summary>
    /// Prune connections that point out of the grid but that have more than 2 inputs or ouputs
    /// </summary>
    /// <param name="lastRow"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private bool PruneConnectionsOutOfGrid(int lastRow, int row, int col)
    {
      bool connectionPruned = false;
      CellType cellType = itsGrid[row, col];
      switch( cellType )
      {
        // T Connections
        case CellType.WestNorthEast:
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.North))
          {
            itsGrid[row, col] = CellType.WestEast;
            connectionPruned = true;
          }
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.West))
          {
            itsGrid[row, col] = CellType.NorthEast;
            connectionPruned = true;
          }
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.East))
          {
            itsGrid[row, col] = CellType.WestNorth;
            connectionPruned = true;
          }
          break;

        case CellType.EastSouthWest: 
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.West))
          {
            itsGrid[row, col] = CellType.EastSouth;
            connectionPruned = true;
          } 
          else if (RemoveEdgeNodes(false, lastRow, row, col, Direction.East))
          {
            itsGrid[row, col] = CellType.SouthWest;
            connectionPruned = true;
          }
          else if (RemoveEdgeNodes(false, lastRow, row, col, Direction.South))
          {
            itsGrid[row, col] = CellType.WestEast;
            connectionPruned = true;
          }
          break;

        case CellType.NorthEastSouth: 
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.North))
          {
            itsGrid[row, col] = CellType.EastSouth;
            connectionPruned = true;
          } 
          else if (RemoveEdgeNodes(false, lastRow, row, col, Direction.East))
          {
            itsGrid[row, col] = CellType.NorthSouth;
            connectionPruned = true;
          }
          else if (RemoveEdgeNodes(false, lastRow, row, col, Direction.South))
          {
            itsGrid[row, col] = CellType.NorthEast;
            connectionPruned = true;
          }
          break;

        case CellType.SouthWestNorth: 
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.North))
          {
            itsGrid[row, col] = CellType.SouthWest;
            connectionPruned = true;
          }
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.West))
          {
            itsGrid[row, col] = CellType.NorthSouth;
            connectionPruned = true;
          }
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.South))
          {
            itsGrid[row, col] = CellType.WestNorth;
            connectionPruned = true;
          }
          break;

        // Full Connection
        case CellType.NorthEastSouthWest: 
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.North))
          {
            itsGrid[row, col] = CellType.EastSouthWest;
            connectionPruned = true;
          }
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.East))
          {
            itsGrid[row, col] = CellType.SouthWestNorth;
            connectionPruned = true;
          }
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.South))
          {
            itsGrid[row, col] = CellType.WestNorthEast;
            connectionPruned = true;
          }
          if (RemoveEdgeNodes(false, lastRow, row, col, Direction.West))
          {
            itsGrid[row, col] = CellType.NorthEastSouth;
            connectionPruned = true;
          }
          break;
      }     
 
      return connectionPruned;
    }

    private bool RemoveConnectionsOutOfGrid(bool nodesRemoved, int lastRow, int row, int col)
    {
      switch (itsGrid[row, col])
      {
        // Straight Connections
        case CellType.NorthSouth:
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.North);
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.South);
          break;
        case CellType.WestEast:
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.East);
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.West);
          break;

        // T Connections
        //case CellType.WestNorthEast: return '┴';
        //case CellType.EastSouthWest: return '┬';
        //case CellType.NorthEastSouth: return '├';
        //case CellType.SouthWestNorth: return '┤';

        // L Connections
        case CellType.NorthEast:
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.North);
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.East);
          break;

        case CellType.EastSouth:
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.East);
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.South);
          break;

        case CellType.SouthWest:
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.West);
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.South);
          break;

        case CellType.WestNorth:
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.North);
          nodesRemoved = RemoveEdgeNodes(nodesRemoved, lastRow, row, col, Direction.West);
          break;

        // Full Connection
        //case CellType.NorthEastSouthWest: return '┼';
      }
      return nodesRemoved;
    }

    /// <summary>
    /// Remove the contents of cells whose output direction points out of the grid at the edges
    /// </summary>
    /// <param name="nodesRemoved"></param>
    /// <param name="lastRow"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool RemoveEdgeNodes(bool nodesRemoved, int lastRow, int row, int col, Direction direction)
    {
      // remove any nodes that point out of the sides of the grid
      if (row == 1 && direction == Direction.North)
      {
        nodesRemoved = ClearCell(nodesRemoved, row, col);
      }
      else if (row == lastRow && direction == Direction.South)
      {
        nodesRemoved = ClearCell(nodesRemoved, row, col);
      }
      else if (col == 1 && direction == Direction.West)
      {
        nodesRemoved = ClearCell(nodesRemoved, row, col);
      }
      else if (col == lastRow && direction == Direction.East)
      {
        nodesRemoved = ClearCell(nodesRemoved, row, col);
      }
      return nodesRemoved;
    }

    private bool ClearCell(bool nodesRemoved, int row, int col)
    {
      itsGrid[row, col] = CellType.EmptyCell;
      nodesRemoved = true;
      return nodesRemoved;
    }

    /// <summary>
    /// Get the direction of the specified node
    /// </summary>
    /// <param name="gridCell"></param>
    private static Direction GetNodeDirection(CellType gridCell)
    {
      return (Direction)((int)gridCell % 4);
    }

    /// <summary>
    /// Process all the cells in the list
    /// - new cells can be added to this list during processing
    /// </summary>
    private void ExpandCells()
    {
      BotDraw botDraw = new BotDraw(this);

      // set the current wave of cell processing to be the level of the first node
      int lastCellLevel = ((itsCellsToProcess.Count > 0) ? itsCellsToProcess.ElementAt(0).itsLevel : 0);

      // the maximum number of growth stages allowed for a bot
      int kMaxCellLevel = 100;


#if DRAW_GRID_STAGES
      // draw the state of the grid at the start
      botDraw.CreateGridImage("c:\\grid_0" + ".bmp"); 
#endif

      while (itsCellsToProcess.Count > 0 && lastCellLevel < kMaxCellLevel)
      {
        // get and process the first cell in the list
        var cell = itsCellsToProcess.ElementAt(0);

        // if the new cell marks the start of the next level of cells to process 
        // terminate the last level of processing by adding all the stored targeted connections
        if (cell.itsLevel > lastCellLevel)
        {
          AddTargetedConnectionsToGrid();

          #if DRAW_GRID_STAGES
          // draw the state of the grid at the end of each level
          botDraw.CreateGridImage("c:\\grid_" + lastCellLevel + ".bmp");         
          #endif
        }
        
        // create the outputs for this cell
        ExpandCell(cell);               

        // remove this cell from the list of cells to process
        itsCellsToProcess.RemoveAt(0);  

        // keep track of the current level of cell processing
        lastCellLevel = cell.itsLevel;
      }

      // if all cells have been expanded add any remaining targeted connections and then expand any cells they might have added
      if (itsConnectionsToProcess.Count > 0)
      {
        AddTargetedConnectionsToGrid();
        ExpandCells();
      }      
    }

    private void ExpandCell(BotCellToProcess botCellToProcess)
    {
      // test if the cell to process contains a targeted connection
      if( botCellToProcess.itsCellInfo.itsType == CellType.ConnectionStart )
      {
        AddTargetedConnection(botCellToProcess.itsLevel, new Point(botCellToProcess.itsRow, botCellToProcess.itsCol), botCellToProcess.itsDirection, botCellToProcess.itsCellInfo);
      }
      else
      {
        CellInformation[] cellInfo = new CellInformation[3];
        int ruleCount = itsChromosomeDecode.GetRule(botCellToProcess.itsCellInfo.itsID, cellInfo);

        SetCellOutput(botCellToProcess.itsRow, botCellToProcess.itsCol, botCellToProcess.itsDirection, ruleCount, cellInfo, botCellToProcess.itsLevel);
      }
    }

    /// <summary>
    /// Start of the development of the grid by creating the outputs for the input cells
    /// </summary>
    private void SetInputCells()
    {
      int middleCell = (kSideLength/2);
      int lastCell = (kSideLength - 1);

      // top row input cell
      ExpandInputConnection(0, middleCell, Direction.North);

      // right col input cell
      ExpandInputConnection(middleCell, lastCell, Direction.East);

      // bottom row input cell
      ExpandInputConnection( lastCell, middleCell, Direction.South);

      // left col input cell
      ExpandInputConnection(middleCell, 0, Direction.West);
    }

    /// <summary>
    /// Expand the input grid cell, which is located at the specified position on the grid
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="aInputDirection"></param>
    private void ExpandInputConnection( int row, int col, Direction aInputDirection)
    {
      // get the input cell rule
      // - this is defined as the first rule in the chromosome
      CellInformation[] cellInfo = new CellInformation[3];

      CellInformation cellInformation = new CellInformation();
      CellType cellType = CellType.EmptyCell;
      int ruleIndex = 0;
      int ruleCount = itsChromosomeDecode.GetInputRule(cellInfo, aInputDirection, out cellType, out ruleIndex);

      cellInformation.itsType = cellType;
      cellInformation.itsID = ruleIndex;

      // the direction of the connection coming from the input cell will be in the opposite direction to
      // where the input cell is located on the grid e.g. the north input cell (at the top of the grid) will
      // have a connection that points south
      Direction outputDirection = Direction.South;
      switch( aInputDirection)
      {
        case Direction.North: outputDirection = Direction.South; break;
        case Direction.East: outputDirection = Direction.West; break;
        case Direction.South: outputDirection = Direction.North; break;
        case Direction.West: outputDirection = Direction.East; break;
      }   
      
      // if the input doesn't specify an empty cell as its target, move to the actual grid square belonging to the input
      if( cellType > CellType.EmptyCell )
      {
        switch (aInputDirection)
        {
          case Direction.North: row++; break;
          case Direction.East: col--;  break;
          case Direction.South: row--; break;
          case Direction.West: col++;  break;
        }
        
        // put the specified node type into the grid (targeted connections will be done by the ExpandCell routine)
        if( cellType != CellType.ConnectionStart )
        {
          //SetGridCell( row, col, cellType,
          Direction newDirection = outputDirection;
          SetCell(row, col, cellType, outputDirection,Movement.Forward,ref newDirection);            
        }
      }

      BotCellToProcess botCellToProcess = new BotCellToProcess(row, col, outputDirection, cellInformation, 0);
      ExpandCell(botCellToProcess);
    }

    /// <summary>
    /// Add the output cells for the specified grid cell using the supplied rule
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <param name="aDirection"></param>
    /// <param name="ruleCount"></param>
    /// <param name="aCellInfo"></param>
    /// <param name="aNodeLevel"></param>
    private void SetCellOutput(int aRow, int aCol, Direction aDirection, int ruleCount, CellInformation[] aCellInfo, int aNodeLevel)
    {
      Point left;
      Point forward;
      Point right;
      Point center;
      GetOutputCellPositions(aRow, aCol, aDirection, out left, out forward, out right, out center);

      if (ruleCount > 0)
      {
        if (ruleCount == 1)
        {
          // the current cell only has one output rule
          AddSingleOutput(aDirection, aCellInfo, aNodeLevel, center);
        }
        else
        {
          // the current cell has more than one output
          AddMultipleOutputs(aDirection, aCellInfo, aNodeLevel, ref left, ref forward, ref right, ref center);
        }
      }
    }

    /// <summary>
    /// Only a single output is defined for this cell. Place this in the central output cell.
    /// </summary>
    /// <param name="aDirection"></param>
    /// <param name="aCellInfo"></param>
    /// <param name="aNodeLevel"></param>
    /// <param name="center"></param>
    /// <returns></returns>
    private void AddSingleOutput(Direction aDirection, CellInformation[] aCellInfo, int aNodeLevel, Point center)
    {
      Direction direction = aDirection;
      Direction newDirection = aDirection;      
      CellInformation? centerCellInformation = null;

      //if (aCellInfo[0].itsType > CellType.EmptyCell)
      if( aCellInfo[0].itsID > 0 )
      {
        // the rule only defines a left output
        centerCellInformation = AddSingleOutputConnection(aDirection, aCellInfo, 0, aNodeLevel, ref center, ref newDirection);
        if (centerCellInformation != null)
        {
          // set the direction for a LH node          
          if (direction == Direction.North)
          {
            direction = Direction.West;
          }
          else
          {
            direction = direction - 1;
          }
        }
      }
      else //if (aCellInfo[1].itsType > CellType.EmptyCell)
        if( aCellInfo[1].itsID > 0 )
      {
        // the rule only defines a forward output
        centerCellInformation = AddSingleOutputConnection(aDirection, aCellInfo, 1, aNodeLevel, ref center, ref newDirection);
      }
      else //if (aCellInfo[2].itsType > CellType.EmptyCell)
          if( aCellInfo[2].itsID > 0 )
      {
        // the rule only defines a right output
        centerCellInformation = AddSingleOutputConnection(aDirection, aCellInfo, 2, aNodeLevel, ref center, ref newDirection);
        if( centerCellInformation != null )        
        {
          // set the direction for a RH node
          if (direction == Direction.West)
          {
            direction = Direction.North;
          }
          else
          {
            direction = direction + 1;
          }
        }
      }

      // only a single cell is to be set            
      if (centerCellInformation != null)
      {
        // test for a targeted connection
        //if (((CellInformation)centerCellInformation).itsType == CellType.WestEast)
        if (((CellInformation)centerCellInformation).itsType == CellType.ConnectionStart)
        {
          AddTargetedConnection(aNodeLevel, center, direction, (CellInformation)centerCellInformation);
        }
        else if (SetCenterCell(center.X, center.Y, ((CellInformation)centerCellInformation).itsType, direction, ref newDirection))
        {
          // if the cell has been added then apply the rule to this
          ApplyRules(center.X, center.Y, newDirection, (CellInformation)centerCellInformation, aNodeLevel + 1);
        }
      }
    }


    /// <summary>
    /// Set up a connection from a cell with a single output
    /// </summary>
    /// <param name="aDirection"></param>
    /// <param name="aCellInfo"></param>
    /// <param name="aIndex"></param>
    /// <param name="aNodeLevel"></param>
    /// <param name="aCenter"></param>
    /// <param name="aNewDirection"></param>
    /// <returns></returns>
    private CellInformation? AddSingleOutputConnection(Direction aDirection, CellInformation[] aCellInfo, int aIndex, int aNodeLevel, ref Point aCenter, ref Direction aNewDirection)
    {
      // test if the specified output is a connection
      if (aCellInfo[aIndex].itsType == CellType.NorthSouth || (aCellInfo[aIndex].itsType == CellType.EmptyCell && aCellInfo[aIndex].itsID > 0 ) )
      {
        // the cell output only contains a connection                          
        if (SetConnectionCell(aCenter.X, aCenter.Y, aCellInfo, aDirection, ref aNewDirection, aNodeLevel))
        {
          // if the cell has been added then apply the rule to this
          ApplyRules(aCenter.X, aCenter.Y, aNewDirection, aCellInfo[aIndex], aNodeLevel + 1);
        }

        // no node needs to be added
        return null;
      }

      // return the cell information
      return aCellInfo[aIndex];
    }

    /// <summary>
    /// Add multiple outputs to the current cell
    /// </summary>
    /// <param name="aDirection">the direction of the current cells output</param>
    /// <param name="aCellInfo">the rules defining the output</param>
    /// <param name="aNodeLevel">the stage in processing of this node. This increases with each step away from the input nodes</param>
    /// <param name="left">the coordinates of the left hand output node</param>
    /// <param name="forward">the coordinates of the central output node</param>
    /// <param name="right">the coordinates of the right hand output node</param>
    /// <param name="center">the coordinates of the central connector node</param>
    private void AddMultipleOutputs(Direction aDirection, CellInformation[] aCellInfo, int aNodeLevel, ref Point left, ref Point forward, ref Point right, ref Point center)
    {
      // if the rules central cell isn't valid don't expand any of the rule
      if (TestGridCellValid(center.X, center.Y))
      {
        // test if any of the rules would place the output off the grid
        TestOutputCellValid(aCellInfo, 0, left);
        TestOutputCellValid(aCellInfo, 1, center);
        TestOutputCellValid(aCellInfo, 2, right);

        // add connections to the cells that are about to be added
        // - the connector is placed on the direct output of the current cell
        Direction newDirection = aDirection;
        bool connectionsAdded = SetConnectionCell(center.X, center.Y, aCellInfo, aDirection, ref newDirection, aNodeLevel);

        
        if (aCellInfo[0].itsType == CellType.ConnectionStart)
        {
          // cell contains a targeted connection
          AddTargetedConnection(aNodeLevel, new Point(left.X, left.Y), GetDirection(aDirection, Movement.Left), aCellInfo[0]);
        }
        // add connection and the specified cell type
        else if (aCellInfo[0].itsType >= CellType.NorthNand || aCellInfo[0].itsType == CellType.NorthSouth)        
        {
          if (SetLeftCell(left.X, left.Y, aCellInfo[0].itsType, aDirection, ref newDirection))
          {
            // if the cell has been added then apply the rule to this
            ApplyRules(left.X, left.Y, newDirection, aCellInfo[0], aNodeLevel + 1);
          }
        }
        else if( aCellInfo[0].itsID > 0 ) // empty cell but will a rule is specified so only add a connection
        {
          // output is an empty cell connection, so rule is applied to current central output cell
          ApplyRules( center.X, center.Y, GetDirection( aDirection, Movement.Left ), aCellInfo[0], aNodeLevel + 1 );
        }

        if (aCellInfo[1].itsType == CellType.ConnectionStart)
        {
          // cell contains a targeted connection
          AddTargetedConnection(aNodeLevel, new Point(forward.X, forward.Y), aDirection, aCellInfo[1]);
        }
        // add connection and the specified cell type
        else if (aCellInfo[1].itsType >= CellType.NorthNand || aCellInfo[1].itsType == CellType.NorthSouth)
        {
          if (SetCenterCell(forward.X, forward.Y, aCellInfo[1].itsType, aDirection, ref newDirection))
          {
            // if the cell has been added then apply the rule to this
            ApplyRules(forward.X, forward.Y, newDirection, aCellInfo[1], aNodeLevel + 1);
          }
        }
        else if( aCellInfo[1].itsID > 0 ) // empty cell but will a rule is specified so only add a connection
        {
          // output is an empty cell connection, so rule is applied to current central output cell
          ApplyRules( center.X, center.Y, GetDirection( aDirection, Movement.Forward ), aCellInfo[1], aNodeLevel + 1 );
        }
        

        if (aCellInfo[2].itsType == CellType.ConnectionStart)
        {
          // cell contains a targeted connection
          AddTargetedConnection(aNodeLevel, new Point(right.X, right.Y), GetDirection(aDirection,Movement.Right), aCellInfo[2]);
        }
        // add connection and the specified cell type
        else if (aCellInfo[2].itsType >= CellType.NorthNand || aCellInfo[2].itsType == CellType.NorthSouth)        
        {
          if (SetRightCell(right.X, right.Y, aCellInfo[2].itsType, aDirection, ref newDirection))
          {
            // if the cell has been added then apply the rule to this
            ApplyRules(right.X, right.Y, newDirection, aCellInfo[2], aNodeLevel + 1);
          }
        }
        else if( aCellInfo[2].itsID > 0 ) // empty cell but will a rule is specified so only add a connection
        {
          // output is an empty cell connection, so rule is applied to current central output cell
          ApplyRules( center.X, center.Y, GetDirection( aDirection, Movement.Right ), aCellInfo[2], aNodeLevel + 1 );          
        }
        
      }
    }

    /// <summary>
    /// calculate the new direction given the current direction and the required direction of movement
    /// </summary>
    /// <param name="aDirection"></param>
    /// <param name="aMovement"></param>
    /// <returns></returns>
    private Direction GetDirection(Direction aDirection, Movement aMovement)
    {
      Direction direction = aDirection;
      if (aMovement == Movement.Left)
      {
        if (direction == Direction.North)
        {
          direction = Direction.West;
        }
        else
        {
          direction = direction - 1;
        }
      }
      else if (aMovement == Movement.Right)
      {
        if (direction == Direction.West)
        {
          direction = Direction.North;
        }
        else
        {
          direction = direction + 1;
        }
      }

      return direction;
    }

    /// <summary>
    /// test that the supplied rule is defined and that it will place its output in a valid location
    /// - delete the rule if it wont
    /// </summary>
    /// <param name="aCellInfo"></param>
    /// <param name="aPosition"></param>
    private void TestOutputCellValid(CellInformation[] aCellInfo, int aRuleIndex, Point aPosition)
    {
      if (!TestGridCellValid(aPosition.X, aPosition.Y))
      {
        aCellInfo[aRuleIndex].itsType = CellType.EmptyCell;
      }
    }

    /// <summary>
    /// apply the rules to create the output for the specified source cell
    /// </summary>
    /// <param name="rules"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void ApplyRules(int aRow, int aCol, Direction aDirection, CellInformation aCellInformation, int aNodeLevel)
    {
      // add the cell to be expanded to the list of cells still to process
      itsCellsToProcess.Add( new BotCellToProcess(aRow, aCol, aDirection, aCellInformation, aNodeLevel));
    }


    /// <summary>
    /// test if the specified grid cell is ok to put a new node into
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool TestGridCellValid(int aRow, int aCol)
    {
      int centerRow = kSideLength / 2;
      int centerCol = kSideLength / 2;

      // cells can not be added to the edge cells nor to the central cells
      if ((aRow > 0 && aRow < (kSideLength - 1))
      && (aCol > 0 && aCol < (kSideLength - 1))
      && !(aRow == centerRow && (aCol >= (centerCol - 1) && aCol <= (centerCol + 1)))
      && !(aCol == centerCol && (aRow >= (centerRow - 1) && aRow <= (centerRow + 1)))
      && itsGrid[aRow, aCol] == CellType.EmptyCell)
      {
        return true;
      }

      return false;
    }


    /// <summary>
    /// check that the specified location is on the grid and that the target cell is unoccupied
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <param name="cellType"></param>
    /// <returns></returns>
    private bool SetGridCell(int aRow, int aCol, CellType cellType)
    {
      // cells can not be added to the edge cells nor to the central cells
      if( TestGridCellValid(aRow, aCol))
      {
        itsGrid[aRow, aCol] = cellType;
        return true;
      }

      return false;
    }

    private bool SetLeftCell(int aRow, int aCol, CellType aCellType, Direction aDirection, ref Direction aNewDirection)
    {
      return SetCell(aRow, aCol, aCellType, aDirection, Movement.Left, ref aNewDirection);
    }

    private bool SetRightCell(int aRow, int aCol, CellType aCellType, Direction aDirection, ref Direction aNewDirection)
    {
      return SetCell(aRow, aCol, aCellType, aDirection, Movement.Right, ref aNewDirection);
    }

    private bool SetCenterCell(int aRow, int aCol, CellType aCellType, Direction aDirection, ref Direction aNewDirection)
    {
      return SetCell(aRow, aCol, aCellType, aDirection, Movement.Forward, ref aNewDirection);
    }


    private bool SetCell(int aRow, int aCol, CellType centerCellType, Direction aDirection, Movement aMovement, ref Direction aNewDirection)
    {
      // set the direction in which the new cell type is pointing
      aNewDirection = GetDirection( aDirection, aMovement );

      CellType cellType = centerCellType;

      // if this is a standard node type modify the type to reflect the direction
      // - if its the start of a targetted connection don't apply the direction
      if( cellType != CellType.ConnectionStart )
      {
        cellType += (int)aNewDirection;
      }

      // test if the cell being set is a connection
      if( centerCellType == CellType.NorthSouth )
      {
        if( aNewDirection == Direction.North || aNewDirection == Direction.South )
        {
          cellType = CellType.NorthSouth;
        }
        else
        {
          cellType = CellType.WestEast;
        }
      }

      return SetGridCell( aRow, aCol, cellType );
    }

    private bool SetConnectionCell(int aRow, int aCol, CellInformation[] aCellInfo, Direction aDirection, ref Direction aNewDirection, int aNodeLevel)
    {
      // test if the target cell already contains a connection
      // - if it does then join onto this
      if( itsGrid[aRow,aCol] > CellType.EmptyCell && itsGrid[aRow,aCol] < CellType.NorthNand )
      {
        CellType currentType = itsGrid[aRow, aCol];

        // get the directions of the current connection
        DirectionFlag currentDirections = Common.GetDirectionFlag(itsGrid[aRow, aCol]);

        // add the direction from the new connection
        switch (aDirection)
        {
          case Direction.North:
            currentDirections |= DirectionFlag.South;                       
            break;
          case Direction.East:             
            currentDirections |= DirectionFlag.West;                       
            break;
          case Direction.South:             
            currentDirections |= DirectionFlag.North;                       
            break;
          case Direction.West:             
            currentDirections |= DirectionFlag.East;                       
            break;
        }

        // place the new connection into the grid
        itsGrid[aRow, aCol] = Common.GetConnectionFromDirectionFlag( currentDirections );
      }
      else
      { 
        // test if all the output cells are occupied
        if (aCellInfo[0].itsType != CellType.EmptyCell
         && aCellInfo[1].itsType != CellType.EmptyCell
         && aCellInfo[2].itsType != CellType.EmptyCell)
        {
          return SetGridCell(aRow, aCol, CellType.NorthEastSouthWest);
        }
        else
        {
          Direction[] direction = new Direction[4];


          // array of connections that need to be expanded
          Direction[] conectionDirection = new Direction[3];
          bool[] connectionSet = new bool[3];


          int directionCount = 0;

          // always require a connection into the source cell
          // - this is in the opposite direction to the output of the source
          switch (aDirection)
          {
            case Direction.North: direction[directionCount] = Direction.South; break;
            case Direction.East: direction[directionCount] = Direction.West; break;
            case Direction.South: direction[directionCount] = Direction.North; break;
            case Direction.West: direction[directionCount] = Direction.East; break;
          }
          directionCount++;

          Movement movement = Movement.Left;
          if (aCellInfo[(int)movement].itsType != CellType.EmptyCell || aCellInfo[(int)movement].itsID != 0)                    
          {
            // add a connection to the left cell
            switch (aDirection)
            {
              case Direction.North: direction[directionCount] = Direction.West; break;
              case Direction.East: direction[directionCount] = Direction.North; break;
              case Direction.South: direction[directionCount] = Direction.East; break;
              case Direction.West: direction[directionCount] = Direction.South; break;
            }

            if (aCellInfo[(int)movement].itsType == CellType.NorthSouth || aCellInfo[(int)movement].itsType == CellType.EmptyCell)
            {
              connectionSet[(int)Movement.Left] = true;
              conectionDirection[(int)Movement.Left] = direction[directionCount];
            }

            directionCount++;
          }

          movement = Movement.Forward;
          if ( aCellInfo[(int)movement].itsType != CellType.EmptyCell || aCellInfo[(int)movement].itsID != 0)
          {
            // add a connection to the forward cell
            direction[directionCount] = aDirection;

            if (aCellInfo[(int)movement].itsType == CellType.NorthSouth || aCellInfo[(int)movement].itsType == CellType.EmptyCell)
            {
              connectionSet[(int)Movement.Forward] = true;
              conectionDirection[(int)Movement.Forward] = direction[directionCount];
            }

            directionCount++;
          }

          movement = Movement.Right;          
          if (aCellInfo[(int)movement].itsType != CellType.EmptyCell || aCellInfo[(int)movement].itsID != 0)
          {
            // add a connection to the right cell
            switch (aDirection)
            {
              case Direction.North: direction[directionCount] = Direction.East; break;
              case Direction.East: direction[directionCount] = Direction.South; break;
              case Direction.South: direction[directionCount] = Direction.West; break;
              case Direction.West: direction[directionCount] = Direction.North; break;
            }

            if (aCellInfo[(int)movement].itsType == CellType.NorthSouth || aCellInfo[(int)movement].itsType == CellType.EmptyCell)
            {
              connectionSet[(int)Movement.Right] = true;
              conectionDirection[(int)Movement.Right] = direction[directionCount];
            }

            directionCount++;
          }

          // now have all the directions that need to be included in the connector cell
          // - this should always be 2 or greater, otherwise the cell has no outputs
          if (directionCount >= 2)
          {
            CellType connectionType = CellType.EmptyCell;
            switch (direction[0])
            {
              case Direction.North:
                switch (direction[1])
                {
                  case Direction.East:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.South: connectionType = CellType.NorthEastSouth; break;
                        case Direction.West: connectionType = CellType.WestNorthEast; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.NorthEast;
                    }
                    break;
                  case Direction.South:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.East: connectionType = CellType.NorthEastSouth; break;
                        case Direction.West: connectionType = CellType.SouthWestNorth; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.NorthSouth;
                    }
                    break;
                  case Direction.West:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.East: connectionType = CellType.WestNorthEast; break;
                        case Direction.South: connectionType = CellType.SouthWestNorth; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.WestNorth;
                    }
                    break;
                }
                break;

              case Direction.East:
                switch (direction[1])
                {
                  case Direction.North:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.South: connectionType = CellType.NorthEastSouth; break;
                        case Direction.West: connectionType = CellType.WestNorthEast; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.NorthEast;
                    }
                    break;
                  case Direction.South:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.North: connectionType = CellType.NorthEastSouth; break;
                        case Direction.West: connectionType = CellType.EastSouthWest; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.EastSouth;
                    }
                    break;
                  case Direction.West:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.North: connectionType = CellType.WestNorthEast; break;
                        case Direction.South: connectionType = CellType.EastSouthWest; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.WestEast;
                    }
                    break;
                }
                break;

              case Direction.South:
                switch (direction[1])
                {
                  case Direction.North:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.East: connectionType = CellType.NorthEastSouth; break;
                        case Direction.West: connectionType = CellType.SouthWestNorth; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.NorthSouth;
                    }
                    break;
                  case Direction.East:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.North: connectionType = CellType.NorthEastSouth; break;
                        case Direction.West: connectionType = CellType.EastSouthWest; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.EastSouth;
                    }
                    break;
                  case Direction.West:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.North: connectionType = CellType.SouthWestNorth; break;
                        case Direction.East: connectionType = CellType.EastSouthWest; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.SouthWest;
                    }
                    break;
                }
                break;

              case Direction.West:
                switch (direction[1])
                {
                  case Direction.North:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.East: connectionType = CellType.WestNorthEast; break;
                        case Direction.South: connectionType = CellType.SouthWestNorth; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.WestNorth;
                    }
                    break;
                  case Direction.East:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.North: connectionType = CellType.WestNorthEast; break;
                        case Direction.South: connectionType = CellType.EastSouthWest; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.WestEast;
                    }
                    break;
                  case Direction.South:
                    if (directionCount == 3)
                    {
                      switch (direction[2])
                      {
                        case Direction.North: connectionType = CellType.SouthWestNorth; break;
                        case Direction.East: connectionType = CellType.EastSouthWest; break;
                      }
                    }
                    else
                    {
                      connectionType = CellType.SouthWest;
                    }
                    break;
                }
                break;
            }

            // set the new direction to which the connection is pointing
            aNewDirection = direction[1];

            if (SetGridCell(aRow, aCol, connectionType))
            {
              // grow the outputs for any of the connections that have an output cell type of empty (but have rules)
              // - not sure why this isn't done on return?
              for (int connections = 0; connections < 3; connections++)
              {
                if (connectionSet[connections])
                {
                  // only apply the rules for output connections that dont have a node type defined
                  // - these rules specify outputs but no cell types so dont move the position in the grid
                  // e.g. if we have A -> B-C-X, where C's type is defined as the empty cell and has the rule C -> D-E-X
                  // then the output would look as follows:
                  //
                  //   BD
                  //  A┴┴E
                  //
                  // if instead C had a type of "local connection" it would appear as:
                  //
                  //   B D
                  //  A┴─┴E
                  //
                  if (aCellInfo[connections].itsType == CellType.EmptyCell)
                  {
                    ApplyRules(aRow, aCol, conectionDirection[connections], aCellInfo[connections], aNodeLevel + 1);
                  }
                }
              }

              return true;
            }
          }
        }
      }

      return false;
    }


    /// <summary>
    /// calculate the output celll positions for the given cell and direction 
    /// 
    ///           aForward
    ///              |
    ///   aLeft - aCenter - aRight
    ///     
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <param name="aDirection"></param>
    /// <param name="aLeft"></param>
    /// <param name="aForward"></param>
    /// <param name="aRight"></param>
    /// <param name="aCenter"></param>
    private void GetOutputCellPositions(int aRow, int aCol, Direction aDirection, out Point aLeft, out Point aForward, out Point aRight, out Point aCenter )
    {
      int centerCellRow = 0;
      int centerCellCol = 0;
      int leftCellRow = 0;
      int leftCellCol = 0;
      int rightCellRow = 0;
      int rightCellCol = 0;
      int forwardCellRow = 0;
      int forwardCellCol = 0;
      switch (aDirection)
      {
        case Direction.North:
          centerCellRow = aRow - 1;
          centerCellCol = aCol;
          leftCellRow = centerCellRow;
          leftCellCol = centerCellCol - 1;
          rightCellRow = centerCellRow;
          rightCellCol = centerCellCol + 1;
          forwardCellRow = centerCellRow - 1;
          forwardCellCol = centerCellCol;
          break;
        case Direction.East:
          centerCellRow = aRow;
          centerCellCol = aCol + 1;
          leftCellRow = centerCellRow - 1;
          leftCellCol = centerCellCol;
          rightCellRow = centerCellRow + 1;
          rightCellCol = centerCellCol;
          forwardCellRow = centerCellRow;
          forwardCellCol = centerCellCol + 1;
          break;
        case Direction.South:
          centerCellRow = aRow + 1;
          centerCellCol = aCol;
          leftCellRow = centerCellRow;
          leftCellCol = centerCellCol + 1;
          rightCellRow = centerCellRow;
          rightCellCol = centerCellCol - 1;
          forwardCellRow = centerCellRow + 1;
          forwardCellCol = centerCellCol;
          break;
        case Direction.West:
          centerCellRow = aRow;
          centerCellCol = aCol - 1;
          leftCellRow = centerCellRow + 1;
          leftCellCol = centerCellCol;
          rightCellRow = centerCellRow - 1;
          rightCellCol = centerCellCol;
          forwardCellRow = centerCellRow;
          forwardCellCol = centerCellCol - 1;
          break;
      }

      aLeft = new Point(leftCellRow,leftCellCol);
      aForward = new Point(forwardCellRow, forwardCellCol);
      aRight = new Point(rightCellRow, rightCellCol);
      aCenter = new Point(centerCellRow, centerCellCol);      
    }

  }
}
