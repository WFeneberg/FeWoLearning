<!-- Exercise 023 — VForKeyList component (reference solution). -->
<script setup lang="ts">
interface Item {
  id: number;
  name: string;
}

interface Props {
  items: Item[];
}

defineProps<Props>();
</script>

<template>
  <ul>
    <li v-for="(item, index) in items" :key="item.id">{{ index }}: {{ item.name }}</li>
  </ul>
</template>
