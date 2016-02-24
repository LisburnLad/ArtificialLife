using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;

namespace ArtificialLife
{
  class TargetedConnection
  {
    public TargetedConnection( int part1, int part2, int part3 )
    {
      SetConnection(part1, part2, part3);
    }

    /// <summary>
    /// the rule used to terminate this line if the target node isn't found
    /// </summary>
    private int itsRule = 0;
    public int Rule 
    { 
      get
      {
        return itsRule;
      }
 
      set
      {
        itsRule = value;
   
        // set the target type based on the rule value
        switch (value)
        {          
          case 2:  itsTargetType = CellType.NorthSouth;   break; // - indicates that output cell should be targeted
          case 3:  itsTargetType = CellType.NorthNand;    break; //  
          case 4:  itsTargetType = CellType.NorthDelay;   break; //  
          case 5:  itsTargetType = CellType.NorthXor;     break; //  
          case 6:  itsTargetType = CellType.NorthTrigger; break; //  
          case 7:  itsTargetType = CellType.NorthOr;      break; //  
          case 8:  itsTargetType = CellType.NorthPulse;   break; // 
          default: itsTargetType = CellType.EmptyCell;    break; // - indicates that rule should be used for termination
        } 
      }
    }

    /// <summary>
    /// the target node type to look for when the connection reaches its destination area
    /// </summary>
    public CellType itsTargetType { get; set; }

    /// <summary>
    /// the proportion of the grid to move before looking for the target connection
    /// </summary>
    public int itsDistance { get; set; }

    /// <summary>
    /// the size of the area to search when looking for the target connection
    /// </summary>
    public int itsArea { get; set; }  

    /// <summary>
    /// the direction to start searching for the target connection
    /// </summary>
    public int itsDirection { get; set; }


    private void SetConnection( int part1, int part2, int part3 )
    {
      Rule = part1;

      // take the first 2 bits of the 2nd rule (by shifting right pushing the 3 least significant bits out)
      itsDistance = (part2 >> 3);

      // take the 3rd and 4th bits of the 2nd rule by ANDing with 00110 and then shifting a bit right
      itsArea = (part2 & 0x06) >> 1;

      // take the last bit of the 2nd rule (ANDing with 00001 then shift left to make MSB of new value)
      // then dump the last 4 bits of 3rd rule by shifting right
      itsDirection = ((part2 & 0x01) << 1) + (part3 >> 4);      
    }

  };



  /// <summary>
  /// Chromosome length = 241 = (4x4) input cells + (15x3x4) rules + (15x3) cell types
  // - a rule of 0000 is now considered to be empty, no rule
  // - a input cell of 0000 is now no input
  // - rule definitions therefore begin at 0001 = A up to 1111 = O
  /// </summary>
  class ChromosomeDecode
  {
    public static int kTotalRules 
    {
      get { return GetNumberOfRules(); }
    }


    /// <summary>
    /// the number of input cells - each points to a seperate rule
    /// </summary>
    static int kNumberOfInputs = 4;


    /// <summary>
    /// each node type can have a possible of 3 output types - left, center and right
    /// </summary>
    static int kOutputRules = 3;

    // the number of bits used to encode each rule: (0000) is reserved for "no-rule"
    //
    // 00001 = A
    // 00010 = B
    // 00011 = C
    // 00100 = D
    // 00101 = E
    // 00110 = F
    // 00111 = G
    // 01000 = H
    // 01001 = I
    // 01010 = J
    // 01011 = K
    // 01100 = L
    // 01101 = M
    // 01110 = N
    // 01111 = O
    // ...
    // 10000
    const int kBitsPerRule = 5;

    // the number of bits for each cell type (e.g. Nand, Xor, etc.)
    const int kBitsPerCellType = 5;

    /// <summary>
    /// Each input can specify its own rule letter A-O
    /// </summary>
    int[] itsInputRules;

    /// <summary>
    /// the set of rules extracted from the chromosome
    /// </summary>
    int[,] itsRules;

    /// <summary>
    /// the definition of rule letter to cell type
    /// </summary>
    CellType[] itsCellTypes;

    public CellType GetCellType(int aID)
    {
      if( itsCellTypes != null && itsCellTypes.Length >= aID)
      {
        return itsCellTypes[aID];
      }
      return CellType.EmptyCell;
    }


