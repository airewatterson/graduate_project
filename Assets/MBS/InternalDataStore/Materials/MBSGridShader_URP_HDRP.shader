Shader "MBS/Grid/Grid_URP_HDRP"
{
   Properties
   {
      _MainColor("Main Color", Color) = (0.5, 1.0, 1.0, 1.0)
      _SecondaryColor("Secondary Color", Color) = (0.0, 0.0, 0.0, 0.5)
      _BackgroundColor("Background Color", Color) = (0.0, 0.0, 0.0, 0.0)

      [Header(Grid)]
      _GridScale("Grid Scale", Range(1, 10)) = 1.0
      _ThicknessMain("Lines Thickness", Range(1, 10)) = 5
      _ThicknessSecondary("Lines Thickness", Range(1, 10)) = 3
      _Fade("Color Fade", Range(0.0, 1.0)) = 0.2

      [Header(Mesh)]
      _MeshScale("Mesh Scale", Float) = 50.0
      _GridHeight("Grid Height", Float) = 0.0
      _GridPosition("Grid Position", Vector) = (0.0, 0.0, 0.0)
   }
   SubShader
   {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType" = "Quad"}
      Blend SrcAlpha OneMinusSrcAlpha
      Pass
      {
         CGPROGRAM
         #pragma vertex vert alpha
         #pragma fragment frag alpha
         #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
         
         #include "UnityCG.cginc"

         struct VertIn
         {
            float2 uv : TEXCOORD0;
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
         };

         struct VertOut
         {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            UNITY_VERTEX_OUTPUT_STEREO
         };

         

         float _MeshScale;
         float4 _GridPosition;
         float _GridHeight;

         float _GridScale;
         float _Fade;

         float _ThicknessMain;
         float _ThicknessSecondary;
         float _SecondaryFadeInSpeed;

         fixed4 _MainColor;
         fixed4 _SecondaryColor;
         fixed4 _BackgroundColor;

      
         VertOut vert (VertIn IN)
         {
            VertOut OUT;

            UNITY_SETUP_INSTANCE_ID(IN);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

            float _MeshSizeMeters = 0.001f;
            float meshConstScale =  max(1, 1 / _MeshSizeMeters);

            IN.vertex.x *= _MeshScale * meshConstScale;
            IN.vertex.y = 1 * _GridHeight;
            IN.vertex.z *= _MeshScale * meshConstScale;

            OUT.vertex = UnityObjectToClipPos(IN.vertex);
            OUT.uv = IN.uv - 0.5f;

            return OUT;
         }

         fixed4 frag (VertOut IN) : SV_Target
         {           
            float _MeshSizeMeters = 0.001f;
            float2 pos;
            float4 col = float4(1,0,0,1);
            float4 shift = _GridPosition / _GridScale;
            
            float lessOneCompensator = 1;
            if(_MeshSizeMeters < 1){
               lessOneCompensator = 10;
               _GridPosition /= lessOneCompensator;
               _GridScale *= lessOneCompensator;
               shift /= lessOneCompensator;
            }
            
            float localScale = 1 / _GridScale;
            
            float scaleMul = _MeshScale * localScale;
            float offset = (0.5 / scaleMul);

            _ThicknessMain *= (0.001f * lessOneCompensator) * localScale;
            _ThicknessSecondary *= ( 0.001f * lessOneCompensator) * localScale;
            shift /= scaleMul;
                        
            pos.x = floor(frac((IN.uv.x - offset * _ThicknessMain + shift.x) * scaleMul) + _ThicknessMain);
            pos.y = floor(frac((IN.uv.y - offset * _ThicknessMain + shift.z) * scaleMul) + _ThicknessMain);
            
            float dist = distance( float2(0,0), IN.uv)  * _Fade * 10;

            if (pos.x == 1 || pos.y == 1) 
            {
               col = _MainColor;
               col.a = smoothstep(_MainColor.a, 0,  dist);
            } else 
            {
               pos.x = floor(frac((IN.uv.x - offset * _ThicknessSecondary  + shift.x)  * 10.0 * scaleMul) + _ThicknessSecondary * 10.0);
               pos.y = floor(frac((IN.uv.y - offset * _ThicknessSecondary  + shift.z) * 10.0 * scaleMul) + _ThicknessSecondary * 10.0);
         
               if (pos.x == 1 || pos.y == 1) 
               {
                  col = _SecondaryColor;
                  col.a = smoothstep(_SecondaryColor.a, 0.0, dist);
               } else 
               {
                  col = _BackgroundColor;
                  col.a = max(0, lerp(_BackgroundColor.a , 0.0, dist));
               }
            }    
            return col;
         }
         ENDCG
      }
   }
}