package goroutinepipelinecontext

import (
	"context"
	"fmt"
	"reflect"
	"testing"
	"time"
)

// runWithDeadlockGuard runs RunPipeline on its own goroutine and fails the
// test if it does not return within a generous bound, which would indicate
// a stuck/leaked goroutine rather than a clean shutdown.
func runWithDeadlockGuard(t *testing.T, values []int, cancelAfter int) []int {
	t.Helper()

	resultCh := make(chan []int, 1)
	go func() {
		resultCh <- RunPipeline(context.Background(), values, cancelAfter)
	}()

	select {
	case got := <-resultCh:
		return got
	case <-time.After(2 * time.Second):
		t.Fatal("RunPipeline did not return: pipeline goroutines likely leaked/deadlocked")
		return nil
	}
}

func TestRunPipeline(t *testing.T) {
	cases := []struct {
		values      []int
		cancelAfter int
		want        []int
	}{
		{values: []int{1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, cancelAfter: 3, want: []int{1, 2, 3}},
		{values: []int{1, 2, 3, 4, 5}, cancelAfter: 5, want: []int{1, 2, 3, 4, 5}},
		{values: []int{}, cancelAfter: 1, want: nil},
		{values: []int{42}, cancelAfter: 1, want: []int{42}},
	}

	for i, tc := range cases {
		tc := tc
		t.Run(fmt.Sprintf("case_%d", i), func(t *testing.T) {
			got := runWithDeadlockGuard(t, tc.values, tc.cancelAfter)
			if !reflect.DeepEqual(got, tc.want) {
				t.Errorf("RunPipeline(%v, %d) = %v, want %v", tc.values, tc.cancelAfter, got, tc.want)
			}
		})
	}
}

// TestRunPipeline_StopsEarly makes sure the consumer never sees values
// produced after the cancellation point, even when the source has plenty
// more values queued up.
func TestRunPipeline_StopsEarly(t *testing.T) {
	values := make([]int, 1000)
	for i := range values {
		values[i] = i + 1
	}

	got := runWithDeadlockGuard(t, values, 4)
	want := []int{1, 2, 3, 4}

	if !reflect.DeepEqual(got, want) {
		t.Errorf("RunPipeline with 1000 values, cancelAfter=4 = %v, want %v", got, want)
	}
}
