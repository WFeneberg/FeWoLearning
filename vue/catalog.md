# Vue 3 — Exercise Catalog (100)

Difficulty tiers: **Beginner** 001–035 · **Intermediate** 036–070 ·
**Advanced** 071–090 · **Expert** 091–100.

Legend: ✅ seeded (stub + test + solution present) · ⬜ planned.

This table is the track's progress ledger: it lists every exercise 001–100, and
the ⬜ rows are the work queue. Each exercise is a folder
`exercises/<tier>/exNNN_<slug>/` holding the stub plus its `*.test.ts`.

**Status: 100 ✅ / 0 ⬜**

`vitest.config.ts` collects `solutions/**/*.test.ts` as well, so a test copied
into a solution folder verifies the reference implementation. That coverage is
still partial — see the "Known gaps" section of `docs/exercise-format.md`.

The advanced tier deliberately hand-rolls minimal Pinia- and Router-shaped
helpers instead of depending on `pinia` and `vue-router`, so the track installs
with `vue` alone.

## Beginner (001–035) — reactivity & templates

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 001 | use_counter | ref, returning a reactive API from a composable | ✅ |
| 002 | double_ref | ref, reading/writing .value, deriving a value from a ref | ✅ |
| 003 | reactive_object | reactive() with nested objects, mutating nested properties, template rendering reacting to nested mutations | ✅ |
| 004 | computed_full_name | ref, computed, deriving state from multiple reactive sources | ✅ |
| 005 | watch_basic | watch, side effects, accumulating history in a plain array | ✅ |
| 006 | watch_effect_logger | watchEffect, automatic dependency tracking, side effects | ✅ |
| 007 | conditional_display | v-if / v-else-if / v-else | ✅ |
| 008 | list_rendering | v-for, list keys, binding a prop to template output | ✅ |
| 009 | class_binding | reactive ref, dynamic class binding, event handling. --> | ✅ |
| 010 | style_binding | dynamic inline style binding, computed style objects. --> | ✅ |
| 011 | event_click_counter | v-on click handling, template interpolation, ref binding. --> | ✅ |
| 012 | two_way_text_input | ref, v-model on <input>, template interpolation. --> | ✅ |
| 013 | checkbox_model | ref, v-model on <input type="checkbox">, template interpolation. --> | ✅ |
| 014 | select_model | ref, v-model on <select>, rendering <option>s from a list. --> | ✅ |
| 015 | radio_group_model | ref, v-model on <input type="radio">, shared name attribute. --> | ✅ |
| 016 | props_typed_message | defineProps with TypeScript generic syntax, prop interpolation. --> | ✅ |
| 017 | emits_typed_event | defineEmits with TypeScript generic syntax, emitting a payload. --> | ✅ |
| 018 | lifecycle_mounted_fetch | onMounted, ref, component lifecycle timing | ✅ |
| 019 | computed_filtered_list | ref (array), computed, derived reactive state | ✅ |
| 020 | watch_deep_object | reactive, watch, deep option, mutating nested properties in place | ✅ |
| 021 | ref_array_push | ref<string[]>, mutating an array ref, re-rendering on push | ✅ |
| 022 | v_bind_attribute | v-bind dynamic attribute binding, boolean attribute rendering | ✅ |
| 023 | v_for_key_list | v-for with index, :key binding, stable identity across reorders. --> | ✅ |
| 024 | toggle_visibility | ref, v-if, click handlers | ✅ |
| 025 | computed_setter | computed(), splitting/joining strings, updating multiple refs from one setter | ✅ |
| 026 | watch_multiple_sources | watch([a, b], callback), multi-source watchers | ✅ |
| 027 | composable_toggle | ref, returning a tuple API from a composable | ✅ |
| 028 | composable_local_storage | ref, watch, dependency injection (no direct window.localStorage access) | ✅ |
| 029 | props_default_value | withDefaults, defineProps with TypeScript generic syntax. --> | ✅ |
| 030 | props_validator | prop definitions, validator functions, rendering a prop value | ✅ |
| 031 | emits_validation | defineEmits object syntax, emits payload validation | ✅ |
| 032 | v_model_number_modifier | the v-model .number modifier, exposing internal refs for testing | ✅ |
| 033 | v_model_trim_modifier | v-model.trim modifier, exposing internal refs for testing | ✅ |
| 034 | multiple_class_bindings | static class, array syntax, object syntax, boolean/string state. --> | ✅ |
| 035 | computed_sorted_list | computed, non-mutating array sort, ref arrays | ✅ |

