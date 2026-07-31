package regexploglogparser

import "testing"

func TestParseLogLine(t *testing.T) {
	cases := []struct {
		name    string
		line    string
		wantErr bool
		want    map[string]string
	}{
		{
			name: "info level",
			line: "2024-01-02T15:04:05Z [INFO] server started",
			want: map[string]string{
				"timestamp": "2024-01-02T15:04:05Z",
				"level":     "INFO",
				"message":   "server started",
			},
		},
		{
			name: "error level with punctuation in message",
			line: "2023-12-31T23:59:59Z [ERROR] connection refused: timeout after 30s",
			want: map[string]string{
				"timestamp": "2023-12-31T23:59:59Z",
				"level":     "ERROR",
				"message":   "connection refused: timeout after 30s",
			},
		},
		{
			name:    "missing level brackets",
			line:    "2024-01-02T15:04:05Z INFO server started",
			wantErr: true,
		},
		{
			name:    "malformed timestamp",
			line:    "2024/01/02 15:04:05 [INFO] server started",
			wantErr: true,
		},
		{
			name:    "empty line",
			line:    "",
			wantErr: true,
		},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got, err := ParseLogLine(tc.line)
			if tc.wantErr {
				if err == nil {
					t.Fatalf("ParseLogLine(%q) = %v, nil; want error", tc.line, got)
				}
				return
			}
			if err != nil {
				t.Fatalf("ParseLogLine(%q) returned unexpected error: %v", tc.line, err)
			}
			if len(got) != len(tc.want) {
				t.Fatalf("ParseLogLine(%q) = %v (len %d), want %v (len %d)", tc.line, got, len(got), tc.want, len(tc.want))
			}
			for k, wantV := range tc.want {
				if gotV, ok := got[k]; !ok || gotV != wantV {
					t.Errorf("ParseLogLine(%q)[%q] = %q, ok=%v; want %q", tc.line, k, gotV, ok, wantV)
				}
			}
		})
	}
}
