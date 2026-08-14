package fewolearning.exercises.advanced.ex086_annotation_use_site

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class LegacyBridgeTest {

    @Test
    fun doubledReturnsTwiceTheUnderlyingCount() {
        val bridge = LegacyBridge(21)

        assertEquals(42, bridge.doubled())
    }
}