## Intermediate (036–070) — component patterns

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 036 | greeting | typed props (defineProps), events (defineEmits), template binding. --> | ✅ |
| 037 | named_slots | named slots, <slot name="..."> fallback content, slot ordering in the DOM | ✅ |
| 038 | scoped_slots | scoped slots, v-slot="{ item, index }", rendering a list via v-for while delegating per-item markup to the parent | ✅ |
| 039 | provide_inject_theme | inject() with a default fallback, using injected reactive/plain values in a template | ✅ |
| 040 | define_model_basic | defineModel, v-model on a custom component, script setup props. --> | ✅ |
| 041 | custom_v_model_component | props/emits contracts, custom v-model, computed derived from props | ✅ |
| 042 | dynamic_component_switch | dynamic components, resolving a component by name, exposing reactive state from a component instance for testing | ✅ |
| 043 | teleport_modal | <Teleport to="...">, teleport target selectors, v-if with Teleport, emitting a close event from teleported content | ✅ |
| 044 | suspense_async_component | async `<script setup>`, top-level await, Suspense fallback timing | ✅ |
| 045 | async_component_loader | defineAsyncComponent, async component resolution, suspense-free loading states (loading / loaded) | ✅ |
| 046 | form_validation_basic | v-model, form submit handling, conditional rendering, computed validation state. --> | ✅ |
| 047 | form_validation_async | async composables, race-condition-safe state updates, awaiting reactive side effects in tests | ✅ |
| 048 | list_transition_group | TransitionGroup, keyed v-for, mutating a reactive array from an event handler. --> | ✅ |
| 049 | composable_extraction_fetch | composable extraction, async state modelling (data/error/loading refs) | ✅ |
| 050 | to_refs_destructure | reactive, toRefs, mutating a reactive object from the outside while a destructured ref still tracks the change | ✅ |
| 051 | to_ref_single | toRef for a single prop, keeping a composable in sync with a parent-owned prop without losing reactivity | ✅ |
| 052 | template_ref_focus | `ref` template refs (`useTemplateRef`/`ref<HTMLElement>`), onMounted, calling native DOM methods from Vue | ✅ |
| 053 | template_ref_child_expose | internal component state, `defineExpose`, keeping exposed API minimal and typed | ✅ |
| 054 | slots_fallback_content | <slot> fallback content, rendering a native <button> | ✅ |
| 055 | scoped_slot_table | scoped slots with named slots, v-slot:cell="{ row, column, value }", rendering a table via nested v-for while delegating per-cell markup to the parent (falling back to the plain value when no slot content is supplied for a given cell) | ✅ |
| 056 | provide_inject_default | inject() with a default value, provide/inject across component boundaries, injection keys | ✅ |
| 057 | define_model_modifiers | defineModel with modifiers, the [name, "modifiers"] set/get form, transforming values on write. --> | ✅ |
| 058 | v_model_multiple_props | multiple v-model bindings, defineProps/defineEmits with `update:propName` events, numeric <input type="range">. --> | ✅ |
| 059 | dynamic_component_keep_state | dynamic components, component identity/remount semantics, local component state lifecycle, exposing reactive state for testing | ✅ |
| 060 | teleport_target_change | <Teleport :disabled>, computed booleans, DOM-location assertions | ✅ |
| 061 | suspense_error_fallback | <Suspense> default/fallback slots, async setup(), onErrorCaptured, returning `false` from onErrorCaptured to stop propagation | ✅ |
| 062 | async_component_retry | defineAsyncComponent options, the onError(error, retry, fail, attempts) hook, loading/error components, async component lifecycle | ✅ |
| 063 | form_validation_rules_composable | composable-based validation, computed derived from a Ref + rules, designing a small rule-function contract | ✅ |
| 064 | transition_fade_toggle | Transition component, enter/leave class naming, v-if inside Transition. --> | ✅ |
| 065 | composable_debounced_ref | ref, watch/timers, cleanup, custom debounce logic in a composable | ✅ |
| 066 | to_refs_reactive_object | reactive, toRefs, keeping a two-way link between a reactive object and its refs | ✅ |
| 067 | template_ref_array | function-ref callbacks inside v-for, collecting DOM nodes into a ref() array, clearing stale refs on re-render (onBeforeUpdate) | ✅ |
| 068 | slots_named_with_props | named scoped slots, v-slot:name="{ ... }", exposing callbacks through slot props instead of only emitting events | ✅ |
| 069 | provide_inject_symbol_key | Symbol() as an InjectionKey, provide/inject, avoiding string keys | ✅ |
| 070 | dynamic_component_registry | dynamic components, computed resolution from props, fallback rendering when a key is missing from a registry | ✅ |

