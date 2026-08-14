package fewolearning.exercises.intermediate.ex046_variance_in

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ConsumerTest {

    @Test
    fun consumeAddsEachValueToTheReceivedList() {
        val consumer = LoggingConsumer<String>()

        consumer.consume("a")
        consumer.consume("b")

        assertEquals(listOf("a", "b"), consumer.received)
    }

    @Test
    fun inVarianceAllowsAssigningToASubtypeConsumer() {
        val anyConsumer = LoggingConsumer<Any>()
        val intConsumer: Consumer<Int> = anyConsumer

        intConsumer.consume(42)

        assertEquals(listOf<Any?>(42), anyConsumer.received)
    }
}
