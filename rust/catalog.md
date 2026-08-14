# Rust — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present) · ⬜ planned.

This table is the track's progress ledger: it lists every exercise 001–100, and
the ⬜ rows are the work queue. Each exercise is a single file
`exercises/<tier>/exNNN_<slug>.rs` with an inline `#[cfg(test)] mod tests`, and
**must be registered with a `#[path]` `pub mod` line in `exercises/lib.rs`** —
an unregistered file is never compiled.

**Status: 12 ✅ / 88 ⬜**

> **Toolchain note.** `cargo test` cannot link on this machine yet: Rust is
> installed with the `x86_64-pc-windows-msvc` target, but the MSVC libraries and
> the Windows SDK are missing (`vcvars64.bat` also calls a `vcvarsall.bat` that
> does not exist). The C++ workload has to be added through the Visual Studio
> installer, run elevated, before any of these exercises can be verified.

## Beginner (001–035) — fundamentals

`let`/`mut`, shadowing, scalar & compound types, ownership intro, `String` vs
`&str`, slices, `Vec`, `HashMap`, `match`, `if let`, `Option`, `Result`, `enum`,
`struct`, methods & `impl`, iterators, `for` loops.

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 001 | anagram                 | iterators, chars, sorting                             | ✅     |
| 002 | let_mut_shadowing       | `let`, `mut`, shadowing, scopes                       | ✅     |
| 003 | integer_overflow        | wrapping/checked/saturating arithmetic                | ✅     |
| 004 | float_compare           | float precision, `EPSILON` comparisons                | ✅     |
| 005 | tuple_struct_point      | tuple structs, field access, destructuring            | ✅     |
| 006 | string_vs_str           | `String` vs `&str`, `to_owned`, `as_str`              | ✅     |
| 007 | string_push_format      | `push_str`, `format!`, capacity                       | ✅     |
| 008 | char_classification     | `char` methods, Unicode vs bytes                      | ✅     |
| 009 | slice_basics            | slices, ranges, bounds checking                       | ✅     |
| 010 | vec_push_pop            | `Vec` growth, `push`/`pop`/`insert`/`remove`          | ✅     |
| 011 | vec_iter_sum            | `iter`, `sum`, `max`, `min`                           | ✅     |
| 012 | hashmap_word_count      | `HashMap`, `entry` API, counting                      | ⬜     |
| 013 | hashmap_iteration       | iterating maps, sorting entries                       | ⬜     |
| 014 | match_literals          | `match` on integers and ranges, exhaustiveness        | ⬜     |
| 015 | match_guards            | match guards, binding with `@`                        | ⬜     |
| 016 | if_let_while_let        | `if let`, `while let`, popping until empty            | ⬜     |
| 017 | option_map_unwrap_or    | `Option`, `map`, `unwrap_or`, `and_then`              | ⬜     |
| 018 | option_ok_or            | `Option` to `Result` conversion                       | ⬜     |
| 019 | result_basic            | `Result`, `match` on errors, `is_ok`                  | ⬜     |
| 020 | result_question_mark    | the `?` operator in a fallible function               | ⬜     |
| 021 | enum_with_data          | data-carrying enum variants, matching them            | ⬜     |
| 022 | enum_methods            | `impl` on an enum, `Self`                             | ⬜     |
| 023 | struct_impl_methods     | `impl`, `&self` vs `self`, associated functions       | ⬜     |
| 024 | struct_update_syntax    | `..base` struct update, field init shorthand          | ⬜     |
| 025 | ownership_move          | moves, why a moved value cannot be reused             | ⬜     |
| 026 | borrow_immutable        | `&T`, many readers, no writers                        | ⬜     |
| 027 | borrow_mutable          | `&mut T`, exclusivity, reborrowing                    | ⬜     |
| 028 | clone_vs_copy           | `Clone` vs `Copy` semantics                           | ⬜     |
| 029 | iterator_map_filter     | `map`, `filter`, `collect` into `Vec`                 | ⬜     |
| 030 | iterator_fold           | `fold`, accumulator threading                         | ⬜     |
| 031 | iterator_zip_enumerate  | `zip`, `enumerate`, `rev`                             | ⬜     |
| 032 | sort_by_key             | `sort_by_key`, `sort_unstable`, stability             | ⬜     |
| 033 | vec_dedup_retain        | `dedup`, `retain`, in-place filtering                 | ⬜     |
| 034 | fizz_buzz               | control flow, modulo, `String` building               | ⬜     |
| 035 | matrix_transpose        | nested `Vec`, indexing, allocation                    | ⬜     |

