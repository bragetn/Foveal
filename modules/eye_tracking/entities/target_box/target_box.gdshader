shader_type spatial;
render_mode unshaded, cull_disabled;

uniform vec3 color_a: source_color = vec3(0.0, 0.0, 0.0);
uniform vec3 color_b: source_color = vec3(1.0, 1.0, 1.0);
uniform int scale = 5;

varying vec3 local_position;

void vertex() {
	local_position = VERTEX;
}

void fragment() {
	vec2 pattern = mod(UV * float(scale) + vec2(0.05, 1.05), 1);
	float grid = min(1.0, step(pattern.x, 0.1) + step(pattern.y, 0.1));
	
	if (grid == 0.0) {
		ALBEDO = color_b;
	}
	else {
		ALBEDO = color_a;
	}
}
