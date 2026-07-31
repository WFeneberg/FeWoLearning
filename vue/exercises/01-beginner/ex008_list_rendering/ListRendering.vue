<!--
Exercise 008 — ListRendering component (beginner).
Goal:   render an <li> per item in an `items` array via v-for.
Drills: v-for, list keys, binding a prop to template output.
-->
<script setup lang="ts">
defineProps<{
  items: string[];
}>();
</script>

<template>
  <ul>
    <li>TODO: implement v-for over items</li>
  </ul>
</template>
