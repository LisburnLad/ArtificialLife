using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using System.Diagnostics;

namespace ArtificialLife
{
  class Program
  {
    static void Main( string[] args )
    {
      Console.WriteLine( "Starting Program" );


      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();
      
      //
      // evolve chromosomes
      //

      //ChromosomeEvolution chromosomeEvolution = new ChromosomeEvolution();
      //chromosomeEvolution.StartEvolution();
      ////chromosomeEvolution.EvaluateSpecificChromosome();

      //
      // evolve bots
      //
      int gridSideLength = 15;
      int populationSize = 3000;
      int generations = 2000;
      int botType = 2; // rules based chromosome
      Evolution evolution = new Evolution( gridSideLength, populationSize, generations, botType );
      evolution.itsShowOnTerminate = true;
      evolution.StartEvolution();
      //evolution.EvaluateSpecificChromosome();
      //evolution.TargetedConnectionWithRightOrLeftPlacedTargets();


      Console.WriteLine("Ending Program");


      stopWatch.Stop();
      // Get the elapsed time as a TimeSpan value.
      TimeSpan ts = stopWatch.Elapsed;

      // Format and display the TimeSpan value.
      string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
      Console.WriteLine("RunTime " + elapsedTime);

      Console.ReadLine();
    }
  }
}
