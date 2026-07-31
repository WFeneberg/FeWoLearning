<!-- Exercise 029 — PropsDefaultValue component (reference solution). -->
<script setup lang="ts">
interface Props {
  size?: "small" | "medium" | "large";
}

withDefaults(defineProps<Props>(), {
  size: "medium",
});
</script>

<template>
  <p class="badge">{{ size }}</p>
</template>
