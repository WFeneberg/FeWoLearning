package fewolearning.exercises.beginner.ex010_traffic_light_enum;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class TrafficLightTest {

    @Test
    void nextCyclesRedToGreenToYellowToRed() {
        assertEquals(TrafficLight.GREEN, TrafficLight.RED.next());
        assertEquals(TrafficLight.YELLOW, TrafficLight.GREEN.next());
        assertEquals(TrafficLight.RED, TrafficLight.YELLOW.next());
    }

    @Test
    void canCarsGoIsTrueOnlyForGreen() {
        assertTrue(TrafficLight.GREEN.canCarsGo());
        assertFalse(TrafficLight.RED.canCarsGo());
        assertFalse(TrafficLight.YELLOW.canCarsGo());
    }
}
