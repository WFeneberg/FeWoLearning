package fewolearning.exercises.intermediate.ex052_enum_strategy;

/*
Exercise 052 - Enum strategy (intermediate).

Goal:   Give each operation its own per-constant implementation of apply.
Drills: behavior per enum constant.
*/
public enum EnumStrategy {
    ADD {
        @Override
        public int apply(int left, int right) {
            throw new UnsupportedOperationException("TODO");
        }
    },
    SUBTRACT {
        @Override
        public int apply(int left, int right) {
            throw new UnsupportedOperationException("TODO");
        }
    },
    MULTIPLY {
        @Override
        public int apply(int left, int right) {
            throw new UnsupportedOperationException("TODO");
        }
    };

    public abstract int apply(int left, int right);
}
