using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;

namespace ArtificialLife
{
  class Program
  {
    static void Main( string[] args )
    {
      Console.WriteLine( "Starting Program" );

      //ChromosomeEvolution chromosomeEvolution = new ChromosomeEvolution();
      //chromosomeEvolution.StartEvolution();
      //chromosomeEvolution.EvaluateSpecificChromosome();


      int gridSideLength = 6;
      int populationSize = 2;
      int generations = 1;
      int botType = 2; // rules based chromosome
      Evolution evolution = new Evolution( gridSideLength, populationSize, generations, botType );
      evolution.itsShowOnTerminate = true;
      //evolution.StartEvolution();
      evolution.EvaluateSpecificChromosome();

      Console.ReadLine();
    }
  }
}