## Intermediate (036–070) — idioms & the borrow checker

Borrowing & lifetimes intro, traits & `impl Trait`, generics & bounds, `derive`
macros, error handling with `?` and custom errors, closures & `Fn` traits,
`Iterator` implementations, `From`/`Into`, `Cow`, modules, `Box<dyn Trait>`.

| #   | Slug                      | Concepts                                            | Status |
|-----|---------------------------|-----------------------------------------------------|--------|
| 036 | rle                       | peekable, String, round-trip                        | ✅     |
| 037 | lifetime_longest          | explicit lifetime annotations on returns            | ⬜     |
| 038 | lifetime_struct_ref       | structs holding references, lifetime elision        | ⬜     |
| 039 | trait_definition          | defining a trait, default methods                   | ⬜     |
| 040 | trait_impl_for_type       | implementing a trait for your own type              | ⬜     |
| 041 | trait_generic_bounds      | `<T: Trait>`, `where` clauses                        | ⬜     |
| 042 | impl_trait_argument       | `impl Trait` in argument and return position        | ⬜     |
| 043 | box_dyn_trait             | trait objects, dynamic dispatch, object safety      | ⬜     |
| 044 | derive_debug_clone        | `#[derive(...)]`, what each derive generates         | ⬜     |
| 045 | derive_partialeq_ord      | `PartialEq`/`Eq`/`PartialOrd`/`Ord` semantics        | ⬜     |
| 046 | display_from_str          | `Display` and `FromStr` implementations             | ⬜     |
| 047 | custom_error_enum         | an error enum with `Display` + `std::error::Error`   | ⬜     |
| 048 | error_source_chain        | `source()`, wrapping a lower-level error            | ⬜     |
| 049 | from_into_conversion      | `From`/`Into`, `?` converting error types           | ⬜     |
| 050 | tryfrom_validation        | `TryFrom`, validated construction                   | ⬜     |
| 051 | closure_fn_traits         | `Fn`/`FnMut`/`FnOnce`, captures                      | ⬜     |
| 052 | closure_returning_boxed   | returning closures, `Box<dyn Fn>`                    | ⬜     |
| 053 | iterator_impl_custom      | implementing `Iterator` by hand                     | ⬜     |
| 054 | iterator_chain_flatmap    | `chain`, `flat_map`, `flatten`                        | ⬜     |
| 055 | iterator_take_skip_while  | `take_while`, `skip_while`, `step_by`                | ⬜     |
| 056 | iterator_partition        | `partition`, `all`, `any`, `position`                | ⬜     |
| 057 | collect_into_hashmap      | `collect` into `HashMap`/`HashSet`/`String`          | ⬜     |
| 058 | cow_borrowed_owned        | `Cow`, avoiding needless allocation                 | ⬜     |
| 059 | modules_visibility        | `mod`, `pub`, `pub(crate)`, paths                    | ⬜     |
| 060 | generic_struct_stack      | a generic `Stack<T>` with bounds                     | ⬜     |
| 061 | generic_two_params        | multiple type parameters, monomorphization           | ⬜     |
| 062 | default_trait             | `Default`, `..Default::default()`                    | ⬜     |
| 063 | operator_overloading      | `Add`/`Mul` via `std::ops`                           | ⬜     |
| 064 | index_trait               | `Index`/`IndexMut` for a custom collection          | ⬜     |
| 065 | pattern_match_nested      | nested destructuring, slice patterns                | ⬜     |
| 066 | matches_macro             | `matches!`, terse predicate matching                 | ⬜     |
| 067 | option_result_combinators | chaining combinators without `unwrap`               | ⬜     |
| 068 | sort_with_custom_cmp      | `sort_by`, `Ordering`, `then_with`                   | ⬜     |
| 069 | binary_search_slice       | `binary_search_by`, insertion points                | ⬜     |
| 070 | integration_test_layout   | `tests/` vs inline `#[cfg(test)]`, visibility        | ⬜     |

