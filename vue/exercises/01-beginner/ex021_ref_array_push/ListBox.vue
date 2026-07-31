<!--
Exercise 021 — ListBox component (beginner).
Goal:   a ref holding an array, mutated by pushing a new item.
Drills: ref<string[]>, mutating an array ref, re-rendering on push.
-->
<script setup lang="ts">
import { ref } from "vue";

const list = ref<string[]>(["Apples", "Bread"]);

function addItem(item: string): void {
  throw new Error("TODO: implement addItem");
}

defineExpose({ list, addItem });
</script>

<template>
  <ul>
    <li v-for="(item, index) in list" :key="index">{{ item }}</li>
  </ul>
  <button @click="addItem('Milk')">Add</button>
</template>

