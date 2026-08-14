//! Exercise 043 — Trait objects and dynamic dispatch (reference solution).

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

pub fn speak_all(animals: &[Box<dyn Animal>]) -> Vec<String> {
    animals.iter().map(|a| a.speak()).collect()
}

pub fn make_animal(kind: &str) -> Box<dyn Animal> {
    match kind {
        "dog" => Box::new(Dog),
        "cat" => Box::new(Cat),
        other => panic!("unknown animal kind: {other}"),
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
