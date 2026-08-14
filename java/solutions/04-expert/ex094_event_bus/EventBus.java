package fewolearning.exercises.expert.ex094_event_bus;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.function.Consumer;

/*
Exercise 094 - Event bus (reference solution).
*/
public final class EventBus {
    private final Map<Class<?>, List<Consumer<Object>>> subscribers = new HashMap<>();

    @SuppressWarnings("unchecked")
    public <T> void subscribe(Class<T> eventType, Consumer<T> handler) {
        // Consumer<T> is not a subtype of Consumer<Object>, even though T extends
        // Object, so an explicit unchecked cast is required to erase the type here.
        Consumer<Object> erased = (Consumer<Object>) handler;
        subscribers.computeIfAbsent(eventType, key -> new ArrayList<>()).add(erased);
    }

    public void publish(Object event) {
        List<Consumer<Object>> handlers = subscribers.get(event.getClass());
        if (handlers == null) {
            return;
        }
        for (Consumer<Object> handler : handlers) {
            handler.accept(event);
        }
    }
}
