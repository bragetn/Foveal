shader_type spatial;
render_mode unshaded, cull_disabled;

uniform vec3 color_a: source_color = vec3(0.0, 0.0, 0.0);
uniform vec3 color_b: source_color = vec3(1.0, 1.0, 1.0);
uniform int scale = 5;
uniform float line_width = 0.1;

varying vec3 local_position;

void vertex() {
	local_position = VERTEX;
}

void fragment() {
	vec2 pattern = mod(UV * float(scale) + vec2(line_width / 2.0, 1.0 + line_width / 2.0), 1);
	float grid = min(1.0, step(pattern.x, line_width) + step(pattern.y, line_width));
	
	if (grid == 0.0) {
		ALBEDO = color_b;
	}
	else {
		ALBEDO = color_a;
	}
}
