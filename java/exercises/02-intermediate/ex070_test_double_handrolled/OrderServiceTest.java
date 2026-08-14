package fewolearning.exercises.intermediate.ex070_test_double_handrolled;

import java.util.ArrayList;
import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class OrderServiceTest {

    @Test
    void placeOrderNotifiesTheNotifierExactlyOnceWithTheOrderId() {
        FakeNotifier notifier = new FakeNotifier();
        OrderService service = new OrderService(notifier);

        service.placeOrder("order-123");

        assertEquals(List.of("order-123"), notifier.notifiedOrderIds);
    }

    @Test
    void placeOrderNotifiesOnceForEachOrder() {
        FakeNotifier notifier = new FakeNotifier();
        OrderService service = new OrderService(notifier);

        service.placeOrder("order-1");
        service.placeOrder("order-2");

        assertEquals(List.of("order-1", "order-2"), notifier.notifiedOrderIds);
    }

    /**
     * Hand-rolled test double for {@link OrderService.Notifier}: records every
     * invocation instead of relying on a mocking library.
     */
    private static final class FakeNotifier implements OrderService.Notifier {
        private final List<String> notifiedOrderIds = new ArrayList<>();

        @Override
        public void notifyOrderPlaced(String orderId) {
            notifiedOrderIds.add(orderId);
        }
    }
}
