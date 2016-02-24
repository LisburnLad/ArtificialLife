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
  public partial class BotStructure
  {


    #region Targeted Connections

    /// <summary>
    /// Process all targeted connections in the list of connections from the current level
    /// </summary>
    private void AddTargetedConnectionsToGrid()
    {
      while (itsConnectionsToProcess.Count > 0)
      {
        // get and process the first connection
        ConnectionsToProcess connection = itsConnectionsToProcess.ElementAt(0);
        ProcessTargetedConnection(connection.itsLevel, connection.itsConnectionStart, connection.itsDirection, connection.itsCellInfo);

        // remove this connection from the list
        itsConnectionsToProcess.RemoveAt(0);
      }
    }

    /// <summary>
    /// Add a targeted connection to the list of connections still to process
    /// </summary>
    /// <param name="aNodeLevel"></param>
    /// <param name="aConnectionStart"></param>
    /// <param name="aDirection"></param>
    /// <param name="aStartCellInformation"></param>
    private void AddTargetedConnection(int aNodeLevel, Point aConnectionStart, Direction aDirection, CellInformation aStartCellInformation)
    {
      itsConnectionsToProcess.Add(new ConnectionsToProcess(aNodeLevel, aConnectionStart, aDirection, aStartCellInformation));
    }

    /// <summary>
    /// Add a targeted connection to the grid
    /// </summary>
    /// <param name="aNodeLevel"></param>
    /// <param name="aConnectionStart"></param>
    /// <param name="aDirection"></param>
    /// <param name="aStartCellInformation"></param>
    private void ProcessTargetedConnection(int aNodeLevel, Point aConnectionStart, Direction aDirection, CellInformation aStartCellInformation)
    {
      // get the targeted connection rule
      TargetedConnection targetedConnection = itsChromosomeDecode.GetTargetedConnection(aStartCellInformation.itsID);

      // calculate the distance from the current point to the edge of the grid given the connection direction
      int distanceToEdge = GetDistanceToEdge(ref aConnectionStart, aDirection);

      // test that the start of the targeted connection isn't at the edge of the grid (when pointing off the grid)
      // and that the cell for the start of the connection isn't already occupied
      if (distanceToEdge > 0 
      && (itsGrid[aConnectionStart.X, aConnectionStart.Y] == CellType.ConnectionStart || TestGridCellValid(aConnectionStart.X, aConnectionStart.Y)))
      {
        DirectedConnection connection = new DirectedConnection();
        connection.itsStart = aConnectionStart;

        double cellsPerArea;
        int startTargetArea;
        int endTargetArea;
        CalculateTargetAreaBounds(targetedConnection, distanceToEdge, out cellsPerArea, out startTargetArea, out endTargetArea);

        // begin looking for a target node
        bool targetFound = LookForTargetNode(ref aConnectionStart, aDirection, targetedConnection, connection, startTargetArea, endTargetArea );

        // if the target has been found, connect directly to this
        if (targetFound)
        {
          AddTargetedConnectionToGrid(aConnectionStart, connection);
        }
        else
        {
          CellType targetType = itsChromosomeDecode.GetCellType(targetedConnection.Rule);

          // if the target type is NorthSouth, this indicates a targeted connection for the output cells, and this can't be added to the end of a targeted connection
          if (targetType != CellType.NorthSouth)
          {
            // when a target node isn't found create one in the center of the target area       
            AddTargetConnectionWithNode(aNodeLevel, aConnectionStart, aDirection, targetedConnection, distanceToEdge, connection, cellsPerArea, startTargetArea);
          }
        }
      }
    }

    /// <summary>
    /// Create a targeted connection with its own target node
    /// This is called when the specified target has not been found
    /// </summary>
    /// <param name="aNodeLevel"></param>
    /// <param name="aConnectionStart"></param>
    /// <param name="aDirection"></param>
    /// <param name="aTargetedConnection"></param>
    /// <param name="aDistanceToEdge"></param>
    /// <param name="aConnection"></param>
    /// <param name="aCellsPerArea"></param>
    /// <param name="aStartTargetArea"></param>
    /// <returns></returns>
    private void AddTargetConnectionWithNode( int aNodeLevel, 
                                               Point aConnectionStart, 
                                               Direction aDirection, 
                                               TargetedConnection aTargetedConnection, 
                                               int aDistanceToEdge, 
                                               DirectedConnection aConnection, 
                                               double aCellsPerArea, 
                                               int aStartTargetArea )
    {
      // calculate the mid-point of the target area
      int middleTargetArea = aStartTargetArea + (int)(aCellsPerArea / 2.0) + 1;

      // if the targeted connection specifies a left or right direction place its generated target in that direction
      if (aTargetedConnection.itsDirection == (int)RelativeDirection.Left || aTargetedConnection.itsDirection == (int)RelativeDirection.Right)
      {
        // calculate the end of the first part of the connection from the connection start point
        Point connectionMiddle = CalculateConnectionMiddle(ref aConnectionStart, aDirection, aConnection, middleTargetArea);

        // calculate the distance from the current point to the edge of the grid given the connection direction
        Direction secondDirection = GetDirection( aDirection, (aTargetedConnection.itsDirection == (int)RelativeDirection.Left) ? Movement.Left : Movement.Right);
        int distanceToEdge = GetDistanceToEdge(ref connectionMiddle, secondDirection );

        if (distanceToEdge > 0)
        {
          // create a second connection for the directed part of the target connection
          DirectedConnection connection = new DirectedConnection();
          connection.itsStart = connectionMiddle;

          // calculate the start and end of the area for the second part of the directed connection
          double cellsPerArea = (double)distanceToEdge / 4.0;
          int startTargetArea = (int)(aTargetedConnection.itsArea * cellsPerArea);         

          // calculate the mid-point of the target area
          int middleSecondArea = startTargetArea + (int)(cellsPerArea / 2.0) + 1;
          
          // test that there's enough space to put the output of the connection
          int remainingCells = (distanceToEdge - middleSecondArea);
          if (remainingCells < 2)
          {
            middleSecondArea -= (2 - remainingCells);
          }

          // calculate the position of the end of the connection and the target node using the direction of the connection
          Point nextEnd = CalculateConnectionEnd(ref connectionMiddle, secondDirection, connection, middleSecondArea);

          aConnection.itsEnd = connection.itsEnd;
          AddConnectionAndTargetToGrid(aNodeLevel, ref aConnectionStart, secondDirection, aTargetedConnection, aConnection, ref nextEnd);
        }
      }
      else
      {
        // straight ahead connection

        // test that there's enough space to put the output of the connection
        int remainingCells = (aDistanceToEdge - middleTargetArea);
        if (remainingCells < 2)
        {
          middleTargetArea -= (2 - remainingCells);
        }

        // calculate the position of the end of the connection and the target node using the direction of the connection
        Point next = CalculateConnectionEnd(ref aConnectionStart, aDirection, aConnection, middleTargetArea);

        AddConnectionAndTargetToGrid(aNodeLevel, ref aConnectionStart, aDirection, aTargetedConnection, aConnection, ref next);
      }
    }

    private void AddConnectionAndTargetToGrid(int aNodeLevel, ref Point aConnectionStart, Direction aDirection, TargetedConnection targetedConnection, DirectedConnection connection, ref Point next)
    {
      // test if the end of the connection or its target node would lie in the output area
      // - if it does shorten the connection to fit
      next = TestForConnectionEndInOutputArea(aDirection, connection, next);

      // test that the cell for the end of the targeted connection and the target node aren't already occupied
      if (TestGridCellValid(connection.itsEnd.X, connection.itsEnd.Y))
      {
        AddTargetedConnectionToGrid(aConnectionStart, connection);

        // test if the target cell can be added to the grid
        if (TestGridCellValid(next.X, next.Y))
        {
          int targetNode = targetedConnection.Rule;
          CellType targetType = itsChromosomeDecode.GetCellType(targetNode);

          // test if this targeted connection targets another one
          //if (targetType == CellType.WestEast)
          if(targetType == CellType.ConnectionStart)
          {
            // add the rule to expand the target node
            CellInformation cellInfo = new CellInformation();
            cellInfo.itsID = targetNode;
            cellInfo.itsType = targetType;
            ApplyRules(next.X, next.Y, aDirection, cellInfo, aNodeLevel + 1);
          }
          else if (targetType != CellType.EmptyCell)
          {
            // add the target node
            Direction newDirection = aDirection;
            if (SetCenterCell(next.X, next.Y, targetType, aDirection, ref newDirection))
            {
              // add the rule to expand the target node
              CellInformation cellInfo = new CellInformation();
              cellInfo.itsID = targetNode;
              cellInfo.itsType = targetType;
              ApplyRules(next.X, next.Y, newDirection, cellInfo, aNodeLevel + 1);
            }
          }
        }
      }
    }

    private Point AddTargetedConnectionToGrid(Point aConnectionStart, DirectedConnection connection)
    {
      // add the targeted connection
      SetGridCell(aConnectionStart.X, aConnectionStart.Y, CellType.ConnectionStart);
      SetGridCell(connection.itsEnd.X, connection.itsEnd.Y, CellType.ConnectionEnd);

      // add to the list of targeted connections
      itsDirectedConnections.Add(connection);
      return aConnectionStart;
    }

    /// <summary>
    /// Calculate the end position of a targeted connection
    /// </summary>
    /// <param name="aConnectionStart"></param>
    /// <param name="aDirection"></param>
    /// <param name="connection"></param>
    /// <param name="middleTargetArea"></param>
    /// <returns></returns>
    private static Point CalculateConnectionEnd(ref Point aConnectionStart, Direction aDirection, DirectedConnection connection, int middleTargetArea)
    {
      Point next = new Point(0, 0);
      switch (aDirection)
      {
        case Direction.North:
          connection.itsEnd = new Point(aConnectionStart.X - middleTargetArea, aConnectionStart.Y);
          next = new Point(connection.itsEnd.X - 1, connection.itsEnd.Y);
          break;
        case Direction.South:
          connection.itsEnd = new Point(aConnectionStart.X + middleTargetArea, aConnectionStart.Y);
          next = new Point(connection.itsEnd.X + 1, connection.itsEnd.Y);
          break;
        case Direction.East:
          connection.itsEnd = new Point(aConnectionStart.X, aConnectionStart.Y + middleTargetArea);
          next = new Point(connection.itsEnd.X, connection.itsEnd.Y + 1);
          break;
        case Direction.West:
          connection.itsEnd = new Point(aConnectionStart.X, aConnectionStart.Y - middleTargetArea);
          next = new Point(connection.itsEnd.X, connection.itsEnd.Y - 1);
          break;
      }

      // save the direction of the end of the targeted connection
      connection.itsDirection = aDirection;

      return next;
    }

    /// <summary>
    /// Calculate the position of the middle of a targeted connection that is going in a left or right direction
    /// </summary>
    /// <param name="aConnectionStart"></param>
    /// <param name="aDirection"></param>
    /// <param name="connection"></param>
    /// <param name="middleTargetArea"></param>
    /// <returns></returns>
    private static Point CalculateConnectionMiddle(ref Point aConnectionStart, Direction aDirection, DirectedConnection connection, int middleTargetArea)
    {
      Point next = new Point(0, 0);
      switch (aDirection)
      {
        case Direction.North:
          connection.itsEnd = new Point(aConnectionStart.X - middleTargetArea, aConnectionStart.Y);
          next = new Point(connection.itsEnd.X, connection.itsEnd.Y);
          break;
        case Direction.South:
          connection.itsEnd = new Point(aConnectionStart.X + middleTargetArea, aConnectionStart.Y);
          next = new Point(connection.itsEnd.X, connection.itsEnd.Y);
          break;
        case Direction.East:
          connection.itsEnd = new Point(aConnectionStart.X, aConnectionStart.Y + middleTargetArea);
          next = new Point(connection.itsEnd.X, connection.itsEnd.Y);
          break;
        case Direction.West:
          connection.itsEnd = new Point(aConnectionStart.X, aConnectionStart.Y - middleTargetArea);
          next = new Point(connection.itsEnd.X, connection.itsEnd.Y);
          break;
      }
      return next;
    }

    private Point TestForConnectionEndInOutputArea(Direction aDirection, DirectedConnection connection, Point next)
    {
      // test if the cell for the end of the targeted connection and the target node are located in the central output area
      if (!(TestGridCellValid(connection.itsEnd.X, connection.itsEnd.Y) && TestGridCellValid(next.X, next.Y)))
      {
        // test if the targeted cells contain an output - if it does then shorten the connection until it no longer lies in central area
        if (itsGrid[connection.itsEnd.X, connection.itsEnd.Y] == CellType.OutputCell || itsGrid[next.X, next.Y] == CellType.OutputCell)
        {
          do
          {
            switch (aDirection)
            {
              case Direction.North:
                connection.itsEnd = new Point(connection.itsEnd.X + 1, connection.itsEnd.Y);
                next.X++;
                break;
              case Direction.South:
                connection.itsEnd = new Point(connection.itsEnd.X - 1, connection.itsEnd.Y);
                next.X--;
                break;
              case Direction.East:
                connection.itsEnd = new Point(connection.itsEnd.X, connection.itsEnd.Y - 1);
                next.Y--;
                break;
              case Direction.West:
                connection.itsEnd = new Point(connection.itsEnd.X, connection.itsEnd.Y + 1);
                next.Y++;
                break;
            }

            if ((connection.itsEnd.X == 0 || connection.itsEnd.Y == 0 || connection.itsEnd.X >= (kSideLength - 1) || connection.itsEnd.Y >= (kSideLength - 1))
            || (next.X == 0 || next.Y == 0 || next.X >= (kSideLength - 1) || next.Y >= (kSideLength - 1)))
            {
              break;
            }
          }
          while (itsGrid[connection.itsEnd.X, connection.itsEnd.Y] == CellType.OutputCell || itsGrid[connection.itsEnd.X, connection.itsEnd.Y] == CellType.OutOfBounds);
        }
      }

      return next;
    }

    private bool LookForTargetNode(ref Point aConnectionStart, Direction aDirection, TargetedConnection targetedConnection, DirectedConnection connection, int startTargetArea, int endTargetArea )
    {
      bool targetFound = false;

      switch (aDirection)
      {
        case Direction.North:
          // straight ahead
          if (targetedConnection.itsDirection == 0 || targetedConnection.itsDirection == 2)
          {
            targetFound = TestAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          else if (targetedConnection.itsDirection == 1)
          {
            // Left
            targetFound = TestLeftAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          else if (targetedConnection.itsDirection == 3)
          {
            // Right
            targetFound = TestRightAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          break;
        case Direction.South:
          // straight ahead
          if (targetedConnection.itsDirection == 0 || targetedConnection.itsDirection == 2)
          {
            targetFound = TestAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          else if (targetedConnection.itsDirection == 1)
          {
            // Left
            targetFound = TestLeftAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          else if (targetedConnection.itsDirection == 3)
          {
            // Right
            targetFound = TestRightAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          break;
        case Direction.East:
          // straight ahead
          if (targetedConnection.itsDirection == 0 || targetedConnection.itsDirection == 2)
          {
            targetFound = TestAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          else if (targetedConnection.itsDirection == 1)
          {
            // Left
            targetFound = TestLeftAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          else if (targetedConnection.itsDirection == 3)
          {
            // Right
            targetFound = TestRightAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          break;
        case Direction.West:
          // straight ahead
          if (targetedConnection.itsDirection == 0 || targetedConnection.itsDirection == 2)
          {
            targetFound = TestAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          else if (targetedConnection.itsDirection == 1)
          {
            // Left
            targetFound = TestLeftAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          else if (targetedConnection.itsDirection == 3)
          {
            // Right
            targetFound = TestRightAreaForTarget(ref aConnectionStart, targetedConnection, connection, startTargetArea, endTargetArea, aDirection);
          }
          break;
      }

      // set the direction of the targeted connection for straight ahead connections
      if (targetFound && targetedConnection.itsDirection == 0 || targetedConnection.itsDirection == 2)
      {
        connection.itsDirection = aDirection;
      }

      return targetFound;
    }

    private static void CalculateTargetAreaBounds(TargetedConnection targetedConnection, int distanceToEdge, out double cellsPerArea, out int startTargetArea, out int endTargetArea)
    {
      cellsPerArea = (double)distanceToEdge / 4.0;
      startTargetArea = (int)(targetedConnection.itsDistance * cellsPerArea);
      endTargetArea = startTargetArea + (int)cellsPerArea;
    }

    private int GetDistanceToEdge(ref Point aConnectionStart, Direction aDirection)
    {
      // calculate the number of cells between the start of the connection and the edge of the grid
      int distanceToEdge = 0;
      if (aDirection == Direction.North)
      {
        distanceToEdge = (aConnectionStart.X - 1);
      }
      else if (aDirection == Direction.South)
      {
        distanceToEdge = (kSideLength - 2) - aConnectionStart.X;
      }
      else if (aDirection == Direction.East)
      {
        distanceToEdge = (kSideLength - 2) - aConnectionStart.Y;
      }
      else // must be west
      {
        distanceToEdge = (aConnectionStart.Y - 1);
      }
      return distanceToEdge;
    }


    private bool TestLeftAreaForTarget(ref Point aConnectionStart,
                                       TargetedConnection aTargetedConnection,
                                       DirectedConnection aConnection,
                                       int aStartTargetArea,
                                       int aEndTargetArea,
                                       Direction aConnectionDirection)
    {
      // calculate the direction for the second part of the connection
      Direction targetDirection = GetDirection(aConnectionDirection, Movement.Left);
      return TestSecondPartOfConnectionForTarget(ref aConnectionStart, aTargetedConnection, aConnection, aStartTargetArea, aEndTargetArea, aConnectionDirection, targetDirection);
    }

    private bool TestRightAreaForTarget(ref Point aConnectionStart,
                                        TargetedConnection aTargetedConnection,
                                        DirectedConnection aConnection,
                                        int aStartTargetArea,
                                        int aEndTargetArea,
                                        Direction aConnectionDirection)
    {
      // calculate the direction for the second part of the connection
      Direction targetDirection = GetDirection(aConnectionDirection, Movement.Right);
      return TestSecondPartOfConnectionForTarget(ref aConnectionStart, aTargetedConnection, aConnection, aStartTargetArea, aEndTargetArea, aConnectionDirection, targetDirection);
    }

    private bool TestSecondPartOfConnectionForTarget(ref Point aConnectionStart, TargetedConnection aTargetedConnection, DirectedConnection aConnection, int aStartTargetArea, int aEndTargetArea, Direction aConnectionDirection, Direction targetDirection)
    {
      // the offset is one greater, since it is relative to the start of the targeted connection
      // - the end is always at least one cell greater
      for (int offset = aStartTargetArea + 1; offset <= aEndTargetArea; offset++)
      {
        // The targeted connections target type specifies the rule letter that will be processed if the 
        // connection is not found. So, for example, this could specify rule 'G', which might be a Nand node 
        // with a set of rules for its outputs.
        // Therefore the targeted connection should first look to see if the type of node, belonging to the
        // specified rule, is currently in the search area.

        // get the type of node that is created by the specified rule
        // (this gives the North version of the node - therefore need to test if the direction of this node
        // type that would allow side-connections for the direction of the current targeted connection)
        int targetNode = aTargetedConnection.Rule;
        CellType targetType = itsChromosomeDecode.GetCellType(targetNode);

        Point newPosition = aConnectionStart;
        switch (aConnectionDirection)
        {
          case Direction.North:
            newPosition.Offset(-offset, 0);
            break;

          case Direction.South:
            newPosition.Offset(offset, 0);
            break;

          case Direction.East:
            newPosition.Offset(0, offset);
            break;

          case Direction.West:
            newPosition.Offset(0, -offset);
            break;
        }

        // calculate the distance from the current point to the edge of the grid given the connection direction
        int distanceToEdge = GetDistanceToEdge(ref newPosition, targetDirection);

        // test that the start of the targeted connection isn't at the edge of the grid (when pointing off the grid)
        if (distanceToEdge > 0)
        {
          double cellsPerArea = (double)distanceToEdge / 4.0;
          int startTargetArea = (int)(aTargetedConnection.itsArea * cellsPerArea);
          int endTargetArea = startTargetArea + (int)cellsPerArea;

          if (TestAreaForTarget(ref newPosition, aTargetedConnection, aConnection, startTargetArea, endTargetArea, targetDirection, true))
          {
            // set the direction of the output node from the second part of the connection
            aConnection.itsDirection = targetDirection;
            return true;
          }
        }
      }

      return false;
    }


    /// <summary>
    /// test the target area to see if it contains the required target node
    /// The target area will begin one square after the start offset, to leave a cell for the connection end
    /// </summary>
    /// <param name="aConnectionStart"></param>
    /// <param name="aTargetedConnection"></param>
    /// <param name="aConnection"></param>
    /// <param name="aStartTargetArea"></param>
    /// <param name="aEndTargetArea"></param>
    /// <param name="aConnectionDirection"></param>
    /// <returns></returns>
    private bool TestAreaForTarget(ref Point aConnectionStart,
                                    TargetedConnection aTargetedConnection,
                                    DirectedConnection aConnection,
                                    int aStartTargetArea,
                                    int aEndTargetArea,
                                    Direction aConnectionDirection)
    {
      return TestAreaForTarget( ref  aConnectionStart,
                                aTargetedConnection,
                                aConnection,
                                aStartTargetArea,
                                aEndTargetArea,
                                aConnectionDirection, 
                                false);
    }


    /// <summary>
    /// test the target area to see if it contains the required target node
    /// </summary>
    /// <param name="aConnectionStart"></param>
    /// <param name="aTargetedConnection"></param>
    /// <param name="aConnection"></param>
    /// <param name="aStartTargetArea"></param>
    /// <param name="aEndTargetArea"></param>
    /// <param name="aConnectionDirection"></param>
    /// <returns></returns>
    private bool TestAreaForTarget(ref Point aConnectionStart,
                                    TargetedConnection aTargetedConnection,
                                    DirectedConnection aConnection,
                                    int aStartTargetArea,
                                    int aEndTargetArea,
                                    Direction aConnectionDirection,
                                    bool aSecondConnectionPart )
    {
      // when this is first connection part the offset is one greater, since it is relative to the start of the targeted connection
      // - the end is always at least one cell greater
      int start = aStartTargetArea + (aSecondConnectionPart ? 0 : 1);

      for (int offset = start; offset <= aEndTargetArea; offset++)
      {
        // The targeted connections target type specifies the rule letter that will be processed if the 
        // connection is not found. So, for example, this could specify rule 'G', which might be a Nand node 
        // with a set of rules for its outputs.
        // Therefore the targeted connection should first look to see if the type of node, belonging to the
        // specified rule, is currently in the search area.

        // get the type of node that is created by the specified rule
        // (this gives the North version of the node - therefore need to test if the direction of this node
        // type that would allow side-connections for the direction of the current targeted connection)
        int targetNode = aTargetedConnection.Rule;
        CellType targetType = itsChromosomeDecode.GetCellType(targetNode);

        // if the target type is NorthSouth, this indicates a targeted connection for the output cells
        if( targetType == CellType.NorthSouth )
        {
          switch (aConnectionDirection)
          {
            case Direction.North:
              if (TestForTargetConnectionNorth(ref aConnectionStart, aConnection, offset, CellType.OutputCell))// West
              {
                return true;
              }
              break;

            case Direction.South:
              if (TestForTargetConnectionSouth(ref aConnectionStart, aConnection, offset, CellType.OutputCell))// West
              {
                return true;
              }
              break;

            case Direction.East:
              if (TestForTargetConnectionEast(ref aConnectionStart, aConnection, offset, CellType.OutputCell))// South
              {
                return true;
              }
              break;

            case Direction.West:
              if (TestForTargetConnectionWest(ref aConnectionStart, aConnection, offset, CellType.OutputCell))// South
              {
                return true;
              }
              break;
          }
        }
        else if (targetType >= CellType.NorthNand)
        {
          switch (aConnectionDirection)
          {
            case Direction.North:
              if (TestForTargetConnectionNorth(ref aConnectionStart, aConnection, offset, targetType + 1) // East
               || TestForTargetConnectionNorth(ref aConnectionStart, aConnection, offset, targetType + 3))// West
              {
                return true;
              }
              break;

            case Direction.South:
              if (TestForTargetConnectionSouth(ref aConnectionStart, aConnection, offset, targetType + 1) // East
               || TestForTargetConnectionSouth(ref aConnectionStart, aConnection, offset, targetType + 3))// West
              {
                return true;
              }
              break;

            case Direction.East:
              if (TestForTargetConnectionEast(ref aConnectionStart, aConnection, offset, targetType)     // North
               || TestForTargetConnectionEast(ref aConnectionStart, aConnection, offset, targetType + 2))// South
              {
                return true;
              }
              break;

            case Direction.West:
              if (TestForTargetConnectionWest(ref aConnectionStart, aConnection, offset, targetType)     // North
               || TestForTargetConnectionWest(ref aConnectionStart, aConnection, offset, targetType + 2))// South
              {
                return true;
              }
              break;
          }
        }
      }

      return false;
    }


    /// <summary>
    /// test if the required target node exists at the current offset from the targeted connections starting point and that the end of 
    /// the connection can be placed beside the target node
    /// </summary>
    /// <param name="aConnectionStart"></param>
    /// <param name="aConnection"></param>
    /// <param name="aOffset"></param>
    /// <param name="aTargetType"></param>
    /// <returns></returns>
    private bool TestForTargetConnectionNorth(ref Point aConnectionStart, DirectedConnection aConnection, int aOffset, CellType aTargetType)
    {
      // test if the rquired target lies at the current offset from the targeted connections start position
      if (itsGrid[aConnectionStart.X - aOffset, aConnectionStart.Y] == aTargetType)
      {
        // test that the cell immediately before the target node is empty
        // (to allow the end of the targeted connection to be placed)                    
        if (itsGrid[aConnectionStart.X - aOffset + 1, aConnectionStart.Y] == CellType.EmptyCell)
        {
          aConnection.itsEnd = new Point(aConnectionStart.X - aOffset + 1, aConnectionStart.Y);
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// test if the required target node exists at the current offset from the targeted connections starting point and that the end of 
    /// the connection can be placed beside the target node
    /// </summary>
    /// <param name="aConnectionStart"></param>
    /// <param name="aConnection"></param>
    /// <param name="aOffset"></param>
    /// <param name="aTargetType"></param>
    /// <returns></returns>
    private bool TestForTargetConnectionSouth(ref Point aConnectionStart, DirectedConnection aConnection, int aOffset, CellType aTargetType)
    {
      // test if the rquired target lies at the current offset from the targeted connections start position
      if (itsGrid[aConnectionStart.X + aOffset, aConnectionStart.Y] == aTargetType)
      {
        // test that the cell immediately before the target node is empty
        // (to allow the end of the targeted connection to be placed)                    
        if (itsGrid[aConnectionStart.X + aOffset - 1, aConnectionStart.Y] == CellType.EmptyCell)
        {
          aConnection.itsEnd = new Point(aConnectionStart.X + aOffset - 1, aConnectionStart.Y);
          return true;
        }
      }

      return false;
    }



    /// <summary>
    /// test if the required target node exists at the current offset from the targeted connections starting point and that the end of 
    /// the connection can be placed beside the target node
    /// </summary>
    /// <param name="aConnectionStart"></param>
    /// <param name="aConnection"></param>
    /// <param name="aOffset"></param>
    /// <param name="aTargetType"></param>
    /// <returns></returns>
    private bool TestForTargetConnectionWest(ref Point aConnectionStart, DirectedConnection aConnection, int aOffset, CellType aTargetType)
    {
      // test if the rquired target lies at the current offset from the targeted connections start position
      if (itsGrid[aConnectionStart.X, aConnectionStart.Y - aOffset] == aTargetType)
      {
        // the target node exists
        // - now test that the cell immediately before the target node is empty
        // (to allow the end of the targeted connection to be placed)                 
        if (itsGrid[aConnectionStart.X, aConnectionStart.Y - aOffset + 1] == CellType.EmptyCell)
        {
          aConnection.itsEnd = new Point(aConnectionStart.X, aConnectionStart.Y - aOffset + 1);
          return true;
        }
      }

      return false;
    }


    /// <summary>
    /// test if the required target node exists at the current offset from the targeted connections starting point and that the end of 
    /// the connection can be placed beside the target node
    /// </summary>
    /// <param name="aConnectionStart"></param>
    /// <param name="aConnection"></param>
    /// <param name="aOffset"></param>
    /// <param name="aTargetType"></param>
    /// <returns></returns>
    private bool TestForTargetConnectionEast(ref Point aConnectionStart, DirectedConnection aConnection, int aOffset, CellType aTargetType)
    {
      // test if the rquired target lies at the current offset from the targeted connections start position
      if (itsGrid[aConnectionStart.X, aConnectionStart.Y + aOffset] == aTargetType)
      {
        // the target node exists
        // - now test that the cell immediately before the target node is empty
        // (to allow the end of the targeted connection to be placed)                    
        if (itsGrid[aConnectionStart.X, aConnectionStart.Y + aOffset - 1] == CellType.EmptyCell)
        {
          aConnection.itsEnd = new Point(aConnectionStart.X, aConnectionStart.Y + aOffset - 1);
          return true;
        }
      }

      return false;
    }

    #endregion Targeted Connections
  }
}
