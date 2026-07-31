package sortpeople

import "testing"

func TestSortByAge(t *testing.T) {
	people := []Person{
		{Name: "Alice", Age: 30},
		{Name: "Bob", Age: 25},
		{Name: "Carol", Age: 40},
		{Name: "Dave", Age: 25},
	}

	SortByAge(people)

	wantAges := []int{25, 25, 30, 40}
	for i, want := range wantAges {
		if people[i].Age != want {
			t.Fatalf("people[%d].Age = %d, want %d (full: %+v)", i, people[i].Age, want, people)
		}
	}
}

func TestSortByAgeSingleAndEmpty(t *testing.T) {
	empty := []Person{}
	SortByAge(empty)
	if len(empty) != 0 {
		t.Fatalf("expected empty slice to remain empty, got %+v", empty)
	}

	single := []Person{{Name: "Solo", Age: 42}}
	SortByAge(single)
	if single[0].Age != 42 || single[0].Name != "Solo" {
		t.Fatalf("expected single element unchanged, got %+v", single)
	}
}
