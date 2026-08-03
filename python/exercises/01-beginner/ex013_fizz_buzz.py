"""Exercise 013 — FizzBuzz and friends (beginner).

Goal:   Branch on divisibility and build strings from rules.
Drills: modulo, if/elif ordering (the composite case must come first),
        ranges, joining, generalising a hard-coded rule set.
Passes: when `pytest exercises/01-beginner/test_ex013_fizz_buzz.py` is green.
"""


def fizz_buzz(n: int) -> str:
    """Return "Fizz" for multiples of 3, "Buzz" for 5, "FizzBuzz" for both,
    otherwise the number as a string.

    Mind the branch order: 15 must not be caught by the 3-only case first.
    """
    raise NotImplementedError


def fizz_buzz_range(start: int, stop: int) -> list[str]:
    """Return `fizz_buzz` for each number in ``range(start, stop)``.

    An empty or reversed range yields an empty list.
    """
    raise NotImplementedError


def apply_rules(n: int, rules: list[tuple[int, str]]) -> str:
    """Generalised FizzBuzz: concatenate the label of every rule whose divisor
    divides `n`, in the order given.

    ``apply_rules(15, [(3, "Fizz"), (5, "Buzz")])`` -> ``"FizzBuzz"``.
    When no rule matches, return the number as a string. A divisor of 0 raises
    ValueError.
    """
    raise NotImplementedError


def count_multiples(stop: int, divisor: int) -> int:
    """Count how many numbers in ``range(1, stop)`` are divisible by `divisor`.

    A divisor of 0 raises ValueError. A `stop` of 1 or less yields 0.
    """
    raise NotImplementedError


def is_leap_year(year: int) -> bool:
    """Report whether `year` is a Gregorian leap year.

    Divisible by 4, except centuries, unless divisible by 400.
    """
    raise NotImplementedError
