// Package pubsubbroker — Exercise 094 (expert).
// Goal:   A concurrency-safe publish/subscribe Broker. Subscribe(topic) hands
//         back a channel that only ever receives messages Published to that
//         exact topic; subscribers to other topics must never see them, and
//         Publish must be safe to call from many goroutines concurrently.
// Drills: mutexes guarding shared maps/slices, channels as delivery
//         mechanism, fan-out to multiple subscribers, goroutine safety.
package pubsubbroker

import "sync"

// subscriberBuffer is the per-subscriber channel capacity. It must be large
// enough that Publish (which sends synchronously) never blocks against the
// message volumes exercised by the tests.
const subscriberBuffer = 256

// Broker is a concurrency-safe, in-memory publish/subscribe hub.
type Broker struct {
	// TODO: add fields (a mutex plus a map from topic to subscriber channels)
}

// New creates an empty, ready-to-use Broker.
func New() *Broker {
	panic("TODO: implement New")
}

// Subscribe registers a new subscriber for topic and returns a channel that
// will receive every message subsequently Published to that exact topic.
// Calling Subscribe multiple times for the same topic yields independent
// channels that each receive every message.
func (b *Broker) Subscribe(topic string) <-chan string {
	panic("TODO: implement Subscribe")
}

// Publish delivers msg to every current subscriber of topic. Subscribers of
// other topics never see it. Publishing to a topic with no subscribers is a
// no-op. Publish must be safe for concurrent use by multiple goroutines.
func (b *Broker) Publish(topic string, msg string) {
	panic("TODO: implement Publish")
}

var _ = sync.Mutex{} // keep the sync import ready for the implementation
