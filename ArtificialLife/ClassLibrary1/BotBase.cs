using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using System.Threading;
using System.IO;


namespace ArtificialLife
{
  public class BotBase
  {
    /// <summary>
    /// the chromosome representing this bot
    /// </summary>
    Chromosome itsChromosome;

    /// <summary>
    /// the test representing the environment for the bot
    /// </summary>
    Test itsTest;

    /// <summary>
    /// the structure of the grid formed from the chromosome
    /// </summary>
    BotStructure itsStructure;

    /// <summary>
    /// the processor that evaluates the supplied bot grid
    /// </summary>
    BotProcess itsProcessor;

    /// <summary>
    /// The instance of the event broker class used to pass events around for this bot
    /// </summary>
    EventBroker itsEventBroker = new EventBroker();


    /// <summary>
    /// flag to indicate when information should be written 
    /// </summary>
    public bool itsShowGrid { get; set; }

    /// <summary>
    /// When set true each step of propagation will be written to the output if a test file name is specified
    /// </summary>
    public bool ShowAllPropagationSteps
    {
      get
      {
        return itsProcessor.ShowAllPropagationSteps;
      }
      set
      {
        itsProcessor.ShowAllPropagationSteps = value;
      }
    }


    /// <summary>
    /// create the bot's grid from the supplied chromosome
    /// </summary>
    /// <param name="aChromosome"></param>
    public BotBase(Chromosome aChromosome, Test aTest, int aSideLength, bool aShowGrid, Pruning aPruneGrid = Pruning.All)      
    {
      Initialize(aChromosome, aTest, aSideLength, aShowGrid, aPruneGrid, null);
    }

    /// <summary>
    /// create the bot's grid from the specified bot structure file
    /// </summary>
    /// <param name="aBotStructureFile"></param>
    public BotBase(string aBotStructureFile)
    {
      itsStructure = new BotStructure(aBotStructureFile);
    }

    
    /// <summary>
    /// create the bot's grid from the supplied chromosome
    /// - place the supplied target nodes into the grid before growing the cells
    /// </summary>
    /// <param name="aChromosome"></param>
    /// <param name="aTest"></param>
    /// <param name="aSideLength"></param>
    /// <param name="aShowGrid"></param>
    /// <param name="placedTargets"></param>
    public BotBase(Chromosome aChromosome, Test aTest, int aSideLength, bool aShowGrid, Pruning aPruneGrid, CellDefinition[] aPlacedTargets)      
    {
      Initialize( aChromosome, aTest, aSideLength, aShowGrid, aPruneGrid, aPlacedTargets);
    }


    private void Initialize(Chromosome aChromosome, Test aTest, int aSideLength, bool aShowGrid, Pruning aPruneGrid, CellDefinition[] aPlacedTargets)
    {
      // register this bot with the system event broker to allow event publication and subscription
      itsEventBroker.Register(this);
      
      // take a copy of the chromosome
      itsChromosome = aChromosome;

      // create the grid for this chromosome
      if (aPlacedTargets == null)
      {
        CreateGrid(aSideLength, aShowGrid, aPruneGrid);
      }
      else
      {
        CreateGrid(aSideLength, aPlacedTargets, aShowGrid, aPruneGrid);
      }

      // take a copy of the test to perform
      itsTest = aTest;
    }

    /// <summary>
    /// create a bot grid from a random chromosome
    /// </summary>
    /// <param name="aChromosome"></param>
    public BotBase(Test aTest, int aSideLength, bool aShowGrid, Pruning aPruneGrid = Pruning.All)
    {
      int chromosomeLength = ChromosomeDecode.GetChromosomeLength();
      itsChromosome = new Chromosome(chromosomeLength);

      // create the grid for this chromosome
      CreateGrid(aSideLength, aShowGrid, aPruneGrid);

      // take a copy of the test to perform
      itsTest = aTest;
    }

    private void CreateGrid(int aSideLength, bool aShowGrid, Pruning aPruneGrid)
    {
      itsStructure = new BotStructure(aSideLength);
      itsStructure.Generate(itsChromosome, aShowGrid, aPruneGrid);
    }


    private void CreateGrid(int aSideLength, CellDefinition[] aPlacedTargets, bool aShowGrid, Pruning aPruneGrid)
    {
      itsStructure = new BotStructure(aSideLength);
      itsStructure.Generate(itsChromosome, aPlacedTargets, aShowGrid, aPruneGrid);
    }

    /// <summary>
    /// get the grid structure
    /// </summary>
    /// <returns></returns>
    public CellType[,] GetGrid()
    {
      if (itsStructure != null)
      {
        return itsStructure.itsGrid;
      }
      return null;
    }

    /// <summary>
    /// Handle the event fired after each propagation within the bot
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="aArgs"></param>
    [EventSubscription(ArtificialLifeProperties.PropagationEndEvent, typeof(OnPublisher))]
    public void OnPropagationEndEvent(object sender, PropagationEventArgs aArgs)
    {
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);

      if (itsTest != null)
      {
        itsTest.UpdateBotPosition(aNorthOutput, aEastOutput, aSouthOutput, aWestOutput);

        // swap the outputs, so that a west input is more likely to trigger the west output and cause a move in the opposite direction
        //itsTest.UpdateBotPosition( aSouthOutput ,aWestOutput , aNorthOutput, aEastOutput );

        itsTest.EvaluateOutput(itsProcessor.CellState, 15, aArgs.StepNumber, false);
      }
    }

    public void CreateProcessor(string aTestDirectory)
    {
      // initialize the output
      itsProcessor = new BotProcess(itsStructure,itsEventBroker);

      if (string.IsNullOrEmpty(aTestDirectory) == false)
      {
        itsProcessor.WriteOutputToFile(aTestDirectory + "\\InitialOutput.txt");
      }
    }

