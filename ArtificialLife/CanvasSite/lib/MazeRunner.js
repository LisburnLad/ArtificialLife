
ArtificialLife.MazeRunner = ArtificialLife.MazeUtils.extend(
{
  // extension function to allow inheritance
  extend: function ( props )
  {
    var prop, obj;
    obj = Object.create( this );
    for( prop in props )
    {
      if( props.hasOwnProperty( prop ) )
      {
        obj[prop] = props[prop];
      }
    }
    return obj;
  },

  init: function ( aCanvasContainerId )
  {
    var me = this;

    // create 2 canvases
    // - one for the maze
    // - one for the bot
    var canvases = ArtificialLife.CanvasCreate.extend();
    canvases.addCanvasesToDom( aCanvasContainerId );

    // draw the maze onto the bottom canvas
    var maze = ArtificialLife.MazeDraw.extend();
    maze.drawGrid( canvases );

    // animate the bot to move on the top canvas
    var bot = ArtificialLife.BotDraw.extend();
    bot.init( canvases );
    bot.animate();
  }
});