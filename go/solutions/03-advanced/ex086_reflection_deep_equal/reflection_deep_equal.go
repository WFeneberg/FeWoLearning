// Package reflectiondeepequal — Exercise 086 (reference solution).
package reflectiondeepequal

import "reflect"

// DeepEqual reports whether a and b are structurally equal, recursing into
// slices, arrays, maps, structs, and pointers as needed.
func DeepEqual(a, b interface{}) bool {
	if a == nil || b == nil {
		return a == nil && b == nil
	}
	va := reflect.ValueOf(a)
	vb := reflect.ValueOf(b)
	return deepEqual(va, vb)
}

func deepEqual(a, b reflect.Value) bool {
	if a.Type() != b.Type() {
		return false
	}

	switch a.Kind() {
	case reflect.Ptr:
		if a.IsNil() || b.IsNil() {
			return a.IsNil() == b.IsNil()
		}
		return deepEqual(a.Elem(), b.Elem())

	case reflect.Interface:
		if a.IsNil() || b.IsNil() {
			return a.IsNil() == b.IsNil()
		}
		return deepEqual(a.Elem(), b.Elem())

	case reflect.Slice:
		if a.IsNil() != b.IsNil() {
			return false
		}
		if a.Len() != b.Len() {
			return false
		}
		for i := 0; i < a.Len(); i++ {
			if !deepEqual(a.Index(i), b.Index(i)) {
				return false
			}
		}
		return true

	case reflect.Array:
		for i := 0; i < a.Len(); i++ {
			if !deepEqual(a.Index(i), b.Index(i)) {
				return false
			}
		}
		return true

	case reflect.Map:
		if a.IsNil() != b.IsNil() {
			return false
		}
		if a.Len() != b.Len() {
			return false
		}
		for _, key := range a.MapKeys() {
			bv := b.MapIndex(key)
			if !bv.IsValid() {
				return false
			}
			if !deepEqual(a.MapIndex(key), bv) {
				return false
			}
		}
		return true

	case reflect.Struct:
		for i := 0; i < a.NumField(); i++ {
			if !deepEqual(a.Field(i), b.Field(i)) {
				return false
			}
		}
		return true

	default:
		// Fallback for primitives (numbers, strings, bools, etc.) and any
		// kind not specially handled above.
		return reflect.DeepEqual(a.Interface(), b.Interface())
	}
}
