package fewolearning.exercises.beginner.ex010_traffic_light_enum;

/*
Exercise 010 - Traffic light enum (beginner).

Goal:   Model traffic light states and the next state transition.
Drills: enums, switch, behavior by variant.
*/
public enum TrafficLight {
    RED,
    GREEN,
    YELLOW;

    public TrafficLight next() {
        throw new UnsupportedOperationException("TODO");
    }

    public boolean canCarsGo() {
        throw new UnsupportedOperationException("TODO");
    }
}