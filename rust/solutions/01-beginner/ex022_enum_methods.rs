//! Exercise 022 — Methods on an enum (reference solution).

#[derive(Debug, PartialEq, Clone, Copy)]
pub enum TrafficLight {
    Red,
    Green,
    Yellow,
}

impl TrafficLight {
    pub fn next(&self) -> Self {
        match self {
            TrafficLight::Red => TrafficLight::Green,
            TrafficLight::Green => TrafficLight::Yellow,
            TrafficLight::Yellow => TrafficLight::Red,
        }
    }

    pub fn duration_secs(&self) -> u32 {
        match self {
            TrafficLight::Red => 30,
            TrafficLight::Green => 25,
            TrafficLight::Yellow => 5,
        }
    }
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn cycles_red_green_yellow_red() {
        assert_eq!(TrafficLight::Red.next(), TrafficLight::Green);
        assert_eq!(TrafficLight::Green.next(), TrafficLight::Yellow);
        assert_eq!(TrafficLight::Yellow.next(), TrafficLight::Red);
    }

    #[test]
    fn reports_a_duration_per_state() {
        assert_eq!(TrafficLight::Red.duration_secs(), 30);
        assert_eq!(TrafficLight::Green.duration_secs(), 25);
        assert_eq!(TrafficLight::Yellow.duration_secs(), 5);
    }
}
