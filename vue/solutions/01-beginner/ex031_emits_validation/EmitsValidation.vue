<!-- Exercise 031 — EmitsValidation component (reference solution). -->
<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "EmitsValidation",
  emits: {
    change: (payload: unknown): boolean => typeof payload === "number",
  },
  methods: {
    emitValid(): void {
      this.$emit("change", 42);
    },
    emitInvalid(): void {
      this.$emit("change", "not-a-number");
    },
  },
});
</script>

<template>
  <div>
    <button data-testid="valid" @click="emitValid">Emit valid</button>
    <button data-testid="invalid" @click="emitInvalid">Emit invalid</button>
  </div>
</template>
