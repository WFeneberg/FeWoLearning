package fewolearning.exercises.expert.ex094_event_bus;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.function.Consumer;

/*
Exercise 094 - Event bus (expert).

Goal:   Register subscribers per event type and dispatch events to them synchronously.
Drills: publish/subscribe, synchronous dispatch.
*/
public final class EventBus {
    private final Map<Class<?>, List<Consumer<Object>>> subscribers = new HashMap<>();

    public <T> void subscribe(Class<T> eventType, Consumer<T> handler) {
        throw new UnsupportedOperationException("TODO");
    }

    public void publish(Object event) {
        throw new UnsupportedOperationException("TODO");
    }
}
