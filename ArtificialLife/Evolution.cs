using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using GAF.Operators;

namespace ArtificialLife
{
  class Evolution
  {
    const int kPopulationSize = 1000;
    const int kMaxEvaluationCount = 500;
    
    public void StartEvolution()
    {
      const double crossoverProbability = 0.65;
      const double mutationProbability = 0.08;
      const int elitismPercentage = 5;

      //create a Population of 1000 random chromosomes of length 50
      var population = new Population( kPopulationSize, Bot.GetChromosomeLength(), false, false );

      //create the genetic operators 
      var elite = new Elite( elitismPercentage );

      var crossover = new Crossover( crossoverProbability, true )
      {
        CrossoverType = CrossoverType.SinglePoint
      };

      var mutation = new BinaryMutate( mutationProbability, true );

      //create the GA itself 
      var ga = new GeneticAlgorithm( population, EvaluateChromosome );

      //subscribe to the GAs Generation Complete event 
      ga.OnGenerationComplete += ga_OnGenerationComplete;

      //add the operators to the ga process pipeline 
      ga.Operators.Add( elite );
      ga.Operators.Add( crossover );
      ga.Operators.Add( mutation );

      //run the GA 
      ga.Run( TerminateFunction );
    }


    private bool TerminateFunction( Population population, int currentGeneration, long currentEvaluation )
    {
      //example termination criterion 
      return (currentEvaluation >= kMaxEvaluationCount);
    }

    private void ga_OnGenerationComplete( object sender, GaEventArgs e )
    {
      //get the best solution 
      var chromosome = e.Population.GetTop( 1 )[0];

      //decode chromosome
      double fitness = CreateAndEvaluateBot( chromosome, true );

      Console.WriteLine( "Final Chromosome Fitmess: " + fitness );
      Console.WriteLine( "Evaluations: " + e.Evaluations );
    }

    /// <summary>
    /// Create the bot and evaluate its performance without any output
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double EvaluateChromosome( Chromosome chromosome )
    {
      return CreateAndEvaluateBot( chromosome, false );
    }


    /// <summary>
    /// function used for testing of a specific chromosome
    /// </summary>
    public void EvaluateSpecificChromosome()
    {
      string testChromosome = "0000000100110110100001111010010101100000100111001111001110011000111000001100110000110011111001110110";

      Chromosome chromosome = new Chromosome( testChromosome );
      CreateAndEvaluateBot(  chromosome, true );
    }

    /// <summary>
    /// generate and evaluate a bot using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double CreateAndEvaluateBot( Chromosome chromosome, bool aShowGrid )
    {
      // create the test that the bot is to perform
      //Test currentTest = new TestStraightLineMove();
      //Test currentTest = new TestDiagonalMove();
      //Test currentTest = new TestForMoveAndStop();
      Test currentTest = new TestForFastMoveAndStop();

      // create the bot
      Bot bot = new Bot( chromosome, currentTest, aShowGrid );

      // see how this bot gets on with the supplied test
      return bot.Evaluate( aShowGrid );
    }
  }
}
