// Package middlewarechain — Exercise 093 (expert).
// Goal:   Compose a base Handler with a chain of Middleware (logging, auth, ...)
//         so that middleware execute in the order given, wrapping the base
//         handler, and any middleware may short-circuit the chain before the
//         base handler (or downstream middleware) ever runs.
// Drills: higher-order functions, function composition, closures, decorator pattern.
package middlewarechain

// Request is the minimal input passed through the chain.
type Request struct {
	Path      string
	AuthToken string
}

// Response is the result produced by a Handler.
type Response struct {
	Status int
	Body   string
}

// Handler processes a Request and produces a Response.
type Handler func(req *Request) *Response

// Middleware wraps a Handler to produce a new Handler, typically running
// logic before and/or after delegating to (or instead of) the wrapped Handler.
type Middleware func(next Handler) Handler

// Chain composes mws around handler so that mws[0] is the outermost
// middleware (runs first on the way in, last on the way out) and handler is
// the innermost call. Each middleware decides whether to call its "next"
// handler; a middleware that does not call next short-circuits the chain,
// preventing downstream middleware and the base handler from running.
//
// Chain(handler, a, b, c) behaves like: a(b(c(handler))).
func Chain(handler Handler, mws ...Middleware) Handler {
	panic("TODO: implement Chain")
}
