package pipelinecancelcontext

import (
	"context"
	"testing"
)

func TestPipelineStopsOnCancelMidStream(t *testing.T) {
	ctx, cancel := context.WithCancel(context.Background())
	defer cancel()

	p := New()
	items := []int{1, 2, 3, 4, 5, 6, 7, 8, 9, 10}
	square := func(v int) int { return v * v }
	out := p.Run(ctx, items, square)

	const cancelAt = 3
	var got []int
	for v := range out {
		got = append(got, v)
		if len(got) == cancelAt {
			cancel()
		}
	}

	// Once the output channel has drained closed, every pipeline goroutine
	// must have exited.
	p.Wait()
	if active := p.Active(); active != 0 {
		t.Fatalf("Active() = %d after cancellation + Wait, want 0", active)
	}

	if len(got) < cancelAt {
		t.Fatalf("got %d items, want at least %d before cancellation", len(got), cancelAt)
	}
	if len(got) >= len(items) {
		t.Fatalf("got %d items, want fewer than the full %d (cancellation should stop production)", len(got), len(items))
	}

	for i, v := range got {
		want := square(items[i])
		if v != want {
			t.Errorf("got[%d] = %d, want %d (square of %d)", i, v, want, items[i])
		}
	}
}

func TestPipelineEmitsAllItemsWithoutCancel(t *testing.T) {
	ctx := context.Background()
	p := New()
	items := []int{2, 4, 6, 8}
	double := func(v int) int { return v * 2 }
	out := p.Run(ctx, items, double)

	var got []int
	for v := range out {
		got = append(got, v)
	}
	p.Wait()

	if active := p.Active(); active != 0 {
		t.Fatalf("Active() = %d after completion, want 0", active)
	}
	if len(got) != len(items) {
		t.Fatalf("got %d items, want %d", len(got), len(items))
	}
	for i, v := range got {
		want := double(items[i])
		if v != want {
			t.Errorf("got[%d] = %d, want %d", i, v, want)
		}
	}
}

func TestActiveZeroBeforeAnyRun(t *testing.T) {
	p := New()
	if active := p.Active(); active != 0 {
		t.Fatalf("Active() = %d before any Run, want 0", active)
	}
}

func TestPipelineCancelBeforeStart(t *testing.T) {
	ctx, cancel := context.WithCancel(context.Background())
	cancel() // already canceled

	p := New()
	items := []int{1, 2, 3}
	out := p.Run(ctx, items, func(v int) int { return v })

	var got []int
	for v := range out {
		got = append(got, v)
	}
	p.Wait()

	if active := p.Active(); active != 0 {
		t.Fatalf("Active() = %d after Wait, want 0", active)
	}
	if len(got) != 0 {
		t.Fatalf("got %d items from an already-canceled context, want 0", len(got))
	}
}
