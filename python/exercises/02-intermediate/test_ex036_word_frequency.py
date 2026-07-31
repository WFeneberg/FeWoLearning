from ex036_word_frequency import top_words


def test_basic_counts() -> None:
    text = "the cat sat on the mat, the CAT ran."
    assert top_words(text, 2) == [("the", 3), ("cat", 2)]


def test_tie_break_alphabetical() -> None:
    text = "b a c a b c"
    # all counts equal (2) -> alphabetical order
    assert top_words(text, 3) == [("a", 2), ("b", 2), ("c", 2)]


def test_n_larger_than_vocab() -> None:
    assert top_words("hi hi there", 10) == [("hi", 2), ("there", 1)]


def test_empty() -> None:
    assert top_words("", 5) == []
