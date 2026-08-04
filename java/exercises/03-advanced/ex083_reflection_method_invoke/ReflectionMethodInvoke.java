package fewolearning.exercises.advanced.ex083_reflection_method_invoke;

import java.lang.reflect.InvocationTargetException;

/*
Exercise 083 - Reflection method invoke (advanced).

Goal:   Invoke a named method on an object via reflection, unwrapping its result.
Drills: reflection, invocation, accessibility.
*/
public final class ReflectionMethodInvoke {
    private ReflectionMethodInvoke() {
    }

    public static Object invoke(Object target, String methodName, Object... args)
            throws NoSuchMethodException, InvocationTargetException, IllegalAccessException {
        throw new UnsupportedOperationException("TODO");
    }
}
