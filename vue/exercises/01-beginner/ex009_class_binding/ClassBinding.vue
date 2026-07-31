<!-- Exercise 009 — ClassBinding component (beginner). -->
<!-- Goal:   toggle an 'active' CSS class on a div via :class binding. -->
<!-- Drills: reactive ref, dynamic class binding, event handling. -->
<script setup lang="ts">
import { ref } from "vue";

const isActive = ref(false);

function toggle() {
  throw new Error("TODO: implement toggle");
}
</script>

<template>
  <div>
    <div class="box" data-testid="box">Box</div>
    <button @click="toggle">Toggle</button>
  </div>
</template>