## Advanced (071–090) — ownership at scale, concurrency

| #   | Slug                     | Concepts                                             | Status |
|-----|--------------------------|------------------------------------------------------|--------|
| 071 | lru_cache                | generics, eviction, `HashMap` + list bookkeeping     | ⬜     |
| 072 | rc_shared_ownership      | `Rc`, strong counts, shared immutable graphs         | ⬜     |
| 073 | refcell_interior_mut     | `RefCell`, runtime borrow checking, panics           | ⬜     |
| 074 | rc_refcell_tree          | `Rc<RefCell<T>>`, parent/child trees, `Weak`         | ⬜     |
| 075 | arc_mutex_counter        | `Arc<Mutex<T>>`, sharing across threads              | ⬜     |
| 076 | rwlock_reader_writer     | `RwLock`, many readers or one writer                 | ⬜     |
| 077 | thread_spawn_join        | `std::thread`, `join`, moving captures               | ⬜     |
| 078 | mpsc_channel_pipeline    | `mpsc` channels, producer/consumer                   | ⬜     |
| 079 | scoped_threads           | `thread::scope`, borrowing locals across threads     | ⬜     |
| 080 | send_sync_bounds         | `Send`/`Sync`, why some types are neither            | ⬜     |
| 081 | atomic_counter           | `AtomicUsize`, `Ordering`, lock-free increment       | ⬜     |
| 082 | trait_object_vs_generic  | dispatch cost, code size, when each fits             | ⬜     |
| 083 | deref_smart_pointer      | `Deref`/`DerefMut`, a custom smart pointer           | ⬜     |
| 084 | drop_order               | `Drop`, deterministic teardown, drop order           | ⬜     |
| 085 | unsafe_raw_pointer       | raw pointers, invariants an `unsafe` block must hold | ⬜     |
| 086 | macro_rules_basics       | `macro_rules!`, repetition, hygiene                  | ⬜     |
| 087 | zero_cost_iterator_chain | building an iterator adapter without allocation      | ⬜     |
| 088 | phantom_data_marker      | `PhantomData`, type-level tagging                    | ⬜     |
| 089 | const_generics_array     | const generics, fixed-size array APIs                | ⬜     |
| 090 | no_std_friendly_module   | avoiding `std`, `core`-only code                     | ⬜     |

## Expert (091–100) — systems & abstractions

| #   | Slug                    | Concepts                                              | Status |
|-----|-------------------------|-------------------------------------------------------|--------|
| 091 | typestate_builder       | typestate pattern, compile-time build validation      | ⬜     |
| 092 | arena_allocator         | arena/slab allocation, index handles instead of refs   | ⬜     |
| 093 | mini_async_executor     | `Waker`, task queue, polling to completion            | ⬜     |
| 094 | lock_free_stack         | CAS loops, `AtomicPtr`, ABA hazards                   | ⬜     |
| 095 | parser_combinators      | combinator traits, composition, error positions       | ⬜     |
| 096 | mini_interpreter        | tokenizer, AST, evaluation, environments              | ⬜     |
| 097 | manual_future_impl      | implementing `Future`, `Poll`, state machines         | ⬜     |
| 098 | thread_pool             | worker threads, job channel, graceful shutdown        | ⬜     |
| 099 | zero_copy_binary_decoder | byte slices, alignment, borrowed views                | ⬜     |
| 100 | declarative_macro_dsl   | a `macro_rules!` DSL with nested repetition           | ⬜     |
