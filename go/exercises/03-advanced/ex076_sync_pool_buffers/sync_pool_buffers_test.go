package syncpoolbuffers

import "testing"

func TestGetReturnsEmptyBuffer(t *testing.T) {
	p := NewBufferPool()
	buf := p.Get()
	if buf == nil {
		t.Fatal("Get() returned nil")
	}
	if buf.Len() != 0 {
		t.Errorf("Get() buffer Len() = %d, want 0", buf.Len())
	}
}

func TestPutResetsContentBeforeReuse(t *testing.T) {
	cases := []string{"leftover data", "another payload", "x"}
	p := NewBufferPool()

	for _, content := range cases {
		buf1 := p.Get()
		buf1.WriteString(content)
		if buf1.Len() == 0 {
			t.Fatalf("setup: expected written content, got empty buffer")
		}
		p.Put(buf1)

		buf2 := p.Get()
		if buf2.Len() != 0 {
			t.Errorf("after Put/Get cycle: Len() = %d, want 0 (content %q leaked)", buf2.Len(), content)
		}
		if got := buf2.String(); got != "" {
			t.Errorf("after Put/Get cycle: String() = %q, want empty (content %q leaked)", got, content)
		}
		p.Put(buf2)
	}
}

func TestPutReusesUnderlyingArray(t *testing.T) {
	p := NewBufferPool()

	buf1 := p.Get()
	buf1.Grow(4096)
	buf1.WriteString("seed")
	wantCap := cap(buf1.Bytes())
	if wantCap < 4096 {
		t.Fatalf("setup: expected grown capacity >= 4096, got %d", wantCap)
	}
	p.Put(buf1)

	buf2 := p.Get()
	if buf2.Len() != 0 {
		t.Fatalf("Get() after Put returned non-empty buffer, Len() = %d", buf2.Len())
	}
	if gotCap := cap(buf2.Bytes()); gotCap < wantCap {
		t.Errorf("expected pooled buffer to retain capacity >= %d, got %d", wantCap, gotCap)
	}
}

func TestPutNilDoesNotPanic(t *testing.T) {
	p := NewBufferPool()
	p.Put(nil)
	buf := p.Get()
	if buf == nil {
		t.Fatal("Get() returned nil after Put(nil)")
	}
	if buf.Len() != 0 {
		t.Errorf("Get() buffer Len() = %d, want 0", buf.Len())
	}
}
