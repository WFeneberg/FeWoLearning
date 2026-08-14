package fewolearning.exercises.intermediate.ex070_test_double_handrolled;

/*
Exercise 070 - Hand-rolled test double (reference solution).
*/
public final class OrderService {
    private final Notifier notifier;

    public OrderService(Notifier notifier) {
        this.notifier = notifier;
    }

    public void placeOrder(String orderId) {
        notifier.notifyOrderPlaced(orderId);
    }

    public interface Notifier {
        void notifyOrderPlaced(String orderId);
    }
}
