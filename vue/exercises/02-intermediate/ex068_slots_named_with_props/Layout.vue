<!--
  Exercise 068 — Layout component (intermediate).
  Goal:   a named scoped slot ("item") that passes both the item and a
          `remove` callback down to the parent template.
  Drills: named scoped slots, v-slot:name="{ ... }", exposing callbacks
          through slot props instead of only emitting events.
-->
<script setup lang="ts">
import { ref } from "vue";

export interface LayoutItem {
  id: number;
  name: string;
}

const props = defineProps<{
  items: LayoutItem[];
}>();

// TODO: keep an internal, mutable copy of the incoming items so the
// component can remove entries on its own without mutating the prop.
const internalItems = ref<LayoutItem[]>([]);

// TODO: implement remove(id) to drop the matching item from internalItems.
function remove(_id: number) {
  throw new Error("TODO: implement remove");
}
</script>

<template>
  <ul class="layout">
    <!--
      TODO: render one <li> per item in internalItems, using the named
      "item" scoped slot and passing both `item` and `remove` as slot props,
      e.g.:
        <slot name="item" :item="item" :remove="() => remove(item.id)" />
      Fall back to plain text when no slot is supplied.
    -->
  </ul>
</template>
