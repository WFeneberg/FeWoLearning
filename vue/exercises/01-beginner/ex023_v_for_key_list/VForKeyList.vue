<!-- Exercise 023 — VForKeyList component (beginner). -->
<!-- Goal:   render a list with v-for, exposing index and item, keyed by a unique id. -->
<!-- Drills: v-for with index, :key binding, stable identity across reorders. -->
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
    <!-- TODO: implement the v-for loop -->
    <li v-for="_todo in []" :key="0">TODO: implement VForKeyList</li>
  </ul>
</template>
