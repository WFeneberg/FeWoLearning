package fewolearning.exercises.beginner.ex032_inheritance_override;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class InheritanceOverrideTest {

    @Test
    void baseAnimalDescribesItselfGenerically() {
        InheritanceOverride.Animal animal = new InheritanceOverride.Animal();

        assertEquals("an animal", animal.describe());
    }

    @Test
    void dogExtendsTheBaseDescriptionUsingSuper() {
        InheritanceOverride.Dog dog = new InheritanceOverride.Dog();

        assertEquals("an animal (a dog)", dog.describe());
    }

    @Test
    void dogIsUsableThroughTheAnimalReferenceAndStillOverrides() {
        InheritanceOverride.Animal animal = new InheritanceOverride.Dog();

        assertEquals("an animal (a dog)", animal.describe());
    }
}
