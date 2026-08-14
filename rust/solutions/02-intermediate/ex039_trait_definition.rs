//! Exercise 039 — Defining a trait with a default method (reference solution).

pub trait Greet {
    fn name(&self) -> String;

    fn greeting(&self) -> String {
        format!("Hello, {}!", self.name())
    }
}

pub struct Person {
    pub name: String,
}

impl Greet for Person {
    fn name(&self) -> String {
        self.name.clone()
    }
}

pub struct Robot {
    pub id: u32,
}

impl Greet for Robot {
    fn name(&self) -> String {
        format!("Unit-{}", self.id)
    }

    fn greeting(&self) -> String {
        format!("BEEP BOOP, {}", self.name())
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn person_uses_the_default_greeting() {
        let ada = Person { name: "Ada".to_string() };
        assert_eq!(ada.greeting(), "Hello, Ada!");
    }

    #[test]
    fn another_person_also_uses_the_default() {
        let grace = Person { name: "Grace".to_string() };
        assert_eq!(grace.greeting(), "Hello, Grace!");
    }

    #[test]
    fn robot_overrides_the_default_greeting() {
        // A Person still needs the default greeting to be implemented.
        let ada = Person { name: "Ada".to_string() };
        assert_eq!(ada.greeting(), "Hello, Ada!");

        let unit = Robot { id: 7 };
        assert_eq!(unit.greeting(), "BEEP BOOP, Unit-7");
    }

    #[test]
    fn name_is_still_reachable_after_override() {
        // A Person still needs the default greeting to be implemented.
        let ada = Person { name: "Ada".to_string() };
        assert_eq!(ada.greeting(), "Hello, Ada!");

        let unit = Robot { id: 42 };
        assert_eq!(unit.name(), "Unit-42");
    }
}
