package runecounter

import "testing"

func TestCountRunesAndBytes(t *testing.T) {
	cases := []struct {
		name      string
		input     string
		wantRunes int
		wantBytes int
	}{
		{"ascii", "hello", 5, 5},
		{"empty", "", 0, 0},
		{"multibyte", "héllo", 5, 6},
		{"japanese", "こんにちは", 5, 15},
		{"emoji", "gö", 2, 3},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			gotRunes, gotBytes := CountRunesAndBytes(tc.input)
			if gotRunes != tc.wantRunes {
				t.Errorf("CountRunesAndBytes(%q) runes = %d, want %d", tc.input, gotRunes, tc.wantRunes)
			}
			if gotBytes != tc.wantBytes {
				t.Errorf("CountRunesAndBytes(%q) bytes = %d, want %d", tc.input, gotBytes, tc.wantBytes)
			}
			if tc.name == "multibyte" || tc.name == "japanese" || tc.name == "emoji" {
				if gotRunes == gotBytes {
					t.Errorf("CountRunesAndBytes(%q): expected rune count (%d) to differ from byte length (%d) for multi-byte input", tc.input, gotRunes, gotBytes)
				}
			}
		})
	}
}
