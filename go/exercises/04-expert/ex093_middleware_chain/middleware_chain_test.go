package middlewarechain

import (
	"reflect"
	"testing"
)

// tracingMiddleware returns a Middleware that appends "<name>:before" to log
// before calling next, and "<name>:after" after next returns. name is also
// used to identify which middleware ran in what order.
func tracingMiddleware(log *[]string, name string) Middleware {
	return func(next Handler) Handler {
		return func(req *Request) *Response {
			*log = append(*log, name+":before")
			resp := next(req)
			*log = append(*log, name+":after")
			return resp
		}
	}
}

// authMiddleware short-circuits the chain with a 401 Response whenever the
// request's AuthToken does not equal want, never invoking next.
func authMiddleware(log *[]string, want string) Middleware {
	return func(next Handler) Handler {
		return func(req *Request) *Response {
			if req.AuthToken != want {
				*log = append(*log, "auth:denied")
				return &Response{Status: 401, Body: "unauthorized"}
			}
			*log = append(*log, "auth:allowed")
			return next(req)
		}
	}
}

func TestChainOrdersMiddlewareOutsideIn(t *testing.T) {
	var log []string
	base := Handler(func(req *Request) *Response {
		log = append(log, "handler")
		return &Response{Status: 200, Body: "ok:" + req.Path}
	})

	h := Chain(base,
		tracingMiddleware(&log, "outer"),
		tracingMiddleware(&log, "middle"),
		tracingMiddleware(&log, "inner"),
	)

	resp := h(&Request{Path: "/widgets"})

	if resp == nil || resp.Status != 200 || resp.Body != "ok:/widgets" {
		t.Fatalf("h(req) = %+v, want Status=200 Body=%q", resp, "ok:/widgets")
	}

	want := []string{
		"outer:before", "middle:before", "inner:before",
		"handler",
		"inner:after", "middle:after", "outer:after",
	}
	if !reflect.DeepEqual(log, want) {
		t.Errorf("execution order = %v, want %v", log, want)
	}
}

func TestChainAuthMiddlewareShortCircuits(t *testing.T) {
	var log []string
	handlerCalled := false
	base := Handler(func(req *Request) *Response {
		handlerCalled = true
		return &Response{Status: 200, Body: "ok"}
	})

	h := Chain(base,
		tracingMiddleware(&log, "outer"),
		authMiddleware(&log, "secret-token"),
		tracingMiddleware(&log, "inner"),
	)

	resp := h(&Request{Path: "/admin", AuthToken: "wrong-token"})

	if resp == nil || resp.Status != 401 || resp.Body != "unauthorized" {
		t.Fatalf("h(req) = %+v, want Status=401 Body=%q", resp, "unauthorized")
	}
	if handlerCalled {
		t.Error("base handler was called despite failed auth")
	}

	want := []string{"outer:before", "auth:denied", "outer:after"}
	if !reflect.DeepEqual(log, want) {
		t.Errorf("execution order = %v, want %v", log, want)
	}
}

func TestChainAuthMiddlewareAllowsValidToken(t *testing.T) {
	var log []string
	h := Chain(
		Handler(func(req *Request) *Response {
			log = append(log, "handler")
			return &Response{Status: 200, Body: "ok"}
		}),
		authMiddleware(&log, "secret-token"),
	)

	resp := h(&Request{Path: "/admin", AuthToken: "secret-token"})

	if resp == nil || resp.Status != 200 {
		t.Fatalf("h(req) = %+v, want Status=200", resp)
	}
	want := []string{"auth:allowed", "handler"}
	if !reflect.DeepEqual(log, want) {
		t.Errorf("execution order = %v, want %v", log, want)
	}
}

func TestChainWithNoMiddlewareCallsHandlerDirectly(t *testing.T) {
	h := Chain(Handler(func(req *Request) *Response {
		return &Response{Status: 204, Body: ""}
	}))
	resp := h(&Request{Path: "/ping"})
	if resp == nil || resp.Status != 204 {
		t.Fatalf("h(req) = %+v, want Status=204", resp)
	}
}
