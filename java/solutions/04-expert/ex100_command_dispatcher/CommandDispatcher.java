package fewolearning.exercises.expert.ex100_command_dispatcher;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.NoSuchElementException;
import java.util.function.Function;

/*
Exercise 100 - Command dispatcher (reference solution).
*/
public final class CommandDispatcher {
    private final Map<String, Function<List<String>, String>> handlers = new HashMap<>();

    public void register(String commandName, Function<List<String>, String> handler) {
        handlers.put(commandName, handler);
    }

    public String dispatch(String commandName, List<String> arguments) {
        Function<List<String>, String> handler = handlers.get(commandName);
        if (handler == null) {
            throw new NoSuchElementException("No handler registered for command: " + commandName);
        }
        return handler.apply(arguments);
    }
}
