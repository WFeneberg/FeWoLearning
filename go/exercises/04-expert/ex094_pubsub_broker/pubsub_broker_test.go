package pubsubbroker

import (
	"sync"
	"testing"
)

// drainCount reads every currently-buffered message off ch without blocking
// and returns how many there were.
func drainCount(ch <-chan string) int {
	n := 0
	for {
		select {
		case <-ch:
			n++
		default:
			return n
		}
	}
}

func TestSubscriberReceivesOnlyOwnTopic(t *testing.T) {
	b := New()
	chA := b.Subscribe("weather.A")
	chB := b.Subscribe("weather.B")

	b.Publish("weather.A", "a1")
	b.Publish("weather.B", "b1")
	b.Publish("weather.A", "a2")
	b.Publish("weather.B", "b2")
	b.Publish("weather.A", "a3")

	wantA := []string{"a1", "a2", "a3"}
	for i, want := range wantA {
		select {
		case got := <-chA:
			if got != want {
				t.Errorf("chA message %d = %q, want %q", i, got, want)
			}
		default:
			t.Fatalf("chA missing message %d (%q)", i, want)
		}
	}
	select {
	case msg := <-chA:
		t.Errorf("chA received unexpected extra message %q (topic B leaked?)", msg)
	default:
	}

	wantB := []string{"b1", "b2"}
	for i, want := range wantB {
		select {
		case got := <-chB:
			if got != want {
				t.Errorf("chB message %d = %q, want %q", i, got, want)
			}
		default:
			t.Fatalf("chB missing message %d (%q)", i, want)
		}
	}
	select {
	case msg := <-chB:
		t.Errorf("chB received unexpected extra message %q (topic A leaked?)", msg)
	default:
	}
}

func TestMultipleSubscribersToSameTopicAllReceive(t *testing.T) {
	b := New()
	ch1 := b.Subscribe("news")
	ch2 := b.Subscribe("news")
	other := b.Subscribe("sports")

	b.Publish("news", "headline")

	for name, ch := range map[string]<-chan string{"ch1": ch1, "ch2": ch2} {
		select {
		case got := <-ch:
			if got != "headline" {
				t.Errorf("%s = %q, want %q", name, got, "headline")
			}
		default:
			t.Fatalf("%s never received the published message", name)
		}
	}
	select {
	case msg := <-other:
		t.Errorf("sports subscriber received unexpected message %q", msg)
	default:
	}
}

func TestPublishWithNoSubscribersIsANoOp(t *testing.T) {
	b := New()
	// Must not panic, block, or otherwise misbehave when nobody is listening.
	b.Publish("void", "nobody home")
}

func TestConcurrentPublishStaysIsolatedByTopic(t *testing.T) {
	b := New()
	chA := b.Subscribe("A")
	chB := b.Subscribe("B")

	const perTopic = 50
	var wg sync.WaitGroup
	wg.Add(2)
	go func() {
		defer wg.Done()
		for i := 0; i < perTopic; i++ {
			b.Publish("A", "a")
		}
	}()
	go func() {
		defer wg.Done()
		for i := 0; i < perTopic; i++ {
			b.Publish("B", "b")
		}
	}()
	wg.Wait()

	if got := drainCount(chA); got != perTopic {
		t.Errorf("topic A delivered %d messages, want %d", got, perTopic)
	}
	if got := drainCount(chB); got != perTopic {
		t.Errorf("topic B delivered %d messages, want %d", got, perTopic)
	}
}
