package bufferreusepool

import (
	"reflect"
	"testing"
)

func TestBucketCapacityRoundsUpToPowerOfTwo(t *testing.T) {
	cases := map[int]int{
		0:    64,
		1:    64,
		64:   64,
		65:   128,
		127:  128,
		128:  128,
		129:  256,
		200:  256,
		1024: 1024,
		1025: 2048,
	}
	for size, want := range cases {
		if got := BucketCapacity(size); got != want {
			t.Errorf("BucketCapacity(%d) = %d, want %d", size, got, want)
		}
	}
}

func TestBucketCapacityPanicsOnNegative(t *testing.T) {
	defer func() {
		if recover() == nil {
			t.Fatal("expected panic for negative size")
		}
	}()
	BucketCapacity(-1)
}

func TestGetReturnsCorrectLengthAndBucketCapacity(t *testing.T) {
	p := NewSizedBufferPool()
	buf := p.Get(100)
	if len(buf) != 100 {
		t.Fatalf("len(buf) = %d, want 100", len(buf))
	}
	if cap(buf) != 128 {
		t.Fatalf("cap(buf) = %d, want 128", cap(buf))
	}
}

// TestPutThenGetReusesSameUnderlyingArray is the core assertion: a buffer
// returned to the pool and re-requested at the same size bucket must be the
// exact same backing array (not merely the same capacity by coincidence),
// and its contents must have been cleared.
func TestPutThenGetReusesSameUnderlyingArray(t *testing.T) {
	p := NewSizedBufferPool()

	buf1 := p.Get(100) // bucket 128
	if cap(buf1) != 128 {
		t.Fatalf("cap(buf1) = %d, want 128", cap(buf1))
	}
	for i := range buf1 {
		buf1[i] = 0xFF // dirty it so we can detect whether Put clears it
	}
	ptr1 := reflect.ValueOf(buf1).Pointer()

	p.Put(buf1)

	buf2 := p.Get(90) // same bucket 128
	ptr2 := reflect.ValueOf(buf2).Pointer()

	if ptr1 != ptr2 {
		t.Fatalf("Get after Put did not reuse the same backing array: ptr1=%v ptr2=%v", ptr1, ptr2)
	}
	if cap(buf2) != 128 {
		t.Fatalf("cap(buf2) = %d, want 128", cap(buf2))
	}
	if len(buf2) != 90 {
		t.Fatalf("len(buf2) = %d, want 90", len(buf2))
	}
	for i, b := range buf2 {
		if b != 0 {
			t.Fatalf("buf2[%d] = %d, want 0 (buffer must be cleared on reuse)", i, b)
		}
	}
	// Even the full reclaimed capacity beyond len must have been cleared.
	full := buf2[:cap(buf2)]
	for i, b := range full {
		if b != 0 {
			t.Fatalf("full[%d] = %d, want 0 (full capacity must be cleared)", i, b)
		}
	}
}

// TestDifferentBucketsAreNotReused ensures a returned buffer from one bucket
// is not handed back for a request that rounds up to a different bucket.
func TestDifferentBucketsAreNotReused(t *testing.T) {
	p := NewSizedBufferPool()

	small := p.Get(10) // bucket 64
	ptrSmall := reflect.ValueOf(small).Pointer()
	p.Put(small)

	large := p.Get(1000) // bucket 1024, different bucket
	ptrLarge := reflect.ValueOf(large).Pointer()

	if ptrSmall == ptrLarge {
		t.Fatal("buffers from different buckets must not share a backing array")
	}
	if cap(large) != 1024 {
		t.Fatalf("cap(large) = %d, want 1024", cap(large))
	}
}

func TestPutNilOrEmptyIsNoop(t *testing.T) {
	p := NewSizedBufferPool()
	p.Put(nil)
	p.Put([]byte{})
	// Should not panic and should not affect subsequent Get behavior.
	buf := p.Get(5)
	if len(buf) != 5 || cap(buf) != 64 {
		t.Fatalf("Get(5) after Put(nil/empty) = len %d cap %d, want len 5 cap 64", len(buf), cap(buf))
	}
}
