<!-- Exercise 034 — MultipleClassBindings component (beginner). -->
<!-- Goal:   combine array and object :class syntax on one element. -->
<!-- Drills: static class, array syntax, object syntax, boolean/string state. -->
<script setup lang="ts">
import { ref } from "vue";

const isActive = ref(false);
const size = ref<"small" | "large">("small");

function toggleActive() {
  throw new Error("TODO: implement toggleActive");
}

function toggleSize() {
  throw new Error("TODO: implement toggleSize");
}
</script>

<template>
  <div>
    <!--
      TODO: bind :class on the box below using BOTH array and object syntax:
        - always include the static class "box"
        - include { active: isActive } as an object entry
        - include `size-${size}` (e.g. "size-small" / "size-large") from the array
    -->
    <div class="box" data-testid="box">Box</div>
    <button data-testid="toggle-active" @click="toggleActive">Toggle Active</button>
    <button data-testid="toggle-size" @click="toggleSize">Toggle Size</button>
  </div>
</template>
