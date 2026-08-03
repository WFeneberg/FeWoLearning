"""Exercise 002 — String formatting (reference solution)."""


def format_price(amount: float, currency: str = "EUR") -> str:
    return f"{amount:.2f} {currency}"


def format_percent(fraction: float, decimals: int = 1) -> str:
    # The nested {decimals} makes the precision itself a runtime value.
    return f"{fraction * 100:.{decimals}f}%"


def align_columns(rows: list[tuple[str, int]], width: int) -> list[str]:
    # `<` left-aligns and pads to at least `width`; `>` right-aligns in 5 columns.
    return [f"{name:<{width}}{number:>5}" for name, number in rows]


def thousands(value: int) -> str:
    return f"{value:,}"