    /// <summary>
    /// get the output values for the specified rule
    /// </summary>
    /// <param name="aIndex"></param>
    /// <param name="aLeft"></param>
    /// <param name="aCenter"></param>
    /// <param name="aRight"></param>
    /// <returns></returns>
    private int GetRule(int aIndex, ref int aLeft, ref int aCenter, ref int aRight )
    {
      if( aIndex < kTotalRules )
      {
        aLeft   = itsRules[aIndex, 0];
        aCenter = itsRules[aIndex, 1];
        aRight  = itsRules[aIndex, 2];

        int ruleCount = ((aLeft > 0) ? 1 : 0) + ((aCenter > 0) ? 1 : 0) + ((aRight > 0) ? 1 : 0);

        return ruleCount;
      }

      return 0;
    }


    /// <summary>
    /// get the rule at the specified index on the chromosome
    /// </summary>
    /// <param name="aRuleIndex"></param>
    /// <param name="aCellInfo"></param>
    /// <returns></returns>
    private int GetRuleByIndex(int aRuleIndex, CellInformation[] aCellInfo)
    {
      // a rule index of zero is the reserved "no-rule"
      if (aRuleIndex > 0)
      {
        try
        {
          if (aRuleIndex < kTotalRules)
          {
            int left = 0;
            int center = 0;
            int right = 0;
            int ruleCount = GetRule(aRuleIndex, ref left, ref center, ref right);

            // the cell IDs specified by the rule - these are the type letters A, B, C etc.
            // and the cell types, which are the actual node types (e.g. Nand,Xor, etc.)
            aCellInfo[0].Set(left, itsCellTypes[left]);
            aCellInfo[1].Set(center, itsCellTypes[center]);
            aCellInfo[2].Set(right, itsCellTypes[right]);

            return ruleCount;
          }
        }
        catch
        {
        }
      }

      return 0;
    }

    /// <summary>
    /// get the rule at the specified index and set the rule cell types and IDs
    /// </summary>
    /// <param name="aIndex"></param>
    /// <param name="aCellTypes"></param>
    /// <param name="aCellIDs"></param>
    /// <returns></returns>
    public int GetRule(int aRuleID, CellInformation[] aCellInfo)
    {       
      return GetRuleByIndex( aRuleID, aCellInfo );
    }

    /// <summary>
    /// Get the first rule from the chromosome. This is defined as the rule to use for input cells.
    /// </summary>
    /// <param name="aCellInfo"></param>
    public int GetInputRule( CellInformation[] aCellInfo, Direction aDirection, out CellType aCellType, out int aRuleIndex )
    {
      // get the index of the rule for the specifed input connection
      aRuleIndex = GetInputRule(aDirection);

      // get the type of cell the rule represents
      aCellType = itsCellTypes[aRuleIndex];

      // get the first rule on the chromosome
      return GetRuleByIndex(aRuleIndex, aCellInfo);
    }

    /// <summary>
    /// Get the index of the rule for the specifed input connection
    /// (so each input cell can specify one of the rules A-P)
    /// </summary>
    /// <param name="aDirection"></param>
    /// <returns></returns>
    private int GetInputRule(Direction aDirection)
    {
      return itsInputRules[(int)aDirection];
    }

    public TargetedConnection GetTargetedConnection(int aRuleID)
    {
      // get the parts of the rule for the defined rule ID
      int part1 = 0;
      int part2 = 0;
      int part3 = 0;
      
      GetRule(aRuleID, ref part1, ref part2, ref part3);

      TargetedConnection targetedConnection = new TargetedConnection( part1, part2, part3 );
      return targetedConnection;
    }

    /// <summary>
    /// the chromosome being processed
    /// </summary>
    public Chromosome itsChromosome;


    public ChromosomeDecode( Chromosome aChromosome )
    {
      // save a reference to the chromosome
      itsChromosome = aChromosome;
    }

    public static int GetNumberOfRules()
    {
      // one rule per type 
      return GetNumberOfCellTypes();
    }
    
    /// <summary>
    /// the number of different cell types
    /// </summary>
    /// <returns></returns>
    public static int GetNumberOfCellTypes()
    {
      // one rule per type with the 0000 rule excluded
      return (int)System.Math.Pow(2, kBitsPerRule);
    }

    public static int GetChromosomeLength()
    {
      int ruleSize = (kTotalRules * kOutputRules * kBitsPerRule);
      int nodeDefinitionSize = GetNumberOfCellTypes() * kBitsPerRule;

      //int nodeDefinitionSize = kBitsPerCellType * GetNumberOfCellTypes();

      return ruleSize + nodeDefinitionSize;
    }

