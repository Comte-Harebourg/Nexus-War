//J'y comprends rien me demande pas comment ça marche
shader_type spatial;
render_mode blend_mix, unshaded;

uniform vec3 border_color: source_color = vec3(0.0);
uniform float grid_alpha = 0.5;
uniform float grid_size = 6.25;
uniform float border_width = 1.0;
uniform bool show_borders = true;
uniform sampler2D color_data_map: filter_nearest;
uniform vec2 map_size;
varying vec3 world_pos;

void vertex() {
	world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
	vec2 cell_uv = world_pos.xz / grid_size;
	vec2 grid = abs(fract(cell_uv - 0.5) - 0.5) / fwidth(cell_uv);
	float line = min(grid.x, grid.y);

	vec2 cell_coord = floor(cell_uv);
	vec2 data_uv = (cell_coord + map_size * 0.5 + 0.5) / map_size;
	vec4 color_data = texture(color_data_map, data_uv);

	// 1.0 sur la bordure, 0.0 à l'intérieur
	float border_mask = show_borders ? 1.0 - clamp(line - border_width + 1.0, 0.0, 1.0) : 0.0;

	// Alpha du remplissage : 0 si aucune couleur appliquée
	float fill_alpha = color_data.a * grid_alpha;

	// Alpha de la bordure : même valeur que celui des tuiles colorées
	float border_alpha = grid_alpha;

	ALBEDO = mix(color_data.rgb, border_color, border_mask);
	ALPHA = mix(fill_alpha, border_alpha, border_mask);
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
