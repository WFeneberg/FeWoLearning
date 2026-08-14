package fewolearning.exercises.advanced.ex090_hexagonal_port_adapter;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class HexagonalPortAdapterTest {

    @Test
    void checkoutDelegatesToThePaymentPortAndReturnsItsResult() {
        FakePaymentPort port = new FakePaymentPort(true);
        HexagonalPortAdapter.CheckoutService service = new HexagonalPortAdapter.CheckoutService(port);

        boolean result = service.checkout("acc-1", 49.99);

        assertTrue(result);
        assertEquals("acc-1", port.lastAccountId);
        assertEquals(49.99, port.lastAmount, 1e-9);
    }

    @Test
    void checkoutReturnsFalseWhenThePaymentPortDeclinesTheCharge() {
        FakePaymentPort port = new FakePaymentPort(false);
        HexagonalPortAdapter.CheckoutService service = new HexagonalPortAdapter.CheckoutService(port);

        assertFalse(service.checkout("acc-2", 10.0));
    }

    /**
     * Hand-rolled fake for {@link HexagonalPortAdapter.PaymentPort}: records the last
     * charge request and returns a configured result instead of relying on a mocking
     * library.
     */
    private static final class FakePaymentPort implements HexagonalPortAdapter.PaymentPort {
        private final boolean chargeResult;
        private String lastAccountId;
        private double lastAmount;

        FakePaymentPort(boolean chargeResult) {
            this.chargeResult = chargeResult;
        }

        @Override
        public boolean charge(String accountId, double amount) {
            this.lastAccountId = accountId;
            this.lastAmount = amount;
            return chargeResult;
        }
    }
}
