// Package httprouterscratch — Exercise 092 (reference solution).
package httprouterscratch

import (
	"net/http"
	"strings"
)

// Params holds the named path parameters extracted while matching a route.
type Params map[string]string

// Get returns the value for key, or "" if it was not captured.
func (p Params) Get(key string) string {
	return p[key]
}

// Handler is invoked for a matched route with the captured Params.
type Handler func(w http.ResponseWriter, r *http.Request, p Params)

// node is one segment level of the routing trie for a single HTTP method.
type node struct {
	static       map[string]*node // literal segment -> child
	param        *node            // ":name" child
	paramName    string
	wildcard     *node // "*name" child (must be leaf)
	wildcardName string
	handler      Handler
	hasHandler   bool
}

func newNode() *node {
	return &node{static: make(map[string]*node)}
}

// Router dispatches requests to registered Handlers based on method and path.
type Router struct {
	NotFound http.Handler
	roots    map[string]*node
}

// New creates an empty Router ready for route registration.
func New() *Router {
	return &Router{roots: make(map[string]*node)}
}

func splitPath(path string) []string {
	trimmed := strings.Trim(path, "/")
	if trimmed == "" {
		return nil
	}
	return strings.Split(trimmed, "/")
}

// Handle registers h for the given HTTP method and path pattern.
func (rt *Router) Handle(method, pattern string, h Handler) {
	if rt.roots == nil {
		rt.roots = make(map[string]*node)
	}
	root, ok := rt.roots[method]
	if !ok {
		root = newNode()
		rt.roots[method] = root
	}

	segments := splitPath(pattern)
	cur := root
	for i, seg := range segments {
		switch {
		case strings.HasPrefix(seg, "*"):
			name := seg[1:]
			if i != len(segments)-1 {
				panic("httprouterscratch: wildcard must be the last segment")
			}
			if cur.wildcard == nil {
				cur.wildcard = newNode()
			}
			cur.wildcard.wildcardName = name
			cur = cur.wildcard
		case strings.HasPrefix(seg, ":"):
			name := seg[1:]
			if cur.param == nil {
				cur.param = newNode()
			}
			cur.param.paramName = name
			cur = cur.param
		default:
			child, ok := cur.static[seg]
			if !ok {
				child = newNode()
				cur.static[seg] = child
			}
			cur = child
		}
	}

	if cur.hasHandler {
		panic("httprouterscratch: route already registered: " + method + " " + pattern)
	}
	cur.handler = h
	cur.hasHandler = true
}

// GET is shorthand for Handle(http.MethodGet, pattern, h).
func (rt *Router) GET(pattern string, h Handler) { rt.Handle(http.MethodGet, pattern, h) }

// POST is shorthand for Handle(http.MethodPost, pattern, h).
func (rt *Router) POST(pattern string, h Handler) { rt.Handle(http.MethodPost, pattern, h) }

// Lookup finds the Handler registered for method and path, returning the
// captured Params and whether a match was found.
func (rt *Router) Lookup(method, path string) (Handler, Params, bool) {
	root, ok := rt.roots[method]
	if !ok {
		return nil, nil, false
	}
	segments := splitPath(path)
	params := Params{}
	h, ok := match(root, segments, params)
	if !ok {
		return nil, nil, false
	}
	return h, params, true
}

// match walks the trie for the remaining segments, preferring static matches
// over named-parameter matches, and falling back to a wildcard capture.
func match(n *node, segments []string, params Params) (Handler, bool) {
	if len(segments) == 0 {
		if n.hasHandler {
			return n.handler, true
		}
		return nil, false
	}

	seg, rest := segments[0], segments[1:]

	if child, ok := n.static[seg]; ok {
		if h, ok := match(child, rest, params); ok {
			return h, true
		}
	}

	if n.param != nil {
		params[n.param.paramName] = seg
		if h, ok := match(n.param, rest, params); ok {
			return h, true
		}
		delete(params, n.param.paramName)
	}

	if n.wildcard != nil && n.wildcard.hasHandler {
		params[n.wildcard.wildcardName] = strings.Join(segments, "/")
		return n.wildcard.handler, true
	}

	return nil, false
}

// ServeHTTP implements http.Handler.
func (rt *Router) ServeHTTP(w http.ResponseWriter, r *http.Request) {
	h, params, ok := rt.Lookup(r.Method, r.URL.Path)
	if !ok {
		if rt.NotFound != nil {
			rt.NotFound.ServeHTTP(w, r)
			return
		}
		w.WriteHeader(http.StatusNotFound)
		_, _ = w.Write([]byte("404 page not found\n"))
		return
	}
	h(w, r, params)
}
