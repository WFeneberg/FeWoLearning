<!--
  Exercise 067 — ItemList component (intermediate).
  Goal:   render a v-for list where each item registers itself into a
          template ref array so the parent can inspect the real DOM nodes.
  Drills: function-ref callbacks inside v-for, collecting DOM nodes into a
          ref() array, clearing stale refs on re-render (onBeforeUpdate).
-->
<script setup lang="ts">
import { onBeforeUpdate, ref } from "vue";

const props = defineProps<{
  items: string[];
}>();

// Exposed so tests (and parents) can inspect the collected DOM nodes.
const itemRefs = ref<HTMLLIElement[]>([]);

onBeforeUpdate(() => {
  itemRefs.value = [];
});

function setItemRef(_el: Element | null): void {
  throw new Error("TODO: implement setItemRef to collect the DOM node");
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
