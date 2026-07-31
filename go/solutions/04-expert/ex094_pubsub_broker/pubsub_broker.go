// Package pubsubbroker — Exercise 094 (reference solution).
package pubsubbroker

import "sync"

// subscriberBuffer is the per-subscriber channel capacity. It must be large
// enough that Publish (which sends synchronously) never blocks against the
// message volumes exercised by the tests.
const subscriberBuffer = 256

// Broker is a concurrency-safe, in-memory publish/subscribe hub.
type Broker struct {
	mu   sync.Mutex
	subs map[string][]chan string
}

// New creates an empty, ready-to-use Broker.
func New() *Broker {
	return &Broker{subs: make(map[string][]chan string)}
}

// Subscribe registers a new subscriber for topic and returns a channel that
// will receive every message subsequently Published to that exact topic.
func (b *Broker) Subscribe(topic string) <-chan string {
	ch := make(chan string, subscriberBuffer)

	b.mu.Lock()
	b.subs[topic] = append(b.subs[topic], ch)
	b.mu.Unlock()

	return ch
}

// Publish delivers msg to every current subscriber of topic. Subscribers of
// other topics never see it. Publishing to a topic with no subscribers is a
// no-op. Publish is safe for concurrent use by multiple goroutines.
func (b *Broker) Publish(topic string, msg string) {
	b.mu.Lock()
	defer b.mu.Unlock()

	for _, ch := range b.subs[topic] {
		ch <- msg
	}
}
