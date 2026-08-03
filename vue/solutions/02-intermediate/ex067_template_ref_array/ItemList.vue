<!--
  Exercise 067 — ItemList component (reference solution).
-->
<script setup lang="ts">
import { onBeforeUpdate, ref, type ComponentPublicInstance } from "vue";

const props = defineProps<{
  items: string[];
}>();

// Exposed so tests (and parents) can inspect the collected DOM nodes.
const itemRefs = ref<HTMLLIElement[]>([]);

onBeforeUpdate(() => {
  itemRefs.value = [];
});

function setItemRef(el: Element | ComponentPublicInstance | null): void {
  if (el) {
    itemRefs.value.push(el as HTMLLIElement);
  }
}

defineExpose({
  itemRefs,
});
</script>

<template>
  <ul>
    <li v-for="item in props.items" :key="item" :ref="setItemRef">
      {{ item }}
    </li>
  </ul>
</template>
