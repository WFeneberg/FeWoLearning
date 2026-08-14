package fewolearning.exercises.expert.ex091_mini_di_container;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertSame;

class MiniDiContainerTest {

    public interface ServiceC {
        String describe();
    }

    public static final class ServiceCImpl implements ServiceC {
        public ServiceCImpl() {
        }

        @Override
        public String describe() {
            return "C";
        }
    }

    public interface ServiceB {
        String describe();
    }

    public static final class ServiceBImpl implements ServiceB {
        private final ServiceC serviceC;

        public ServiceBImpl(ServiceC serviceC) {
            this.serviceC = serviceC;
        }

        @Override
        public String describe() {
            return "B->" + serviceC.describe();
        }
    }

    public interface ServiceA {
        String describe();
    }

    public static final class ServiceAImpl implements ServiceA {
        private final ServiceB serviceB;

        public ServiceAImpl(ServiceB serviceB) {
            this.serviceB = serviceB;
        }

        @Override
        public String describe() {
            return "A->" + serviceB.describe();
        }
    }

    @Test
    void resolvesARecursiveDependencyChain() {
        MiniDiContainer container = new MiniDiContainer();
        container.register(ServiceC.class, ServiceCImpl.class);
        container.register(ServiceB.class, ServiceBImpl.class);
        container.register(ServiceA.class, ServiceAImpl.class);

        ServiceA serviceA = container.resolve(ServiceA.class);

        assertNotNull(serviceA);
        assertEquals("A->B->C", serviceA.describe());
    }

    @Test
    void resolveReturnsTheSameCachedSingletonInstanceOnEveryCall() {
        MiniDiContainer container = new MiniDiContainer();
        container.register(ServiceC.class, ServiceCImpl.class);

        ServiceC first = container.resolve(ServiceC.class);
        ServiceC second = container.resolve(ServiceC.class);

        assertSame(first, second);
    }

    @Test
    void dependenciesSharedAcrossResolutionsAreTheSameCachedSingletonInstance() {
        MiniDiContainer container = new MiniDiContainer();
        container.register(ServiceC.class, ServiceCImpl.class);
        container.register(ServiceB.class, ServiceBImpl.class);
        container.register(ServiceA.class, ServiceAImpl.class);

        container.resolve(ServiceA.class);
        ServiceC directC = container.resolve(ServiceC.class);
        ServiceB serviceB = container.resolve(ServiceB.class);

        assertSame(directC, ((ServiceBImpl) serviceB).serviceC);
    }
}
