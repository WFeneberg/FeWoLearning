# Go — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present) · ⬜ planned.

This table is the track's progress ledger: it lists every exercise 001–100. Each
exercise is its own package under `exercises/<tier>/exNNN_<slug>/`; the package
clause drops the `exNNN_` prefix and the underscores, because Go identifiers
cannot start with a digit.

**Status: 100 ✅ / 0 ⬜**

Verified with the reference solutions overlaid onto the stubs: `go vet ./...`
clean, all 100 stubs red, all 100 solutions green. Note that `ex092` needs
`GOTMPDIR` set outside `%TEMP%` on this machine — on-access scanning removes its
test binary before `go test` can exec it.

## Beginner (001–035) — syntax, types, collections, I/O

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | fizzbuzz | control flow, strconv, modulo | ✅ |
| 002 | temperature_converter | variables & types, float conversion | ✅ |
| 003 | slice_reverse | slices, in-place mutation, two-pointer swapping | ✅ |
| 004 | array_sum | fixed-size arrays, value semantics, iteration | ✅ |
| 005 | word_count | maps, string splitting | ✅ |
| 006 | range_filter | range, slice filtering | ✅ |
| 007 | min_max | functions, multiple return values | ✅ |
| 008 | point_struct | struct definition, methods, math.Sqrt | ✅ |
| 009 | rectangle_area | structs, methods, value receivers | ✅ |
| 010 | pointer_swap | pointers, dereferencing, address-of | ✅ |
| 011 | counter_increment | structs, pointer receivers, methods | ✅ |
| 012 | divide_safe | error values, errors.New, multiple return values | ✅ |
| 013 | stack_ops | struct, custom errors, methods, LIFO ordering | ✅ |
| 014 | string_reverse | strings, rune handling | ✅ |
| 015 | str_to_int_sum | strconv.Atoi, error handling, string splitting | ✅ |
| 016 | title_case | strings manipulation, unicode, strings.Fields/Join | ✅ |
| 017 | fmt_table | fmt formatting, Sprintf width specifiers, strings.Builder | ✅ |
| 018 | rune_counter | runes vs bytes, len(), []rune conversion, utf8 encoding | ✅ |
| 019 | byte_palindrome | byte slices, indexing, ASCII case folding | ✅ |
| 020 | sort_people | sort.Slice, structs, closures | ✅ |
| 021 | sort_ints_custom | sort.Interface, method sets, sort.Sort | ✅ |
| 022 | file_line_reader | basic I/O, io.Reader, bufio.Scanner | ✅ |
| 023 | slice_dedupe | slices, maps for tracking seen | ✅ |
| 024 | matrix_transpose | 2D slices, nested loops, allocation of slice-of-slices | ✅ |
| 025 | map_invert | maps, error handling, fmt.Errorf | ✅ |
| 026 | variadic_sum | variadic functions, multiple return values | ✅ |
| 027 | struct_embedding | struct embedding, promoted methods, method overriding | ✅ |
| 028 | pointer_linked_list | pointers, structs, linked lists | ✅ |
| 029 | custom_error_type | custom error types, error interface, type assertion | ✅ |
| 030 | string_builder_join | strings.Builder, efficient string concatenation | ✅ |
| 031 | slice_chunk | slices, sub-slicing, loops | ✅ |
| 032 | map_group_by | maps, grouping, slices | ✅ |
| 033 | fmt_stringer | interfaces, fmt.Stringer, method receivers | ✅ |
| 034 | bufio_scanner_words | bufio.Scanner, bufio.ScanWords, basic I/O | ✅ |
| 035 | sort_stable_multikey | sort.Stable, sort.Interface, multi-key ordering | ✅ |

## Intermediate (036–070) — interfaces, errors, concurrency, generics

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 036 | shape_interface | interfaces, interface satisfaction, polymorphism via slices | ✅ |
| 037 | error_wrapping_chain | error wrapping, %w verb, errors.Is, sentinel errors, error chains | ✅ |
| 038 | custom_error_as | custom error types, error wrapping (%w), errors.As | ✅ |
| 039 | worker_waitgroup | goroutines, sync.WaitGroup, sync.Mutex, shared-state coordination | ✅ |
| 040 | goroutine_counter_mutex | sync.Mutex, concurrency safety, race detector (-race) | ✅ |
| 041 | rwmutex_cache | sync.RWMutex, concurrent map access, goroutines, race safety | ✅ |
| 042 | channel_pipeline | channels, pipelines, goroutines, channel direction types | ✅ |
| 043 | fan_in_channels | goroutines, channels, sync.WaitGroup, fan-in pattern | ✅ |
| 044 | select_timeout | select, time.After, channel receive with ok idiom | ✅ |
| 045 | select_multiplex | select, channel multiplexing, non-deterministic fan-in | ✅ |
| 046 | context_cancel_worker | context cancellation, select on ctx.Done(), goroutine shutdown | ✅ |
| 047 | context_timeout_fetch | context.WithTimeout, select over ctx.Done() and time.After, propagating context.DeadlineExceeded | ✅ |
| 048 | generic_stack | generics, type parameters, LIFO data structures | ✅ |
| 049 | generic_map_filter | generics, type parameters, constraints (any) | ✅ |
| 050 | generic_min_ordered | generics, type parameters, constraints.Ordered | ✅ |
| 051 | json_marshal_struct | encoding/json, struct tags, marshaling | ✅ |
| 052 | json_unmarshal_nested | encoding/json, nested structs, struct tags, error handling | ✅ |
| 053 | json_custom_marshaler | json.Marshaler interface, encoding/json, time.Duration formatting | ✅ |
| 054 | io_reader_counter | io.Reader interface, embedding/composition, error propagation | ✅ |
| 055 | io_writer_multiplex | io.Writer interface, composition, error aggregation | ✅ |
| 056 | bufio_csv_parser | bufio.Scanner, io.Reader, strings.Split, error handling | ✅ |
| 057 | bufio_writer_flush | bufio.Writer, manual flush control, io.Writer | ✅ |
| 058 | table_driven_calculator | table-driven tests, error handling, switch statements | ✅ |
| 059 | time_duration_parser | time.Duration arithmetic, string building, custom parsing, strconv | ✅ |
| 060 | time_business_days | time.Time arithmetic, weekday handling, date normalization | ✅ |
| 061 | regexp_email_validator | regexp package, compiling patterns once, string matching | ✅ |
| 062 | regexp_log_parser | regexp compilation, named subexpressions, SubexpNames, error handling | ✅ |
| 063 | functional_options_server | functional options, variadic parameters, closures over struct fields, API design for extensible constructors | ✅ |
| 064 | functional_options_client | functional options, variadic parameters, error wrapping | ✅ |
| 065 | interface_embedding_readwriter | interface embedding, io.Reader/io.Writer semantics, byte slices | ✅ |
| 066 | goroutine_pipeline_context | goroutines, channels, context cancellation, select, sync.WaitGroup | ✅ |
| 067 | sync_once_singleton | sync.Once, lazy initialization, singleton pattern, concurrency safety | ✅ |
| 068 | channel_semaphore | buffered channels as semaphores, goroutine coordination, sync.WaitGroup, atomic counters | ✅ |
| 069 | generic_linked_list | generics, type parameters, linked data structures | ✅ |
| 070 | json_stream_decoder | encoding/json streaming API, json.Decoder, io.Reader, json.Token | ✅ |

