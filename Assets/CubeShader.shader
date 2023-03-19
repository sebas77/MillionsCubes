Shader "Unlit/InstancedCube"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
 
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                uint instanceID: SV_InstanceID;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            StructuredBuffer<float3> centers;
             
            v2f vert (appdata v)
            {
                v2f o;

                // Get the cube's center position from the structured buffer
                 o.vertex = UnityObjectToClipPos(v.vertex.xyz + centers[v.instanceID]);
                o.color.rgb = centers[v.instanceID].xyz;

                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDHLSL
        }
    }
}