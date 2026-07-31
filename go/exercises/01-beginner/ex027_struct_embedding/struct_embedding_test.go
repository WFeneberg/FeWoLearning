package structembedding

import "testing"

func TestSpeak(t *testing.T) {
	cases := []struct {
		name string
		got  string
		want string
	}{
		{"Animal", Animal{Name: "Generic"}.Speak(), "Generic makes a sound."},
		{"Dog", Dog{Animal{Name: "Rex"}}.Speak(), "Rex says Woof!"},
	}

	for _, c := range cases {
		if c.got != c.want {
			t.Errorf("%s.Speak() = %q, want %q", c.name, c.got, c.want)
		}
	}
}

func TestDogPromotedName(t *testing.T) {
	d := Dog{Animal{Name: "Fido"}}
	if d.Name != "Fido" {
		t.Errorf("d.Name = %q, want %q (promoted field access)", d.Name, "Fido")
	}
}
