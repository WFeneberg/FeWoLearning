package fewolearning.exercises.advanced.ex083_reflection_method_invoke;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

/*
Exercise 083 - Reflection method invoke (reference solution).
*/
public final class ReflectionMethodInvoke {
    private ReflectionMethodInvoke() {
    }

    public static Object invoke(Object target, String methodName, Object... args)
            throws NoSuchMethodException, InvocationTargetException, IllegalAccessException {
        for (Method method : target.getClass().getMethods()) {
            if (method.getName().equals(methodName) && method.getParameterCount() == args.length) {
                try {
                    return method.invoke(target, args);
                } catch (IllegalArgumentException ignored) {
                    // Parameter types of this overload did not accept the supplied
                    // arguments; keep looking at the remaining candidates.
                }
            }
        }
        throw new NoSuchMethodException(methodName);
    }
}
