//! Exercise 010 — Vec push/pop/insert/remove (beginner).
//! Goal:   starting from an initial `Vec`, push `99` onto the end, insert
//!         `-1` at the front, then pop the last element off and discard it.
//!         Return what remains.
//! Drills: `Vec::push`, `Vec::insert`, `Vec::pop`.

pub fn process_queue(initial: Vec<i32>) -> Vec<i32> {
    todo!("process_queue({initial:?})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn pushes_inserts_and_pops() {
        assert_eq!(process_queue(vec![1, 2, 3]), vec![-1, 1, 2, 3]);
    }

    #[test]
    fn works_on_an_empty_vec() {
        assert_eq!(process_queue(vec![]), vec![-1]);
    }
}
