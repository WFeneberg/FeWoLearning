package fewolearning.exercises.beginner.ex007_safe_calls_chain

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertNull

class SafeCallsChainTest {

    @Test
    fun cityOfReturnsTheCityWhenEverythingIsPresent() {
        assertEquals("Berlin", cityOf(Customer(Address("Berlin"))))
    }

    @Test
    fun cityOfReturnsNullWhenAddressHasNoCity() {
        assertNull(cityOf(Customer(Address(null))))
    }

    @Test
    fun cityOfReturnsNullWhenCustomerHasNoAddress() {
        assertNull(cityOf(Customer(null)))
    }

    @Test
    fun cityOfReturnsNullWhenCustomerIsNull() {
        assertNull(cityOf(null))
    }
}
