shader_type spatial;

uniform float line_width : hint_range(0.0, 1.0) = 0.1;
uniform vec4 color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
    vec2 uv = UV * 2.0 - vec2(1.0);
	float radius = length(uv);

    float alpha =  step(1.0 - line_width, radius) * step(radius, 1.0);

	ALBEDO = color.rgb;
	EMISSION = color.rgb;
    ALPHA = alpha;
}
