#version 100

attribute vec2 aPos;
attribute vec4 aColor;
attribute vec2 aTexCoord;

uniform int aPointSize;
uniform vec3 aViewport;

varying vec2 texCoord; // output a color to the fragment shader
varying vec4 color; // output a color to the fragment shader

void main(void)
{
    gl_Position = vec4(aPos.x / aViewport.x * 2.0 - 1.0, 1.0 - aPos.y / aViewport.y * 2.0, 0.0, 1.0);
	texCoord = aTexCoord;
	color = aColor;
	gl_PointSize  = float(aPointSize);
}