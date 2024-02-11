#version 100
precision mediump float;

varying vec2 texCoord;
varying vec4 color;

uniform sampler2D aTexture;
uniform int aMode;

void main()
{
	// float alpha = texture2D(aTexture, texCoord).w * color.w;
	// gl_FragColor = vec4(color.xyz, alpha <=0.3 ? 0.2 : alpha);

	if (aMode == 0) {
		gl_FragColor = color;
	}
	else if (aMode == 1) {
		gl_FragColor = texture2D(aTexture, texCoord);
	}

}