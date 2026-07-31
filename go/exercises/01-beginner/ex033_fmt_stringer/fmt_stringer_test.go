package fmtstringer

import (
	"fmt"
	"testing"
)

func TestMoneyString(t *testing.T) {
	cases := []struct {
		cents int
		want  string
	}{
		{0, "$0.00"},
		{5, "$0.05"},
		{100, "$1.00"},
		{1234, "$12.34"},
		{999999, "$9999.99"},
	}
	for _, c := range cases {
		m := Money{Cents: c.cents}
		if got := m.String(); got != c.want {
			t.Errorf("Money{%d}.String() = %q, want %q", c.cents, got, c.want)
		}
	}
}

func TestMoneySprintf(t *testing.T) {
	m := Money{Cents: 4250}
	got := fmt.Sprintf("%s", m)
	want := "$42.50"
	if got != want {
		t.Errorf("fmt.Sprintf(%%s, m) = %q, want %q", got, want)
	}
}

func TestMoneySprint(t *testing.T) {
	m := Money{Cents: 150}
	got := fmt.Sprint(m)
	want := "$1.50"
	if got != want {
		t.Errorf("fmt.Sprint(m) = %q, want %q", got, want)
	}
}
