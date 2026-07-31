package lrucache

import "testing"

func TestEvictsLeastRecentlyUsed(t *testing.T) {
	c := New[string, int](2)
	c.Put("a", 1)
	c.Put("b", 2)
	if _, ok := c.Get("a"); !ok { // touch 'a'
		t.Fatal("expected 'a' present")
	}
	c.Put("c", 3) // evicts 'b'
	if _, ok := c.Get("b"); ok {
		t.Error("expected 'b' evicted")
	}
	if v, ok := c.Get("c"); !ok || v != 3 {
		t.Errorf("Get(c) = %d,%v want 3,true", v, ok)
	}
	if c.Len() != 2 {
		t.Errorf("Len = %d want 2", c.Len())
	}
}

func TestUpdateRefreshesRecency(t *testing.T) {
	c := New[string, int](2)
	c.Put("a", 1)
	c.Put("b", 2)
	c.Put("a", 10) // refresh 'a'
	c.Put("c", 3)  // evicts 'b'
	if _, ok := c.Get("b"); ok {
		t.Error("expected 'b' evicted")
	}
	if v, _ := c.Get("a"); v != 10 {
		t.Errorf("Get(a) = %d want 10", v)
	}
}
