package httprouterscratch

import (
	"net/http"
	"net/http/httptest"
	"testing"
)

func writeOK(body string) Handler {
	return func(w http.ResponseWriter, r *http.Request, p Params) {
		w.WriteHeader(http.StatusOK)
		_, _ = w.Write([]byte(body))
	}
}

func TestStaticRoute(t *testing.T) {
	rt := New()
	rt.GET("/health", writeOK("ok"))

	rec := httptest.NewRecorder()
	req := httptest.NewRequest(http.MethodGet, "/health", nil)
	rt.ServeHTTP(rec, req)

	if rec.Code != http.StatusOK {
		t.Fatalf("status = %d, want %d", rec.Code, http.StatusOK)
	}
	if rec.Body.String() != "ok" {
		t.Fatalf("body = %q, want %q", rec.Body.String(), "ok")
	}
}

func TestNamedParameter(t *testing.T) {
	rt := New()
	var gotID string
	rt.GET("/users/:id", func(w http.ResponseWriter, r *http.Request, p Params) {
		gotID = p.Get("id")
		w.WriteHeader(http.StatusOK)
		_, _ = w.Write([]byte("user:" + p.Get("id")))
	})

	rec := httptest.NewRecorder()
	req := httptest.NewRequest(http.MethodGet, "/users/42", nil)
	rt.ServeHTTP(rec, req)

	if rec.Code != http.StatusOK {
		t.Fatalf("status = %d, want %d", rec.Code, http.StatusOK)
	}
	if gotID != "42" {
		t.Errorf("captured id = %q, want %q", gotID, "42")
	}
	if rec.Body.String() != "user:42" {
		t.Errorf("body = %q, want %q", rec.Body.String(), "user:42")
	}
}

func TestMultipleNamedParameters(t *testing.T) {
	rt := New()
	var gotUser, gotPost string
	rt.GET("/users/:id/posts/:postID", func(w http.ResponseWriter, r *http.Request, p Params) {
		gotUser = p.Get("id")
		gotPost = p.Get("postID")
		w.WriteHeader(http.StatusOK)
	})

	rec := httptest.NewRecorder()
	req := httptest.NewRequest(http.MethodGet, "/users/7/posts/99", nil)
	rt.ServeHTTP(rec, req)

	if rec.Code != http.StatusOK {
		t.Fatalf("status = %d, want %d", rec.Code, http.StatusOK)
	}
	if gotUser != "7" || gotPost != "99" {
		t.Errorf("captured id=%q postID=%q, want id=7 postID=99", gotUser, gotPost)
	}
}

func TestStaticRouteTakesPriorityOverParam(t *testing.T) {
	rt := New()
	rt.GET("/users/:id", writeOK("param"))
	rt.GET("/users/new", writeOK("static"))

	rec := httptest.NewRecorder()
	req := httptest.NewRequest(http.MethodGet, "/users/new", nil)
	rt.ServeHTTP(rec, req)

	if rec.Body.String() != "static" {
		t.Errorf("body = %q, want %q (static should win over :id)", rec.Body.String(), "static")
	}

	// A different value still falls through to the param route.
	rec2 := httptest.NewRecorder()
	req2 := httptest.NewRequest(http.MethodGet, "/users/123", nil)
	rt.ServeHTTP(rec2, req2)
	if rec2.Body.String() != "param" {
		t.Errorf("body = %q, want %q", rec2.Body.String(), "param")
	}
}

func TestWildcardCapturesRemainder(t *testing.T) {
	rt := New()
	var gotPath string
	rt.GET("/files/*filepath", func(w http.ResponseWriter, r *http.Request, p Params) {
		gotPath = p.Get("filepath")
		w.WriteHeader(http.StatusOK)
	})

	rec := httptest.NewRecorder()
	req := httptest.NewRequest(http.MethodGet, "/files/a/b/c.txt", nil)
	rt.ServeHTTP(rec, req)

	if rec.Code != http.StatusOK {
		t.Fatalf("status = %d, want %d", rec.Code, http.StatusOK)
	}
	if gotPath != "a/b/c.txt" {
		t.Errorf("captured filepath = %q, want %q", gotPath, "a/b/c.txt")
	}
}

func TestMethodIsRespected(t *testing.T) {
	rt := New()
	rt.GET("/users/:id", writeOK("get"))
	rt.POST("/users/:id", writeOK("post"))

	recGet := httptest.NewRecorder()
	rt.ServeHTTP(recGet, httptest.NewRequest(http.MethodGet, "/users/1", nil))
	if recGet.Body.String() != "get" {
		t.Errorf("GET body = %q, want %q", recGet.Body.String(), "get")
	}

	recPost := httptest.NewRecorder()
	rt.ServeHTTP(recPost, httptest.NewRequest(http.MethodPost, "/users/1", nil))
	if recPost.Body.String() != "post" {
		t.Errorf("POST body = %q, want %q", recPost.Body.String(), "post")
	}
}

func TestUnmatchedPathReturns404(t *testing.T) {
	rt := New()
	rt.GET("/users/:id", writeOK("user"))

	rec := httptest.NewRecorder()
	req := httptest.NewRequest(http.MethodGet, "/unknown/path", nil)
	rt.ServeHTTP(rec, req)

	if rec.Code != http.StatusNotFound {
		t.Fatalf("status = %d, want %d", rec.Code, http.StatusNotFound)
	}
	if rec.Body.String() != "404 page not found\n" {
		t.Errorf("body = %q, want default 404 body", rec.Body.String())
	}
}

func TestUnmatchedMethodReturns404(t *testing.T) {
	rt := New()
	rt.GET("/users/:id", writeOK("user"))

	rec := httptest.NewRecorder()
	req := httptest.NewRequest(http.MethodPost, "/users/1", nil)
	rt.ServeHTTP(rec, req)

	if rec.Code != http.StatusNotFound {
		t.Fatalf("status = %d, want %d", rec.Code, http.StatusNotFound)
	}
}

func TestLookupWithoutServingHTTP(t *testing.T) {
	rt := New()
	rt.GET("/repos/:owner/:name", writeOK("repo"))

	h, params, ok := rt.Lookup(http.MethodGet, "/repos/anthropic/claude")
	if !ok {
		t.Fatal("expected a match")
	}
	if h == nil {
		t.Fatal("expected non-nil handler")
	}
	if params.Get("owner") != "anthropic" || params.Get("name") != "claude" {
		t.Errorf("params = %+v, want owner=anthropic name=claude", params)
	}

	if _, _, ok := rt.Lookup(http.MethodGet, "/nope"); ok {
		t.Error("expected no match for unregistered path")
	}
}

func TestDuplicateRegistrationPanics(t *testing.T) {
	rt := New()
	rt.GET("/dup", writeOK("first"))

	defer func() {
		if recover() == nil {
			t.Error("expected panic on duplicate route registration")
		}
	}()
	rt.GET("/dup", writeOK("second"))
}
