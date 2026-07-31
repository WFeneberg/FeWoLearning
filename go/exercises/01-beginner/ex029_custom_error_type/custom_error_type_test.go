package customerrortype

import "testing"

func TestValidateAge(t *testing.T) {
	cases := []struct {
		name      string
		age       int
		wantErr   bool
		wantField string
	}{
		{"valid age", 30, false, ""},
		{"zero age", 0, false, ""},
		{"negative age", -1, true, "age"},
	}

	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			err := ValidateAge(tc.age)
			if !tc.wantErr {
				if err != nil {
					t.Fatalf("ValidateAge(%d) = %v, want nil", tc.age, err)
				}
				return
			}

			if err == nil {
				t.Fatalf("ValidateAge(%d) = nil, want error", tc.age)
			}

			ve, ok := err.(*ValidationError)
			if !ok {
				t.Fatalf("ValidateAge(%d) error type = %T, want *ValidationError", tc.age, err)
			}
			if ve.Field != tc.wantField {
				t.Errorf("ValidationError.Field = %q, want %q", ve.Field, tc.wantField)
			}
			if ve.Error() == "" {
				t.Errorf("ValidationError.Error() returned empty string")
			}
		})
	}
}
