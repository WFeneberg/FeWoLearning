<!--
  Exercise 068 — Layout component (reference solution).
  A named scoped slot ("item") passes both the item and a `remove`
  callback down to the parent template.
-->
<script setup lang="ts">
import { ref, watch } from "vue";

export interface LayoutItem {
  id: number;
  name: string;
}

const props = defineProps<{
  items: LayoutItem[];
}>();

// Own reactive copy so removals never mutate the parent's array/prop.
const internalItems = ref<LayoutItem[]>(props.items.map((item) => ({ ...item })));

watch(
  () => props.items,
  (next) => {
    internalItems.value = next.map((item) => ({ ...item }));
  },
);

function remove(id: number) {
  internalItems.value = internalItems.value.filter((item) => item.id !== id);
}
</script>

<template>
  <ul class="layout">
    <li v-for="item in internalItems" :key="item.id">
      <slot name="item" :item="item" :remove="() => remove(item.id)">
        {{ item.name }}
      </slot>
    </li>
  </ul>
</template>
