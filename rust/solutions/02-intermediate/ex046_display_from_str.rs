//! Exercise 046 — `Display` and `FromStr` (reference solution).

#[derive(Debug, Clone, PartialEq)]
pub struct Point {
    pub x: i32,
    pub y: i32,
}

impl std::fmt::Display for Point {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "({}, {})", self.x, self.y)
    }
}

impl std::str::FromStr for Point {
    type Err = String;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let inner = s
            .trim()
            .strip_prefix('(')
            .and_then(|rest| rest.strip_suffix(')'))
            .ok_or_else(|| format!("expected \"(x, y)\", got {s:?}"))?;

        let mut parts = inner.split(',').map(str::trim);
        let x = parts
            .next()
            .ok_or_else(|| "missing x coordinate".to_string())?
            .parse::<i32>()
            .map_err(|e| e.to_string())?;
        let y = parts
            .next()
            .ok_or_else(|| "missing y coordinate".to_string())?
            .parse::<i32>()
            .map_err(|e| e.to_string())?;
        if parts.next().is_some() {
            return Err(format!("too many components in {s:?}"));
        }

        Ok(Point { x, y })
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
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
