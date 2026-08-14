package fewolearning.exercises.beginner.ex010_traffic_light_enum;

/*
Exercise 010 - Traffic light enum (reference solution).
*/
public enum TrafficLight {
    RED,
    GREEN,
    YELLOW;

    public TrafficLight next() {
        return switch (this) {
            case RED -> GREEN;
            case GREEN -> YELLOW;
            case YELLOW -> RED;
        };
    }

    public boolean canCarsGo() {
        return this == GREEN;
    }
}