## Advanced (071–090) — concurrency patterns, performance, reflection

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 071 | lrucache | generics, container/list, eviction policy | ✅ |
| 072 | worker_pool_fixed | goroutines, channels, sync.WaitGroup, fan-out/fan-in | ✅ |
| 073 | fan_out_fan_in | fan-out/fan-in, channels, sync.WaitGroup, goroutine lifecycle | ✅ |
| 074 | pipeline_cancel_context | goroutines, channels, context cancellation, sync.WaitGroup, atomic counters, pipeline shutdown | ✅ |
| 075 | rate_limiter_token_bucket | rate limiting algorithms, injectable clocks for deterministic time based tests, floating point accounting | ✅ |
| 076 | sync_pool_buffers | sync.Pool, bytes.Buffer, avoiding allocation churn | ✅ |
| 077 | atomic_counter | sync/atomic, lock-free data access, memory model basics | ✅ |
| 078 | atomic_cas_retry | sync/atomic, optimistic concurrency, CAS retry loops | ✅ |
| 079 | errgroup_parallel_fetch | errgroup.Group, errgroup.WithContext, context cancellation propagation | ✅ |
| 080 | race_safe_map | race debugging, sync.RWMutex, read-modify-write atomicity | ✅ |
| 081 | deadlock_free_transfer | deadlock avoidance, lock ordering, sync.Mutex | ✅ |
| 082 | error_tree_multierror | custom error trees, Unwrap() []error, errors.Is traversal | ✅ |
| 083 | heap_priority_queue | container/heap.Interface, heap.Push/heap.Pop/heap.Fix, indices | ✅ |
| 084 | heap_k_largest | container/heap, heap.Interface, bounded min-heap selection | ✅ |
| 085 | reflection_struct_tags | reflect.Type/Value, struct tags, StructTag.Lookup, unexported fields, pointer dereferencing | ✅ |
| 086 | reflection_deep_equal | reflect.Value/Kind, recursion, generic equality without == | ✅ |
| 087 | benchmark_string_concat | testing.B, strings.Builder, allocation-aware string building | ✅ |
| 088 | buffer_reuse_pool | sync.Pool, size-class bucketing, bit tricks, slice aliasing | ✅ |
| 089 | graceful_shutdown_server | sync.WaitGroup, sync.Mutex, context cancellation, select | ✅ |
| 090 | rate_limited_worker_pool | goroutines, channels, select, context cancellation, worker pools, token-bucket rate limiting | ✅ |

## Expert (091–100) — systems design

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 091 | concurrent_kv_store | sync.RWMutex, hashing for shard selection, generics, goroutine-safe data structures, designing APIs that must survive `go test -race` | ✅ |
| 092 | http_router_scratch | tries/trees, string parsing, http.Handler, generics-free interfaces | ✅ |
| 093 | middleware_chain | higher-order functions, function composition, closures, decorator pattern | ✅ |
| 094 | pubsub_broker | mutexes guarding shared maps/slices, channels as delivery mechanism, fan-out to multiple subscribers, goroutine safety | ✅ |
| 095 | job_scheduler_cron | goroutines, channels, select, sync.Mutex, sync.Once, graceful shutdown, designing concurrent APIs for testability | ✅ |
| 096 | streaming_line_processor | pipeline concurrency, bounded channels as backpressure, goroutine lifetime management, error propagation across pipeline stages | ✅ |
| 097 | connection_pool | sync.Mutex + sync.Cond, bounded resource pools, goroutine blocking | ✅ |
| 098 | state_machine_traffic_light | finite state machines, sentinel errors with errors.Is, Stringer | ✅ |
| 099 | plugin_architecture_interfaces | interfaces as extension points, dynamic dispatch, error wrapping, concurrency-safe registration/lookup | ✅ |
| 100 | load_shedding_backpressure | concurrency-safe counters, mutexes, sentinel errors, errors.Is | ✅ |
