<!--
  Exercise 051 — ToRefSingle component (reference solution).
-->
<script setup lang="ts">
import { computed, toRef, type Ref } from "vue";

const props = defineProps<{ count: number }>();

// A tiny "composable" that only ever sees the ref it was given — it must
// stay in sync with the parent's prop purely through that ref.
function useDoubled(countRef: Ref<number>) {
  return computed(() => countRef.value * 2);
}

const countRef = toRef(props, "count");
const doubled = useDoubled(countRef);
</script>

<template>
  <div>
    <span data-testid="count">{{ props.count }}</span>
    <span data-testid="doubled">{{ doubled }}</span>
  </div>
</template>
