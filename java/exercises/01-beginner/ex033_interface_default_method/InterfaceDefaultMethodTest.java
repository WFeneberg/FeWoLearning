package fewolearning.exercises.beginner.ex033_interface_default_method;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class InterfaceDefaultMethodTest {

    @Test
    void formalGreeterUsesTheDefaultGreeting() {
        InterfaceDefaultMethod.FormalGreeter greeter = new InterfaceDefaultMethod.FormalGreeter("Ann");

        assertEquals("Hello, Ann!", greeter.greet());
    }

    @Test
    void aCustomGreeterCanOverrideTheDefaultMethod() {
        InterfaceDefaultMethod.Greeter greeter = new InterfaceDefaultMethod.Greeter() {
            @Override
            public String name() {
                return "Bo";
            }

            @Override
            public String greet() {
                return "Yo, " + name() + "!";
            }
        };

        assertEquals("Yo, Bo!", greeter.greet());
    }
}
