package fewolearning.exercises.intermediate.ex052_enum_strategy;

/*
Exercise 052 - Enum strategy (reference solution).
*/
public enum EnumStrategy {
    ADD {
        @Override
        public int apply(int left, int right) {
            return left + right;
        }
    },
    SUBTRACT {
        @Override
        public int apply(int left, int right) {
            return left - right;
        }
    },
    MULTIPLY {
        @Override
        public int apply(int left, int right) {
            return left * right;
        }
    };

    public abstract int apply(int left, int right);
}
