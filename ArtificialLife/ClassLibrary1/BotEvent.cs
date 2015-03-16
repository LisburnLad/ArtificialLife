using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public delegate void TestBotHandler( object sender, BotEvent e );

  public class BotEvent : EventArgs
  {
    public int Row { get; set; }
    public int Col { get; set; }
  }
}