## Advanced (071–090) — state, routing, performance

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 071 | pinia_counter_store | Pinia's store-per-active-instance model, singleton stores keyed by id, actions mutating shared reactive state, and isolating state between tests via a fresh `setActivePinia(createPinia())` | ✅ |
| 072 | pinia_getters_computed | Pinia getters as `computed()` over store state, chaining getters, keeping getters read-only and recomputed only from their action | ✅ |
| 073 | pinia_actions_async | async actions, injected dependencies, loading/error state machines | ✅ |
| 074 | router_navigation_guard | Vue Router navigation guards, guard composition, redirect resolution, ref-based current-route state | ✅ |
| 075 | router_lazy_routes | dynamic `component: () => import(...)`-style route loaders, `defineAsyncComponent`, resolving a route match reactively, awaiting a navigation before asserting on the rendered output | ✅ |
| 076 | render_function_basic | options-API `render()`, `h()`, dynamic tag names, `this.$slots` | ✅ |
| 077 | render_function_jsx | defineComponent + setup() returning a JSX render function, the classic `/** @jsx */` pragma (no template compiler involved), conditional rendering driven by props instead of v-if in a template. /** @jsx h */ /** @jsxFrag Fragment */ | ✅ |
| 078 | custom_directive_highlight | custom directive definition, directive lifecycle hooks (mounted/updated), binding.value, local directive registration. --> | ✅ |
| 079 | custom_directive_click_outside | custom directive lifecycle hooks (mounted/unmounted), global event listeners, DOM containment checks (`Node.contains`), cleaning up listeners to avoid leaks. --> | ✅ |
| 080 | keep_alive_cache | <KeepAlive>, dynamic components (<component :is>), component instance lifecycle vs. local reactive state. --> | ✅ |
| 081 | virtualized_list_basic | computed index-window math, scroll event handling, absolute positioning against a full-height spacer, overscan padding. --> | ✅ |
| 082 | debounced_input_composable | composables that manage timers, debouncing user input, | ✅ |
| 083 | error_boundary_capture | onErrorCaptured, returning false to stop propagation, error state, conditional rendering of slot vs. fallback UI. --> | ✅ |
| 084 | plugin_authoring_basic | the `Plugin` interface, `app.component()`, `app.config .globalProperties`, per-app install isolation, default options | ✅ |
| 085 | plugin_global_property | Vue plugins (the Plugin/install contract), app.config.globalProperties, augmenting ComponentCustomProperties, options-API access to plugin-provided globals | ✅ |
| 086 | test_utils_event_emit | @vue/test-utils event emission assertions — wrapper.emitted(), asserting event name + call count independently per button. --> | ✅ |
| 087 | test_utils_async_update | @vue/test-utils async DOM updates — a triggered async method does NOT update the DOM synchronously; the update only appears after the underlying promise resolves AND Vue flushes via `$nextTick()` (or an equivalent promise/timer flush). --> | ✅ |
| 088 | pinia_store_reset | Pinia's `$patch`/`$reset` store-instance methods, bulk reactive updates vs. one-field-at-a-time assignment, and capturing an immutable "initial state" snapshot that survives later patches | ✅ |
| 089 | router_dynamic_params | dynamic route matching (`:param` segments), provide/inject to expose the current route to descendant components, `useRoute()`-style composables, reactive re-resolution on navigation | ✅ |
| 090 | render_function_slots | `setup(props, { slots })`, `h()`, invoking `slots.default?.()`, forwarding arbitrary slot content instead of hard-coded children | ✅ |

## Expert (091–100) — architecture

| #   | Slug | Concepts | Status |
|-----|------|----------|--------|
| 091 | typed_store_module | generics across multiple type parameters, mapped types, conditional (infer) types, reactive()/computed(), runtime shape-guards standing in for what TypeScript's static checking would reject at compile time | ✅ |
| 092 | data_fetching_cache_composable | module-scoped cache shared across composable instances, dedupe of in-flight promises, reactive loading/error state, manual refetch that bypasses the cache | ✅ |
| 093 | ssr_safe_component | SSR-safe setup (no window/document access), hydration-stable ids, onMounted-only side effects | ✅ |
| 094 | headless_component_library | headless components, slot-driven API, behaviour without prescribing markup | ✅ |
| 095 | design_system_tokens | token layer, CSS custom properties, provide/inject theming | ✅ |
| 096 | schema_driven_form_engine | schema to form generation, declarative validation rules, dynamic components | ✅ |
| 097 | undo_redo_history | history composable, snapshot/patch stack, undo/redo invariants | ✅ |
| 098 | websocket_live_store | WebSocket-backed reactive store, reconnect, optimistic updates | ✅ |
| 099 | micro_frontend_mount | multiple app instances, isolated mounting and teardown | ✅ |
| 100 | a11y_audited_widget | ARIA roles and states, keyboard navigation, focus management | ✅ |
