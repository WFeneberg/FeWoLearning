<!--
  Exercise 051 — ToRefSingle component (intermediate).
  Goal:   a child component receives a `count` prop and derives a local,
          individually-reactive ref from it using `toRef(props, 'count')`,
          then hands that ref to a composable that doubles it.
  Drills: toRef for a single prop, keeping a composable in sync with a
          parent-owned prop without losing reactivity.
-->
<script setup lang="ts">
import { computed, toRef, type Ref } from "vue";

const props = defineProps<{ count: number }>();

// A tiny "composable" that only ever sees the ref it was given — it must
// stay in sync with the parent's prop purely through that ref.
function useDoubled(countRef: Ref<number>) {
  return computed(() => countRef.value * 2);
}

// TODO: derive `countRef` from `props` with `toRef` and pass it to
// `useDoubled` to compute `doubled`. Until then, this throws.
const countRef: Ref<number> = (() => {
  throw new Error("TODO: implement ToRefSingle using toRef(props, 'count')");
})();
const doubled = useDoubled(countRef);
</script>

<template>
  <div>
    <span data-testid="count">{{ props.count }}</span>
    <span data-testid="doubled">{{ doubled }}</span>
  </div>
</template>
