//! Exercise 015 — Match guards & @ bindings (beginner).
//! Goal:   classify a test score, keeping the original number visible in the
//!         message via an `@` binding and `if` guards.
//! Drills: match guards (`if` conditions on arms), binding a pattern to a
//!         name with `@`.

pub fn classify_score(score: i32) -> String {
    todo!("classify_score({score})")
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn a_perfect_score_is_called_out_specially() {
        assert_eq!(classify_score(100), "perfect: 100");
    }

    #[test]
    fn high_scores_are_excellent() {
        assert_eq!(classify_score(95), "excellent: 95");
    }

    #[test]
    fn passing_scores_pass() {
        assert_eq!(classify_score(75), "pass: 75");
    }

    #[test]
    fn low_scores_fail() {
        assert_eq!(classify_score(50), "fail: 50");
    }
}
