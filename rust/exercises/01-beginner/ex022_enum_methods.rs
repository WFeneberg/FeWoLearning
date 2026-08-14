//! Exercise 022 — Methods on an enum (beginner).
//! Goal:   cycle a traffic light through its states and report how long
//!         each state lasts.
//! Drills: `impl` blocks on an `enum`, returning `Self`.

#[derive(Debug, PartialEq, Clone, Copy)]
pub enum TrafficLight {
    Red,
    Green,
    Yellow,
}

impl TrafficLight {
    pub fn next(&self) -> Self {
        todo!("next state after {self:?}")
    }

    pub fn duration_secs(&self) -> u32 {
        todo!("duration for {self:?}")
    }
}

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
