// Package mapgroupby — Exercise 032 (reference solution).
package mapgroupby

// GroupByLength groups words by their length, preserving input order within
// each group.
func GroupByLength(words []string) map[int][]string {
	groups := make(map[int][]string)
	for _, w := range words {
		l := len(w)
		groups[l] = append(groups[l], w)
	}
	return groups
}
