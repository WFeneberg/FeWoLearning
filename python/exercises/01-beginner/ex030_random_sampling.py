"""Exercise 030 — Random with a seed (beginner).

Goal:   Use randomness that can be reproduced, which is what makes it testable.
Drills: random.Random instances vs the module-level functions, seeding,
        choice/sample/shuffle/randint, sampling without replacement.
Passes: when `pytest exercises/01-beginner/test_ex030_random_sampling.py` is green.

Note:   every function takes a seed and must build its own ``random.Random(seed)``.
        The module-level ``random.choice`` and friends share one global generator,
        so seeding that would make these functions interfere with each other and
        with anything else in the process.
"""


def pick_one(values: list[str], seed: int) -> str:
    """Return one value, chosen reproducibly for a given seed.

    An empty list raises IndexError.
    """
    raise NotImplementedError


def pick_many(values: list[str], count: int, seed: int) -> list[str]:
    """Return `count` distinct values — sampling **without** replacement.

    A `count` larger than the list raises ValueError. A `count` of 0 yields [].
    """
    raise NotImplementedError


def pick_with_repeats(values: list[str], count: int, seed: int) -> list[str]:
    """Return `count` values **with** replacement, so repeats are possible."""
    raise NotImplementedError


def shuffled(values: list[int], seed: int) -> list[int]:
    """Return a shuffled **copy**, leaving the input untouched.

    ``random.shuffle`` works in place, so copy first.
    """
    raise NotImplementedError


def roll_dice(count: int, seed: int) -> list[int]:
    """Roll `count` six-sided dice, each an int from 1 to 6 inclusive.

    A negative `count` raises ValueError.
    """
    raise NotImplementedError


def weighted_pick(options: dict[str, int], seed: int) -> str:
    """Pick a key with probability proportional to its weight.

    An empty mapping, or any non-positive weight, raises ValueError.
    """
    raise NotImplementedError
