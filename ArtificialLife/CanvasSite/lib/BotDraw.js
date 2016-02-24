// draw the maze
ArtificialLife.BotDraw = ArtificialLife.MazeUtils.extend(
{
  // bot movement
  x: 0,
  y: 0,
  xIncrement: 0,
  yIncrement: 0,

  // the last time that the animation was updated
  lastTime: 0,

  canvases: null,

  init: function( aCanvases )
  {
    var me = this;

    // set the starting time for bot movement
    me.lastTime = Date.now();

    // keep a reference to the canvases object
    me.canvases = aCanvases;

    // set the initial bot position
    me.x = 1;
    me.y = 1;

    // set the amount the bot moves
    me.xIncrement = me.cellWidth;
    me.yIncrement = me.cellHeight;
  },

  // cross-browser requestAnimationFrame
  // - different browsers specify the requestAnimationFrame in different ways
  // See https://hacks.mozilla.org/2011/08/animating-with-javascript-from-setinterval-to-requestanimationframe/
  requestAnimFrame: ( function ()
  {
    return window.requestAnimationFrame    ||
        window.webkitRequestAnimationFrame ||
        window.mozRequestAnimationFrame    ||
        window.oRequestAnimationFrame      ||
        window.msRequestAnimationFrame     ||
        function ( callback )
        {
          // fallback function when the browser does not support requestAnimationFrame
          window.setTimeout( callback, 1000 / me.updateSpeed );
        };
  })(),

  animate: function () 
  {
    var me = this;

    // pass through the time so that we can get consistent movement xIncrement on all platforms
    var now = Date.now();
    var dt = ( now - me.lastTime ) / 1000.0;

    //me.update( dt );
    //me.drawBot();

    me.updateAndDraw( dt );

    me.lastTime = now;    

    // get the cross-browser requestAnimationFrame
    var requestAnimFrame = me.requestAnimFrame;

    // set this function and scope into requestAnimationFrame
    requestAnimFrame( this.animate.bind( this ) );
  },


  // update the bot position by the predefined amount per second
  update: function ( dt )
  {
    var me = this;

    // increase the horizontal position by the predefined xIncrement (one cell)
    // multiplied by the specified increments per second multiplied by the time since last update
    me.x += me.xIncrement * me.updateSpeed * dt;

    // if the bot has reached the end of a row reverse its direction
    if( me.x <= 0 || me.x >= ( me.mazeWidthCells * me.cellWidth ) )
    {
      me.xIncrement = -me.xIncrement;
      
      me.y += me.yIncrement;

      if( me.y <= 0 || (me.y + me.yIncrement) >= ( me.mazeHeightCells * me.cellHeight ) )
      {
        me.yIncrement = -me.yIncrement;
      }      
    }

    // make sure the bot doesn't go off the left hand side
    if( me.x < 0 )
    {
      me.x = 0;
    }

    // make sure the bot doesn't go off the right hand side
    if( me.x > ( me.mazeWidthCells * me.cellWidth ) )
    {
      me.x = me.mazeWidthCells * me.cellWidth;
    }
  },


  cellsToUpdate: 0,
  wholeCellsUpdated: 0,

  botX: 0,
  botY: 0,

  botPositions: 
  [
    [1,0],
    [2,0],
    [3,0],
    [4,0],
    [5,0],
    [6,0],
    [7,0],
    [8,0],
    [9,0],
    [10,0],
    [11,0],
    [12,0],
    [13,0],
    [14,0],
    [15,0],
    [16,0],
    [17,0],
    [18,0],
    [19,0],
    [20,0],
    [20,1],
    [19,1],
    [18,1],
    [17,1],
    [16,1],
    [15,1],
    [14,1],
    [13,1],
    [12,1],
    [11,1],
    [10,1],
    [9,1],
    [8,1],
    [7,1],
    [6,1],
    [5,1],
    [4,1],
    [3,1],
    [2,1],
    [1,1],
    [0,1],
    [0,1],
    [1,1],
    [2,1],
    [3,1],
    [4,1],
    [5,1],
    [6,1],
    [7,1],
    [8,1],
    [9,1],
    [10,1],
    [11,1],
    [12,1],
    [13,1],
    [14,1],
    [15,1],
    [16,1],
    [17,1],
    [18,1]
  ],
  
  updateAndDraw: function(dt)
  {
    var me = this;

    // increase the horizontal position by the predefined xIncrement (one cell)
    // multiplied by the specified increments per second multiplied by the time since last update
    //me.x += me.xIncrement * me.updateSpeed * dt;

    // calculate the number of cells to move on this call
    // dt = the time since the last call
    // updateSpeed = number of cells to move per second
    // cellsToMove will be a small fraction of a cell (unless the updateSpeed is very high)
    var cellsToMove = me.updateSpeed * dt;


    // increase the floating point count of the number of cells to update
    me.cellsToUpdate += cellsToMove;

    
    // get the integer representation of the number of cells to update
    var wholeCellsToUpdate = Math.floor( me.cellsToUpdate );

    // if the integer value of the cells to update is greater than the last whole number of cells
    // to update then move the bot to its next grid position
    if( wholeCellsToUpdate > me.wholeCellsUpdated )
    {
      me.addText( wholeCellsToUpdate );

      var currentIndex = wholeCellsToUpdate % me.botPositions.length;

      me.drawBotAtPos( me.botPositions[currentIndex][1], me.botPositions[currentIndex][0] );

      //me.botX++;
      //if( me.botX >= me.mazeWidthCells )
      //{
      //  me.botX = 0;
      //  me.botY++;

      //  if( me.botY >= me.mazeHeightCells )
      //  {
      //    me.botY = 0;
      //  }
      //}
    }

    // store the integer number of cells to update
    me.wholeCellsUpdated = wholeCellsToUpdate;
  },

  addText: function(aValue)
  {
    var me = this;
    var context = me.canvases.botContext;

    // clear the bot canvas to remove the last position
    context.clearRect( 0, 0, me.canvases.canvasWidth, me.canvases.canvasHeight );

    context.font = "20px Arial";
    context.textAlign = 'center';
    context.textBaseline = 'bottom';
    context.fillStyle = 'black';
    context.fillText( aValue, 20, 20 );
  },

  // if the bot has moved cells then redraw its position
  drawBot: function ()
  {
    var me = this;

    // get the number of cells the bot has currently moved
    var cellX = Math.floor( me.x / me.cellWidth );
    var cellY = Math.floor( me.y / me.cellHeight );

    // move the cell by one pixel right to avoid the grid line
    var x = me.canvases.leftOffset + ( cellX * me.cellWidth ) + 1;
    var y = me.canvases.topOffset + ( cellY * me.cellHeight ) + 1;

    var context = me.canvases.botContext;

    // clear the bot canvas to remove the last position
    context.clearRect( 0, 0, me.canvases.canvasWidth, me.canvases.canvasHeight );

    context.beginPath();
    context.rect( x, y, me.cellWidth, me.cellHeight-1 );
    context.fillStyle = me.botBodyColor;
    context.fill();
    context.lineWidth = (me.cellWidth/10);
    context.strokeStyle = me.botBorderColor;
    context.stroke();
  },

  // draw the bot at the given grid location
  drawBotAtPos: function ( aX, aY )
  {
    var me = this;

    // move the cell by one pixel right to avoid the grid line
    var x = me.canvases.leftOffset + ( aX * me.cellWidth ) + 1;
    var y = me.canvases.topOffset + ( aY * me.cellHeight ) + 1;

    var context = me.canvases.botContext;

      // clear the bot canvas to remove the last position
    context.clearRect( 0, 0, me.canvases.canvasWidth, me.canvases.canvasHeight );

    context.beginPath();
    context.rect( x, y, me.cellWidth, me.cellHeight-1 );
    context.fillStyle = me.botBodyColor;
    context.fill();
    context.lineWidth = (me.cellWidth/10);
    context.strokeStyle = me.botBorderColor;
    context.stroke();
  }
});