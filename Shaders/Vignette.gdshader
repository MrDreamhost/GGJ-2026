shader_type canvas_item;

uniform vec4 vignette_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float intensity : hint_range(0.0, 1.0) = 0.4;
uniform float softness : hint_range(0.0, 1.0) = 0.6;

void fragment() {
    float distance_to_center = distance(UV, vec2(0.5));
    float vignette = smoothstep(intensity, intensity + softness, distance_to_center);
    
    // We use the RGB from our uniform and the calculated 'vignette' for alpha
    COLOR = vec4(vignette_color.rgb, vignette);
}
