shader_type spatial;
render_mode blend_mix;

uniform vec3 grid_color: source_color = vec3(1.0);
uniform vec3 border_color: source_color = vec3(0.0);
uniform float grid_alpha = 0.5;
uniform float grid_size = 6.25;
uniform float border_width = 1.0;
uniform sampler2D color_data_map: filter_nearest;
uniform vec2 map_size;
varying vec3 world_pos;

void vertex() {
	world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
	vec2 grid = abs(fract(world_pos.xz / grid_size - 0.5) - 0.5) / fwidth(world_pos.xz / grid_size);
	float line = min(grid.x, grid.y);
	vec2 cell_coord = floor(world_pos.xz / grid_size);
	vec2 data_uv = (cell_coord + map_size * 0.5 + 0.5) / map_size;
	vec4 color_data = texture(color_data_map, data_uv);
	float mask = clamp(line - border_width + 1.0, 0.0, 1.0);
	ALBEDO = mix(border_color, color_data.xyz, mask);
	ALPHA = grid_alpha;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
