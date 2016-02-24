// draw the maze
ArtificialLife.MazeDraw = ArtificialLife.MazeUtils.extend(
{
  mazeContext: null,

  drawGrid: function ( aCanvases )
  {
    var me = this;

    me.mazeContext = aCanvases.mazeContext;

    // get half the width of the border as lines are drawn around the mid-point
    var borderMiddle = ( me.borderWidth / 2 );

    var offX = aCanvases.leftOffset + borderMiddle;
    var offY = aCanvases.topOffset + borderMiddle;
    var width = aCanvases.canvasWidth - offX - me.borderWidth;
    var height = aCanvases.canvasHeight - offY - me.borderWidth;

    // draw the outer rectangle
    me.mazeContext.beginPath();
    me.mazeContext.rect( offX, offY, width, height );
    me.mazeContext.fillStyle = me.mazeBodyColor;
    me.mazeContext.fill();
    me.mazeContext.lineWidth = me.borderWidth;
    me.mazeContext.strokeStyle = me.mazeBorderColor;
    me.mazeContext.stroke();

    // draw the grid lines
    me.mazeContext.lineWidth = 1;
    var dashSize = me.cellWidth / 10;
    if( dashSize < 1 )
    {
      dashSize = 1;
    }
    me.mazeContext.setLineDash( [dashSize, dashSize] );

    // draw the vertical lines
    for( var x = 0; x < me.mazeWidthCells; x++ )
    {
      var xPos = ( x * me.cellWidth ) + offX;

      if( x > 0 )
      {
        me.mazeContext.beginPath();
        me.mazeContext.moveTo( xPos, offY );
        me.mazeContext.lineTo( xPos, offY + height );
        me.mazeContext.stroke();
      }

      // add numbers to the cells if turned on
      me.addHorizontalCellNumbers( xPos + ( me.cellWidth / 2 ), x );
    }

    // draw the horizontal lines
    for( var y = 0; y < me.mazeHeightCells; y++ )
    {
      var yPos = ( y * me.cellHeight ) + offY;

      if( y > 0 )
      {
        me.mazeContext.beginPath();
        me.mazeContext.moveTo( offX, yPos );
        me.mazeContext.lineTo( offX + width, yPos );
        me.mazeContext.stroke();
      }

      // add numbers to the cells if turned on
      me.addVerticalCellNumbers( yPos + ( me.cellHeight / 2 ), y );
    }  
  },

  // add numbers across the top if turned on
  addHorizontalCellNumbers: function( aPos, aValue )
  {
    var me = this;
    if( me.addCellNumbers )
    {
      var height = me.cellWidth * 0.6;
      me.mazeContext.font = height + "px Arial";
      me.mazeContext.textAlign = 'center';
      me.mazeContext.textBaseline = 'bottom';
      me.mazeContext.fillStyle = me.mazeTextColor;
      me.mazeContext.fillText( aValue, aPos, me.marginWidth * 0.9 );
    }
  },

  // add numbers down the sides if turned on
  addVerticalCellNumbers: function( aPos, aValue )
  {
    var me = this;
    if( me.addCellNumbers )
    {
      var height = me.cellHeight * 0.6;
      me.mazeContext.font = height + "px Arial";
      me.mazeContext.textAlign = 'right';
      me.mazeContext.textBaseline = 'middle';
      me.mazeContext.fillStyle = me.mazeTextColor;
      me.mazeContext.fillText( aValue, me.marginWidth * 0.9, aPos );
    }
  }
});