// Package errgroupparallelfetch — Exercise 079 (advanced).
// Goal:   Run a batch of "fetches" concurrently with golang.org/x/sync/errgroup,
//         returning the first error and cancelling the shared context so that
//         remaining (or still in-flight) fetches observe the cancellation.
// Drills: errgroup.Group, errgroup.WithContext, context cancellation propagation.
package errgroupparallelfetch

import "context"

// FetchAll runs fetch(ctx, url) for every url in urls concurrently using an
// errgroup.Group created via errgroup.WithContext. All fetches share the same
// context: as soon as one fetch returns a non-nil error, that context is
// canceled, so every other fetch that is watching ctx.Done() (in flight or
// about to start) is signalled to stop early.
//
// FetchAll returns the first non-nil error reported by any fetch call, or nil
// if every fetch succeeded.
func FetchAll(urls []string, fetch func(ctx context.Context, url string) error) error {
	panic("TODO: implement FetchAll using errgroup.WithContext")
}
