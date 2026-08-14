//! Exercise 032 — sort_by_key & stability (reference solution).

#[derive(Debug, Clone, PartialEq)]
pub struct Person {
    pub name: String,
    pub age: u32,
}

pub fn sort_by_age(people: &mut Vec<Person>) {
    // `sort_by_key` is a stable sort: equal keys keep their relative order.
    people.sort_by_key(|p| p.age);
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    fn person(name: &str, age: u32) -> Person {
        Person {
            name: name.to_string(),
            age,
        }
    }

    #[test]
    fn sorts_ascending_by_age() {
        let mut people = vec![person("Bob", 30), person("Alice", 25), person("Carol", 40)];
        sort_by_age(&mut people);
        assert_eq!(
            people,
            vec![person("Alice", 25), person("Bob", 30), person("Carol", 40)]
        );
    }

    #[test]
    fn preserves_original_order_for_equal_ages() {
        let mut people = vec![person("Bob", 30), person("Alice", 25), person("Carol", 30)];
        sort_by_age(&mut people);
        // Bob comes before Carol in the input and both are age 30, so a
        // stable sort must keep Bob before Carol in the output too.
        assert_eq!(
            people,
            vec![person("Alice", 25), person("Bob", 30), person("Carol", 30)]
        );
    }
}
