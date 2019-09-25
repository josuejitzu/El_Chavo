// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PowerUp_Focos"
{
	Properties
	{
		_Emission("Emission", 2D) = "white" {}
		_Albedo("Albedo", 2D) = "white" {}
		_EmissionIntensidad("EmissionIntensidad", Range( 0 , 1)) = 0.5
		_EmissionColor("EmissionColor", Color) = (1,1,1,0)
		_VelocidadLuces("VelocidadLuces", Range( 1 , 30)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _EmissionColor;
		uniform float _VelocidadLuces;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float _EmissionIntensidad;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = tex2D( _Albedo, uv_Albedo ).rgb;
			float2 temp_cast_1 = (_VelocidadLuces).xx;
			float2 uv_TexCoord11 = i.uv_texcoord * float2( 10,5 );
			float2 temp_cast_2 = (uv_TexCoord11.x).xx;
			float2 panner9 = ( 1.0 * _Time.y * temp_cast_1 + temp_cast_2);
			float2 temp_output_12_0 = sin( panner9 );
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			o.Emission = ( _EmissionColor * ( float4( temp_output_12_0, 0.0 , 0.0 ) * ( tex2D( _Emission, uv_Emission ) * _EmissionIntensidad * float4( temp_output_12_0, 0.0 , 0.0 ) ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
360;72.66667;1731;874;1906.997;332.4005;1.3;True;False
Node;AmplifyShaderEditor.Vector2Node;22;-1848.499,278.5996;Float;False;Constant;_Vector1;Vector 1;6;0;Create;True;0;0;False;0;10,5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-1651.017,243.9797;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-1490.623,541.0353;Float;False;Property;_VelocidadLuces;VelocidadLuces;4;0;Create;True;0;0;False;0;1;5.6;1;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;9;-1186.217,448.2869;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1398.096,85.28421;Float;False;Property;_EmissionIntensidad;EmissionIntensidad;2;0;Create;True;0;0;False;0;0.5;0.995;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;12;-909.3159,443.4583;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-1439.841,-206.2756;Float;True;Property;_Emission;Emission;0;0;Create;True;0;0;False;0;None;e4b73443a8bdfdc40ba9428b1b503195;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-905.7625,-197.8644;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;6;-493.2484,-61.3887;Float;False;Property;_EmissionColor;EmissionColor;3;0;Create;True;0;0;False;0;1,1,1,0;0.7450981,0.408173,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-461.4926,220.534;Float;True;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;19;-808.9406,846.4993;Float;False;Constant;_Vector0;Vector 0;7;0;Create;True;0;0;False;0;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RotatorNode;15;-595.8295,749.9333;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;17;-998.7424,884.7926;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;18;-1073.664,724.9594;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-189.5868,758.2576;Float;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;False;0;None;e4b73443a8bdfdc40ba9428b1b503195;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-65.6792,-54.20555;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-469.5248,-390.547;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;1cf89f61804a9824d92df95452be9923;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;358.5769,-291.7209;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;PowerUp_Focos;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;22;0
WireConnection;9;0;11;1
WireConnection;9;2;10;0
WireConnection;12;0;9;0
WireConnection;5;0;1;0
WireConnection;5;1;4;0
WireConnection;5;2;12;0
WireConnection;13;0;12;0
WireConnection;13;1;5;0
WireConnection;15;0;18;0
WireConnection;15;1;19;0
WireConnection;15;2;17;0
WireConnection;16;1;15;0
WireConnection;7;0;6;0
WireConnection;7;1;13;0
WireConnection;0;0;2;0
WireConnection;0;2;7;0
ASEEND*/
//CHKSM=508067016E4747ABDCBF03CC285480B1572917F1