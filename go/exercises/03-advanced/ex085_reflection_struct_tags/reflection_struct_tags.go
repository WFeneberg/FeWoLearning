// Package reflectionstructtags — Exercise 085 (advanced).
// Goal:   Read a given struct tag key from every exported field of a struct
//         (or pointer to struct) using reflection, returning a map of field
//         name -> raw tag value.
// Drills: reflect.Type/Value, struct tags, StructTag.Lookup, unexported
//         fields, pointer dereferencing.
package reflectionstructtags

// ExtractTags inspects v (a struct or a pointer to a struct) and returns a
// map from exported field name to the raw value of the tagKey struct tag.
//
// Rules:
//   - Unexported fields are skipped entirely.
//   - A field with no tagKey tag at all is skipped.
//   - A field whose tagKey tag value is exactly "-" is skipped (mirrors the
//     common encoding/json convention for "omit this field").
//   - The full raw tag value is returned as-is (e.g. "name,omitempty" is
//     returned unmodified, not split on the comma).
//   - A nil pointer of struct type yields an empty, non-nil map.
//
// ExtractTags panics if v (after dereferencing any pointer) is not a struct.
func ExtractTags(v interface{}, tagKey string) map[string]string {
	panic("TODO: implement ExtractTags")
}
