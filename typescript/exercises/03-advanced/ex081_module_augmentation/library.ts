// Stands in for a third-party module: something you depend on and do not
// control. Do not change it — that is the whole premise.
export interface Request {
  url: string;
  headers: Record<string, string>;
}

export function makeRequest(url: string): Request {
  return { url, headers: {} };
}

export const VERSION = "1.0.0";
