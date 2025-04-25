shader_type spatial;

instance uniform bool completed = false;
instance uniform float t = 0.0;
uniform vec4 base_color: source_color = vec4(0.25, 0.35, 0.68, 1.0);
uniform vec4 hover_color: source_color = vec4(0.592, 0.847, 0.769, 1.0);
uniform vec4 completed_color: source_color = vec4(0.957, 0.725, 0.259, 1.0);

void fragment() {
	if (completed) {
		ALBEDO = completed_color.rgb;
	}
	else {
		ALBEDO = mix(base_color.rgb, hover_color.rgb, t);
	}

	ROUGHNESS = 0.25;
}
