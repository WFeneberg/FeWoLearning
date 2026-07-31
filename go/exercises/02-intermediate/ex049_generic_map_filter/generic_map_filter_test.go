package genericmapfilter

import (
	"reflect"
	"strconv"
	"testing"
)

func TestMap(t *testing.T) {
	ints := []int{1, 2, 3, 4}
	got := Map(ints, func(n int) string { return strconv.Itoa(n * n) })
	want := []string{"1", "4", "9", "16"}
	if !reflect.DeepEqual(got, want) {
		t.Errorf("Map(ints, square-string) = %v, want %v", got, want)
	}

	words := []string{"go", "generics", "map"}
	gotLens := Map(words, func(s string) int { return len(s) })
	wantLens := []int{2, 8, 3}
	if !reflect.DeepEqual(gotLens, wantLens) {
		t.Errorf("Map(words, len) = %v, want %v", gotLens, wantLens)
	}
}

func TestFilter(t *testing.T) {
	nums := []int{1, 2, 3, 4, 5, 6, 7, 8}
	got := Filter(nums, func(n int) bool { return n%2 == 0 })
	want := []int{2, 4, 6, 8}
	if !reflect.DeepEqual(got, want) {
		t.Errorf("Filter(nums, even) = %v, want %v", got, want)
	}

	words := []string{"a", "bb", "ccc", "dddd"}
	gotWords := Filter(words, func(s string) bool { return len(s) > 2 })
	wantWords := []string{"ccc", "dddd"}
	if !reflect.DeepEqual(gotWords, wantWords) {
		t.Errorf("Filter(words, len>2) = %v, want %v", gotWords, wantWords)
	}

	empty := Filter([]int{1, 3, 5}, func(n int) bool { return n%2 == 0 })
	if len(empty) != 0 {
		t.Errorf("Filter(odds, even) = %v, want empty slice", empty)
	}
}
