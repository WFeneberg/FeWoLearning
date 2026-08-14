package fewolearning.exercises.advanced.ex083_reflection_method_invoke;

import java.lang.reflect.InvocationTargetException;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class ReflectionMethodInvokeTest {

    @Test
    void invokesAMethodWithArgumentsAndReturnsItsResult() throws Exception {
        Object result = ReflectionMethodInvoke.invoke(new SampleTarget(), "add", 2, 3);

        assertEquals(5, result);
    }

    @Test
    void invokesANoArgumentMethod() throws Exception {
        Object result = ReflectionMethodInvoke.invoke(new SampleTarget(), "greeting");

        assertEquals("hello", result);
    }

    @Test
    void throwsNoSuchMethodExceptionWhenNoMethodMatches() {
        assertThrows(NoSuchMethodException.class,
                () -> ReflectionMethodInvoke.invoke(new SampleTarget(), "missing"));
    }

    @Test
    void wrapsAnExceptionThrownByTheTargetMethodInInvocationTargetException() {
        InvocationTargetException thrown = assertThrows(InvocationTargetException.class,
                () -> ReflectionMethodInvoke.invoke(new SampleTarget(), "explode"));

        assertEquals("boom", thrown.getCause().getMessage());
    }

    public static final class SampleTarget {
        public int add(int left, int right) {
            return left + right;
        }

        public String greeting() {
            return "hello";
        }

        public void explode() {
            throw new IllegalStateException("boom");
        }
    }
}
