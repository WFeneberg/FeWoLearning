package bufiowriterflush

import (
	"bytes"
	"testing"
)

func TestWriteBuffered(t *testing.T) {
	cases := []struct {
		name  string
		lines []string
		want  string
	}{
		{
			name:  "multiple lines",
			lines: []string{"alpha", "beta", "gamma"},
			want:  "alpha\nbeta\ngamma\n",
		},
		{
			name:  "single line",
			lines: []string{"solo"},
			want:  "solo\n",
		},
		{
			name:  "no lines",
			lines: []string{},
			want:  "",
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			var buf bytes.Buffer

			bw := WriteBuffered(&buf, tc.lines)

			if got := buf.Len(); got != 0 {
				t.Fatalf("before Flush: underlying writer has %d bytes, want 0 (data leaked before flush)", got)
			}

			if err := bw.Flush(); err != nil {
				t.Fatalf("Flush() returned error: %v", err)
			}

			if got := buf.String(); got != tc.want {
				t.Errorf("after Flush: underlying writer = %q, want %q", got, tc.want)
			}
		})
	}
}
