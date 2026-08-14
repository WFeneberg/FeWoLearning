//! Exercise 066 — `matches!` for terse predicate matching (reference solution).

pub enum Status {
    Active,
    Inactive,
    Pending(u32),
}

pub fn is_active(status: &Status) -> bool {
    matches!(status, Status::Active)
}

pub fn is_pending_over(status: &Status, threshold: u32) -> bool {
    matches!(status, Status::Pending(n) if *n > threshold)
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn active_is_active() {
        assert!(is_active(&Status::Active));
    }

    #[test]
    fn inactive_is_not_active() {
        assert!(!is_active(&Status::Inactive));
    }

    #[test]
    fn pending_is_not_active() {
        assert!(!is_active(&Status::Pending(5)));
    }

    #[test]
    fn pending_over_threshold_is_true() {
        assert!(is_pending_over(&Status::Pending(10), 5));
    }

    #[test]
    fn pending_at_or_under_threshold_is_false() {
        assert!(!is_pending_over(&Status::Pending(5), 5));
        assert!(!is_pending_over(&Status::Pending(3), 5));
    }

    #[test]
    fn non_pending_is_never_over_threshold() {
        assert!(!is_pending_over(&Status::Active, 0));
        assert!(!is_pending_over(&Status::Inactive, 0));
    }
}
