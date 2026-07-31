// Package sortstablemultikey — Exercise 035 (reference solution).
package sortstablemultikey

import "sort"

// Student represents a single student record.
type Student struct {
	Name  string
	Grade int
}

// SortByGradeThenName sorts students by Grade ascending using sort.Stable,
// so that students sharing the same grade retain their original relative order.
func SortByGradeThenName(students []Student) {
	sort.Stable(byGrade(students))
}

type byGrade []Student

func (s byGrade) Len() int           { return len(s) }
func (s byGrade) Less(i, j int) bool { return s[i].Grade < s[j].Grade }
func (s byGrade) Swap(i, j int)      { s[i], s[j] = s[j], s[i] }
