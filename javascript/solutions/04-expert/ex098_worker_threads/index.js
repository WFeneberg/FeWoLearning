// Reference solution — exercise 098.
import { Worker } from "node:worker_threads";

// A real file URL, resolved from THIS module's location — which is what
// makes the same code work from exercises/ and from solutions/.
const workerUrl = () => new URL("./worker.js", import.meta.url);

function once(send) {
  const worker = new Worker(workerUrl());
  return new Promise((resolve, reject) => {
    worker.once("message", resolve);
    worker.once("error", reject);
    send(worker);
  }).finally(() => worker.terminate());
}

export async function sumInWorker(values) {
  const message = await once((worker) => worker.postMessage({ type: "sum", values }));
  return message.value;
}

export async function fillInWorker(size, value) {
  const bytes = new Uint8Array(size);
  const { buffer } = bytes;
  const message = await once((worker) =>
    worker.postMessage({ type: "fill", buffer, value }, [buffer]),
  );
  return {
    filled: new Uint8Array(message.buffer),
    // After a transfer the sender's buffer is detached: byteLength 0.
    senderDetached: bytes.byteLength === 0,
  };
}

export async function failInWorker(reason) {
  return once((worker) => worker.postMessage({ type: "fail", reason }));
}

export async function sendFunction() {
  const worker = new Worker(workerUrl());
  try {
    worker.postMessage({ type: "sum", values: [() => 1] });
    return "no error";
  } catch (error) {
    return error.name;
  } finally {
    await worker.terminate();
  }
}
