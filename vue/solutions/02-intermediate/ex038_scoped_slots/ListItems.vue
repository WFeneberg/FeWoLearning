<!--
Exercise 038 — ListItems component (reference solution).
Goal:   expose each `item` and its `index` to a scoped default slot.
-->
<template>
  <ul class="list-items">
    <li v-for="(item, index) in items" :key="index">
      <slot :item="item" :index="index" />
    </li>
  </ul>
</template>

<script setup lang="ts">
defineProps<{ items: string[] }>();

defineSlots<{
  default(props: { item: string; index: number }): unknown;
}>();
</script>
