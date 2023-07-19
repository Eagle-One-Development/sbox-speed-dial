//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	CompileTargets = ( IS_SM_50 && ( PC || VULKAN ) );
	Description = "Speed-Dial King of The Hill Shader";
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{
    #include "common/features.hlsl"
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
MODES
{
    VrForward();													// Indicates this shader will be used for main rendering
    ToolsVis( S_MODE_TOOLS_VIS ); 									// Ability to see in the editor
    ToolsWireframe( "vr_tools_wireframe.vfx" ); 					// Allows for mat_wireframe to work
	ToolsShadingComplexity( "vr_tools_shading_complexity.vfx" ); 	// Shows how expensive drawing is in debug view
}

//=========================================================================================================================
COMMON
{
	#include "common/shared.hlsl"
	#define COLOR_WRITE_ALREADY_SET
	#define S_TRANSLUCENT 1
	#define BLEND_MODE_ALREADY_SET
	
    
}

//=========================================================================================================================

struct VertexInput
{
	#include "common/vertexinput.hlsl"
};

//=========================================================================================================================

struct PixelInput
{
	#include "common/pixelinput.hlsl"
};

//=========================================================================================================================

VS
{
	#include "common/vertex.hlsl"
	//
	// Main
	//
	PixelInput MainVs( INSTANCED_SHADER_PARAMS( VS_INPUT i ) )
	{
		PixelInput o = ProcessVertex( i );
		// Add your vertex manipulation functions here
		return FinalizeVertex( o );
	}
}

//=========================================================================================================================

PS
{
	CreateInputTexture2D( DecoreTexture,   Srgb,       8,  "",     "_color",  "Material,10/90", Default3( 1.0, 1.0, 1.0 ) );
    CreateTexture2D( g_tDecoreTexture ) < Channel( RGBA, Box( DecoreTexture ), Srgb ); OutputFormat( BC7 ); SrgbRead( true ); >;
	float3 g_tBaseColor < UiType( Color ); Default3( 1.0, 1.0, 1.0 ); UiGroup( "Material,10/90" ); >;
	float3 g_tHilightColor < UiType( Color ); Default3( 1.0, 1.0, 1.0 ); UiGroup( "Material,10/90" ); >;
	float g_flAnimTime < Default(2.0f); Range(0.0, 1.0); UiGroup("Material,10/90");>;
	float g_flLowOpacity < Default(1.0f); Range(0.0, 1.0); UiGroup("Material,10/90");>;
	float g_flHighOpacity < Default(1.0f); Range(0.0, 1.0); UiGroup("Material,10/90");>;
	float g_flHighlightIntensity < Default(1.0f); Range(0.0, 10.0); UiGroup("Material,10/90");>;
	
	float g_flSmallGridScaling < Default(1.0f); Range(0.0, 10.0); UiGroup("Material,10/90");>;
	float g_flSmallHighlightIntensity < Default(1.0f); Range(0.0, 10.0); UiGroup("Material,10/90");>;
	
    #include "common/pixel.hlsl"
	RenderState( BlendEnable, true );
	RenderState( SrcBlend, SRC_ALPHA );
    RenderState( DstBlend, INV_SRC_ALPHA );
	RenderState( ColorWriteEnable0, RGBA );
	RenderState( FillMode, SOLID );
	RenderState( AlphaToCoverageEnable, true );

	//
	// Main
	//
	PixelOutput MainPs( PixelInput i )
	{
		PixelOutput o;
		
		float2 uv = i.vTextureCoords.xy + g_flTime * g_flAnimTime;
		float2 uv2 = i.vTextureCoords.xy;
		
		float3 col = Tex2D(g_tDecoreTexture, uv).rgb;
		float3 col2 = Tex2D(g_tDecoreTexture, uv2 * g_flSmallGridScaling ).rgb * g_tHilightColor.rgb;
		
		float3 finalCol = g_tHilightColor.rgb * g_flHighlightIntensity;
		
		o.vColor.rgb = lerp(g_tBaseColor.rgb,finalCol,col.r); 
		o.vColor.rgb +=  col2 * g_flSmallHighlightIntensity;
		
		o.vColor.a = lerp(g_flLowOpacity,g_flHighOpacity,col.r);
		return o;
	}
}