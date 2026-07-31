<!-- Exercise 029 — PropsDefaultValue component (beginner). -->
<!-- Goal:   declare a prop with a default value applied when omitted. -->
<!-- Drills: withDefaults, defineProps with TypeScript generic syntax. -->
<script setup lang="ts">
interface Props {
  size?: "small" | "medium" | "large";
}

withDefaults(defineProps<Props>(), {
  size: "medium",
});

throw new Error("TODO: implement PropsDefaultValue");
</script>

<template>
  <p class="badge">{{ size }}</p>
</template>
