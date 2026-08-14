//! Exercise 010 — Vec push/pop/insert/remove (reference solution).

pub fn process_queue(initial: Vec<i32>) -> Vec<i32> {
    let mut v = initial;
    v.push(99);
    v.insert(0, -1);
    v.pop();
    v
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
