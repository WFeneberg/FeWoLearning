//! Exercise 015 — Match guards & @ bindings (reference solution).

pub fn classify_score(score: i32) -> String {
    match score {
        s @ 100 => format!("perfect: {s}"),
        s if s >= 90 => format!("excellent: {s}"),
        s if s >= 70 => format!("pass: {s}"),
        s => format!("fail: {s}"),
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
