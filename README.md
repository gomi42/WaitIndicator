# Wait Indicator

Here you find different flavors of an resizable wait indicator control. All versions have in common that the code to position the dots along a circle and to calculate the appropriate size of the dots is placed the control's C# code.

While version 1 to 3 are more for demonstration purposes they might serve as a basis for very special implementations. Version 4 to 6, the dot, segment and stick wait indicators, are ready-to-use controls in your production environment.

### Version 1: minimal code behind, most in XAML, ColorAnimation

The strenght of this version is it implements (besides the mentioned placement of the dots) everything in a XAML style. The animations are done via a ColorAnimation.

### Version 2: minimal code behind, most in XAML, opacity animation

This version is very similar to version 1, exept the opacity of the dots is animated (instead of its color).

### Version 3: animation in code, dots in XAML

In case a different number of dots is needed it is a quite elaborate task to calculate the exact timing of the animations (which was done by hand in version 1 and 2). This version keeps the definition of the dots in the XAML style but moves the creation of the animation to the C# part of the control.

### Version 4: Dots

![example](/example.jpg)

This version implements all in C#, no XAML required. Just define the number of dots, the time for one circle and the fill color.

**Dots:** Number of dots

**Fill**: The brush to fill each dot

**Duration**: The duration for one round (one circle)

### Version 5: Segments

![example](/block.png)

This version implements circle segments instead of dots. The control is highly configurable. Just test out what fits your needs, what you like best:

**Segments:** Number of segments

**SegmentHeight:** The hight of each segment along the radius, given in percent of the current radius

**SegmentGap:** The gap between each segment, given in percent of the length of a segment

**Fill**: The brush to fill each segment

**Duration**: The duration for one round (one circle)

### Version 6: Sticks

![example](/sticks.png)

This version implements sticks. The control is highly configurable. Just test out what fits your needs, what you like best:

**Sticks:** Number of sticks

**StickHeight:** The hight of each stick along the radius, given in percent of the current radius

**StickGap:** The gap between each stick, given in percent of the length of a segment

**Fill**: The brush to fill each segment

**Duration**: The duration for one round (one circle)