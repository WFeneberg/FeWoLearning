// Support file for exercise 098 — this is the worker's ENTRY POINT, and
// it is identical in exercises/ and solutions/.
//
// It is loaded by Node directly, not through Vitest: no `@ex` alias, no
// transform, no test globals. Keep it plain ESM with node: imports only.
import { parentPort } from "node:worker_threads";

parentPort.on("message", (message) => {
  switch (message.type) {
    case "sum": {
      // A deliberately dull CPU job: the point is that it runs elsewhere.
      let total = 0;
      for (const value of message.values) total += value;
      parentPort.postMessage({ type: "result", value: total });
      break;
    }
    case "fill": {
      // Writes into a buffer that was TRANSFERRED to us. The sender's
      // view of it is detached by now.
      const bytes = new Uint8Array(message.buffer);
      bytes.fill(message.value);
      parentPort.postMessage({ type: "filled", buffer: message.buffer }, [message.buffer]);
      break;
    }
    case "fail": {
      throw new Error(message.reason);
    }
    default: {
      parentPort.postMessage({ type: "unknown" });
    }
  }
});
