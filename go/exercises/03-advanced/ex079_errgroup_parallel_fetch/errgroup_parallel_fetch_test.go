package errgroupparallelfetch

import (
	"context"
	"errors"
	"testing"
)

var errBoom = errors.New("boom: fetch failed")

// TestFetchAllReturnsFirstErrorAndCancelsRemaining exercises the core
// contract: one fetch fails, and every other fetch — which blocks on
// ctx.Done() instead of doing real work — must observe that the shared
// context was canceled as a result. FetchAll must still return exactly the
// original error, never context.Canceled, since errgroup only ever reports
// the first error recorded.
func TestFetchAllReturnsFirstErrorAndCancelsRemaining(t *testing.T) {
	urls := []string{"bad", "slow-1", "slow-2"}
	observedCancel := make(chan string, 2)

	fetch := func(ctx context.Context, url string) error {
		if url == "bad" {
			return errBoom
		}
		<-ctx.Done() // blocks until the failing fetch cancels the group's context
		observedCancel <- url
		return ctx.Err()
	}

	err := FetchAll(urls, fetch)
	if !errors.Is(err, errBoom) {
		t.Fatalf("FetchAll() error = %v, want %v", err, errBoom)
	}

	seen := map[string]bool{}
	for i := 0; i < 2; i++ {
		seen[<-observedCancel] = true
	}
	if !seen["slow-1"] || !seen["slow-2"] {
		t.Fatalf("expected both slow fetches to observe cancellation, got %v", seen)
	}
}

// TestFetchAllAllSucceedReturnsNil is the happy path: every fetch succeeds,
// FetchAll must return nil and every url must have been fetched exactly once.
func TestFetchAllAllSucceedReturnsNil(t *testing.T) {
	urls := []string{"one", "two", "three"}
	calls := make(chan string, len(urls))

	fetch := func(ctx context.Context, url string) error {
		if err := ctx.Err(); err != nil {
			return err
		}
		calls <- url
		return nil
	}

	if err := FetchAll(urls, fetch); err != nil {
		t.Fatalf("FetchAll() error = %v, want nil", err)
	}

	close(calls)
	got := map[string]int{}
	for url := range calls {
		got[url]++
	}
	for _, url := range urls {
		if got[url] != 1 {
			t.Errorf("fetch(%q) called %d times, want 1", url, got[url])
		}
	}
	if len(got) != len(urls) {
		t.Errorf("got %d distinct fetched urls, want %d", len(got), len(urls))
	}
}

// TestFetchAllEmptyReturnsNil verifies the trivial zero-work case.
func TestFetchAllEmptyReturnsNil(t *testing.T) {
	called := false
	fetch := func(ctx context.Context, url string) error {
		called = true
		return nil
	}
	if err := FetchAll(nil, fetch); err != nil {
		t.Fatalf("FetchAll(nil, ...) error = %v, want nil", err)
	}
	if called {
		t.Error("fetch should not be called for an empty url list")
	}
}
