package reflectiondeepequal

import "testing"

type address struct {
	City string
	Tags []string
}

type person struct {
	Name    string
	Age     int
	Aliases []string
	Meta    map[string]int
	Home    *address
}

func samplePerson() person {
	return person{
		Name:    "Ada",
		Age:     36,
		Aliases: []string{"lovelace", "countess"},
		Meta:    map[string]int{"rank": 1, "score": 42},
		Home:    &address{City: "London", Tags: []string{"uk", "capital"}},
	}
}

func TestDeepEqualIdenticalStructuresWithNestedSlicesAndMaps(t *testing.T) {
	a := samplePerson()
	b := samplePerson()
	if !DeepEqual(a, b) {
		t.Fatal("expected structurally identical persons to be DeepEqual")
	}
	if !DeepEqual(&a, &b) {
		t.Fatal("expected pointers to identical persons to be DeepEqual")
	}
}

func TestDeepEqualNestedSliceElementDiffers(t *testing.T) {
	a := samplePerson()
	b := samplePerson()
	b.Home.Tags[1] = "metropolis" // differs deep inside pointer->struct->slice
	if DeepEqual(a, b) {
		t.Fatal("expected DeepEqual to detect difference nested inside pointer->slice")
	}
}

func TestDeepEqualNestedMapValueDiffers(t *testing.T) {
	a := samplePerson()
	b := samplePerson()
	b.Meta["score"] = 43
	if DeepEqual(a, b) {
		t.Fatal("expected DeepEqual to detect difference in nested map value")
	}
}

func TestDeepEqualMapKeyMissing(t *testing.T) {
	a := map[string][]int{"x": {1, 2, 3}, "y": {4, 5}}
	b := map[string][]int{"x": {1, 2, 3}}
	if DeepEqual(a, b) {
		t.Fatal("expected maps with different key sets to be unequal")
	}
}

func TestDeepEqualSliceOfMapsEqual(t *testing.T) {
	a := []map[string]int{{"a": 1}, {"b": 2}}
	b := []map[string]int{{"a": 1}, {"b": 2}}
	if !DeepEqual(a, b) {
		t.Fatal("expected slices of equal maps to be DeepEqual")
	}
}

func TestDeepEqualDifferentLengthSlices(t *testing.T) {
	a := []int{1, 2, 3}
	b := []int{1, 2, 3, 4}
	if DeepEqual(a, b) {
		t.Fatal("expected slices of different length to be unequal")
	}
}

func TestDeepEqualNilVsEmptySlice(t *testing.T) {
	var a []int
	b := []int{}
	if DeepEqual(a, b) {
		t.Fatal("expected nil slice and empty slice to be unequal, matching reflect.DeepEqual semantics")
	}
}

func TestDeepEqualDifferentTypesAreUnequal(t *testing.T) {
	if DeepEqual(1, "1") {
		t.Fatal("expected values of different dynamic types to be unequal")
	}
}

func TestDeepEqualNilPointersEqual(t *testing.T) {
	var a, b *address
	if !DeepEqual(a, b) {
		t.Fatal("expected two nil pointers of the same type to be DeepEqual")
	}
}

func TestDeepEqualOnePointerNil(t *testing.T) {
	a := &address{City: "Paris"}
	var b *address
	if DeepEqual(a, b) {
		t.Fatal("expected non-nil pointer and nil pointer to be unequal")
	}
}

func TestDeepEqualPrimitivesEqualAndUnequal(t *testing.T) {
	if !DeepEqual(42, 42) {
		t.Error("expected equal ints to be DeepEqual")
	}
	if DeepEqual(42, 43) {
		t.Error("expected different ints to be unequal")
	}
	if !DeepEqual("hello", "hello") {
		t.Error("expected equal strings to be DeepEqual")
	}
}
