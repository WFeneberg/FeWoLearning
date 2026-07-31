<!-- Exercise 014 — SelectModel component (beginner). -->
<!-- Goal:   bind a <select> dropdown with v-model to a `selected` ref. -->
<!-- Drills: ref, v-model on <select>, rendering <option>s from a list. -->
<script setup lang="ts">
import { ref } from "vue";

const fruits = ["apple", "banana", "cherry"];
const selected = ref(fruits[0]);

// TODO: bind the select below to `selected` with v-model so the status
// text updates when a different option is chosen.
throw new Error("TODO: implement v-model binding on the select");
</script>

<template>
  <div>
    <select data-testid="fruit-select">
      <option v-for="fruit in fruits" :key="fruit" :value="fruit">
        {{ fruit }}
      </option>
    </select>
    <p data-testid="status">Selected: {{ selected }}</p>
  </div>
</template>
