// Package middlewarechain — Exercise 093 (reference solution).
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

// Middleware wraps a Handler to produce a new Handler.
type Middleware func(next Handler) Handler

// Chain composes mws around handler so that mws[0] is the outermost
// middleware and handler is the innermost call: Chain(handler, a, b, c)
// behaves like a(b(c(handler))).
func Chain(handler Handler, mws ...Middleware) Handler {
	h := handler
	for i := len(mws) - 1; i >= 0; i-- {
		h = mws[i](h)
	}
	return h
}
