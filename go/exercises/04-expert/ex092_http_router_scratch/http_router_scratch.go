// Package httprouterscratch — Exercise 092 (expert).
// Goal:   An HTTP request router built from scratch (no net/http.ServeMux),
//         supporting static segments, named parameters ("/users/:id") and a
//         single trailing wildcard ("/files/*filepath"), with static routes
//         taking priority over named-parameter routes at the same depth.
// Drills: tries/trees, string parsing, http.Handler, generics-free interfaces.
package httprouterscratch

import "net/http"

// Params holds the named path parameters extracted while matching a route.
type Params map[string]string

// Get returns the value for key, or "" if it was not captured.
func (p Params) Get(key string) string {
	return p[key]
}

// Handler is invoked for a matched route with the captured Params.
type Handler func(w http.ResponseWriter, r *http.Request, p Params)

// Router dispatches requests to registered Handlers based on method and path.
type Router struct {
	// NotFound is invoked when no route matches. If nil, ServeHTTP writes a
	// default 404 response.
	NotFound http.Handler
}

// New creates an empty Router ready for route registration.
func New() *Router {
	panic("TODO: implement New")
}

// Handle registers h for the given HTTP method and path pattern.
// Pattern segments starting with ':' capture a named parameter
// (e.g. "/users/:id"). A segment starting with '*' must be the last
// segment and captures the remainder of the path (e.g. "/files/*filepath").
// Handle panics if the same method+pattern is registered twice.
func (rt *Router) Handle(method, pattern string, h Handler) {
	panic("TODO: implement Handle")
}

// GET is shorthand for Handle(http.MethodGet, pattern, h).
func (rt *Router) GET(pattern string, h Handler) {
	panic("TODO: implement GET")
}

// POST is shorthand for Handle(http.MethodPost, pattern, h).
func (rt *Router) POST(pattern string, h Handler) {
	panic("TODO: implement POST")
}

// Lookup finds the Handler registered for method and path, returning the
// captured Params and whether a match was found. It performs no I/O.
func (rt *Router) Lookup(method, path string) (Handler, Params, bool) {
	panic("TODO: implement Lookup")
}

// ServeHTTP implements http.Handler, dispatching to the matched route's
// Handler or to NotFound (default: 404 status, "404 page not found\n" body).
func (rt *Router) ServeHTTP(w http.ResponseWriter, r *http.Request) {
	panic("TODO: implement ServeHTTP")
}
