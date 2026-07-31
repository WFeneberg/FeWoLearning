package temperatureconverter

import "testing"

const epsilon = 1e-9

func floatsClose(a, b float64) bool {
	diff := a - b
	if diff < 0 {
		diff = -diff
	}
	return diff < epsilon
}

func TestCelsiusToFahrenheit(t *testing.T) {
	cases := map[float64]float64{
		0:   32,
		100: 212,
		-40: -40,
		37:  98.6,
	}
	for c, want := range cases {
		if got := CelsiusToFahrenheit(c); !floatsClose(got, want) {
			t.Errorf("CelsiusToFahrenheit(%v) = %v, want %v", c, got, want)
		}
	}
}

func TestFahrenheitToCelsius(t *testing.T) {
	cases := map[float64]float64{
		32:  0,
		212: 100,
		-40: -40,
		98.6: 37,
	}
	for f, want := range cases {
		if got := FahrenheitToCelsius(f); !floatsClose(got, want) {
			t.Errorf("FahrenheitToCelsius(%v) = %v, want %v", f, got, want)
		}
	}
}

func TestRoundTrip(t *testing.T) {
	values := []float64{-100, -40, 0, 21.5, 37, 100, 1000}
	for _, v := range values {
		if got := FahrenheitToCelsius(CelsiusToFahrenheit(v)); !floatsClose(got, v) {
			t.Errorf("round trip C->F->C for %v = %v, want %v", v, got, v)
		}
		if got := CelsiusToFahrenheit(FahrenheitToCelsius(v)); !floatsClose(got, v) {
			t.Errorf("round trip F->C->F for %v = %v, want %v", v, got, v)
		}
	}
}
