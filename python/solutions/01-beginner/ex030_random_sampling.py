"""Exercise 030 — Random with a seed (reference solution)."""

import random


def pick_one(values: list[str], seed: int) -> str:
    # A private Random instance keeps this reproducible without touching the
    # module-level generator that the rest of the process shares.
    return random.Random(seed).choice(values)


def pick_many(values: list[str], count: int, seed: int) -> list[str]:
    # sample() draws without replacement and raises ValueError when count exceeds
    # the population.
    return random.Random(seed).sample(values, count)


def pick_with_repeats(values: list[str], count: int, seed: int) -> list[str]:
    return random.Random(seed).choices(values, k=count)


def shuffled(values: list[int], seed: int) -> list[int]:
    result = values.copy()
    # shuffle() works in place, hence the copy.
    random.Random(seed).shuffle(result)
    return result


def roll_dice(count: int, seed: int) -> list[int]:
    if count < 0:
        raise ValueError("roll_dice() count must not be negative")
    rng = random.Random(seed)
    # randint's upper bound is inclusive, unlike range().
    return [rng.randint(1, 6) for _ in range(count)]


def weighted_pick(options: dict[str, int], seed: int) -> str:
    if not options:
        raise ValueError("weighted_pick() needs at least one option")
    if any(weight <= 0 for weight in options.values()):
        raise ValueError("weighted_pick() weights must be positive")
    keys = list(options)
    weights = [options[key] for key in keys]
    return random.Random(seed).choices(keys, weights=weights, k=1)[0]
