package fmttable

import "testing"

func TestFormatTable(t *testing.T) {
	rows := []Person{
		{Name: "Alice", Age: 30},
		{Name: "Bob", Age: 25},
		{Name: "Carol", Age: 5},
	}

	want := "Name      Age\n" +
		"Alice      30\n" +
		"Bob        25\n" +
		"Carol       5\n"

	if got := FormatTable(rows); got != want {
		t.Errorf("FormatTable(%v) = %q, want %q", rows, got, want)
	}
}

func TestFormatTableEmpty(t *testing.T) {
	want := "Name      Age\n"
	if got := FormatTable(nil); got != want {
		t.Errorf("FormatTable(nil) = %q, want %q", got, want)
	}
}
