Shader "Custom/Hand Transparent"
{
  Properties {
    [KeywordEnum(None, SinglePassInstanced)] _InstancingMode("Instancing Mode", Float) = 0
    _InnerColor ("Inner Color", Color) = (1.0, 1.0, 1.0, 1.0)
    _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
    _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
  }
  SubShader {
    Tags { "Queue" = "Transparent" "RenderType"="Transparent" }

    Cull Back
    Blend One One

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      // Add this pragma to enable instancing support.
      #pragma multi_compile_instancing

      #include "UnityCG.cginc"
      // Include the instancing header file.
      #include "UnityInstancing.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        UNITY_VERTEX_INPUT_INSTANCE_ID // Add instancing ID to vertex input.
      };

      struct v2f {
        float4 pos : SV_POSITION;
        float3 viewDir : TEXCOORD0;
        float3 normal : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO // Add this for stereo rendering (VR).
        UNITY_VERTEX_INPUT_INSTANCE_ID // Add instancing ID to vertex output.
      };

      // Define the instanced properties in a buffer.
      UNITY_INSTANCING_BUFFER_START(PerInstance)
          UNITY_DEFINE_INSTANCED_PROP(float4, _InnerColor)
          UNITY_DEFINE_INSTANCED_PROP(float4, _RimColor)
          UNITY_DEFINE_INSTANCED_PROP(float, _RimPower)
      UNITY_INSTANCING_BUFFER_END(PerInstance)

      v2f vert (appdata v) {
        v2f o;
        // Setup instance ID in vertex shader.
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Transfer instance ID to the fragment shader.
        UNITY_TRANSFER_INSTANCE_ID(o, v);

        o.pos = UnityObjectToClipPos(v.vertex);
        o.normal = UnityObjectToWorldNormal(v.normal);
        o.viewDir = WorldSpaceViewDir(v.vertex);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        // Setup instance ID in fragment shader.
        UNITY_SETUP_INSTANCE_ID(i);

        // Access the instanced properties.
        float4 innerColor = UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InnerColor);
        float4 rimColor = UNITY_ACCESS_INSTANCED_PROP(PerInstance, _RimColor);
        float rimPower = UNITY_ACCESS_INSTANCED_PROP(PerInstance, _RimPower);

        float3 worldNormal = normalize(i.normal);
        float3 viewDirection = normalize(i.viewDir);

        half rim = 1.0 - saturate(dot(viewDirection, worldNormal));
        
        float4 finalColor = innerColor;
        finalColor.rgb += rimColor.rgb * pow(rim, rimPower);
        
        return finalColor;
      }
      ENDCG
    }
  }
}