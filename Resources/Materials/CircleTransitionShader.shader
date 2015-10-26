Shader "Custom/CircleTransitionShader"
{
	Properties {
      _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
      _Color2 ("Color 2", Color) = (0.0, 0.0, 0.0, 0.0)
      _X ("X", Float) = 0.0
      _Y ("Y", Float) = 0.0
      _Width ("Width", Float) = 128
      _Height ("Height", Float) = 128
      
      _CenterX ("CenterX", Float) = 1
      _CenterY ("CenterY", Float) = 0.5
      _Size ("Size", Range(0, 100)) = 50
   }
   SubShader {
      Tags { "Queue" = "Overlay" } // render after everything else
 
      Pass {
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
         ZTest Always // deactivate depth test
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 

         #include "UnityCG.cginc" 
           // defines float4 _ScreenParams with x = width;  
           // y = height; z = 1 + 1.0/width; w = 1 + 1.0/height
           // and defines float4 _ProjectionParams 
           // with x = 1 or x = -1 for flipped projection matrix;
           // y = near clipping plane; z = far clipping plane; and
           // w = 1 / far clipping plane
 
         // User-specified uniforms
         uniform float4 _Color;
         uniform float4 _Color2;
         uniform float _X;
         uniform float _Y;
         uniform float _Width;
         uniform float _Height;
         uniform float _CenterX;
         uniform float _CenterY;
         uniform float _Size;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float2 rasterPosition = float2(
               _X + _ScreenParams.x / 2.0 
               + _Width * (input.vertex.x + 0.5),
               _Y + _ScreenParams.y / 2.0 
               + _Height * (input.vertex.y + 0.5));
            output.pos = float4(
               2.0 * rasterPosition.x / _ScreenParams.x - 1.0,
               _ProjectionParams.x * (2.0 * rasterPosition.y / _ScreenParams.y - 1.0),
               _ProjectionParams.y, // near plane is at -1.0 or at 0.0
               1.0);
 
            output.tex = float4(input.vertex.x + 0.5, 
               input.vertex.y + 0.5, 0.0, 0.0);
               // for a cube, vertex.x and vertex.y 
               // are -0.5 or 0.5
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	float d = distance(float2(_CenterX, _CenterY), input.tex.xy);
         	
         	if (d < _Size)
            	return float4(0, 0, 0, 0);
            else if (d < _Size + 0.6)
            	return _Color2;
            else
            	return _Color;
         }
 
         ENDCG
      }
   }
}