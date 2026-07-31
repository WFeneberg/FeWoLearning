package reflectionstructtags

import (
	"reflect"
	"testing"
)

// sampleStruct exercises the rules ExtractTags must implement:
//   - a field with both a json and a validate tag
//   - a field with json options (",omitempty") that must be returned raw
//   - a field with only a validate tag (no json tag at all)
//   - a field explicitly excluded from json via "-"
//   - an unexported field that must never appear in the result
type sampleStruct struct {
	ID       int    `json:"id" validate:"required"`
	Name     string `json:"name" validate:"required,min=2"`
	Email    string `json:"email,omitempty"`
	Age      int    `validate:"gte=0"`
	Password string `json:"-" validate:"required"`
	internal string
}

func TestExtractTagsJSON(t *testing.T) {
	got := ExtractTags(sampleStruct{}, "json")
	want := map[string]string{
		"ID":    "id",
		"Name":  "name",
		"Email": "email,omitempty",
	}
	if !reflect.DeepEqual(got, want) {
		t.Errorf("ExtractTags(json) = %#v, want %#v", got, want)
	}
}

func TestExtractTagsValidate(t *testing.T) {
	got := ExtractTags(sampleStruct{}, "validate")
	want := map[string]string{
		"ID":       "required",
		"Name":     "required,min=2",
		"Age":      "gte=0",
		"Password": "required",
	}
	if !reflect.DeepEqual(got, want) {
		t.Errorf("ExtractTags(validate) = %#v, want %#v", got, want)
	}
}

func TestExtractTagsPointerInput(t *testing.T) {
	s := &sampleStruct{}
	got := ExtractTags(s, "json")
	want := map[string]string{
		"ID":    "id",
		"Name":  "name",
		"Email": "email,omitempty",
	}
	if !reflect.DeepEqual(got, want) {
		t.Errorf("ExtractTags(pointer) = %#v, want %#v", got, want)
	}
}

func TestExtractTagsNilPointer(t *testing.T) {
	var s *sampleStruct
	got := ExtractTags(s, "json")
	if len(got) != 0 {
		t.Errorf("ExtractTags(nil pointer) = %#v, want empty map", got)
	}
}

func TestExtractTagsMissingKey(t *testing.T) {
	got := ExtractTags(sampleStruct{}, "xml")
	if len(got) != 0 {
		t.Errorf("ExtractTags(unknown key) = %#v, want empty map", got)
	}
}

func TestExtractTagsNonStructPanics(t *testing.T) {
	defer func() {
		if recover() == nil {
			t.Fatal("expected ExtractTags to panic for a non-struct argument")
		}
	}()
	ExtractTags(42, "json")
}
