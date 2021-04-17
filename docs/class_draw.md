---
layout: default
title: Draw
---
# Draw
[Back](index.html)
## **Void** loadFont(fontPath)
*Load a font for the software renderer. All future Draw.text calls will use the specified font.*
- String **fontPath** - Path to the font texture. Fonts are 2-color images where pixels with a red value above zero are considered filled.

## **Void** clear()
*Clear the 2D canvas.*

## **Void** setFog(color, fogStart, fogDistance)
*Sets fog properties.*
- Color32 { r, g, b, a } **color** - Fog color
- Double **fogStart** - Distance at which the fog starts
- Double **fogDistance** - Distance after fog start when the fog will be 100% dense

## **Void** enableFog()
*Enable 3D fog. See also: setFog, disableFog*

## **Void** disableFog()
*Disable 3D fog. See also: setFog, enableFog*

## **Void** setClearColor(color, fogStart, fogDistance)
- ObjectInstance **color**
- Double **fogStart**
- Double **fogDistance**

## **Void** offset(x, y)
*Set a global offset for 2D rendering.*
- Int32 **x** - Pixels in the x axis to offset by
- Int32 **y** - Pixels in the y axis to offset by

## **Void** strokeRect(x, y, width, height, color)
*Draw a rectangle outline. See also: fillRect*
- Int32 **x** - x position of the rectangle
- Int32 **y** - y position of the rectangle
- Int32 **width** - width of the rectangle
- Int32 **height** - height of the rectangle
- Color32 { r, g, b, a } **color** - Rectangle color

## **Void** fillRect(x, y, width, height, color)
*Draw a filled rectangle. See also: strokeRect*
- Int32 **x** - x position of the rectangle
- Int32 **y** - y position of the rectangle
- Int32 **width** - width of the rectangle
- Int32 **height** - height of the rectangle
- Color32 { r, g, b, a } **color** - Rectangle color

## **Void** line(x1, y1, x2, y2, color)
*Draw a 2d line.*
- Int32 **x1** - starting x position
- Int32 **y1** - starting y position
- Int32 **x2** - ending x position
- Int32 **y2** - ending y position
- Color32 { r, g, b, a } **color** - line color

## **Void** text(x, y, text, color)
*Draw a line of text.*
- Int32 **x** - x position of the text
- Int32 **y** - x position of the text
- String **text** - the text content to draw
- Color32 { r, g, b, a } **color** - text color

## **Void** model(position, rotation, modelPath, texturePath)
- ObjectInstance **position**
- ObjectInstance **rotation**
- String **modelPath**
- String **texturePath**

## **Void** texture(x, y, texturePath)
- Int32 **x**
- Int32 **y**
- String **texturePath**

## **Void** textureTransformed(x, y, transformation, texturePath)
- Int32 **x**
- Int32 **y**
- ObjectInstance **transformation**
- String **texturePath**

## **Void** texturePart(x, y, rectangle, texturePath)
- Int32 **x**
- Int32 **y**
- ObjectInstance **rectangle**
- String **texturePath**

## **Void** texturePartTransformed(x, y, rectangle, transformation, texturePath)
- Int32 **x**
- Int32 **y**
- ObjectInstance **rectangle**
- ObjectInstance **transformation**
- String **texturePath**

## **String** toLocaleString()

## **ObjectInstance** valueOf()

