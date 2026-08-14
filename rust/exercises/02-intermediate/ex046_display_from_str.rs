//! Exercise 046 — `Display` and `FromStr` (intermediate).
//! Goal:   render a type as text and parse it back, mirroring how
//!         `to_string()`/`.parse()` work for built-in types.
//! Drills: `impl std::fmt::Display`, `impl std::str::FromStr`, round-tripping
//!         through text.

#[derive(Debug, Clone, PartialEq)]
pub struct Point {
    pub x: i32,
    pub y: i32,
}

impl std::fmt::Display for Point {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        todo!("Display for {self:?}")
    }
}

impl std::str::FromStr for Point {
    type Err = String;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        todo!("FromStr for {s:?}")
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn displays_as_a_coordinate_pair() {
        let p = Point { x: 3, y: 4 };
        assert_eq!(p.to_string(), "(3, 4)");
    }

    #[test]
    fn displays_negative_coordinates() {
        let p = Point { x: -1, y: -2 };
        assert_eq!(p.to_string(), "(-1, -2)");
    }

    #[test]
    fn parses_a_coordinate_pair() {
        let p: Point = "(3, 4)".parse().unwrap();
        assert_eq!(p, Point { x: 3, y: 4 });
    }

    #[test]
    fn rejects_malformed_input() {
        assert!("(3, )".parse::<Point>().is_err());
        assert!("nonsense".parse::<Point>().is_err());
    }

    #[test]
    fn round_trips_through_text() {
        let original = Point { x: 7, y: -9 };
        let parsed: Point = original.to_string().parse().unwrap();
        assert_eq!(parsed, original);
    }
}
