<!--
Exercise 021 — ListBox component (reference solution).
-->
<script setup lang="ts">
import { ref } from "vue";

const list = ref<string[]>(["Apples", "Bread"]);

function addItem(item: string): void {
  list.value.push(item);
}

defineExpose({ list, addItem });
</script>

<template>
  <ul>
    <li v-for="(item, index) in list" :key="index">{{ item }}</li>
  </ul>
  <button @click="addItem('Milk')">Add</button>
</template>
