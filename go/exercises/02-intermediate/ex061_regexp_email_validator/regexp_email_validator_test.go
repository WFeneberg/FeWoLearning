package regexpemailvalidator

import "testing"

func TestIsValidEmail(t *testing.T) {
	cases := map[string]bool{
		"john.doe@example.com":       true,
		"jane_doe123@sub.domain.co":  true,
		"a@b.io":                     true,
		"first.last+tag@example.org": true,
		"":                           false,
		"plainaddress":               false,
		"@missingusername.com":       false,
		"missingat.com":              false,
		"user@.com":                  false,
		"user@domain":                false,
		"user@domain..com":           false,
		"user name@example.com":      false,
		"user@exa mple.com":          false,
	}
	for input, want := range cases {
		if got := IsValidEmail(input); got != want {
			t.Errorf("IsValidEmail(%q) = %v, want %v", input, got, want)
		}
	}
}
