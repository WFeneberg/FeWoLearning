package fewolearning.exercises.advanced.ex090_hexagonal_port_adapter;

/*
Exercise 090 - Hexagonal port/adapter (reference solution).
*/
public final class HexagonalPortAdapter {
    private HexagonalPortAdapter() {
    }

    public interface PaymentPort {
        boolean charge(String accountId, double amount);
    }

    public static final class CheckoutService {
        private final PaymentPort paymentPort;

        public CheckoutService(PaymentPort paymentPort) {
            this.paymentPort = paymentPort;
        }

        public boolean checkout(String accountId, double amount) {
            return paymentPort.charge(accountId, amount);
        }
    }
}
