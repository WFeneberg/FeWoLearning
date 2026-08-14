package fewolearning.exercises.expert.ex094_event_bus;

import java.util.ArrayList;
import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class EventBusTest {

    record OrderPlaced(String orderId) {
    }

    record UserRegistered(String username) {
    }

    @Test
    void dispatchesPublishedEventsToAllSubscribersOfTheMatchingType() {
        EventBus bus = new EventBus();
        List<String> received = new ArrayList<>();
        bus.subscribe(OrderPlaced.class, event -> received.add("first:" + event.orderId()));
        bus.subscribe(OrderPlaced.class, event -> received.add("second:" + event.orderId()));

        bus.publish(new OrderPlaced("order-1"));

        assertEquals(List.of("first:order-1", "second:order-1"), received);
    }

    @Test
    void onlySubscribersOfTheMatchingEventTypeAreInvoked() {
        EventBus bus = new EventBus();
        List<String> orderEvents = new ArrayList<>();
        List<String> userEvents = new ArrayList<>();
        bus.subscribe(OrderPlaced.class, event -> orderEvents.add(event.orderId()));
        bus.subscribe(UserRegistered.class, event -> userEvents.add(event.username()));

        bus.publish(new UserRegistered("alice"));

        assertEquals(List.of(), orderEvents);
        assertEquals(List.of("alice"), userEvents);
    }

    @Test
    void publishingAnEventWithNoSubscribersDoesNotThrow() {
        EventBus bus = new EventBus();

        bus.publish(new OrderPlaced("order-1"));
    }
}
