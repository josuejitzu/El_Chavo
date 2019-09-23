// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Plantas_d"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_velocidadonda("velocidad onda", Range( 0 , 5)) = 0.5
		_frequencia("frequencia", Range( 0 , 5)) = 1
		_worldIntensity("worldIntensity", Float) = 0.04
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_AlphaIntensity("AlphaIntensity", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _velocidadonda;
		uniform float _frequencia;
		uniform float _worldIntensity;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _AlphaIntensity;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime3 = _Time.y * _velocidadonda;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float4 appendResult24 = (float4(( ( ( cos( _Time.y ) * ( 1.0 - v.texcoord.xy.y ) ) * _worldIntensity ) * sin( ase_worldPos ) ) , 0.0));
			v.vertex.xyz += ( float4( ase_vertexNormal , 0.0 ) * (0.0 + (sin( ( ( ase_worldPos.y + mulTime3 ) * 6.28318548202515 * _frequencia ) ) - 0.0) * (1.0 - 0.0) / (1.0 - 0.0)) * ( float4( ase_vertex3Pos , 0.0 ) * appendResult24 ) ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode13 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = tex2DNode13.rgb;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			o.Occlusion = ( tex2D( _TextureSample0, uv_TextureSample0 ) / _AlphaIntensity ).r;
			o.Alpha = 1;
			clip( tex2DNode13.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
472;72.66667;1079;874;399.4949;717.546;1;True;False
Node;AmplifyShaderEditor.TimeNode;17;-2502.006,722.0623;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-2807.853,403.4693;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-2803.547,-631.1439;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;4;-2699.547,-240.1436;Float;False;Property;_velocidadonda;velocidad onda;2;0;Create;True;0;0;False;0;0.5;0.37;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;16;-1973.141,445.9493;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;18;-2143.626,802.3179;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;2;-2414.547,-584.1438;Float;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;3;-2397.547,-220.1436;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TauNode;7;-1940.265,-251.9569;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-2126.546,-360.1437;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1989.265,-179.9569;Float;False;Property;_frequencia;frequencia;3;0;Create;True;0;0;False;0;1;0.65;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;21;-1321.304,165.7248;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-1570.626,607.3179;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1667.867,329.9658;Float;False;Property;_worldIntensity;worldIntensity;4;0;Create;True;0;0;False;0;0.04;0.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1675.265,-355.957;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-1266.626,469.3179;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;22;-950.3042,189.7248;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-612.3042,457.7248;Float;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SinOpNode;9;-1325.07,-356.593;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;10;-1003.069,-352.593;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-337.3042,483.7248;Float;True;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;25;-334.3042,180.7248;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-39.3042,305.7248;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NormalVertexDataNode;11;-169.2115,-212.2406;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;13;-57.59389,-707.2195;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;dab1dd4717e22fb42879fe3e9fcfd50b;eaa6e44190e0b1543a2d7c4dcf769a45;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;29;-477.6887,-25.05365;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;27;-62.78831,-494.2293;Float;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;False;0;None;68736760ed2d6d3479da2811d886834c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;5.505127,-194.546;Float;True;Property;_AlphaIntensity;AlphaIntensity;6;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;62.11685,-13.60404;Float;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;28;564.8822,-63.50408;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;32;351.5051,-341.546;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;629.6398,-312.5595;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Plantas_d;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;0;14;2
WireConnection;18;0;17;2
WireConnection;2;0;1;2
WireConnection;3;0;4;0
WireConnection;5;0;2;0
WireConnection;5;1;3;0
WireConnection;19;0;18;0
WireConnection;19;1;16;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;6;2;8;0
WireConnection;20;0;19;0
WireConnection;20;1;15;0
WireConnection;22;0;21;0
WireConnection;23;0;20;0
WireConnection;23;1;22;0
WireConnection;9;0;6;0
WireConnection;10;0;9;0
WireConnection;24;0;23;0
WireConnection;26;0;25;0
WireConnection;26;1;24;0
WireConnection;29;0;10;0
WireConnection;12;0;11;0
WireConnection;12;1;29;0
WireConnection;12;2;26;0
WireConnection;28;0;13;4
WireConnection;32;0;27;0
WireConnection;32;1;31;0
WireConnection;0;0;13;0
WireConnection;0;5;32;0
WireConnection;0;10;28;0
WireConnection;0;11;12;0
ASEEND*/
//CHKSM=C780AB592DA4DC38AB723E7540A392CF43C4454A