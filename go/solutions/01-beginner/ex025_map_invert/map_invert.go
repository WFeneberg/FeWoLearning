// Package mapinvert — Exercise 025 (reference solution).
package mapinvert

import "fmt"

func Invert(m map[string]int) (map[int]string, error) {
	out := make(map[int]string, len(m))
	for k, v := range m {
		if existing, ok := out[v]; ok {
			return nil, fmt.Errorf("duplicate value %d: keys %q and %q both map to it", v, existing, k)
		}
		out[v] = k
	}
	return out, nil
}