    public void Evaluate(bool aNorthInput, bool aEastInput, bool aSouthInput, bool aWestInput, string aTestDirectory, string aFileName)
    {
      Evaluate(aNorthInput, aEastInput, aSouthInput, aWestInput, aTestDirectory, aFileName, false);
    }


    public void Evaluate(bool aNorthInput, bool aEastInput, bool aSouthInput, bool aWestInput, string aTestDirectory, string aFileName, bool aResetPropagationStep )
    {
      itsProcessor.Evaluate(aNorthInput, aEastInput, aSouthInput, aWestInput, aTestDirectory + "\\" + aFileName, aResetPropagationStep);

      if (string.IsNullOrEmpty(aTestDirectory) == false)
      {
        itsProcessor.WriteOutputToFile(aTestDirectory + "\\" + aFileName);
      }
    }

    
    public void GetOutputs(out bool aNorthOutput, out bool aEastOutput, out bool aSouthOutput, out bool aWestOutput)
    {
      itsProcessor.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
    }

    public double Evaluate(bool aShowGrid)
    {
      return Evaluate(aShowGrid, string.Empty, 0);
    }

    public double Evaluate(bool aShowGrid, int aDelay )
    {
      return Evaluate(aShowGrid, string.Empty, aDelay);
    }

    /// <summary>
    /// generate and evaluate a grid using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    public double Evaluate(bool aShowGrid, string aTestDirectory, int aDelay )    
    {
      try
      {
        // initialize the output
        itsProcessor = new BotProcess(itsStructure, itsEventBroker);

        if (string.IsNullOrEmpty(aTestDirectory) == false)
        {
          itsProcessor.WriteOutputToFile(aTestDirectory + "\\InitialOutput.txt");
        }

        //itsProcessor.Evaluate(false, false, false, false, string.Empty, false);

        //int kNumberOfPasses = 20; - standard tests
        int kNumberOfPasses = 400; // eat cheese
        double result = TestBot(aShowGrid, aTestDirectory, kNumberOfPasses, aDelay);

        if (string.IsNullOrEmpty(aTestDirectory) == false)
        {
          itsProcessor.WriteOutputToFile(aTestDirectory + "\\FirstOutput.txt");

          itsTest.WriteTestOutput(aTestDirectory, "botmove.txt");
        }
      }
      catch
      {
        Console.WriteLine("Chromosome: " + itsChromosome.ToBinaryString());
      }

      return itsTest.GetFinalScore(aShowGrid);
    }

    public event TestBotHandler BotEvent;

    private void FireEvent(int aRow, int aCol, int aStep)
    {
      BotEvent botEvent = new BotEvent();
      botEvent.Step = aStep;
      botEvent.Row = aRow;
      botEvent.Col = aCol;

      if (BotEvent != null)
      {
        BotEvent(this, botEvent);
      }

      botEvent = null;
    } 


    public double TestBot(bool aShowGrid, string aTestDirectory, int aNumberOfPasses, int aDelay)
    {
      // initialize the output
      itsProcessor = new BotProcess(itsStructure, itsEventBroker);

      if (string.IsNullOrEmpty(aTestDirectory) == false)
      {
        itsProcessor.WriteOutputToFile(aTestDirectory + "\\InitialOutput.txt");
      }


      bool northSideTouch = false;
      bool eastSideTouch = false;
      bool southSideTouch = false;
      bool westSideTouch = false;

      itsTest.SetNumberOfPasses(aNumberOfPasses);


      FireEvent(((MazeTest)itsTest).itsRow, ((MazeTest)itsTest).itsCol, 0);

      for (int pass = 0; pass < itsTest.GetNumberOfPasses(); pass++)
      {
        itsTest.GetBotPosition(ref northSideTouch, ref  eastSideTouch, ref  southSideTouch, ref westSideTouch);

        itsProcessor.SetGridInputCells(northSideTouch, eastSideTouch, southSideTouch, westSideTouch);

        itsProcessor.PropagateCells(aTestDirectory + "\\OutputStep_", pass + 1);

        itsProcessor.PropagationEnd();

        if (string.IsNullOrEmpty(aTestDirectory) == false)
        {
          itsProcessor.WriteOutputToFile(aTestDirectory + "\\OutputStep_" + pass.ToString() + ".txt");

          if (aShowGrid)
          {
            string filename = aTestDirectory + "\\Positions.txt";
            using (TextWriter writer = new StreamWriter(filename, (pass > 0)))
            {
              writer.WriteLine("{0} : {1},{2}", pass, ((MazeTest)itsTest).itsRow, ((MazeTest)itsTest).itsCol);
            }
          }
        }

        FireEvent(((MazeTest)itsTest).itsRow, ((MazeTest)itsTest).itsCol, pass+1);

        if (aDelay > 0)
        {
          Thread.Sleep(aDelay);
        }
      }

      if (aShowGrid)
      {
        itsTest.ShowTestOutput();

        SaveGrid(aTestDirectory + "\\grid.bmp");
        //ShowGrid("c:\\grid.bmp");
      }

      double result = itsTest.GetFinalScore(aShowGrid);

      return result;
    }

    /// <summary>
    /// Save the current grid as an image
    /// </summary>
    /// <param name="aImageName"></param>
    public void SaveGrid(string aImageName)
    {
      if( itsStructure != null )
      {
        // save an image of the grid
        itsStructure.SaveGrid(aImageName);
      }
    }

    public void SaveGridToTextFile(string aFileName)
    {
      if (itsStructure != null)
      {
        // save a text representation of the grid
        itsStructure.WriteGridToFile(aFileName);
      }
    }
  }
}
