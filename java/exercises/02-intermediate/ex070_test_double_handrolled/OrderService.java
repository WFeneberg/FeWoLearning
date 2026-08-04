package fewolearning.exercises.intermediate.ex070_test_double_handrolled;

/*
Exercise 070 - Hand-rolled test double (intermediate).

Goal:   Implement a notifier-driven order service so it can be exercised with a fake notifier.
Drills: hand-rolled test doubles, interaction checks.
*/
public final class OrderService {
    private final Notifier notifier;

    public OrderService(Notifier notifier) {
        this.notifier = notifier;
    }

    public void placeOrder(String orderId) {
        throw new UnsupportedOperationException("TODO");
    }

    public interface Notifier {
        void notifyOrderPlaced(String orderId);
    }
}
