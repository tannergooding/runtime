// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import test from "node:test";
import { fileURLToPath } from "node:url";
import { runInNewContext } from "node:vm";
import ts from "typescript";

const sourcePath = fileURLToPath(new URL("../native/scheduling.ts", import.meta.url));
const compiled = ts.transpileModule(readFileSync(sourcePath, "utf8"), {
    compilerOptions: { module: ts.ModuleKind.CommonJS, target: ts.ScriptTarget.ES2022 }
}).outputText;

function createHost(onFinalization = () => {}) {
    const timers = new Map();
    const state = { scheduled: 0, canceled: 0, finalized: 0, keepalive: 0, aborted: 0 };
    let nextId = 0;

    function enqueue(callback) {
        const id = ++nextId;
        timers.set(id, callback);
        return id;
    }

    const host = {
        ABORT: false,
        DOTNET: { isAborting: false, lastScheduledFinalizationId: undefined },
        safeSetTimeout(callback, delay) {
            assert.equal(delay, 0);
            state.scheduled++;
            state.keepalive++;
            return enqueue(() => {
                host.runtimeKeepalivePop();
                callback();
            });
        },
        runtimeKeepalivePop() {
            state.keepalive--;
            assert.ok(state.keepalive >= 0);
        },
        _SystemJS_ExecuteFinalizationCallback() {
            state.finalized++;
            onFinalization();
        },
        dotnetBrowserUtilsExports: {
            abortPosix() {
                state.aborted++;
            }
        }
    };
    const exports = {};
    runInNewContext(compiled, {
        exports,
        require(name) {
            assert.equal(name, "../../Common/JavaScript/ems-ambient");
            return { _ems_: host };
        },
        clearTimeout(id) {
            assert.ok(timers.delete(id));
            state.canceled++;
        }
    }, { filename: sourcePath });

    return {
        host,
        state,
        schedule: exports.SystemJS_ScheduleFinalization,
        enqueue,
        runOne() {
            assert.ok(timers.size > 0);
            const [id, callback] = timers.entries().next().value;
            timers.delete(id);
            callback();
        }
    };
}

for (const requests of [1, 2, 10000]) {
    test(`coalesces ${requests} pending finalization requests`, () => {
        const h = createHost();
        h.schedule();
        const firstId = h.host.DOTNET.lastScheduledFinalizationId;
        for (let i = 1; i < requests; i++) {
            h.schedule();
        }
        assert.equal(h.host.DOTNET.lastScheduledFinalizationId, firstId);
        assert.equal(h.state.scheduled, 1);
        assert.equal(h.state.canceled, 0);
        assert.equal(h.state.keepalive, 1);
        h.runOne();
        assert.equal(h.state.finalized, 1);
        assert.equal(h.state.keepalive, 0);
        assert.equal(h.host.DOTNET.lastScheduledFinalizationId, undefined);
    });
}

test("does not move finalization behind a producer's next turn", () => {
    const h = createHost();
    let produced = 0;
    function producer() {
        produced++;
        h.enqueue(producer);
        h.schedule();
    }
    h.enqueue(producer);
    h.schedule();
    h.runOne();
    h.runOne();
    assert.equal(produced, 1);
    assert.equal(h.state.finalized, 1);
    assert.equal(h.state.keepalive, 0);
});

test("preserves a new request made during finalization", () => {
    const h = createHost(() => {
        if (h.state.finalized == 1) {
            h.schedule();
        }
    });
    h.schedule();
    h.runOne();
    assert.equal(h.state.finalized, 1);
    assert.equal(h.state.keepalive, 1);
    assert.notEqual(h.host.DOTNET.lastScheduledFinalizationId, undefined);
    h.runOne();
    assert.equal(h.state.finalized, 2);
    assert.equal(h.state.keepalive, 0);
});

for (const flag of ["ABORT", "isAborting"]) {
    test(`does not schedule during ${flag}`, () => {
        const h = createHost();
        const target = flag == "ABORT" ? h.host : h.host.DOTNET;
        target[flag] = true;
        h.schedule();
        assert.equal(h.state.scheduled, 0);
        assert.equal(h.state.keepalive, 0);
    });
}

test("preserves callback error handling and keepalive balance", () => {
    const error = new Error("finalization callback failed");
    const h = createHost(() => {
        throw error;
    });
    h.schedule();
    assert.throws(() => h.runOne(), value => value === error);
    assert.equal(h.state.aborted, 1);
    assert.equal(h.state.keepalive, 0);
});

test("does not propagate an exit status", () => {
    const h = createHost(() => {
        throw { status: 100 };
    });
    h.schedule();
    h.runOne();
    assert.equal(h.state.aborted, 0);
    assert.equal(h.state.keepalive, 0);
});
