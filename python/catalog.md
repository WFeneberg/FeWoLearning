# Python — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present) · ⬜ planned.

This table is the track's progress ledger: it lists every exercise 001–100, and
the ⬜ rows are the work queue. Slugs on ⬜ rows are the intended module name
(`exNNN_<slug>.py`) and may still be adjusted when the exercise is written.

**Status: 62 ✅ / 38 ⬜**

## Beginner (001–035) — fundamentals

Numbers & strings, f-strings, lists/tuples/sets/dicts, comprehensions, slicing,
`for`/`while`/`enumerate`/`zip`, functions & default args, `*args`/`**kwargs`,
exceptions, file I/O, `pathlib`, `datetime`, `collections.Counter`, sorting with
`key`, `math`/`random`.

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 001 | temperature             | functions, arithmetic, floats                         | ✅     |
| 002 | string_formatting       | f-strings, format specs, alignment, rounding          | ✅     |
| 003 | string_methods          | split/join/strip/replace, case folding                | ✅     |
| 004 | list_operations         | append/extend/insert/pop, in-place sorting            | ✅     |
| 005 | tuple_unpacking         | packing, star-targets, swapping                       | ✅     |
| 006 | set_operations          | set algebra, membership, deduplication                | ✅     |
| 007 | dict_lookup_default     | `dict.get`, `setdefault`, missing keys                | ✅     |
| 008 | list_comprehension      | comprehensions with filters, nested iteration         | ✅     |
| 009 | dict_comprehension      | dict/set comprehensions, inverting a mapping          | ✅     |
| 010 | slicing                 | slice syntax, negative indices and steps, copies      | ✅     |
| 011 | enumerate_zip           | `enumerate`, `zip`, strict parallel iteration         | ✅     |
| 012 | sum_of_digits           | `while`, `divmod`, integer arithmetic                 | ✅     |
| 013 | fizz_buzz               | conditionals, modulo, string building                 | ✅     |
| 014 | default_arguments       | default parameters, the mutable-default trap          | ✅     |
| 015 | args_kwargs             | `*args`, `**kwargs`, argument forwarding              | ✅     |
| 016 | keyword_only_params     | keyword-only and positional-only markers              | ✅     |
| 017 | exception_handling      | `try`/`except`/`else`/`finally`, re-raising           | ✅     |
| 018 | input_validation        | raising `ValueError` with useful messages             | ✅     |
| 019 | file_read_lines         | `open`, context manager, iterating lines              | ✅     |
| 020 | file_write_text         | writing text files, newline handling, encodings       | ✅     |
| 021 | pathlib_paths           | `pathlib.Path`, joining, suffixes, existence          | ✅     |
| 022 | datetime_arithmetic     | `datetime`, `timedelta`, comparisons                  | ✅     |
| 023 | date_parsing            | `strptime`/`strftime`, ISO formats                    | ✅     |
| 024 | counter_most_common     | `collections.Counter`, `most_common`                  | ✅     |
| 025 | sort_with_key           | `sorted(key=…)`, `reverse`, stability                 | ✅     |
| 026 | sort_multiple_keys      | tuple keys, `operator.itemgetter`                     | ✅     |
| 027 | min_max_key             | `min`/`max` with `key` and `default`                  | ✅     |
| 028 | any_all                 | `any`/`all`, generator expressions                    | ✅     |
| 029 | math_functions          | `math`: floor/ceil/sqrt/isclose                       | ✅     |
| 030 | random_sampling         | seeded `random`, `choice`/`sample`/`shuffle`          | ✅     |
| 031 | nested_data_access      | traversing nested dicts and lists safely              | ✅     |
| 032 | palindrome              | normalization, reversal, comparison                   | ✅     |
| 033 | range_step              | `range` with step, `reversed`                         | ✅     |
| 034 | list_rotation           | slicing assignment, rotation in place                 | ✅     |
| 035 | matrix_transpose        | nested lists, `zip(*matrix)`                          | ✅     |

## Intermediate (036–070) — idioms & stdlib

Generators & `yield`, decorators, context managers, `dataclasses`, `enum`,
`typing` (generics, `Protocol`), `itertools`, `functools`, regular expressions,
`json`/`csv`, `argparse`, `NamedTuple`/`TypedDict`, custom exceptions,
`defaultdict`, comprehension pipelines.

