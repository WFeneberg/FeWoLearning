package fewolearning.exercises.intermediate.ex049_data_object_singleton

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertSame

class BasketTest {

    @Test
    fun describeLabelsAnEmptyBasket() {
        assertEquals("empty", describe(Basket.Empty))
    }

    @Test
    fun describeLabelsAFilledBasketWithItsItemCount() {
        assertEquals("filled with 3 items", describe(Basket.Filled(3)))
    }

    @Test
    fun emptyIsASingleSharedInstance() {
        assertSame(Basket.Empty, Basket.Empty)
    }
}
