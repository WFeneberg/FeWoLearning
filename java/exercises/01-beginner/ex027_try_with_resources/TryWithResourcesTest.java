package fewolearning.exercises.beginner.ex027_try_with_resources;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class TryWithResourcesTest {

    @Test
    void readAndCloseReturnsTheResourcesData() {
        TryWithResources.TrackedResource resource = new TryWithResources.TrackedResource();

        String data = TryWithResources.readAndClose(resource);

        assertEquals("data", data);
    }

    @Test
    void readAndCloseAlwaysClosesTheResourceAfterwards() {
        TryWithResources.TrackedResource resource = new TryWithResources.TrackedResource();

        TryWithResources.readAndClose(resource);

        assertTrue(resource.isClosed());
    }

    @Test
    void readThrowsAfterTheResourceHasAlreadyBeenClosed() {
        TryWithResources.TrackedResource resource = new TryWithResources.TrackedResource();
        resource.close();

        assertThrows(IllegalStateException.class, resource::read);
    }
}
