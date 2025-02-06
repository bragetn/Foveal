shader_type spatial;

uniform vec3 color_a: source_color = vec3(1.0);
uniform vec3 color_b: source_color = vec3(0.0);
uniform float scale = 1.0;
uniform vec3 offset = vec3(0.0);

varying vec3 local_position;

void vertex() {
	local_position = VERTEX;
}

void fragment() {
	vec3 scaled_position = floor((local_position + offset + 0.001) / scale);
	float pattern = mod(scaled_position.x + scaled_position.y + scaled_position.z, 2);
	ALBEDO = (pattern == 0.0) ? color_a : color_b;
}
