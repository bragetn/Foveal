shader_type spatial;
render_mode unshaded, depth_test_disabled;

uniform vec4 color: source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
	vec2 center = vec2(0.5, 0.5);
	float dist = length(UV - center);
	ALPHA = (dist < 0.5) ? 1.0 : 0.0;
	ALBEDO = color.rgb;
}