    /// <summary>
    /// decode the chromosome to form the rules
    /// </summary>
    public void Run(bool aShowGrid)
    {
      ExtractRules(aShowGrid);
    }

    //private void ExtractRules(bool aShowGrid)
    //{
    //  // create the array of input rules
    //  itsInputRules = new int[kNumberOfInputs];

    //  int rulePosition = 0;
    //  for (int ruleNumber = 0; ruleNumber < kNumberOfInputs; ruleNumber++)
    //  {        
    //    ExtractInputRules(ruleNumber, ref rulePosition);
    //  }

    //  // create the array of rules to extract
    //  itsRules = new int[kTotalRules, kOutputRules];

    //  // rules begin at 1 (=A) 
    //  for (int ruleNumber = 1; ruleNumber < kTotalRules; ruleNumber++)
    //  {
    //    ExtractRules(ref ruleNumber, ref rulePosition);
    //  }

    //  // the mapping of rule letter to cell type
    //  itsCellTypes = new CellType[GetNumberOfCellTypes()];

    //  // cell types also begin at 1 (=a) - cell type 0 is blank
    //  for (int ruleType = 1; ruleType < GetNumberOfCellTypes(); ruleType++ )
    //  {
    //    int cell = GetGeneValue(ref rulePosition, kBitsPerCellType);        
        
    //    // get the type of cell this value represents
    //    CellType cellType = cellType = GetCellTypeFromGeneValue( cell );     

    //    itsCellTypes[ruleType] = cellType;
    //  }

    //  if (aShowGrid)
    //  {
    //    ShowRules();
    //  }
    //}


    private void ExtractRules(bool aShowGrid)
    {
      // create the array of input rules
      itsInputRules = new int[kNumberOfInputs];

      int rulePosition = 0;
      for (int ruleNumber = 0; ruleNumber < kNumberOfInputs; ruleNumber++)
      {
        ExtractInputRules(ruleNumber, ref rulePosition);
      }

      // create the array of rules to extract
      itsRules = new int[kTotalRules, kOutputRules];

      // the mapping of rule letter to cell type
      itsCellTypes = new CellType[GetNumberOfCellTypes()];

      // rules begin at 1 (=A) 
      for (int ruleNumber = 1; ruleNumber < kTotalRules; ruleNumber++)
      {
        ExtractRules(ref ruleNumber, ref rulePosition);


        int cell = GetGeneValue(ref rulePosition, kBitsPerCellType);

        // get the type of cell this value represents
        CellType cellType = cellType = GetCellTypeFromGeneValue(cell);

        itsCellTypes[ruleNumber] = cellType;
      }



      //// cell types also begin at 1 (=a) - cell type 0 is blank
      //for (int ruleType = 1; ruleType < GetNumberOfCellTypes(); ruleType++)
      //{
      //  int cell = GetGeneValue(ref rulePosition, kBitsPerCellType);

      //  // get the type of cell this value represents
      //  CellType cellType = cellType = GetCellTypeFromGeneValue(cell);

      //  itsCellTypes[ruleType] = cellType;
      //}

      if (aShowGrid)
      {
        ShowRules();
      }
    }



    /// <summary>
    /// Get the cell type from the supplied gene value
    /// </summary>
    /// <param name="aGeneValue"></param>
    /// <returns></returns>
    public static CellType GetCellTypeFromGeneValue( int aGeneValue )
    {
      CellType cellType = CellType.EmptyCell;

      // set all values that have 1xxxx to be targeted connections
      if( (aGeneValue >> 4) == 1 )
      {
        return CellType.ConnectionStart;
      }

      switch (aGeneValue)
      {
        case 1: cellType = CellType.NorthSouth; break; // 00001 
        //case 2: cellType = CellType.WestEast; break; // 00010 
        case 3: cellType = CellType.NorthNand; break; // 00011 
        case 4: cellType = CellType.NorthDelay; break; // 00100 
        case 5: cellType = CellType.NorthXor; break; // 00101 
        case 6: cellType = CellType.NorthTrigger; break; // 00110 
        case 7: cellType = CellType.NorthOr; break; // 00111 
        case 8: cellType = CellType.NorthPulse; break; // 01000 
        default: cellType = CellType.EmptyCell; break; // 00000
      }

      return cellType;
    }


