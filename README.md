# Wait Indicator

Implementation of different wait indicator controls. The main advantage of these wait indicators is they are fully resizable and they are highly configurable (see properties). Different settings create very different looks. Test different combinations of the number of shapes, the height of a shape and the gap betweeen shapes. See below some examples.

### DotsWaitIndicator

![example](/dots.gif)

### SegmentsWaitIndicator

![example](/block.gif)

### SticksWaitIndicator

![example](/sticks.gif)

### Properties

The properties of the wait indicators are:

**Shapes:** Number of shapes

**ShapeHeight:** The hight of each shape along the radius, given in percent of the current radius (not supported by DotsWaitIndicator)

**ShapeGap:** The gap between each shape, given in percent of the length of a segment

**Fill**: The brush to fill each segment

**Duration**: The duration for one round (one circle)
