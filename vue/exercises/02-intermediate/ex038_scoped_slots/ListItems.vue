<!--
Exercise 038 — ListItems component (intermediate).
Goal:   expose each `item` and its `index` to a scoped default slot.
Drills: scoped slots, v-slot="{ item, index }", rendering a list via v-for
        while delegating per-item markup to the parent.
-->
<template>
  <ul class="list-items">
    <!-- TODO: for each item in `items`, render an <li> that yields the
         default slot with `item` and `index` as slot props -->
  </ul>
</template>

<script setup lang="ts">
defineProps<{ items: string[] }>();

defineSlots<{
  default(props: { item: string; index: number }): unknown;
}>();

throw new Error("TODO: implement ListItems scoped slot rendering");
</script>
