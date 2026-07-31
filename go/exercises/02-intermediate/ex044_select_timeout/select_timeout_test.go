package selecttimeout

import (
	"testing"
	"time"
)

func TestReceiveWithTimeout(t *testing.T) {
	cases := []struct {
		name      string
		send      bool
		value     int
		wantOK    bool
		wantValue int
	}{
		{name: "value arrives in time", send: true, value: 42, wantOK: true, wantValue: 42},
		{name: "channel stays silent", send: false, wantOK: false, wantValue: 0},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			ch := make(chan int)
			if tc.send {
				go func() {
					ch <- tc.value
				}()
			}

			got, ok := ReceiveWithTimeout(ch, 20*time.Millisecond)
			if ok != tc.wantOK {
				t.Errorf("ok = %v, want %v", ok, tc.wantOK)
			}
			if got != tc.wantValue {
				t.Errorf("value = %d, want %d", got, tc.wantValue)
			}
		})
	}
}
