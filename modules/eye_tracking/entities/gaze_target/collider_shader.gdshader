shader_type spatial;
render_mode unshaded;

uniform vec3 color: source_color = vec3(1.0, 1.0, 1.0);
uniform int grid_scale = 15;
uniform float line_width = 0.075;

uniform float collider_scale = 1.0;
uniform bool enabled = false;

void vertex() {
	VERTEX *= collider_scale;
}

void fragment() {
	if (!enabled) {
		ALPHA = 0.0;
	}
	else {
		vec2 pattern = mod(UV * float(grid_scale) + vec2(line_width / 2.0, 1.0 + line_width / 2.0), 1);
		float grid = min(1.0, step(pattern.x, line_width) + step(pattern.y, line_width));
		
		if (grid == 0.0) {
			ALPHA = 0.25;
		}
		else {
			ALPHA = 1.0;
		}
		
		ALBEDO = color;
	}
}
