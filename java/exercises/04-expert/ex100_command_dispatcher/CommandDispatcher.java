package fewolearning.exercises.expert.ex100_command_dispatcher;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.function.Function;

/*
Exercise 100 - Command dispatcher (expert).

Goal:   Route named commands with arguments to their registered handlers.
Drills: command routing, extensibility.
*/
public final class CommandDispatcher {
    private final Map<String, Function<List<String>, String>> handlers = new HashMap<>();

    public void register(String commandName, Function<List<String>, String> handler) {
        throw new UnsupportedOperationException("TODO");
    }

    public String dispatch(String commandName, List<String> arguments) {
        throw new UnsupportedOperationException("TODO");
    }
}
