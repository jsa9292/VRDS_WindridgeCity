// Upgrade NOTE: upgraded instancing buffer 'CarBackLights' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CarBackLights"
{
	Properties
	{
		_Cars_18_Taillights_D("Cars_1-8_Taillights_D", 2D) = "black" {}
		_Intensity0("Intensity0", Float) = 0
		_Color0("Color 0", Color) = (0.990566,0.6716096,0.2009167,0)
		_Intensity1("Intensity1", Float) = 0
		_Color1("Color 1", Color) = (1,0,0,0)
		_Bias("Bias", Float) = 0
		_GBbalance("GBbalance", Float) = 0
		_GBmag("GBmag", Float) = 0
		[PerRendererData]_LR("LR", Range( -1 , 1)) = 1
		[PerRendererData]_BrakeOnOff("BrakeOnOff", Range( 0 , 1)) = 1
		[Toggle]_BothOn("BothOn", Float) = 1
		[Toggle]_Flicker0("Flicker0", Float) = 0
		[Toggle]_Flicker1("Flicker1", Float) = 1
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
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _Cars_18_Taillights_D;
		uniform float4 _Color0;
		uniform float _GBbalance;
		uniform float _GBmag;
		uniform float _Bias;
		uniform float _Flicker0;
		uniform float _BothOn;
		uniform float _Flicker1;
		uniform float4 _Color1;

		UNITY_INSTANCING_BUFFER_START(CarBackLights)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Cars_18_Taillights_D_ST)
#define _Cars_18_Taillights_D_ST_arr CarBackLights
			UNITY_DEFINE_INSTANCED_PROP(float, _Intensity0)
#define _Intensity0_arr CarBackLights
			UNITY_DEFINE_INSTANCED_PROP(float, _BrakeOnOff)
#define _BrakeOnOff_arr CarBackLights
			UNITY_DEFINE_INSTANCED_PROP(float, _LR)
#define _LR_arr CarBackLights
			UNITY_DEFINE_INSTANCED_PROP(float, _Intensity1)
#define _Intensity1_arr CarBackLights
		UNITY_INSTANCING_BUFFER_END(CarBackLights)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _Cars_18_Taillights_D_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Cars_18_Taillights_D_ST_arr, _Cars_18_Taillights_D_ST);
			float2 uv_Cars_18_Taillights_D = i.uv_texcoord * _Cars_18_Taillights_D_ST_Instance.xy + _Cars_18_Taillights_D_ST_Instance.zw;
			float4 tex2DNode1 = tex2D( _Cars_18_Taillights_D, uv_Cars_18_Taillights_D );
			o.Albedo = ( tex2DNode1 * float4( 0.2169811,0.2169811,0.2169811,0 ) ).rgb;
			float temp_output_63_0 = saturate( ( 1.0 - ( ( distance( float3( 1,1,1 ) , tex2DNode1.rgb ) - 1.65 ) / max( 0.03 , 1E-05 ) ) ) );
			float4 temp_output_64_0 = ( tex2DNode1 * temp_output_63_0 );
			float4 break90 = temp_output_64_0;
			float lerpResult115 = lerp( break90.g , break90.b , _GBbalance);
			float temp_output_88_0 = ( ( ( break90.r - ( lerpResult115 * _GBmag ) ) + log( _Bias ) ) * 1.0 );
			float _Intensity0_Instance = UNITY_ACCESS_INSTANCED_PROP(_Intensity0_arr, _Intensity0);
			float _BrakeOnOff_Instance = UNITY_ACCESS_INSTANCED_PROP(_BrakeOnOff_arr, _BrakeOnOff);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float _LR_Instance = UNITY_ACCESS_INSTANCED_PROP(_LR_arr, _LR);
			float clampResult128 = clamp( ( ase_vertex3Pos.x * _LR_Instance ) , 0.0 , 1.0 );
			float mulTime131 = _Time.y * 8.0;
			float temp_output_139_0 = ( (( _BothOn )?( abs( ase_vertex3Pos.x ) ):( clampResult128 )) * ( sin( mulTime131 ) + 1.0 ) );
			float normalizeResult108 = normalize( ( -temp_output_88_0 * temp_output_63_0 ) );
			float grayscale107 = Luminance(temp_output_64_0.rgb);
			float _Intensity1_Instance = UNITY_ACCESS_INSTANCED_PROP(_Intensity1_arr, _Intensity1);
			o.Emission = max( ( _Color0 * temp_output_88_0 * _Intensity0_Instance * (( _Flicker0 )?( temp_output_139_0 ):( _BrakeOnOff_Instance )) ) , ( (( _Flicker1 )?( temp_output_139_0 ):( _BrakeOnOff_Instance )) * normalizeResult108 * _Color1 * grayscale107 * _Intensity1_Instance ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18301
0;73;1001;612;1417.458;222.7247;2.594467;False;False
Node;AmplifyShaderEditor.SamplerNode;1;-897.8279,-660.979;Inherit;True;Property;_Cars_18_Taillights_D;Cars_1-8_Taillights_D;0;0;Create;True;0;0;False;0;False;-1;0dbe9cc493fd63a4e96e37ff6acc1e1d;0dbe9cc493fd63a4e96e37ff6acc1e1d;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;63;-782.0715,306.5969;Inherit;True;Color Mask;-1;;7;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;1,1,1;False;4;FLOAT;1.65;False;5;FLOAT;0.03;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-540.7391,-0.952764;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-436.9773,-242.0911;Inherit;False;Property;_GBbalance;GBbalance;6;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;90;-387.9381,-439.2589;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;115;-290.2888,-260.4272;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-206.7576,-91.32797;Inherit;False;Property;_GBmag;GBmag;7;0;Create;True;0;0;False;0;False;0;6.38;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-139.5252,-191.1575;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;46;-576.2023,694.6343;Inherit;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;104;-209.6819,77.58384;Inherit;False;Property;_Bias;Bias;5;0;Create;True;0;0;False;0;False;0;0.93;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-601.3336,569.8655;Inherit;False;InstancedProperty;_LR;LR;8;1;[PerRendererData];Create;True;0;0;False;0;False;1;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;112;-100.3321,-360.355;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LogOpNode;105;-76.62915,-10.06726;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;131;-49.43457,1071.616;Inherit;False;1;0;FLOAT;8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;-280.0706,562.902;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;128;-101.3855,571.0239;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;130;-19.53444,963.7162;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;134;-178.8648,717.0287;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;88;64.60551,-201.5445;Inherit;True;ConstantBiasScale;-1;;9;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;FLOAT;0;False;1;FLOAT;-0.04;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;133;138.7566,560.2335;Inherit;False;Property;_BothOn;BothOn;10;0;Create;True;0;0;False;0;False;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;125;-10.55332,828.482;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;87;311.8208,-23.58956;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;344.0995,220.2702;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;132;249.8433,454.0633;Inherit;False;InstancedProperty;_BrakeOnOff;BrakeOnOff;9;1;[PerRendererData];Create;True;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;391.3834,557.5634;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;140;574.2014,117.2094;Inherit;False;Property;_Flicker0;Flicker0;11;0;Create;True;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;557.8404,793.1416;Inherit;False;InstancedProperty;_Intensity1;Intensity1;3;0;Create;True;0;0;False;0;False;0;7.88;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;330.8698,-127.402;Inherit;False;InstancedProperty;_Intensity0;Intensity0;1;0;Create;True;0;0;False;0;False;0;1.21;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;141;582.7189,238.0594;Inherit;False;Property;_Flicker1;Flicker1;12;0;Create;True;0;0;False;0;False;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;107;-27.49463,332.0892;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;489.7198,-401.646;Inherit;False;Property;_Color0;Color 0;2;0;Create;True;0;0;False;0;False;0.990566,0.6716096,0.2009167,0;0.9921568,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;36;551.2695,896.0934;Inherit;False;Property;_Color1;Color 1;4;0;Create;True;0;0;False;0;False;1,0,0,0;1,0.5639591,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;108;331.5144,356.5672;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;818.8575,453.8625;Inherit;True;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;735.3364,-224.8733;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;232.0913,-753.3414;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.2169811,0.2169811,0.2169811,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;86;905.7911,146.7814;Inherit;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;60;1167.851,-85.94669;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;CarBackLights;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;63;1;1;0
WireConnection;64;0;1;0
WireConnection;64;1;63;0
WireConnection;90;0;64;0
WireConnection;115;0;90;1
WireConnection;115;1;90;2
WireConnection;115;2;116;0
WireConnection;117;0;115;0
WireConnection;117;1;118;0
WireConnection;112;0;90;0
WireConnection;112;1;117;0
WireConnection;105;0;104;0
WireConnection;123;0;46;1
WireConnection;123;1;124;0
WireConnection;128;0;123;0
WireConnection;130;0;131;0
WireConnection;134;0;46;1
WireConnection;88;3;112;0
WireConnection;88;1;105;0
WireConnection;133;0;128;0
WireConnection;133;1;134;0
WireConnection;125;0;130;0
WireConnection;87;0;88;0
WireConnection;114;0;87;0
WireConnection;114;1;63;0
WireConnection;139;0;133;0
WireConnection;139;1;125;0
WireConnection;140;0;132;0
WireConnection;140;1;139;0
WireConnection;141;0;132;0
WireConnection;141;1;139;0
WireConnection;107;0;64;0
WireConnection;108;0;114;0
WireConnection;18;0;141;0
WireConnection;18;1;108;0
WireConnection;18;2;36;0
WireConnection;18;3;107;0
WireConnection;18;4;32;0
WireConnection;12;0;10;0
WireConnection;12;1;88;0
WireConnection;12;2;31;0
WireConnection;12;3;140;0
WireConnection;126;0;1;0
WireConnection;86;0;12;0
WireConnection;86;1;18;0
WireConnection;60;0;126;0
WireConnection;60;2;86;0
ASEEND*/
//CHKSM=60BC5D1ED04D5B6417650C7FC4FF20B70035DF85