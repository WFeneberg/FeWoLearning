package fewolearning.exercises.expert.ex100_command_dispatcher;

import java.util.List;
import java.util.NoSuchElementException;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class CommandDispatcherTest {

    @Test
    void dispatchesToTheHandlerRegisteredForTheCommandName() {
        CommandDispatcher dispatcher = new CommandDispatcher();
        dispatcher.register("greet", arguments -> "Hello, " + arguments.get(0) + "!");

        assertEquals("Hello, Alice!", dispatcher.dispatch("greet", List.of("Alice")));
    }

    @Test
    void differentCommandsRouteToTheirOwnHandlerIndependently() {
        CommandDispatcher dispatcher = new CommandDispatcher();
        dispatcher.register("sum", arguments -> String.valueOf(
                arguments.stream().mapToInt(Integer::parseInt).sum()));
        dispatcher.register("echo", arguments -> String.join(" ", arguments));

        assertEquals("6", dispatcher.dispatch("sum", List.of("1", "2", "3")));
        assertEquals("a b", dispatcher.dispatch("echo", List.of("a", "b")));
    }

    @Test
    void dispatchingAnUnregisteredCommandThrows() {
        CommandDispatcher dispatcher = new CommandDispatcher();

        assertThrows(NoSuchElementException.class, () -> dispatcher.dispatch("missing", List.of()));
    }
}
