package fewolearning.exercises.advanced.ex084_service_loader_plugin;

import org.junit.jupiter.api.Test;

import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ServiceLoaderPluginTest {

    @Test
    void greetAllReturnsGreetingsFromEveryRegisteredImplementation() {
        List<String> greetings = ServiceLoaderPlugin.greetAll();

        assertEquals(2, greetings.size());
        assertTrue(greetings.contains("Hello!"));
        assertTrue(greetings.contains("Bonjour!"));
    }
}
