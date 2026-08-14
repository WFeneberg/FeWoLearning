//! Exercise 045 — `PartialEq`/`Eq`/`PartialOrd`/`Ord` semantics (reference solution).

#[derive(Debug, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub struct Version {
    pub major: u32,
    pub minor: u32,
    pub patch: u32,
}

pub fn newest(versions: &[Version]) -> Option<Version> {
    versions.iter().max().cloned()
}

pub fn sort_versions(versions: &mut Vec<Version>) {
    versions.sort();
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    fn v(major: u32, minor: u32, patch: u32) -> Version {
        Version { major, minor, patch }
    }

    #[test]
    fn newest_picks_the_highest_major_version() {
        let versions = vec![v(1, 9, 9), v(2, 0, 0), v(1, 5, 0)];
        assert_eq!(newest(&versions), Some(v(2, 0, 0)));
    }

    #[test]
    fn newest_breaks_ties_on_minor_then_patch() {
        let versions = vec![v(1, 2, 3), v(1, 2, 9), v(1, 2, 5)];
        assert_eq!(newest(&versions), Some(v(1, 2, 9)));
    }

    #[test]
    fn newest_of_no_versions_is_none() {
        let versions: Vec<Version> = vec![];
        assert_eq!(newest(&versions), None);
    }

    #[test]
    fn sort_versions_orders_ascending() {
        let mut versions = vec![v(2, 0, 0), v(1, 0, 0), v(1, 5, 3)];
        sort_versions(&mut versions);
        assert_eq!(versions, vec![v(1, 0, 0), v(1, 5, 3), v(2, 0, 0)]);
    }
}
