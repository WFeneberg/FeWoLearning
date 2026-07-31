package jsoncustommarshaler

import (
	"encoding/json"
	"testing"
	"time"
)

func TestSpanMarshalJSON(t *testing.T) {
	cases := []struct {
		d    time.Duration
		want string
	}{
		{90 * time.Minute, `"1h30m0s"`},
		{0, `"0s"`},
		{45 * time.Second, `"45s"`},
	}
	for _, c := range cases {
		got, err := json.Marshal(Span{D: c.d})
		if err != nil {
			t.Fatalf("json.Marshal(Span{%v}) error: %v", c.d, err)
		}
		if string(got) != c.want {
			t.Errorf("json.Marshal(Span{%v}) = %s, want %s", c.d, got, c.want)
		}
	}
}

func TestTaskMarshalJSON(t *testing.T) {
	task := Task{
		Name: "deploy",
		ETA:  Span{D: 2*time.Hour + 15*time.Minute},
	}
	got, err := json.Marshal(task)
	if err != nil {
		t.Fatalf("json.Marshal(task) error: %v", err)
	}
	want := `{"name":"deploy","eta":"2h15m0s"}`
	if string(got) != want {
		t.Errorf("json.Marshal(task) = %s, want %s", got, want)
	}
}
