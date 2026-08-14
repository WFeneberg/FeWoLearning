package fewolearning.exercises.intermediate.ex045_variance_out

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ProducerTest {

    @Test
    fun produceReturnsTheStoredValue() {
        val producer = ValueProducer(42)

        assertEquals(42, producer.produce())
    }

    @Test
    fun outVarianceAllowsAssigningToASupertypeProducer() {
        val stringProducer: Producer<String> = ValueProducer("hello")
        val anyProducer: Producer<Any> = stringProducer

        assertEquals("hello", anyProducer.produce())
    }
}
