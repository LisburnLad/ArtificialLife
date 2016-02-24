using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  /// <summary>
  /// Pruning Flags
  /// use the flags attribute for bitwise operations
  /// </summary>
  [Flags]
  public enum Pruning : uint
  {
    None = 0,
    EdgeConnections = 1,
    NoOutput = 2,
    UnjoinedConnections = 4,  
    All = unchecked((uint)~0)
  }


  public enum Direction
  {
    North,
    East,
    South,
    West
  };

  /// <summary>
  /// direction of a connection relative to its current direction
  /// </summary>
  public enum RelativeDirection
  {
    Back,
    Left,
    Forward,
    Right
  }

  enum Movement
  {
    Left,
    Forward,
    Right
  };

  public enum CellType
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

    // Nands
    NorthNand,          // "↑" - 12
    EastNand,           // "→" - 13
    SouthNand,          // "↓" - 14
    WestNand,           // "←" - 15

    // Delay Nodes
    NorthDelay,         // "▲" - 16 - 10000
    EastDelay,          // "►" - 17 - 10001
    SouthDelay,         // "▼" - 18 - 10010
    WestDelay,          // "◄" - 19 - 10011

    NorthXor,           // "˄" - 20 - 10100
    EastXor,            // "˃" - 21 - 10101
    SouthXor,           // "˅" - 22 - 10110
    WestXor,            // "˂" - 23 - 10111

    NorthTrigger,       // "▲" - 20 - 11000
    EastTrigger,        // "►" - 21 - 11001
    SouthTrigger,       // "▼" - 22 - 11010
    WestTrigger,        // "◄" - 23 - 11011

    NorthOr,            // "▲" - 20 - 11100
    EastOr,             // "►" - 21 - 11101
    SouthOr,            // "▼" - 22 - 11110
    WestOr,             // "◄" - 23 - 11111

    NorthPulse,         // "▲" - 24
    EastPulse,          // "►" - 25
    SouthPulse,         // "▼" - 26
    WestPulse,          // "◄" - 27

    ConnectionStart,    // "o"
    ConnectionEnd,      // "X"

    InputCell,
    OutputCell,
    OutOfBounds
  };

  class Common
  {
    public static char GetCellTypeChar(CellType aCellType)
    {
      switch (aCellType)
      {
        case CellType.EmptyCell: return ' ';

        // Straight Connections
        case CellType.NorthSouth: return '|';
        case CellType.WestEast: return '─';

        // T Connections
        case CellType.WestNorthEast: return '┴';
        case CellType.EastSouthWest: return '┬';
        case CellType.NorthEastSouth: return '├';
        case CellType.SouthWestNorth: return '┤';

        // L Connections
        case CellType.NorthEast: return '└';
        case CellType.EastSouth: return '┌';
        case CellType.SouthWest: return '┐';
        case CellType.WestNorth: return '┘';

        // Full Connection
        case CellType.NorthEastSouthWest: return '┼';

        // Nodes
        case CellType.NorthNand: return '↑';
        case CellType.EastNand: return '→';
        case CellType.SouthNand: return '↓';
        case CellType.WestNand: return '←';

        // Delay Nodes
        case CellType.NorthDelay: return '▲';
        case CellType.EastDelay: return '►';
        case CellType.SouthDelay: return '▼';
        case CellType.WestDelay: return '◄';

        case CellType.NorthXor: return '˄';
        case CellType.EastXor: return '˃';
        case CellType.SouthXor: return '˅';
        case CellType.WestXor: return '˂';

        case CellType.NorthTrigger: return 't';
        case CellType.EastTrigger: return 't';
        case CellType.SouthTrigger: return 't';
        case CellType.WestTrigger: return 't';

        case CellType.NorthOr: return 'O';
        case CellType.EastOr: return 'O';
        case CellType.SouthOr: return 'O';
        case CellType.WestOr: return 'O';

        case CellType.NorthPulse: return 'P';
        case CellType.EastPulse: return 'P';
        case CellType.SouthPulse: return 'P';
        case CellType.WestPulse: return 'P';

        case CellType.ConnectionStart: return 'o';
        case CellType.ConnectionEnd: return 'X';
      }
      return ' ';
    }


    /// <summary>
    /// Get the directions of the supplied connection
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public static DirectionFlag GetDirectionFlag(CellType aCellType)
    {
      DirectionFlag connectionDirections = DirectionFlag.None;

      // set the connection output directions
      switch (aCellType)
      {
        // Straight Connections
        case CellType.NorthSouth: connectionDirections = DirectionFlag.North | DirectionFlag.South; break;
        case CellType.WestEast: connectionDirections = DirectionFlag.West | DirectionFlag.East; break;

        // T Connections
        case CellType.WestNorthEast: connectionDirections = DirectionFlag.West | DirectionFlag.North | DirectionFlag.East; break;
        case CellType.EastSouthWest: connectionDirections = DirectionFlag.East | DirectionFlag.South | DirectionFlag.West; break;
        case CellType.NorthEastSouth: connectionDirections = DirectionFlag.North | DirectionFlag.East | DirectionFlag.South; break;
        case CellType.SouthWestNorth: connectionDirections = DirectionFlag.South | DirectionFlag.West | DirectionFlag.North; break;

        // L Connections
        case CellType.NorthEast: connectionDirections = DirectionFlag.North | DirectionFlag.East; break;
        case CellType.EastSouth: connectionDirections = DirectionFlag.East | DirectionFlag.South; break;
        case CellType.SouthWest: connectionDirections = DirectionFlag.South | DirectionFlag.West; break;
        case CellType.WestNorth: connectionDirections = DirectionFlag.West | DirectionFlag.North; break;

        // Full Connection
        case CellType.NorthEastSouthWest: connectionDirections = DirectionFlag.North | DirectionFlag.East | DirectionFlag.South | DirectionFlag.West; break;
      }

      return connectionDirections;
    }


    /// <summary>
    /// Get a connection with all the directions of the supplied flag
    /// </summary>
    /// <param name="currentDirections"></param>
    /// <returns></returns>
    public static CellType GetConnectionFromDirectionFlag(DirectionFlag aDirections)
    {
      bool north = aDirections.HasFlag(DirectionFlag.North);
      bool east = aDirections.HasFlag(DirectionFlag.East);
      bool south = aDirections.HasFlag(DirectionFlag.South);
      bool west = aDirections.HasFlag(DirectionFlag.West);

      if (north && east && south && west )
      {
        return CellType.NorthEastSouthWest;
      }

      if (north && east && west) 
      { 
        return CellType.WestNorthEast;
      }

      if ( east && south && west)
      {
        return CellType.EastSouthWest;
      }

      if (north && east && south)
      {
        return CellType.NorthEastSouth;
      }

      if (north && south && west)
      {
        return CellType.SouthWestNorth;
      }

      if (north && east )
      {
        return CellType.NorthEast;
      }
      if (east && south)
      {
        return CellType.EastSouth;
      }

      if (south && west)
      {
        return CellType.SouthWest;
      }

      if (north && west)
      {
        return CellType.WestNorth;
      }

      if (north && south)
      {
        return CellType.NorthSouth;
      }

      if (east && west)
      {
        return CellType.WestEast;
      }

      return CellType.EmptyCell; 
    }
  }
}
