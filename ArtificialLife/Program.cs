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



      Evolution evolution = new Evolution();
      evolution.StartEvolution();

      //evolution.EvaluateSpecificChromosome();

      Console.ReadLine();
    }
  }
}
