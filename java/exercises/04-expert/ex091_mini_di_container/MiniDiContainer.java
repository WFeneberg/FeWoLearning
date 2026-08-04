package fewolearning.exercises.expert.ex091_mini_di_container;

import java.util.HashMap;
import java.util.Map;

/*
Exercise 091 - Mini DI container (expert).

Goal:   Resolve and construct registered types by wiring their constructor dependencies.
Drills: reflection, constructor wiring, scopes.
*/
public final class MiniDiContainer {
    private final Map<Class<?>, Class<?>> bindings = new HashMap<>();
    private final Map<Class<?>, Object> singletons = new HashMap<>();

    public <T> void register(Class<T> type, Class<? extends T> implementation) {
        throw new UnsupportedOperationException("TODO");
    }

    public <T> T resolve(Class<T> type) {
        throw new UnsupportedOperationException("TODO");
    }
}
