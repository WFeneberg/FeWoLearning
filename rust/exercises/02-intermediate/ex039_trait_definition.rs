//! Exercise 039 — Defining a trait with a default method (intermediate).
//! Goal:   a trait whose default method is expressed in terms of its own
//!         required method, and a type that relies on that default.
//! Drills: `trait` declarations, required vs. default methods, `Self`.

pub trait Greet {
    /// Every implementor must supply its own name.
    fn name(&self) -> String;

    /// Default greeting, built purely from `name()`. Implementors may
    /// override this, but most won't need to.
    fn greeting(&self) -> String {
        todo!("default greeting for {}", self.name())
    }
}

pub struct Person {
    pub name: String,
}

impl Greet for Person {
    fn name(&self) -> String {
        self.name.clone()
    }
    // Uses the default `greeting()` — no override needed.
}

pub struct Robot {
    pub id: u32,
}

impl Greet for Robot {
    fn name(&self) -> String {
        format!("Unit-{}", self.id)
    }

    // Robots override the default entirely.
    fn greeting(&self) -> String {
        format!("BEEP BOOP, {}", self.name())
    }
}

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
        let unit = Robot { id: 7 };
        assert_eq!(unit.greeting(), "BEEP BOOP, Unit-7");
    }

    #[test]
    fn name_is_still_reachable_after_override() {
        let unit = Robot { id: 42 };
        assert_eq!(unit.name(), "Unit-42");
    }
}
