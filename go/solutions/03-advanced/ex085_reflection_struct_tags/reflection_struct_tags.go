// Package reflectionstructtags — Exercise 085 (reference solution).
package reflectionstructtags

import "reflect"

func ExtractTags(v interface{}, tagKey string) map[string]string {
	rv := reflect.ValueOf(v)
	for rv.Kind() == reflect.Ptr {
		if rv.IsNil() {
			return map[string]string{}
		}
		rv = rv.Elem()
	}
	if rv.Kind() != reflect.Struct {
		panic("reflectionstructtags: ExtractTags requires a struct or pointer to struct")
	}

	t := rv.Type()
	result := make(map[string]string, t.NumField())
	for i := 0; i < t.NumField(); i++ {
		field := t.Field(i)
		if field.PkgPath != "" {
			continue // unexported field, no reflect access to its tag semantics
		}
		tagVal, ok := field.Tag.Lookup(tagKey)
		if !ok || tagVal == "-" {
			continue
		}
		result[field.Name] = tagVal
	}
	return result
}
