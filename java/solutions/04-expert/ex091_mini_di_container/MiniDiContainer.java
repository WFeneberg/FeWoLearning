package fewolearning.exercises.expert.ex091_mini_di_container;

import java.lang.reflect.Constructor;
import java.util.HashMap;
import java.util.Map;

/*
Exercise 091 - Mini DI container (reference solution).
*/
public final class MiniDiContainer {
    private final Map<Class<?>, Class<?>> bindings = new HashMap<>();
    private final Map<Class<?>, Object> singletons = new HashMap<>();

    public <T> void register(Class<T> type, Class<? extends T> implementation) {
        bindings.put(type, implementation);
    }

    @SuppressWarnings("unchecked")
    public <T> T resolve(Class<T> type) {
        Object cached = singletons.get(type);
        if (cached != null) {
            return (T) cached;
        }

        Class<?> implementation = bindings.get(type);
        if (implementation == null) {
            throw new IllegalStateException("No binding registered for " + type.getName());
        }

        Constructor<?> constructor = implementation.getConstructors()[0];
        Class<?>[] parameterTypes = constructor.getParameterTypes();
        Object[] resolvedArgs = new Object[parameterTypes.length];
        for (int i = 0; i < parameterTypes.length; i++) {
            resolvedArgs[i] = resolve(parameterTypes[i]);
        }

        try {
            Object instance = constructor.newInstance(resolvedArgs);
            singletons.put(type, instance);
            return (T) instance;
        } catch (ReflectiveOperationException e) {
            throw new IllegalStateException("Unable to construct " + implementation.getName(), e);
        }
    }
}
