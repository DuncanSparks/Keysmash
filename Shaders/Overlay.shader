shader_type canvas_item;
uniform vec4 color : hint_color;

float value(vec4 col) {
	return max(max(col.r, col.g), col.b);
}

void fragment() {
	COLOR = texture(SCREEN_TEXTURE, SCREEN_UV);
	
	if (COLOR.r < 0.5)
		COLOR.r *= 2f * color.r;
	else
		COLOR.r = 1f - (2f * (1f - COLOR.r) * (1f - color.r));
	
	if (COLOR.g < 0.5)
		COLOR.g *= 2f * color.g;
	else
		COLOR.g = 1f - (2f * (1f - COLOR.g) * (1f - color.g));
		
	if (COLOR.b < 0.5)
		COLOR.b *= 2f * color.b;
	else
		COLOR.b = 1f - (2f * (1f - COLOR.b) * (1f - color.b));
}
