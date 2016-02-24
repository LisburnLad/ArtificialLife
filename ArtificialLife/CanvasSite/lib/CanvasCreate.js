
// class to create the canvases required for maze and bot drawing
ArtificialLife.CanvasCreate = ArtificialLife.MazeUtils.extend(
{
  // canvas size
  canvasWidth: 0,
  canvasHeight: 0,

  // the width of the margin at the top and left of the grid
  marginWidth: 30,
  topOffset: 0,
  leftOffset: 0,

  canvasContainer: null,
  mazeContext: null,
  botContext: null,

  // create 2 canvases
  // - one for the maze
  // - one for the bot
  addCanvasesToDom: function ( aCanvasContainerId )
  {
    var me = this;

    // get the element used to hold the canvases
    me.canvasContainer = document.getElementById( aCanvasContainerId );

    // calculate how big the canvas will be
    me.calculateCanvasSize();

    // create the 2 canvases and get their canvas contexts
    me.mazeContext = me.createCanvas( "maze" );
    me.botContext = me.createCanvas( "bot" );
  },

  // calculate how big the canvas will be
  // - this depends on the size of a grid cell, the number of cells and any offset
  calculateCanvasSize: function ()
  {
    var me = this;

    me.canvasWidth = ( me.mazeWidthCells * me.cellWidth ) + ( me.borderWidth * 2 );
    me.canvasHeight = ( me.mazeHeightCells * me.cellHeight ) + ( me.borderWidth * 2 );

    // if grid numbers are to be applied add a margin to the top and left
    if( me.addCellNumbers )
    {
      me.leftOffset = me.marginWidth;
      me.topOffset = me.marginWidth;

      me.canvasWidth += me.leftOffset;
      me.canvasHeight += me.topOffset;
    }
    else
    {
      me.leftOffset = 0;
      me.topOffset = 0;
    }
  },

  createCanvas: function ( aCanvasClass )
  {
    var me = this;

    // Create the canvas
    var canvas = document.createElement( "canvas" );

    canvas.width = me.canvasWidth;
    canvas.height = me.canvasHeight;
    canvas.classList.add( aCanvasClass );

    // add the canvas to the container element
    me.canvasContainer.appendChild( canvas );

    return canvas.getContext( "2d" );
  }
});