    /// <summary>
    /// extract each input rule from the chromosome
    /// </summary>
    /// <param name="aRuleNumber"></param>
    private void ExtractInputRules(int aRuleNumber, ref int aRulePosition)
    {
      itsInputRules[aRuleNumber] = GetGeneValue(ref aRulePosition, kBitsPerRule);
    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="aChromosome"></param>
    private void ExtractRules( ref int aRuleNumber, ref int aRulePosition )
    {
      int leftType   = GetGeneValue(ref aRulePosition, kBitsPerRule);
      int centerType = GetGeneValue(ref aRulePosition, kBitsPerRule);
      int rightType  = GetGeneValue(ref aRulePosition, kBitsPerRule);

      itsRules[aRuleNumber, 0] = leftType;
      itsRules[aRuleNumber, 1] = centerType;
      itsRules[aRuleNumber, 2] = rightType;      
    }


    /// <summary>
    /// get the value of the gene at the specified position and the given length
    /// </summary>
    /// <param name="position"></param>
    /// <param name="aGeneLength"></param>
    /// <returns></returns>
    private int GetGeneValue( ref int aPosition, int aGeneLength)
    {
      int bitValue = 0;
      for (int bit = 0; bit < aGeneLength; bit++)
      {
        bitValue += Convert.ToInt32(itsChromosome.ToBinaryString(aPosition + bit, 1)) * (1 << (aGeneLength - (bit + 1)));
      }

      // increase the position by the number of bits read
      aPosition += aGeneLength;

      return bitValue;
    }

    #region Debug
    
    private void ShowRules()
    {
      for (int ruleNumber = 0; ruleNumber < kTotalRules; ruleNumber++)
      {        
        switch (ruleNumber)
        {
          case 0:  Console.Write("    1 -> "); break;
          case 1:  Console.Write("    2 -> "); break;
          case 2:  Console.Write("    3 -> "); break;
          case 3:  Console.Write("    4 -> "); break;
          case 4:  Console.Write("    5 -> "); break;
          case 5:  Console.Write("    6 -> "); break;
          case 6:  Console.Write("    7 -> "); break;
          case 7:  Console.Write("    8 -> "); break;
          case 8:  Console.Write("    9 -> "); break;
          case 9:  Console.Write("   10 -> "); break;
          case 10: Console.Write("   11 -> "); break;
          case 11: Console.Write("   12 -> "); break;
          case 12: Console.Write("   13 -> "); break;
          case 13: Console.Write("   14 -> "); break;
          case 14: Console.Write("   15 -> "); break;
          case 15: Console.Write("   16 -> "); break;
          default: Console.Write("   NS -> "); break;     // haven't done yet for #'s > 15
        }

        for (int outDirection = 0; outDirection < kOutputRules; outDirection++)
        {
          if (outDirection > 0)
          {
            Console.Write("-");
          }

          switch (itsRules[ruleNumber, outDirection])
          {
            case  0: Console.Write("A"); break;
            case  1: Console.Write("B"); break;
            case  2: Console.Write("C"); break;
            case  3: Console.Write("D"); break;
            case  4: Console.Write("E"); break;
            case  5: Console.Write("F"); break;
            case  6: Console.Write("G"); break;
            case  7: Console.Write("H"); break;
            case  8: Console.Write("I"); break;
            case  9: Console.Write("J"); break;
            case 10: Console.Write("K"); break;
            case 11: Console.Write("L"); break;
            case 12: Console.Write("M"); break;
            case 13: Console.Write("N"); break;
            case 14: Console.Write("O"); break;
            case 15: Console.Write("P"); break;
            default: Console.Write("NS"); break;     // haven't done yet for #'s > 15
          }
        }

        Console.WriteLine();
      }

      for (int ruleType = 0; ruleType < GetNumberOfCellTypes(); ruleType++)
      {
        switch (ruleType)
        {
          case  0: Console.Write("A = "); break;
          case  1: Console.Write("B = "); break;
          case  2: Console.Write("C = "); break;
          case  3: Console.Write("D = "); break;
          case  4: Console.Write("E = "); break;
          case  5: Console.Write("F = "); break;
          case  6: Console.Write("G = "); break;
          case  7: Console.Write("H = "); break;
          case  8: Console.Write("I = "); break;
          case  9: Console.Write("J = "); break;
          case 10: Console.Write("K = "); break;
          case 11: Console.Write("L = "); break;
          case 12: Console.Write("M = "); break;
          case 13: Console.Write("N = "); break;
          case 14: Console.Write("O = "); break;
          case 15: Console.Write("P = "); break;
          default: Console.Write("NS = "); break;     // haven't done yet for #'s > 15
        }

        Console.Write(Common.GetCellTypeChar(itsCellTypes[ruleType]));
        Console.WriteLine();
      }      
    }

    #endregion Debug

  }
}