| #   | Slug                        | Concepts                                          | Status |
|-----|-----------------------------|---------------------------------------------------|--------|
| 036 | word_frequency              | Counter, regex, sort keys                         | ✅     |
| 037 | generator_fibonacci         | generators, `yield`, laziness                     | ✅     |
| 038 | generator_pipeline          | chained generators, streaming transformation      | ✅     |
| 039 | yield_from_flatten          | `yield from`, recursive flattening                | ✅     |
| 040 | decorator_timing            | decorators, `functools.wraps`                     | ✅     |
| 041 | decorator_with_args         | decorator factories, nested closures              | ✅     |
| 042 | decorator_retry             | retrying wrappers, exception handling             | ✅     |
| 043 | context_manager_class       | `__enter__`/`__exit__`, exception suppression     | ✅     |
| 044 | context_manager_contextlib  | `@contextmanager`, `try`/`finally` in generators  | ✅     |
| 045 | dataclass_basics            | `@dataclass`, defaults, `__post_init__`           | ✅     |
| 046 | dataclass_frozen_order      | `frozen=True`, `order=True`, hashability          | ✅     |
| 047 | enum_basics                 | `Enum`, `auto()`, iteration, lookup by value      | ✅     |
| 048 | enum_flag                   | `Flag`/`IntFlag`, bitwise combination             | ✅     |
| 049 | typing_generics             | `TypeVar`, `Generic`, a typed container           | ✅     |
| 050 | typing_protocol             | `Protocol`, structural typing                     | ✅     |
| 051 | typed_dict                  | `TypedDict`, `total=False`                        | ✅     |
| 052 | namedtuple_record           | `NamedTuple`, field access, `_replace`            | ✅     |
| 053 | itertools_groupby           | `groupby`, sorting before grouping                | ✅     |
| 054 | itertools_chain_islice      | `chain`, `islice`, `takewhile`/`dropwhile`        | ✅     |
| 055 | itertools_combinatorics     | `product`, `combinations`, `permutations`         | ✅     |
| 056 | functools_reduce            | `reduce` with an initial value, fold semantics    | ✅     |
| 057 | functools_partial           | partial application, freezing keywords            | ✅     |
| 058 | functools_cache             | `cache`/`lru_cache`, memoization                  | ✅     |
| 059 | regex_named_groups          | `re.match`, named groups, `groupdict`             | ✅     |
| 060 | regex_substitution          | `re.sub` with a replacement function              | ✅     |
| 061 | json_roundtrip              | `dumps`/`loads`, `default=`, custom encoder       | ✅     |
| 062 | csv_dictreader              | `csv.DictReader`/`DictWriter`, quoting            | ⬜     |
| 063 | argparse_cli                | `argparse`, subcommands, `type=`                  | ⬜     |
| 064 | custom_exception_hierarchy  | exception base classes, `raise … from`            | ⬜     |
| 065 | defaultdict_grouping        | `defaultdict(list)`, grouping records             | ⬜     |
| 066 | chainmap_layers             | `ChainMap`, layered configuration                 | ⬜     |
| 067 | deque_ring_buffer           | `deque`, `maxlen`, `rotate`                       | ⬜     |
| 068 | comprehension_vs_generator  | memory behaviour, when each is right              | ⬜     |
| 069 | unpacking_operators         | `*`/`**` in calls and literals, merging dicts     | ⬜     |
| 070 | cmp_to_key_sorting          | `functools.cmp_to_key`, stable multi-key sort     | ⬜     |

## Advanced (071–090) — concurrency, performance, internals

`asyncio`, `threading` vs `multiprocessing`, `concurrent.futures`, descriptors,
metaclasses, `__slots__`, custom iterators, `contextvars`, `heapq`,
`singledispatch`, profiling, `weakref`, generator pipelines.

| #   | Slug                          | Concepts                                        | Status |
|-----|-------------------------------|-------------------------------------------------|--------|
| 071 | lru_cache                     | OrderedDict, eviction, generics                 | ✅     |
| 072 | asyncio_gather                | `async def`, `await`, `asyncio.gather`          | ⬜     |
| 073 | asyncio_timeout_cancel        | `wait_for`, `CancelledError`, cleanup           | ⬜     |
| 074 | asyncio_queue_workers         | `asyncio.Queue`, worker tasks, `join`           | ⬜     |
| 075 | asyncio_semaphore             | `Semaphore`, bounded concurrency                | ⬜     |
| 076 | threading_lock_counter        | `Thread`, `Lock`, data races                    | ⬜     |
| 077 | futures_as_completed          | `ThreadPoolExecutor`, `as_completed`            | ⬜     |
| 078 | process_pool_cpu_bound        | `ProcessPoolExecutor`, picklability, the GIL    | ⬜     |
| 079 | descriptor_validation         | `__get__`/`__set__`/`__set_name__`              | ⬜     |
| 080 | property_computed             | `property`, setter validation, cached values    | ⬜     |
| 081 | init_subclass_registry        | `__init_subclass__`, plugin registries          | ⬜     |
| 082 | metaclass_enforcement         | metaclasses, class-creation hooks               | ⬜     |
| 083 | slots_memory                  | `__slots__`, attribute restriction, memory      | ⬜     |
| 084 | custom_iterator               | `__iter__`/`__next__`, `StopIteration`          | ⬜     |
| 085 | contextvars_request_id        | `contextvars`, async-safe ambient context       | ⬜     |
| 086 | heapq_priority_queue          | `heapq`, tuple priorities, tie-breaking         | ⬜     |
| 087 | singledispatch_dispatch       | `functools.singledispatch`, `register`          | ⬜     |
| 088 | weakref_cache                 | `WeakValueDictionary`, object lifetime          | ⬜     |
| 089 | streaming_generator_pipeline   | memory-bounded pipelines over large inputs      | ⬜     |
| 090 | abc_abstract_base             | `abc.ABC`, `abstractmethod`, interface checks   | ⬜     |

## Expert (091–100) — architecture & systems

| #   | Slug                     | Concepts                                             | Status |
|-----|--------------------------|------------------------------------------------------|--------|
| 091 | wsgi_micro_framework     | WSGI callable, routing, request/response objects     | ⬜     |
| 092 | orm_query_builder        | fluent builder, SQL generation, parameter binding    | ⬜     |
| 093 | plugin_entry_points      | plugin discovery, registry, `importlib`              | ⬜     |
| 094 | async_task_queue         | async worker pool, retries, backpressure             | ⬜     |
| 095 | dsl_interpreter          | tokenizer, parser, evaluator                         | ⬜     |
| 096 | typed_event_bus          | typed pub/sub, handler registry, `Protocol`          | ⬜     |
| 097 | streaming_parser         | incremental parsing, explicit state machine          | ⬜     |
| 098 | ttl_lru_cache            | combined TTL + LRU eviction, monotonic clock         | ⬜     |
| 099 | di_container             | dependency injection, lifetimes, resolution graph    | ⬜     |
| 100 | property_based_tests     | generative testing, invariants, shrinking            | ⬜     |
