//! Exercise 043 — Trait objects and dynamic dispatch (intermediate).
//! Goal:   store heterogeneous implementors of a trait behind `Box<dyn
//!         Trait>`, and build one from a runtime-chosen string.
//! Drills: `Box<dyn Trait>`, object safety, a factory returning a trait
//!         object.

pub trait Animal {
    fn speak(&self) -> String;
}

pub struct Dog;
pub struct Cat;

impl Animal for Dog {
    fn speak(&self) -> String {
        "Woof".to_string()
    }
}

impl Animal for Cat {
    fn speak(&self) -> String {
        "Meow".to_string()
    }
}

/// Calls `speak()` on each animal via dynamic dispatch.
pub fn speak_all(animals: &[Box<dyn Animal>]) -> Vec<String> {
    todo!("speak_all over {} animals", animals.len())
}

/// Builds an animal from a runtime string. Panics on an unknown kind.
pub fn make_animal(kind: &str) -> Box<dyn Animal> {
    todo!("make_animal({kind:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn speaks_across_mixed_trait_objects() {
        let animals: Vec<Box<dyn Animal>> = vec![Box::new(Dog), Box::new(Cat), Box::new(Dog)];
        assert_eq!(speak_all(&animals), vec!["Woof", "Meow", "Woof"]);
    }

    #[test]
    fn speak_all_of_no_animals_is_empty() {
        let animals: Vec<Box<dyn Animal>> = vec![];
        assert!(speak_all(&animals).is_empty());
    }

    #[test]
    fn factory_builds_a_dog() {
        assert_eq!(make_animal("dog").speak(), "Woof");
    }

    #[test]
    fn factory_builds_a_cat() {
        assert_eq!(make_animal("cat").speak(), "Meow");
    }

    #[test]
    #[should_panic(expected = "unknown animal kind")]
    fn factory_panics_on_an_unknown_kind() {
        make_animal("dragon");
    }
}
