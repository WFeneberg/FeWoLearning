// Package errgroupparallelfetch — Exercise 079 (reference solution).
package errgroupparallelfetch

import (
	"context"

	"golang.org/x/sync/errgroup"
)

// FetchAll runs fetch(ctx, url) for every url in urls concurrently using an
// errgroup.Group created via errgroup.WithContext. All fetches share the same
// context: as soon as one fetch returns a non-nil error, that context is
// canceled, so every other fetch that is watching ctx.Done() (in flight or
// about to start) is signalled to stop early.
//
// FetchAll returns the first non-nil error reported by any fetch call, or nil
// if every fetch succeeded.
func FetchAll(urls []string, fetch func(ctx context.Context, url string) error) error {
	g, ctx := errgroup.WithContext(context.Background())

	for _, url := range urls {
		url := url // capture for the closure (pre-Go 1.22 loop-var safety)
		g.Go(func() error {
			return fetch(ctx, url)
		})
	}

	return g.Wait()
}
