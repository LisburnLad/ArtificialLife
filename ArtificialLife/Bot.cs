using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace ArtificialLife
{
  class Bot
  {
    static int itsGeneLength = 5;

    /// <summary>
    /// the length of the side of the square grid
    /// </summary>
    public int itsSideLength { get; set; }

    /// <summary>
    /// the type of chromosome used to create the bot
    /// </summary>
    public int itsBotType { get; set; }

    /// <summary>
    /// the chromosome representing this bot
    /// </summary>
    Chromosome itsChromosome;

    Test itsTest;

    /// <summary>
    /// the output of this bot at each stage of evaluation
    /// </summary>
    char[,] itsOutput;

    enum Direction
    {
      North,
      East,
      South,
      West
    };

    enum Movement
    {
      Left,
      Forward,
      Right
    };

    enum CellType
    {
      EmptyCell,          // " " - 0

      // Straight Connections
      NorthSouth,         // "|" - 1
      WestEast,           // "─" - 2

      // T Connections
      WestNorthEast,      // "┴" - 3
      EastSouthWest,      // "┬" - 4
      NorthEastSouth,     // "├" - 5
      SouthWestNorth,     // "┤" - 6

      // L Connections
      NorthEast,          // "└" - 7
      EastSouth,          // "┌" - 8
      SouthWest,          // "┐" - 9
      WestNorth,          // "┘" - 10

      // Full Connection
      NorthEastSouthWest, // "┼" - 11

      // Nodes
      NorthNode,          // "↑" - 12
      EastNode,           // "→" - 13
      SouthNode,          // "↓" - 14
      WestNode,           // "←" - 15

      // Delay Nodes
      NorthDelay,         // "▲" - 16 - 10000
      EastDelay,          // "►" - 17 - 10001
      SouthDelay,         // "▼" - 18 - 10010
      WestDelay          // "◄" - 19 - 10011

      //NorthDelay2,        // "▲" - 20 - 10100
      //EastDelay2,         // "►" - 21 - 10101
      //SouthDelay2,        // "▼" - 22 - 10110
      //WestDelay2,         // "◄" - 23 - 10111

      //NorthDelay3,        // "▲" - 20 - 11000
      //EastDelay3,         // "►" - 21 - 11001
      //SouthDelay3,        // "▼" - 22 - 11010
      //WestDelay3,         // "◄" - 23 - 11011

      //NorthDelay4,        // "▲" - 20 - 11100
      //EastDelay4,         // "►" - 21 - 11101
      //SouthDelay4,        // "▼" - 22 - 11110
      //WestDelay4          // "◄" - 23 - 11111
    };

    /// <summary>
    /// the grid representing the structure of the bot
    /// </summary>
    CellType[,] itsGrid;

    /// <summary>
    /// return the length of chromosome required for the supplied side length (for a square grid)
    /// </summary>
    /// <param name="aSideLength"></param>
    /// <returns></returns>
    public static int GetChromosomeLength(int aSideLength,int aBotType)
    {
      if(aBotType == 2)
      {
        return 21;
      }

      return ((aSideLength * aSideLength) * itsGeneLength);
    }

    /// <summary>
    /// given a chromosome, calculate the length of each grid side (assuming a square grid)
    /// </summary>
    /// <param name="aChromosome"></param>
    /// <returns></returns>
    public static int GetSideLengthFromChromosome( string aChromosome )
    {
      int chromosomeLength = aChromosome.Length;

      int numberOfGenes = chromosomeLength / itsGeneLength;

      return Convert.ToInt32( System.Math.Sqrt(numberOfGenes));
    }


    /// <summary>
    /// create the bot's grid from the supplied chromosome
    /// </summary>
    /// <param name="aChromosome"></param>
    public Bot( Chromosome aChromosome, Test aTest, int aSideLength, int aBotType, bool aShowGrid )      
    {
      itsBotType = aBotType;
      itsSideLength = aSideLength;

      // take a copy of the chromosome
      itsChromosome = aChromosome;

      // create the grid for this chromosome
      CreateGrid( aShowGrid );

      itsTest = aTest;
    }


    /// <summary>
    /// Create the grid from this bots chromosome
    /// </summary>
    /// <param name="aShowGrid"></param>
    private void CreateGrid( bool aShowGrid )
    {
      if(aShowGrid)
      {
        Console.WriteLine( "Chromosome: " + itsChromosome.ToBinaryString() );
      }

      try
      {
        itsGrid = new CellType[itsSideLength, itsSideLength];

        if(itsBotType == 2)
        {
          // rules based encoding

          // get the starting cell type
          // - a node or a delay
          int position = 0;
          int direction = GetGeneValue( ref position, 2 );
          int type = GetGeneValue( ref position, 1 );

          // put this node into the center cell
          CellType startType = (type == 0 ? CellType.NorthNode : CellType.NorthDelay) + direction;
        
          // get the rules defined by the chromosome
          int[,] rules = new int[3, 3];
          for(int rule = 0; rule < 3; rule++)
          {
            int leftType = GetGeneValue( ref position, 2 );
            int centerType = GetGeneValue( ref position, 2 );
            int rightType = GetGeneValue( ref position, 2 );

            rules[rule, 0] = leftType;
            rules[rule, 1] = centerType;
            rules[rule, 2] = rightType;
          }

          // show the rules
          if(aShowGrid)
          {
            for(int rule = 0; rule < 3; rule++)
            {
              switch(rule)
              {
                case 0: Console.Write( "A -> " ); break;
                case 1: Console.Write( "B -> " ); break;
                case 2: Console.Write( "C -> " ); break;
              }

              for(int outDirection = 0; outDirection < 3; outDirection++)
              {
                switch(outDirection)
                {
                  case 0: Console.Write( "L" ); break;
                  case 1: Console.Write( "F" ); break;
                  case 2: Console.Write( "R" ); break;
                }

                switch(rules[rule, outDirection])
                {
                  case 0: Console.Write( "-," ); break;
                  case 1: Console.Write( "A," ); break;
                  case 2: Console.Write( "B," ); break;
                  case 3: Console.Write( "C," ); break;
                }
              }

              Console.WriteLine();
            }
          }

          int row = itsSideLength/2;
          int col = itsSideLength / 2;
          itsGrid[row, col] = startType;

          ApplyRules( rules, row, col, (Direction.North + direction) );

          //ShowGrid( "c:\\grid.bmp" );
          //return;
        }
        else
        {
          // one to one encoding
          for(int row = 0; row < itsSideLength; row++)
          {
            for(int col = 0; col < itsSideLength; col++)
            {
              CellType geneValue = CellType.EmptyCell;
              for(int bit = 0; bit < itsGeneLength; bit++)
              {
                int position = (row * itsGeneLength * itsSideLength) + (col * itsGeneLength) + bit;
                geneValue += Convert.ToInt32( itsChromosome.ToBinaryString( position, 1 ) ) * (1 << (itsGeneLength - (bit + 1)));
              }

              itsGrid[row, col] = geneValue;
            }
          }
        }
      

        // remove cells with no connections
        PruneGrid(aShowGrid);

        if(aShowGrid)
        {
          ShowGrid("c:\\grid.bmp");
        }
      } 

      catch
      {
        Console.WriteLine("An error occurred during grid creation: ");
        Console.WriteLine("Chromosome: " + itsChromosome.ToBinaryString());
        Console.WriteLine("Side Length: " + itsSideLength);
        Console.WriteLine("Chromosome Length: " + GetChromosomeLength(itsSideLength,itsBotType));
      }
    }

    /// <summary>
    /// apply the rules to create the output for the specified source cell
    /// </summary>
    /// <param name="rules"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void ApplyRules( int[,] aRules, int aRow, int aCol, Direction aDirection )
    {
      // test what type of element is contained in the source cell
      if(itsGrid[aRow, aCol] >= CellType.NorthNode && itsGrid[aRow, aCol] <= CellType.WestNode)
      {
        // the source cell contains a node
        CreateCellsFromRule(aRules, aRow, aCol, aDirection, 0);
      }
      else if(itsGrid[aRow, aCol] >= CellType.NorthDelay && itsGrid[aRow, aCol] <= CellType.WestDelay)
      {
        // the source cell contains a delay
        CreateCellsFromRule(aRules, aRow, aCol, aDirection, 1);
      }
      else
      {
        // the source cell must contain a connection
        CreateCellsFromRule(aRules, aRow, aCol, aDirection, 2);
      }
    }

    private void CreateCellsFromRule(int[,] aRules, int aRow, int aCol, Direction aDirection, int ruleNumber)
    {
      int leftRule = aRules[ruleNumber, 0];
      int centerRule = aRules[ruleNumber, 1];
      int rightRule = aRules[ruleNumber, 2];

      int ruleCount = ((leftRule > 0) ? 1 : 0) + ((centerRule > 0) ? 1 : 0) + ((rightRule > 0) ? 1 : 0);

      CellType[] cell = new CellType[3];
      switch (leftRule)
      {
        case 1: cell[0] = CellType.NorthNode; break;
        case 2: cell[0] = CellType.NorthDelay; break;
        case 3: cell[0] = CellType.NorthSouth; break;
        default: cell[0] = CellType.EmptyCell; break;
      }

      switch (centerRule)
      {
        case 1: cell[1] = CellType.NorthNode; break;
        case 2: cell[1] = CellType.NorthDelay; break;
        case 3: cell[1] = CellType.NorthSouth; break;
        default: cell[1] = CellType.EmptyCell; break;
      }

      switch (rightRule)
      {
        case 1: cell[2] = CellType.NorthNode; break;
        case 2: cell[2] = CellType.NorthDelay; break;
        case 3: cell[2] = CellType.NorthSouth; break;
        default: cell[2] = CellType.EmptyCell; break;
      }

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

      Direction newDirection = aDirection;

      // if only a single rule is defined, then only the single output cell will be set
      if (ruleCount == 1)
      {
        Direction direction = aDirection;
        CellType centerCellType = CellType.EmptyCell;
        if (cell[0] > 0)
        {
          if (cell[0] == CellType.NorthSouth)
          {
            // the cell output only contains a connection          
            if (SetConnectionCell(aRules, centerCellRow, centerCellCol, cell, aDirection, ref newDirection))
            {
              // if the cell has been added then apply the rule to this
              ApplyRules(aRules, centerCellRow, centerCellCol, newDirection);
            }
            return;
          }
          else
          {
            // left cell is defined
            // - change direction to left
            centerCellType = cell[0];

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
        else if (cell[1] > 0)
        {
          if (cell[1] == CellType.NorthSouth)
          {
            // the cell output only contains a connection          
            if (SetConnectionCell(aRules, centerCellRow, centerCellCol, cell, aDirection, ref newDirection))
            {
              // if the cell has been added then apply the rule to this
              ApplyRules(aRules, centerCellRow, centerCellCol, newDirection);
            }
            return;
          }
          else
          {
            centerCellType = cell[1];
          }
        }
        else if (cell[2] > 0)
        {
          if (cell[2] == CellType.NorthSouth)
          {
            // the cell output only contains a connection          
            if (SetConnectionCell(aRules, centerCellRow, centerCellCol, cell, aDirection, ref newDirection))
            {
              // if the cell has been added then apply the rule to this
              ApplyRules(aRules, centerCellRow, centerCellCol, newDirection);
            }
            return;
          }
          else
          {
            centerCellType = cell[2];

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
        if (SetCenterCell(centerCellRow, centerCellCol, centerCellType, direction, ref newDirection))
        {
          // if the cell has been added then apply the rule to this
          ApplyRules(aRules, centerCellRow, centerCellCol, newDirection);
        }
      }
      else
      {
        // add connections to the cells that are about to be added
        SetConnectionCell(aRules, centerCellRow, centerCellCol, cell, aDirection, ref newDirection);

        // dont set connections into left, right or forward cells
        if (cell[0] == CellType.NorthNode || cell[0] == CellType.NorthDelay)
        {
          if( SetLeftCell(leftCellRow, leftCellCol, cell[0], aDirection, ref newDirection) )
          {
            // if the cell has been added then apply the rule to this
            ApplyRules(aRules, leftCellRow, leftCellCol, newDirection);
          }
        }

        // dont set connections into left, right or forward cells
        if (cell[1] == CellType.NorthNode || cell[1] == CellType.NorthDelay)
        {
          if(SetCenterCell(forwardCellRow, forwardCellCol, cell[1], aDirection, ref newDirection))
          {
            // if the cell has been added then apply the rule to this
            ApplyRules(aRules, centerCellRow, centerCellCol, newDirection);
          }
        }

        // dont set connections into left, right or forward cells
        if (cell[2] == CellType.NorthNode || cell[2] == CellType.NorthDelay)
        {
          if(SetRightCell(rightCellRow, rightCellCol, cell[2], aDirection, ref newDirection))
          {
            // if the cell has been added then apply the rule to this
            ApplyRules(aRules, rightCellRow, rightCellCol, newDirection);
          }
        }        
      }
    }

    private bool SetConnectionCell(int[,] aRules, int aRow, int aCol, CellType[] cell, Direction aDirection, ref Direction aNewDirection)
    {
      // test if all the output cells are occupied
      if (cell[0] != CellType.EmptyCell && cell[1] != CellType.EmptyCell && cell[2] != CellType.EmptyCell)
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
        switch(aDirection)
        {
          case Direction.North: direction[directionCount] = Direction.South; break;
          case Direction.East: direction[directionCount] = Direction.West; break;
          case Direction.South: direction[directionCount] = Direction.North; break;
          case Direction.West: direction[directionCount] = Direction.East; break;
        }
        directionCount++;

        Movement movement = Movement.Left;
        if( cell[(int)movement] != CellType.EmptyCell )
        {
          // add a connection to the left cell
          switch (aDirection)
          {
            case Direction.North: direction[directionCount] = Direction.West; break;
            case Direction.East: direction[directionCount] = Direction.North; break;
            case Direction.South: direction[directionCount] = Direction.East; break;
            case Direction.West: direction[directionCount] = Direction.South; break;
          }

          if( cell[(int)movement] == CellType.NorthSouth )
          {
            connectionSet[(int)Movement.Left] = true;
            conectionDirection[(int)Movement.Left] = direction[directionCount];
          }

          directionCount++;
        }

        movement = Movement.Forward;
        if (cell[(int)movement] != CellType.EmptyCell)
        {
          // add a connection to the forward cell
          direction[directionCount] = aDirection;

          if (cell[(int)movement] == CellType.NorthSouth)
          {
            connectionSet[(int)Movement.Forward] = true;
            conectionDirection[(int)Movement.Forward] = direction[directionCount];
          }

          directionCount++;
        }

        movement = Movement.Right;
        if (cell[(int)movement] != CellType.EmptyCell)
        {
          // add a connection to the right cell
          switch (aDirection)
          {
            case Direction.North: direction[directionCount] = Direction.East; break;
            case Direction.East: direction[directionCount] = Direction.South; break;
            case Direction.South: direction[directionCount] = Direction.West; break;
            case Direction.West: direction[directionCount] = Direction.North; break;
          }

          if (cell[(int)movement] == CellType.NorthSouth)
          {
            connectionSet[(int)Movement.Right] = true;
            conectionDirection[(int)Movement.Right] = direction[directionCount];
          }

          directionCount++;
        }

        // now have all the directions that need to be included in the connector cell
        // - this should always be 2 or greater, otherwise the cell has no outputs
        if( directionCount >= 2 )
        {
          CellType connectionType = CellType.EmptyCell;
          switch( direction[0] )
          {
            case Direction.North:
              switch (direction[1])
              {                
                case Direction.East:
                  if (directionCount == 3)
                  {
                    switch( direction[2] )
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
                    switch( direction[2] )
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
                    switch( direction[2] )
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
                    switch( direction[2] )
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
                    switch( direction[2] )
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
                    switch( direction[2] )
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
                    switch( direction[2] )
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
                    switch( direction[2] )
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
                    switch( direction[2] )
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
                    switch( direction[2] )
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
                    switch( direction[2] )
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
                    switch( direction[2] )
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
            for (int connections = 0; connections < 3; connections++)
            {
              if (connectionSet[connections])
              {
                ApplyRules(aRules, aRow, aCol, conectionDirection[connections]);
              }
            }

            return true;
          }
        }
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
      } else if (aMovement == Movement.Right)
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

      // set the direction in which the new cell type is pointing
      aNewDirection = direction;

      CellType cellType = centerCellType + (int)direction;
      //if (centerCellType == CellType.NorthSouth)
      //{
      //  if (aDirection == Direction.North || aDirection == Direction.South)
      //  {
      //    cellType = CellType.NorthSouth;
      //  }
      //  else
      //  {
      //    cellType = CellType.WestEast;
      //  }
      //}

      return SetGridCell(aRow, aCol, cellType);
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
      if ((aRow >= 0 && aRow < itsSideLength)
      && (aCol >= 0 && aCol < itsSideLength)
      && itsGrid[aRow, aCol] == CellType.EmptyCell)
      {
        itsGrid[aRow, aCol] = cellType;

        return true;
      }

      return false;
    }

    /// <summary>
    /// get the value of the gene at the specified position and the given length
    /// </summary>
    /// <param name="position"></param>
    /// <param name="geneLength"></param>
    /// <returns></returns>
    private int GetGeneValue( ref int position, int geneLength )
    {
      int bitValue = 0;
      for(int bit = 0; bit < geneLength; bit++)
      {        
        bitValue += Convert.ToInt32( itsChromosome.ToBinaryString( position + bit, 1 ) ) * (1 << (geneLength - (bit + 1)));
      }

      // increase the position by the number of bits read
      position += geneLength;

      return bitValue;
    } 

    /// <summary>
    /// generate and evaluate a grid using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    public double Evaluate( bool aShowGrid )
    {
      // initialize the output
      InitializeOutput();

      return TestBot( aShowGrid );
    }


    #region Output Functionality


    /// <summary>
    /// allocate and initialize the output array
    /// </summary>
    private void InitializeOutput()
    {
      itsOutput = new char[itsSideLength, itsSideLength];

      // initially set all connections to zero and all other cells blank
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          if(itsGrid[row, col] > 0)
          {
            itsOutput[row, col] = '0';
          }
          else
          {
            itsOutput[row, col] = '-';
          }
        }
      }
    }


    private double TestBot( bool aShowGrid )
    {
      int pass = 0;
      for(; pass < itsTest.GetNumberOfPasses(); pass++)
      {
        EvaluateGrid( pass, aShowGrid );

        itsTest.EvaluateOutput( itsOutput, itsSideLength, pass, aShowGrid );
      }

      if(aShowGrid)
      {
        itsTest.ShowTestOutput();
      }

      return itsTest.GetFinalScore( aShowGrid );
    }

    /// <summary>
    /// Do a single pass of the grid to evaluate the value of each cell
    /// </summary>
    /// <param name="grid"></param>
    private void EvaluateGrid( int aPassNumber, bool aShowOutput )
    {
      // create a new itsGrid for itsOutput and reset all its cells to '-'
      char[,] newOutput = new char[itsSideLength, itsSideLength];
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          // retain delay node output
          if(itsGrid[row, col] >= CellType.NorthDelay && itsGrid[row, col] <= CellType.WestDelay)          
          {
            newOutput[row, col] = itsOutput[row, col];
          }
          else
          {
            newOutput[row, col] = '-';
          }
        }
      }

      // set the gate outputs according to the current outputs
      //
      // case 12: Console.Write("↑"); break;
      // case 13: Console.Write("→"); break;
      // case 14: Console.Write("↓"); break;
      // case 15: Console.Write("←"); break;     
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          if(itsGrid[row, col] >= CellType.NorthNode && itsGrid[row, col] <= CellType.WestNode)
          {
            newOutput[row, col] = (CellValue( itsGrid, itsOutput, row, col ) ? '1' : '0');

            switch(itsGrid[row, col])
            {
              // case 12: Console.Write("↑"); break;
              case CellType.NorthNode:
                SetCellAbove( newOutput, row, col, newOutput[row, col], direction.below );
                break;

              // case 13: Console.Write("→"); break;
              case CellType.EastNode:
                SetCellRight( newOutput, row, col, newOutput[row, col], direction.left );
                break;

              // case 14: Console.Write("↓"); break;
              case CellType.SouthNode:
                SetCellBelow( newOutput, row, col, newOutput[row, col], direction.above );
                break;

              // case 15: Console.Write("←"); break;
              case CellType.WestNode:
                SetCellLeft( newOutput, row, col, newOutput[row, col], direction.right );
                break;
            }
          }

          if(itsGrid[row, col] >= CellType.NorthDelay && itsGrid[row, col] <= CellType.WestDelay)
          {
            bool cellValue = CellValue( itsGrid, itsOutput, row, col );
            
            char output = '0';

            // test if state has changed to 1            
            if (cellValue == true)
            {
              // change the state
              // input has gone from 0 to 1
              newOutput[row, col] = '1';
            }
            else
            {
              // change the state 
              // - input has gone from 1 to 0
              newOutput[row, col] = '0';
            }
            
            // set the output according to the current state
            if(newOutput[row, col] == '1')
            {
              output = '1';
            }

            switch(itsGrid[row, col])
            {
              
              case CellType.NorthDelay:
                SetCellAbove( newOutput, row, col, output, direction.below );
                break;

              
              case CellType.EastDelay:
                SetCellRight( newOutput, row, col, output, direction.left );
                break;

              
              case CellType.SouthDelay:
                SetCellBelow( newOutput, row, col, output, direction.above );
                break;

              
              case CellType.WestDelay:
                SetCellLeft( newOutput, row, col, output, direction.right );
                break;
            }
          }
        }
      }

      // maintain the state of any cells that haven't changed
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          if(newOutput[row, col] != '-')
          {
            itsOutput[row, col] = newOutput[row, col];
          }
        }
      }

      if(aShowOutput)
      {
        ShowOutput( aPassNumber );
      }
    }

    enum direction { above, left, right, below };

    private void SetPath( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      // if the cell has already been set to '1' don't change its value
      if(itsGrid[row, col] > 0 && itsGrid[row, col] < CellType.NorthNode && newOutput[row, col] != '1' && !(newOutput[row, col] == '0' && aValue == '0'))
      {
        // set the cell value
        newOutput[row, col] = aValue;

        switch(itsGrid[row, col])
        {
          ///   // straight connections
          ///   case 1:  Console.Write("|");
          case CellType.NorthSouth:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 2:  Console.Write("─");
          case CellType.WestEast:
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   // T connections
          ///   case 3:  Console.Write("┴");
          case CellType.WestNorthEast:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 4:  Console.Write("┬");
          case CellType.EastSouthWest:
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 5:  Console.Write("├");
          case CellType.NorthEastSouth:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 6:  Console.Write("┤");
          case CellType.SouthWestNorth:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            break;

          ///   // L connections 
          ///   case 7:  Console.Write("└");
          case CellType.NorthEast:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 8:  Console.Write("┌");
          case CellType.EastSouth:
            SetCellBelow(  newOutput, row, col, aValue, aDirection );
            SetCellRight(  newOutput, row, col, aValue, aDirection );
            break;

          ///   case 9:  Console.Write("┐");
          case CellType.SouthWest:
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 10: Console.Write("┘");
          case CellType.WestNorth:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            break;

          ///   // full connection
          ///   case 11: Console.Write("┼");
          case CellType.NorthEastSouthWest:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;
        }
      }
    }


    private void SetCellRight( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      // test if the cell to the right has a West connection
      if(aDirection != direction.right && col < (itsSideLength - 1)
       && (itsGrid[row, col + 1] == CellType.WestEast
        || itsGrid[row, col + 1] == CellType.WestNorthEast
        || itsGrid[row, col + 1] == CellType.EastSouthWest
        || itsGrid[row, col + 1] == CellType.SouthWestNorth
        || itsGrid[row, col + 1] == CellType.SouthWest
        || itsGrid[row, col + 1] == CellType.WestNorth
        || itsGrid[row, col + 1] == CellType.NorthEastSouthWest))
      {
        SetPath( newOutput, row, col + 1, aValue, direction.left );
      }
    }

    private void SetCellLeft( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      // test if the cell to the left has an East connection
      if(aDirection != direction.left && col > 0
       && (itsGrid[row, col - 1] == CellType.WestEast
        || itsGrid[row, col - 1] == CellType.WestNorthEast
        || itsGrid[row, col - 1] == CellType.EastSouthWest
        || itsGrid[row, col - 1] == CellType.NorthEastSouth
        || itsGrid[row, col - 1] == CellType.NorthEast
        || itsGrid[row, col - 1] == CellType.EastSouth
        || itsGrid[row, col - 1] == CellType.NorthEastSouthWest))
      {
        SetPath( newOutput, row, col - 1, aValue, direction.right );
      }
    }

    private void SetCellBelow( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      // test if the cell below has a North connection
      if(aDirection != direction.below && row < (itsSideLength - 1)
       && (itsGrid[row + 1, col] == CellType.NorthSouth
        || itsGrid[row + 1, col] == CellType.WestNorthEast
        || itsGrid[row + 1, col] == CellType.NorthEastSouth
        || itsGrid[row + 1, col] == CellType.SouthWestNorth
        || itsGrid[row + 1, col] == CellType.NorthEast
        || itsGrid[row + 1, col] == CellType.WestNorth
        || itsGrid[row + 1, col] == CellType.NorthEastSouthWest))
      {
        SetPath( newOutput, row + 1, col, aValue, direction.above );
      }
    }

    private void SetCellAbove( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      // test if the cell above has a North connection
      if(aDirection != direction.above && row > 0
       && (itsGrid[row - 1, col] == CellType.NorthSouth
        || itsGrid[row - 1, col] == CellType.EastSouthWest
        || itsGrid[row - 1, col] == CellType.NorthEastSouth
        || itsGrid[row - 1, col] == CellType.SouthWestNorth
        || itsGrid[row - 1, col] == CellType.EastSouth
        || itsGrid[row - 1, col] == CellType.SouthWest
        || itsGrid[row - 1, col] == CellType.NorthEastSouthWest))
      {
        SetPath( newOutput, row - 1, col, aValue, direction.below );
      }
    }



    /// <summary>
    ///  NAND gate output = 1, unless all 3 inputs are 1
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="output"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private bool CellValue( CellType[,] grid, char[,] output, int row, int col )
    {
      bool cellValue = true;

      // test if this is a delay node
      // - delay nodes will do an OR operation on the inputs
      bool delayNode = false;
      if(grid[row, col] >= CellType.NorthDelay && grid[row, col] <= CellType.WestDelay)
      {
        delayNode = true;
        cellValue = false;
      }

      switch(grid[row, col])
      {
        case CellType.NorthNode:  // "↑"
          cellValue &= ValueOfLeftCell( row, col );
          cellValue &= ValueOfRightCell( row, col );
          cellValue &= ValueOfCellBelow( row, col );
          break;

        case CellType.EastNode:  // "→"    
          cellValue &= ValueOfCellAbove( row, col );
          cellValue &= ValueOfLeftCell( row, col );
          cellValue &= ValueOfCellBelow( row, col );
          break;

        case CellType.SouthNode:  // "↓"
          cellValue &= ValueOfLeftCell( row, col );
          cellValue &= ValueOfRightCell( row, col );
          cellValue &= ValueOfCellAbove( row, col );
          break;

        case CellType.WestNode:  // "←"
          cellValue &= ValueOfCellAbove( row, col );
          cellValue &= ValueOfRightCell( row, col );
          cellValue &= ValueOfCellBelow( row, col );
          break;

        case CellType.NorthDelay:  // "▲"
          cellValue |= ValueOfLeftCellDelay( row, col );         
          cellValue |= ValueOfRightCellDelay( row, col );        
          cellValue |= ValueOfCellBelowDelay( row, col );        
          break;

        case CellType.EastDelay:  // "►"    
          cellValue |= ValueOfCellAboveDelay( row, col );
          cellValue |= ValueOfLeftCellDelay( row, col );
          cellValue |= ValueOfCellBelowDelay( row, col );
          break;

        case CellType.SouthDelay:  // "▼"
          cellValue |= ValueOfLeftCellDelay( row, col );
          cellValue |= ValueOfRightCellDelay( row, col );
          cellValue |= ValueOfCellAboveDelay( row, col );
          break;

        case CellType.WestDelay:  // "◄"
          cellValue |= ValueOfCellAboveDelay( row, col );
          cellValue |= ValueOfRightCellDelay( row, col );
          cellValue |= ValueOfCellBelowDelay( row, col );
          break;
      }

      if(delayNode)
      {
        return cellValue;
      }

      return !cellValue;
    }


    /// <summary>
    /// Test if the cell at the specified location has a 'South' output
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool TestForSouthConnection( int row, int col )
    {
      return itsGrid[row, col] == CellType.NorthSouth
          || itsGrid[row, col] == CellType.EastSouthWest
          || itsGrid[row, col] == CellType.NorthEastSouth
          || itsGrid[row, col] == CellType.SouthWestNorth
          || itsGrid[row, col] == CellType.EastSouth
          || itsGrid[row, col] == CellType.SouthWest
          || itsGrid[row, col] == CellType.NorthEastSouthWest
          || itsGrid[row, col] == CellType.SouthNode
          || itsGrid[row, col] == CellType.SouthDelay;
    }


    /// <summary>
    /// Test if the cell at the specified location has a 'North' output
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool TestForNorthConnection( int row, int col )
    {
      return itsGrid[row, col] == CellType.NorthSouth
          || itsGrid[row, col] == CellType.WestNorthEast
          || itsGrid[row, col] == CellType.NorthEastSouth
          || itsGrid[row, col] == CellType.SouthWestNorth
          || itsGrid[row, col] == CellType.NorthEast
          || itsGrid[row, col] == CellType.WestNorth
          || itsGrid[row, col] == CellType.NorthEastSouthWest
          || itsGrid[row, col] == CellType.NorthNode
          || itsGrid[row, col] == CellType.NorthDelay;
    }


    /// <summary>
    /// Test if the cell at the specified location has a 'West' output
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool TestForWestConnection( int row, int col )
    {
      return itsGrid[row, col] == CellType.WestEast
          || itsGrid[row, col] == CellType.WestNorthEast
          || itsGrid[row, col] == CellType.EastSouthWest
          || itsGrid[row, col] == CellType.SouthWestNorth
          || itsGrid[row, col] == CellType.SouthWest
          || itsGrid[row, col] == CellType.WestNorth
          || itsGrid[row, col] == CellType.NorthEastSouthWest
          || itsGrid[row, col] == CellType.WestNode
          || itsGrid[row, col] == CellType.WestDelay;
    }


    /// <summary>
    /// Test if the cell at the specified location has a 'East' output
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool TestForEastConnection( int row, int col )
    {
      return itsGrid[row, col] == CellType.WestEast
          || itsGrid[row, col] == CellType.WestNorthEast
          || itsGrid[row, col] == CellType.EastSouthWest
          || itsGrid[row, col] == CellType.NorthEastSouth
          || itsGrid[row, col] == CellType.NorthEast
          || itsGrid[row, col] == CellType.EastSouth
          || itsGrid[row, col] == CellType.NorthEastSouthWest
          || itsGrid[row, col] == CellType.EastNode
          || itsGrid[row, col] == CellType.EastDelay;
    }


    /// <summary>
    /// if the cell above has an output of zero and has a connection into this cell then return false 
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool ValueOfCellAbove( int aRow, int aCol )
    {
      // value of cell above - cells with South connection
      if(aRow > 0)
      {
        int row = aRow - 1;
        int col = aCol;

        if(TestForSouthConnection( row, col ))
        {
          if (itsOutput[row, col] == '0' || itsOutput[row, col] == '<')
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// if the cell below has an output of zero and has a connection into this cell then return false 
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool ValueOfCellBelow( int aRow, int aCol )
    {
      // value of cell below
      if(aRow < (itsSideLength - 1))
      {
        int row = aRow + 1;
        int col = aCol;

        // connections with a North output
        if(TestForNorthConnection( row, col ))
        {
          if (itsOutput[row, col] == '0' || itsOutput[row, col] == '<')
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// if the cell to the right has an output of zero and has a connection into this cell then return false 
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool ValueOfRightCell( int aRow, int aCol )
    {
      // value of right cell
      if(aCol < (itsSideLength - 1))
      {
        int row = aRow;
        int col = aCol + 1;

        // connections with a West output
        if(TestForWestConnection( row, col ))
        {
          if (itsOutput[row, col] == '0' || itsOutput[row, col] == '<')
          {
            return false;
          }
        }
      }
      return true;
    }
    

    /// <summary>
    /// get the value of the left hand cell, if it contains a connection into the current cell
    /// </summary>
    /// <param name="output"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns>
    /// the value of the left hand cell, if it contains a connection into the current cell, 
    /// true otherwise
    /// </returns>
    private bool ValueOfLeftCell( int aRow, int aCol )
    {
      // value of left cell
      if(aCol > 0)
      {
        int row = aRow;
        int col = aCol - 1;

        // connnections with an East output
        if(TestForEastConnection( row, col ))
        {
          if (itsOutput[row, col] == '0' || itsOutput[row, col] == '<')
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// if the cell above has an output of zero and has a connection into this cell then return false 
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool ValueOfCellAboveDelay(int aRow, int aCol)
    {
      // value of cell above - cells with South connection
      if (aRow > 0)
      {
        int row = aRow - 1;
        int col = aCol;

        if (TestForSouthConnection(row, col))
        {
          if (itsOutput[row, col] == '1' || itsOutput[row, col] == '>')
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// if the cell below has an output of zero and has a connection into this cell then return false 
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool ValueOfCellBelowDelay(int aRow, int aCol)
    {
      // value of cell below
      if (aRow < (itsSideLength - 1))
      {
        int row = aRow + 1;
        int col = aCol;

        // connections with a North output
        if (TestForNorthConnection(row, col))
        {
          if (itsOutput[row, col] == '1' || itsOutput[row, col] == '>')
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// if the cell to the right has an output of zero and has a connection into this cell then return false 
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool ValueOfRightCellDelay(int aRow, int aCol)
    {
      // value of right cell
      if (aCol < (itsSideLength - 1))
      {
        int row = aRow;
        int col = aCol + 1;

        // connections with a West output
        if (TestForWestConnection(row, col))
        {
          if (itsOutput[row, col] == '1' || itsOutput[row, col] == '>')
          {
            return true;
          }
        }
      }
      return false;
    }


    /// <summary>
    /// get the value of the left hand cell, if it contains a connection into the current cell
    /// </summary>
    /// <param name="output"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns>
    /// the value of the left hand cell, if it contains a connection into the current cell, 
    /// true otherwise
    /// </returns>
    private bool ValueOfLeftCellDelay(int aRow, int aCol)
    {
      // value of left cell
      if (aCol > 0)
      {
        int row = aRow;
        int col = aCol - 1;

        // connnections with an East output
        if (TestForEastConnection(row, col))
        {
          if (itsOutput[row, col] == '1' || itsOutput[row, col] == '>')
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// write the current state of the grid output to the console
    /// </summary>
    /// <param name="aPassNumber"></param>
    private void ShowOutput( int aPassNumber )
    {
      Console.WriteLine( aPassNumber + "." );

      for(int row = 0; row < itsSideLength; row++)
      {
        Console.Write( "   " );

        for(int col = 0; col < itsSideLength; col++)
        {
          Console.Write( itsOutput[row, col] + " " );
        }
        Console.WriteLine();
      }

      Console.WriteLine( "____________" );
    }

    #endregion


    #region Node Drawing

    /// <summary>
    /// write the structure of the grid to the console
    /// </summary>
    private void ShowGrid( string aImageName )
    {
      Console.WriteLine();

      for(int row = 0; row < itsSideLength; row++)
      {
        Console.Write( "   " );

        for(int col = 0; col < itsSideLength; col++)
        {
          switch(itsGrid[row, col])
          {
            // empty cell
            case CellType.EmptyCell: Console.Write( " " ); break;

            // straight connections
            case CellType.NorthSouth: Console.Write("│"); break;
            case CellType.WestEast: Console.Write( "─" ); break;

            // T connections
            case CellType.WestNorthEast: Console.Write( "┴" ); break;
            case CellType.EastSouthWest: Console.Write("┬"); break;
            case CellType.NorthEastSouth: Console.Write( "├" ); break;
            case CellType.SouthWestNorth: Console.Write("┤"); break;

            // L connections 
            case CellType.NorthEast: Console.Write("└"); break;
            case CellType.EastSouth: Console.Write("┌"); break;
            case CellType.SouthWest: Console.Write("┐"); break;
            case CellType.WestNorth: Console.Write("┘"); break;

            // full connection
            case CellType.NorthEastSouthWest: Console.Write("┼"); break;

            // nodes
            case CellType.NorthNode: Console.Write("↑"); break;
            case CellType.EastNode: Console.Write("→"); break;
            case CellType.SouthNode: Console.Write("↓"); break;
            case CellType.WestNode: Console.Write("←"); break;

            case CellType.NorthDelay: Console.Write( "▲"); break;
            case CellType.EastDelay:  Console.Write( "►"); break;
            case CellType.SouthDelay: Console.Write( "▼"); break;
            case CellType.WestDelay:  Console.Write( "◄"); break;

            default: Console.Write(" "); break;
          }

          Console.Write( " " );
        }
        Console.WriteLine();
      }
      Console.WriteLine( "____________" );

      // create an image of this grid
      CreateGridImage(aImageName);
    }


    private void CreateGridImage(string aImageName)
    {
      int pixelLength = (itsSideLength * 32) + 1;

      Bitmap bitmap = new Bitmap(pixelLength, pixelLength);
      Graphics graphics = Graphics.FromImage(bitmap);

      graphics.FillRectangle(Brushes.White, 0, 0, pixelLength, pixelLength);

      Pen linePen = new Pen(Color.Black,2);
      linePen.StartCap = LineCap.Square;
      linePen.EndCap = LineCap.Square;     


      Pen dashPen = new Pen(Color.DarkGray, 2);
      dashPen.DashStyle = DashStyle.Dash;

      // draw grid lines
      for(int row = 0; row < itsSideLength; row++)
      {
        if(row < (itsSideLength - 1))
        {
          graphics.DrawLine( dashPen, 0, (row * 32) + 32, pixelLength, (row * 32) + 32 );
        }

        for(int col = 0; col < itsSideLength; col++)
        {
          int x = (col * 32);
          int y = (row * 32);

          if(row == 0)
          {
            graphics.DrawLine( dashPen, x, 0, x, pixelLength );
          }
        }
      }
      
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          int x = (col * 32);
          int y = (row * 32);

          switch (itsGrid[row, col])
          {
            // empty cell
            case CellType.EmptyCell: break;

            // straight connections
            case CellType.NorthSouth:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 32);
              break;
            case CellType.WestEast:
              graphics.DrawLine(linePen, x, y + 16, x + 32, y + 16);
              break;

            // L connections 
            case CellType.NorthEast:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 16);
              graphics.DrawLine(linePen, x + 16, y + 16, x + 32, y + 16);
              break;
            case CellType.EastSouth:
              graphics.DrawLine(linePen, x + 16, y + 16, x + 16, y + 32);
              graphics.DrawLine(linePen, x + 16, y + 16, x + 32, y + 16);
              break;
            case CellType.SouthWest:
              graphics.DrawLine(linePen, x + 16, y + 16, x + 16, y + 32);
              graphics.DrawLine(linePen, x, y + 16, x + 16, y + 16);
              break;
            case CellType.WestNorth:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 16);
              graphics.DrawLine(linePen, x, y + 16, x + 16, y + 16);
              break;

            // T connections
            case CellType.WestNorthEast:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 16);
              graphics.DrawLine(linePen, x, y + 16, x + 32, y + 16);
              break;
            case CellType.EastSouthWest:
              graphics.DrawLine(linePen, x + 16, y + 16, x + 16, y + 32);
              graphics.DrawLine(linePen, x, y + 16, x + 32, y + 16);
              break;
            case CellType.NorthEastSouth:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 32);
              graphics.DrawLine(linePen, x + 16, y + 16, x + 32, y + 16);
              break;
            case CellType.SouthWestNorth:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 32);
              graphics.DrawLine(linePen, x, y + 16, x + 16, y + 16);
              break;

            // full connection
            case CellType.NorthEastSouthWest:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 32);
              graphics.DrawLine(linePen, x, y + 16, x + 32, y + 16);
              break;

            // nodes
            case CellType.NorthNode:
              DrawNorthNode(graphics, linePen, x, y);
              break;
            case CellType.EastNode:
              DrawEastNode(graphics, linePen, x, y);
              break;
            case CellType.SouthNode:
              DrawSouthNode(graphics, linePen, x, y);
              break;
            case CellType.WestNode:
              DrawWestNode(graphics, linePen, x, y);
              break;

            // delays
            case CellType.NorthDelay:
              DrawNorthDelay(graphics, linePen, x, y);
              break;
            case CellType.EastDelay:
              DrawEastDelay(graphics, linePen, x, y);
              break;
            case CellType.SouthDelay:
              DrawSouthDelay(graphics, linePen, x, y);
              break;
            case CellType.WestDelay:
              DrawWestDelay(graphics, linePen, x, y);
              break;

            default: break;
          }
        }
      }

      //graphics.DrawRectangle(linePen, 0, 0, pixelLength, pixelLength);

      linePen.Dispose();
      dashPen.Dispose();

      bitmap.Save(aImageName);
    }


    /// <summary>
    /// Draw the main shape of a node
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="linePen"></param>
    /// <param name="linePoints"></param>
    /// <param name="sidePoints"></param>
    private static void DrawNode( Graphics graphics, Pen linePen, Point[] linePoints, Point[] sidePoints )
    {
      using(SolidBrush brush = new SolidBrush( Color.FromArgb( 0x7F, 0x92, 0xFF ) ))
      {
        // draw the shading for the top curve
        GraphicsPath path = new GraphicsPath();
        path.AddCurve( linePoints );
        path.AddLines( sidePoints );
        graphics.FillPath( brush, path );
      }

      // draw the top curve
      graphics.DrawCurve( linePen, linePoints );

      // draw the sides        
      graphics.DrawLines( linePen, sidePoints );
    }


    /// <summary>
    /// Draw the main shape for a delay node
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="linePen"></param>
    /// <param name="linePoints"></param>
    /// <param name="sidePoints"></param>
    private static void DrawDelay( Graphics graphics, Pen linePen, Point[] linePoints, Point[] sidePoints )
    {
      using(SolidBrush brush = new SolidBrush( Color.FromArgb( 0x00, 0x9B, 0x0E ) ))
      {
        // draw the shading for the top curve
        GraphicsPath path = new GraphicsPath();
        path.AddLines( linePoints );
        path.AddLines( sidePoints );
        graphics.FillPath( brush, path );

        // draw the top curve
        graphics.DrawLines( linePen, linePoints );

        // draw the sides        
        graphics.DrawLines( linePen, sidePoints );        
      }
    }


    private static Point[] GetNorthSidePoints( int x, int endX, int midY, int endY )
    {
      Point[] sidePoints = { 
                               new Point(x, midY),                               
                               new Point(x, endY),                               
                               new Point(endX, endY),
                               new Point(endX, midY)                    
                             };
      return sidePoints;
    }

    private static Point[] GetEastSidePoints( int x, int y, int midX, int endY )
    {
      Point[] sidePoints = { 
                               new Point(midX, y),
                               new Point(x, y),                               
                               new Point(x, endY),
                               new Point(midX, endY)                    
                             };
      return sidePoints;
    }

    private static Point[] GetSouthSidePoints( int x, int y, int endX, int midY )
    {
      Point[] sidePoints = { 
                             new Point(x, midY),                               
                             new Point(x, y),                               
                             new Point(endX, y),
                             new Point(endX, midY)                    
                           };
      return sidePoints;
    }

    private static Point[] GetWestSidePoints( int y, int midX, int endX, int endY )
    {
      Point[] sidePoints = { 
                               new Point(midX, y),                               
                               new Point(endX, y),                               
                               new Point(endX, endY),
                               new Point(midX, endY)                    
                             };
      return sidePoints;
    }

    private static void DrawWestNode( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(midX,   y),
                               new Point(x + 12, y+ 4 ),
                               new Point(x + 8,  y+ mid),
                               new Point(x + 12, y+ 28),
                               new Point(midX,   endY - 1)                       
                             };

      Point[] sidePoints = GetWestSidePoints( y, midX, endX, endY );

      DrawNode( graphics, linePen, linePoints, sidePoints );

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse( linePen, x, (midY) - radius, (radius * 2), (radius * 2) );
    }

    private static void DrawEastNode( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(midX, y),
                               new Point(x + 20, y+ 4 ),
                               new Point(x + 24, midY),
                               new Point(x + 20, y+ 28),
                               new Point(midX, endY - 1)                       
                             };

      Point[] sidePoints = GetEastSidePoints( x, y, midX, endY );

      DrawNode( graphics, linePen, linePoints, sidePoints );

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse( linePen, (endX) - (radius * 2), (midY) - radius, (radius * 2), (radius * 2) );
    }

    private static void DrawNorthNode( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x, midY),
                               new Point(x + 4, y + 12),
                               new Point(midX, y + 8),
                               new Point(x + 28, y + 12),
                               new Point(endX - 1, midY)
                             };

      Point[] sidePoints = GetNorthSidePoints( x, endX, midY, endY );

      DrawNode( graphics, linePen, linePoints, sidePoints );

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse( linePen, (midX) - radius, y, (radius * 2), (radius * 2) );
    }
    
    private static void DrawSouthNode( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                             new Point(x, midY),
                             new Point(x + 4, y + 20),
                             new Point(midX, y + 24),
                             new Point(x + 28, y + 20),
                             new Point(endX - 1, midY)                       
                           };

      Point[] sidePoints = GetSouthSidePoints( x, y, endX, midY );

      DrawNode( graphics, linePen, linePoints, sidePoints );

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse( linePen, (midX) - radius, (endY) - (radius * 2), (radius * 2), (radius * 2) );
    }


    //
    // Draw Delay Nodes
    //

    private static void DrawWestDelay(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                              new Point(midX - 1, y ),                               
                              new Point(x     , midY - 1),
                              new Point(x     , midY),
                              new Point(midX - 1, endY - 1)                       
                            };

      Point[] sidePoints = GetWestSidePoints( y, midX, endX, endY );

      DrawDelay( graphics, linePen, linePoints, sidePoints );
    }

    
    private static void DrawEastDelay(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(midX + 1, y ),
                               new Point(endX, y + mid - 1),
                               new Point(endX, y + mid),
                               new Point(midX + 1, endY - 1)                       
                             };

      Point[] sidePoints = GetEastSidePoints( x, y, midX, endY );

      DrawDelay( graphics, linePen, linePoints, sidePoints );
    }



    private static void DrawNorthDelay( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY - 1),
                               new Point(midX - 1, y ),
                               new Point(midX, y ),
                               new Point(endX - 1, midY - 1)
                             };

      Point[] sidePoints = GetNorthSidePoints( x, endX, midY, endY );

      DrawDelay( graphics, linePen, linePoints, sidePoints );
    }


    private static void DrawSouthDelay( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY + 1),
                               new Point(midX - 1, endY),
                               new Point(midX, endY),
                               new Point(endX - 1, midY + 1)                       
                             };

      Point[] sidePoints = GetSouthSidePoints( x, y, endX, midY );

      DrawDelay( graphics, linePen, linePoints, sidePoints );
    }


    #endregion Node Drawing
    

    /// <summary>
    /// remove unused nodes or connections
    /// </summary>
    /// <param name="grid"></param>
    private void PruneGrid(bool aShowGrid)
    {
      CellType[,] lastGrid = new CellType[itsSideLength, itsSideLength];

      int pass = 0;
      do
      {
        if (aShowGrid)
        {
          ShowGrid("c:\\prune_" + pass + ".bmp");
        }

        // copy the current bot grid to the 'lastGrid' 
        CopyGrid( lastGrid );

        RemoveNodesWithNoOutput();
        RemoveNodesWithNoInput();

        pass++;
      }
      while(CompareWithLastGrid( lastGrid ) == false);
    }


    /// <summary>
    /// copy the grid to the last grid
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    private void CopyGrid(CellType[,] lastGrid)
    {
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          lastGrid[row, col] = itsGrid[row, col];
        }
      }
    }


    /// <summary>
    /// test if the two supplied grids are the same
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    /// <returns>true if the grids are identical</returns>
    private bool CompareWithLastGrid(CellType[,] lastGrid)
    {
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          if(itsGrid[row, col] != lastGrid[row, col])
          {
            return false;
          }
        }
      }

      return true;
    }




    private void RemoveNodesWithNoInput()
    {
      // remove nodes with no input
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          // test for input from above - connections that point south
          bool above = (row > 0 && TestForSouthConnection((row-1),col));

          // test for input from below - connections that point north
          bool below = (row < (itsSideLength - 1) && TestForNorthConnection((row+1),col));

          // test for input from left - connections that point east
          bool left = (col > 0 && TestForEastConnection(row,(col-1)));

          // test for input from right - connections that point west
          bool right = (col < (itsSideLength - 1) && TestForWestConnection(row,(col+1)));

          if(!above && !below && !left && !right)
          {
            itsGrid[row, col] = 0;
          }

          // vertical '|'
          if(itsGrid[row, col] == CellType.NorthSouth && !above && !below)
          {
            itsGrid[row, col] = 0;
          }

          // horizontal '-'
          if(itsGrid[row, col] == CellType.WestEast && !left && !right)
          {
            itsGrid[row, col] = 0;
          }
          
          /// "┴"                           
          if(itsGrid[row, col] == CellType.WestNorthEast && !left && !above && !right)
          {
            itsGrid[row, col] = 0;
          }

          /// "┬"
          if(itsGrid[row, col] == CellType.EastSouthWest && !left && !below && !right)
          {
            itsGrid[row, col] = 0;
          }

          /// "├"
          if(itsGrid[row, col] == CellType.NorthEastSouth && !above && !right && !below)
          {
            itsGrid[row, col] = 0;
          }

          /// "┤"
          if(itsGrid[row, col] == CellType.SouthWestNorth && !below && !left && !above)
          {
            itsGrid[row, col] = 0;
          }


          /// "└"                            
          if(itsGrid[row, col] == CellType.NorthEast && !above && !right)
          {
            itsGrid[row, col] = 0;
          }

          /// "┌"
          if(itsGrid[row, col] == CellType.EastSouth && !below && !right)
          {
            itsGrid[row, col] = 0;
          }

          /// "┐"
          if(itsGrid[row, col] == CellType.SouthWest && !left && !below)
          {
            itsGrid[row, col] = 0;
          }

          /// "┘"
          if(itsGrid[row, col] == CellType.WestNorth && !left && !above)
          {
            itsGrid[row, col] = 0;
          }
        }
      }
    }

    private void RemoveNodesWithNoOutput()
    {
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          // remove any nodes that point only into an empty cell
          if(itsGrid[row, col] == 0)
          {
            // examine cell above
            if(row > 0
            && (itsGrid[row - 1, col] == CellType.NorthSouth
            || itsGrid[row - 1, col] == CellType.EastSouth
            || itsGrid[row - 1, col] == CellType.SouthWest
            || itsGrid[row - 1, col] == CellType.SouthNode))
            {
              itsGrid[row - 1, col] = 0;
            }

            // examine cell below
            if(row < (itsSideLength - 1)
            && (itsGrid[row + 1, col] == CellType.NorthSouth
              || itsGrid[row + 1, col] == CellType.NorthEast
              || itsGrid[row + 1, col] == CellType.WestNorth
              || itsGrid[row + 1, col] == CellType.NorthNode))
            {
              itsGrid[row + 1, col] = 0;
            }

            // examine cell to left
            if(col > 0
            && (itsGrid[row, col - 1] == CellType.WestEast
            || itsGrid[row, col - 1] == CellType.NorthEast
            || itsGrid[row, col - 1] == CellType.EastSouth
            || itsGrid[row, col - 1] == CellType.EastNode))
            {
              itsGrid[row, col - 1] = 0;
            }

            // examine cell to right
            if(col < (itsSideLength - 1)
            && (itsGrid[row, col + 1] == CellType.WestEast
            || itsGrid[row, col + 1] == CellType.SouthWest
            || itsGrid[row, col + 1] == CellType.WestNorth
            || itsGrid[row, col + 1] == CellType.WestNode))
            {
              itsGrid[row, col + 1] = 0;
            }
          }


          //
          // remove nodes with outputs into each other

          // vertical pair
          if(itsGrid[row, col] == CellType.NorthNode && (row > 0 && itsGrid[row - 1, col] == CellType.SouthNode))
          {
            itsGrid[row, col] = 0;
          }

          // horizontal pair
          if(itsGrid[row, col] == CellType.WestNode && (col > 0 && itsGrid[row, col - 1] == CellType.EastNode))
          {
            itsGrid[row, col] = 0;
          }

          // vertical pair
          if(itsGrid[row, col] == CellType.NorthDelay && (row > 0 && itsGrid[row - 1, col] == CellType.SouthDelay))
          {
            itsGrid[row, col] = 0;
          }

          // horizontal pair
          if(itsGrid[row, col] == CellType.WestDelay && (col > 0 && itsGrid[row, col - 1] == CellType.EastDelay))
          {
            itsGrid[row, col] = 0;
          }
        }
      }
    }
  }
}



