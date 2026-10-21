const { initializeApp, deleteApp } = require("firebase-admin/app");
const { getDatabase } = require("firebase-admin/database");
const { verifySessionTransaction } = require("../lib/verification.js");

let input = "";
process.stdin.setEncoding("utf8");
process.stdin.on("data", (chunk) => { input += chunk; });
process.stdin.on("end", async () => {
  let app;
  try {
    const request = JSON.parse(input);
    app = initializeApp({ databaseURL: request.databaseUrl }, `verification-worker-${process.pid}`);
    const ref = getDatabase(app).ref(request.path);

    if (request.mode === "legacy") {
      let firstCallbackWasNull = false;
      let callbackCount = 0;
      const transaction = await ref.transaction((raw) => {
        callbackCount += 1;
        if (callbackCount === 1 && raw === null) firstCallbackWasNull = true;
        if (!raw) return;
        return raw;
      }, undefined, false);
      const existsAfterAbort = (await ref.get()).exists();
      process.stdout.write(JSON.stringify({
        firstCallbackWasNull,
        committed: transaction.committed,
        existsAfterAbort,
      }));
      return;
    }

    const result = await verifySessionTransaction({
      ref,
      email: request.email,
      sessionId: request.sessionId,
      code: request.code,
      secret: request.secret,
      now: request.now,
      grantLifetimeMs: request.grantLifetimeMs,
    });
    process.stdout.write(JSON.stringify({
      outcome: result.outcome,
      diagnostic: result.diagnostic,
      committed: result.committed,
      grantIssued: typeof result.grant === "string" && result.grant.length > 0,
    }));
  } catch {
    process.stdout.write(JSON.stringify({ workerError: true }));
    process.exitCode = 1;
  } finally {
    if (app) await deleteApp(app);
  }
});
