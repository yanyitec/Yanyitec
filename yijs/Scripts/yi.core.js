"strict";
(function (Global, document) {

    var yi = Global.yi = Global.$y = Global.yi || {};
    var objProto = Object.prototype;
    var arrProto = Array.prototype;
    var aslice = arrProto.slice;
    var otoStr = objProto.toString;
    var invalid = yi.invalid = function () { throw new Error("Invalid operation."); }
    var each = yi.each = function (obj, cb, arg) { for (var n in obj) if (cb.call(obj, obj[n], n, arg) === false) break; }

    if (!yi.log) {
        var log = yi.log = Global.$log = function () { console.log.apply(console, arguments); }
        var emptyLog = function () { };
        yi.log.enable = emptyLog.enable = function () {
            yi.log = Global.$log = log;
        }
        yi.log.disable = emptyLog.disable = function () {
            yi.log = Global.$log = emptyLog;
        }
    }


    ///-----------------
    /// Eventable
    ///-----------------
    yi.Eventable = function () {

        this.subscribe = function (evtname, subscriber) {
            var ob = this["@eventable.observers"] || (this["@eventable.observers"] = {});
            var subscribers = ob[evtname] || (ob[evtname] = []);
            subscribers.push(subscriber);
            return this;
        }
        this.unsubscribe = function (evtname, subscriber) {

            var ob = this["@eventable.observers"]; if (!ob) return this;
            var subscribers = ob[evtname]; if (!subscribers) return this;
            for (var i = 0, j = subscribers.length; i < j; i++) {
                var existed;
                if ((existed = subscribers.shift()) !== subscriber) subscribers.push(existed);
            }
            return this;
        }
        this.emit = function (evtname, evtArgs, isApply) {
            var ob = this["@eventable.observers"]; if (!ob) return this;
            var subscribers = ob[evtname]; if (!subscribers) return this;
            for (var i = 0, j = subscribers.length; i < j; i++) {
                var subscriber = subscribers.shift();
                var rs = (isApply) ? subscriber.apply(this, evtArgs) : subscriber.call(this, evtArgs);
                if (rs !== '%discard' && rs !== '%discard&interrupt') subscribers.push(subscriber);
                if (rs === '%interrupt' || rs === '%discard&interrupt' || rs === false) return this;
            }
            return this;
        }


    }
    yi.Eventable.prototype = { toString: function () { return "yi.Eventable"; } }
    yi.Eventable.make = function (name, code) {
        var subscribeCode = "(this[\"!" + name + "\"]||(this[\"!" + name + "\"]=[])).push(listener);return this;";
        var unsubscribeCode = "var sb,fn;if(!(sb=this[\"!" + name + "\"]))return this;for(var i=0,j=sb.length;i<j;i++)if((fn=sb.shift())!==listener) sb.push(fn);return this;";
        var publishCode = "var sb,fn;if(!(sb=this[\"!" + name + "\"]))return this;for(var i=0,j=sb.length;i<j;i++){var fn = sb.shift();var rs = apply?fn.apply(this,params):fn.call(this,params);if(rs!==\"%discard\" && rs!=='%discard&interrupt')sb.push(fn);if(rs==='%interrupt' || rs==='%discard&interrupt' || rs===false)return this;}return this;";
        if (code) publishCode = code + publishCode;
        var result = {};
        result.subscribe = new Function("listener", subscribeCode);
        result.unsubscribe = new Function("listener", unsubscribeCode);
        result.emitCode = new Function("params", "apply", publishCode);
        return result;
    }
    //Observable自己就是全局监听器
    yi.Eventable.call(yi.Observable);
    yi.bind = function (func, me, args) { return function () { return func.apply(me || this, args || arguments); } }

    ///-----------------
    /// async
    ///-----------------
    yi.async = (function (yi) {
        var Async = function (interval, next, max) {
            this["@async.next"] = next;
            this["@async.interval"] = interval;
            this["@async.maxTimes"] = parseInt(max) || 5000;
            this["@async.contexts"] = [];
            var me = this;
            this["@async.handler"] = function () {
                var self = me, ctxs = self["@async.contexts"], interval = self["@async.interval"], max = self["@async.maxTimes"];
                var now = new Date();
                for (var i = 0, j = ctxs.length; i < j; i++) {
                    var ctx = ctxs.shift();
                    var result = ctx["@async.func"].call(ctx, ctx["@async.param"], ctx, self);
                    var keep = result === '%keep', reenter = result === '%reenter';
                    if (keep || reenter) {
                        if ((reenter && (++ctx["@async.times"] > max) || interval)) {
                            ctx["@async.times"] = 0;
                            var next = me["@async.next"];
                            if (next) next.add(ctx);
                            else ctxs.push(ctx);
                        } else {
                            ctxs.push(ctx);
                        }
                    }
                }
                if (ctxs.length === 0) {
                    var tick = self["@async.tick"];
                    self["@async.tick"] = 0;
                    if (interval) clearInterval(tick); 
                }else{
					if(interval===0) setTimeout(self["@async.handler"]);
				}
            }

            this.add = function (fn, arg) {
                var contexts = this["@async.contexts"];
                var me = this;
                contexts.push(fn["@async.func"] ? fn : { "@async.func": fn, "@async.param": arg, "@async.times": 0 });

                var interval = this["@async.interval"];
                if (!this["@async.tick"]) {
                    if (interval) this["@async.tick"] = setInterval(this["@async.handler"], interval);
                    else this["@async.tick"] = setTimeout(this["@async.handler"]);
                }

                return this;
            }
            this.remove = function (fn) {
                var contexts = this["@async.contexts"];
                for (var i = 0, j = contexts.length; i < j; i++) {
                    var ctx = contexts.shift();
                    if (ctx["@async.func"] !== fn) contexts.push(ctx);
                }
                return this;
            }

        }
        Async[7] = new Async(10000);//10秒
        Async[6] = new Async(5000, Async[7], 10);//5秒
        Async[5] = new Async(1000, Async[6], 20);//1秒
        Async[4] = new Async(500, Async[5], 30);//半秒
        Async[3] = new Async(200, Async[4], 50);//200毫秒
        Async[2] = new Async(100, Async[3], 50);//100毫秒
        Async[1] = new Async(40, Async[2], 65);//40毫秒
        var async_stack = Async[0] = new Async(0, Async[1], 0);// 0毫秒
        return yi.async = function (fn, arg) {
            if (fn === false) {
                for (var i = 0; i < 8; i++) Async[i].remove(arg);
                return;
            }
            async_stack.add(fn, arg);
        }
    })(yi);//end async

    ///-----------------
    /// Promise
    ///-----------------

    yi.Promise = (function (yi, async, each) {
        var Thenable = function (src, clearTgt) {
            this["@promise.invalid"] = invalid;

            this.done = function (onFullfill, remove) {
                if (typeof onFullfill !== 'function') throw new Error("Thenable.done expect a function as the first argument.");
                if (remove) {
                    var its = this["@promise.onFullfilled"]
                    if (its) for (var i = 0, j = its.length; i < j; i++) {
                        if ((it = its.shift()) !== onFullfill) its.push(it);
                    }
                    return this;
                }
                if (this["@promise.isFullfilled"]) {
                    var ret = onFullfill.call(this, this["@promise.value"], this);
                    return this;
                }
                (this["@promise.onFullfilled"] || (this["@promise.onFullfilled"] = [])).push(onFullfill);
                return this;
            };
            this.fail = function (onReject, remove) {
                if (typeof onReject !== 'function') throw new Error("Thenable.fail expect a function as the first argument.");

                if (remove) {
                    var its = this["@promise.onRejected"]
                    if (its) for (var i = 0, j = its.length; i < j; i++) {
                        if ((it = its.shift()) !== cb) its.push(it);
                    }
                    return this;
                }
                if (this["@promise.isRejected"]) {
                    onReject.call(this, this["@promise.value"], this); return this;
                }
                (this["@promise.onRejected"] || (this["@promise.onRejected"] = [])).push(onReject);
                return this;
            };
            this.then = function (onFullfilled, onRejected, onChanged) {
                if (onFullfilled) this.done(onFullfilled);
                if (onRejected) this.fail(onRejected);
                if (onChanged) this.change(onChanged);
                return this;
            }
            this.change = function (onChange, remove) {
                if (typeof onChange !== 'function') throw new Error("Thenable.done expect a function as the first argument.");
                if (remove) {
                    var its = this["@promise.onChanged"]
                    if (its) for (var i = 0, j = its.length; i < j; i++) {
                        if ((it = its.shift()) !== onChange) its.push(it);
                    }
                    return this;
                }
                (this["@promise.onChange"] || (this["@promise.onChanged"] = [])).push(onChange);
                return this;
            };

            this.always = function (cb, remove) {
                if (remove) {
                    var its = this["@promise.onFullfilled"]
                    if (its) for (var i = 0, j = its.length; i < j; i++) {
                        if ((it = its.shift()) !== cb) its.push(it);
                    }
                    its = this["@promise.onRejected"]
                    if (its) for (var i = 0, j = its.length; i < j; i++) {
                        if ((it = its.shift()) !== cb) its.push(it);
                    }
                    return this;
                }
                var rs = this["@promise.value"];
                if (this.isRejected || this.isFullfilled) {
                    cb.call(this, rs, this);
                    return this;
                }
                (this["@promise.onRejected"] || (this["@promise.onRejected"] = [])).push(cb);
                (this["@promise.onRullfilled"] || (this["@promise.onFullfilled"] = [])).push(cb);
            };
            this.thenable = function (tgt, clearTgt) {
                if (this["@promise.source"]) return this;
                var result = tgt;
                if (!tgt || tgt === this) result = new Then(this);
                else Thenable.call(tgt, this, clearTgt);

                return result;
            }

            if (src) {
                Then.call(this, src, clearTgt);
            } else {
                this.isFullfilled = function () { return this["@promise.isFullfilled"]; }
                this.isRejected = function () { return this["@promise.isRejected"]; }
            }
        }
        Thenable.prototype = { toString: function () { return "[object yi.Then.Thenable]"; } };

        var Then = function (src, clearTgt) {
            if (src) {
                this["@promise.source"] = src;
                if (src.await) this.await = function (m) { this["@promise.source"].await(m); return this; }
                comine("@promise.onFullfilled", this, src);
                comine("@promise.onRejected", this, src);
                comine("@promise.onChanged", this, src);
                if (src["@promise.isFullfilled"]) {
                    resolve.call(this, src["@promise.value"]);
                } else if (src["@promise.isRejected"]) {
                    reject.call(this, src["@promise.value"]);
                } else {
                    this.isFullfilled = function () { return this["@promise.source"]["@promise.isFullfilled"]; }
                    this.isRejected = function () { return this["@promise.source"]["@promise.isRejected"]; }
                }
            }
        }
        Then.prototype = new Thenable();
        Then.prototype.toString = function () { return "[object yi.Then]"; }
        var comine = function (name, dest, src) {
            var d = dest[name];
            var s = src[name];
            if (d) {
                if (s) {
                    for (var i = 0, j = d.length; i < j; i++) s.push(d[j]);
                    dest[name] = s;
                } else {
                    src[name] = d;
                }
            } else {
                dest[name] = src[name] = s || [];
            }

        }

        var resolve = function (value) {
            if (this.resolve) this.notify = this.resolve = this.reject = this["@promise.invalid"];

            this["@promise.isFullfilled"] = true; this["@promise.isRejected"] = false;
            this["@promise.status"] = 'fullfilled';
            this["@promise.value"] = value;
            //var dfd = this;
            //if(this["@promise.onChanged"]){
            //async(function(dfd){
            var its = this["@promise.onChanged"], status = this["@promise.status"];
            if (its) for (var i = 0, j = its.length; i < j; i++) {
                var it = its.shift();
                var rs = it.call(this, 'fullfilled', value, status, this);
                if (rs !== '%discard' && rs !== '%discard&interrupt') its.push(it);
                if (rs === '%interrupt' || rs == "%discard&interrupt" || rs == false) break;
            }
            //},this);
            //}
            //if(this["@promise.onFullfilled"]){
            //async(function(dfd){
            var its = this["@promise.onFullfilled"], value = this["@promise.value"];
            if (its) for (var i = 0, j = its.length; i < j; i++) its.shift().call(this, value, this);
            //},this);
            //}

            return this;
        }
        var reject = function (reason) {
            if (this.reject) this.notify = this.resolve = this.reject = this["@promise.invalid"];

            this["@promise.isFullfilled"] = false; this["@promise.isRejected"] = true;
            this["@promise.status"] = 'rejected';
            this["@promise.value"] = reason;
            //var dfd = this;
            //if(this["@promise.onChanged"]){
            //async(function(dfd){
            var its = this["@promise.onChanged"], status = this["@promise.status"];
            if (its) for (var i = 0, j = its.length; i < j; i++) {
                var it = its.shift();
                var rs = it.call(this, 'rejected', value, status, this);
                if (rs !== '%discard' && rs !== '%discard&interrupt') its.push(it);
                if (rs === '%interrupt' || rs == "%discard&interrupt" || rs == false) break;
            }
            //},this);
            //}
            //if(this["@promise.onRejected"]){
            //async(function(dfd){
            var its = this["@promise.onRejected"], value = this["@promise.value"];
            if (its) for (var i = 0, j = its.length; i < j; i++) its.shift().call(this, value, this);
            //},this);
            //}			
            return this;
        }
        var Promisable = function () {
            this.resolve = resolve;
            this.tryResolve = function (value) {
                if (this.resolve !== this["@promise.invalid"]) return this.resolve(value);
                return false;
            }
            this.reject = reject;
            this.tryReject = function (reason) {
                if (this.reject !== this["@promise.invalid"]) return this.reject(reason);
                return false;
            }
            this.notify = function (stat, value) {
                var its, status = this["@promise,status"];
                this["@promise.status"] = stat;
                this["@promise.value"] = value;
                if (its = this["@promise.onChanged"]) {
                    for (var i = 0, j = its.length; i < j; i++) {
                        var it = its.shift();
                        var rs = it.call(this, stat, value, status, this);
                        if (rs !== '%discard' && rs !== '%discard&interrupt') its.push(it);
                        if (rs === '%interrupt' || rs === '%discard&interrupt' || rs === false) break;
                    }
                }
                return this;
            }
        }
        Promisable.prototype = new Thenable();
        Promisable.prototype.toString = function () { return "[object yi.Promise.Promisable]"; }
        var Promise = function (whenFn, args) {
            if (whenFn) when.call(this, whenFn, args);
            return this;
        }
        Promise.prototype = new Promisable();
        Promise.prototype.toString = function () { return "[object yi.Promise]"; }

        var when = function (obj, args, nothenable) {
            if (this.when) this.when = this.defer = invalid;

            var t = typeof obj;
            if (t === 'function') {
                try {
                    obj.call(this, this, args);
                    return this;
                } catch (ex) {
                    this.reject(ex);
                }
                return nothenable ? this : this.thenable();
            }
            if (t === "object" && typeof obj.then === 'function') {
                var me = this;
                obj.then(function (value) {
                    me.resolve(value);
                }, function (reason) {
                    me.reject(reason);
                }, function (status, old, value) {
                    me.notify(status, value);
                });
                return nothenable ? this : this.thenable();
            }
            this.resolve(obj);
            return nothenable ? this : this.thenable();
        }
        var Whenable = function () {
            this.when = when;
            this.defer = function (func, args) {
                this["@promise.when_obj"] = func;
                this["@promise.when_arg"] = args;
                this["@promise.when"] = this.when;
                this.when = this.defer = invalid;
                async(function (promise) {
                    promise["@promise.when"].call(promise, promise["@promise.when_obj"], promise["@promise.when_arg"], true);
                }, this);
                return this.thenable();
            }

            this.timeout = function (milliseconds) {
                if (!promise["@promise.isFullfilled"] && !promise["@promise.isRejected"]) return this;
                milliseconds = parseInt(milliseconds);
                promise["@promise.milliseconds"] = milliseconds < 0 || Math.isNaN(milliseconds) ? 1000 : milliseconds;
                if (promise["@promise.startWaitingTime"]) {
                    promise["@promise.startWaitingTime"] = new Date().time();
                    return this;
                }
                async(function (promise, now) {
                    if (!promise["@promise.isFullfilled"] && !promise["@promise.isRejected"]) return "%wait";
                    if (!promise["@promise.startWaitingTime"]) promise["@promise.startWaitingTime"] = new Date().time();
                    else {
                        if (now.time() - promise["@promise.startWaitingTime"] > promise["@promise.milliseconds"]) {
                            promise.reject("Timeout");
                        }
                    }
                }, promise);
                return this;
            }
        }
        Whenable.prototype = new Promisable();
        Whenable.prototype.toString = function () { return "[object yi.When.Whenable]"; }
        //***
        //When
        var When = function (whenFn, arg1, arg2) {
            if (whenFn) {
                if (whenFn === '%concurrent') {
                    concurrentPromise(this, args, arg1);
                    this.when = this.defer = invalid;
                }
                if (whenFn === '%sequence') {
                    sequencePromise(this, args, arg1);
                    this.when = this.defer = invalid;
                }
                this.when(obj, args);
            }
        }
        When.prototype = new Whenable();
        When.prototype.toString = function () { return "yi.Promise.When"; }
        var concurrentPromise = function (dfd, obj, args) {
            var result = {};
            var count = 0, waiting_count = 0;
            for (var n in obj) {
                count++; waiting_count++;
                var sub = new Promise(obj[n], args);
                sub.index = count; sub.name = n;
                sub.done(function (value) {
                    var rs = result[this.name] = { name: this.name, value: value, promise: this, status: "fullfilled" };
                    dfd.notify(waiting_count, rs);
                    if (--waiting_count == 0) { dfd.resolve(result); }
                }).fail(function (reason) {
                    var rs = result[this.name] = { name: this.name, reason: value, promise: this, status: "rejected" };
                    dfd.notify(waiting_count, rs);
                    if (--waiting_count == 0) { dfd.resolve(result); }
                });
            }
            result.length = count;
            return dfd;
        }
        var sequencePromise = function (dfd, seq, args, dfd, prevValue) {
            var obj;
            while (!(obj = seq.shift()) || seq.length <= 0);
            if (!obj) return dfd.resolve(prevValue);

            var pomise = new Promise(obj, args);
            promise.done(function (value) {
                sequencePromise(seq, args, dfd, value);
            });
            return dfd;
        }

        each([Thenable, Then, Promise, Whenable, When], function (p) {
            each(p.prototype, function (m) {
                if (typeof m === 'function') m["@promise.primitive"] = true;
            });
        });
        var proxies = {};
        var createProxy = function (id, obj) {
            var Proxy;
            if (obj === undefined) { obj = id; id = null; }
            if (id) {
                Proxy = proxies[id];
                if (Proxy) return new Proxy(obj);
            }
            //msgProxy.load().enable().done(function(){});
            Proxy = function (obj) { this["@promise.proxy"] = obj; }
            var proto = new Whenable();
            proto["@promise.Proxy"] = Proxy;
            proto.toString = function () { return "yi.Promise.Proxy"; }
            each(obj, function (member, name) {
                if (typeof member !== 'function' || member["@promise.primitive"]) return;
                proto[name] = function () {
                    var targetPromise = this["@promise.proxy.promise"];
                    var newProxy = new this["@promise.Proxy"](obj);
                    if (targetPromise || targetPromise === null) {
                        if (targetPromise) {
                            if (targetPromise["@promise.isRejected"]) {
                                reject.call(newProxy, targetPromise["@promise.value"]);
                                return newProxy;
                            } else if (targetPromise["@promise.isFullfilled"]) {
                                resolve.call(newProxy, targetPromise["@promise.value"]);
                                return new Proxy;
                            }
                        }
                        var args = aslice.call(arguments);
                        newProxy["@promise.proxy.promise"] = null;
                        this.done(function () {
                            var newPromise = newProxy["@promise.proxy.promise"] = obj[name].apply(obj, args);
                            Then.call(newProxy, newPromise);
                            newPromise.done(function (value) {
                                resolve.call(newProxy, value);
                            }).fail(function (reason) {
                                reject.call(newProxy, value);
                            });
                        }).fail(function (reason) {
                            reject.call(newProxy, reason);
                        });
                        return newProxy;
                    } else {
                        var promise = newProxy["@promise.proxy.promise"] = obj[name].apply(obj, args);
                        promise.done(function (value) {
                            resolve.call(newProxy, value);
                        }).fail(function (reason) {
                            reject.call(newProxy, reason);
                        });

                    }

                    return newProxy;
                }
            });
            if (id) proxies[id] = Proxy;
            return new Proxy(obj);
        }

        Then.Thenable = Thenable;
        yi.Then = Then;
        Promise.Promisable = Promisable;
        yi.Promise = Promise;
        When.Whenable = Whenable;
        yi.When = When;
        Promise.proxy = createProxy;
        return Promise;
    })(yi, yi.async, each);

    ///-----------------
    /// Uri
    ///-----------------
    yi.Uri = (function (yi) {

        var Uri = yi.Uri = function (url) {
            var path = this.url = url;
            this.isAbsolute = false;
            var q = url.indexOf("?");
            if (q >= 0) {
                path = url.substring(0, q);
                this.qs = url.substr(q);
            }
            for (var i = 0, j = protocols.length; i < j; i++) {
                var protocol = protocols[i];
                if (path.substr(0, protocol.length) === protocol) {
                    this.isAbsolute = true;
                    this.protocol = protocol;
                    path = path.substr(protocol.length);
                    var d = path.indexOf("/");
                    if (d >= 0) {
                        this.domain = path.substring(0, d);
                        path = path.substring(d);
                    }
                    break;
                }
            }


            var f = path.lastIndexOf("/");
            if (f >= 0 && f < path.length) this.file = path.substr(f + 1);
            else this.file = path;
            var e = this.file.lastIndexOf(".");
            if (e >= 0) this.ext = this.file.substr(e);
            this.path = path;
            this.toString = function () { return this.url; }
        }
        var resolvedPaths = Uri.resolved = {};
        Uri.maps = {};
        var protocols = Uri.protocols = ["http://", "https://"];
        Uri.bas = "";

        var resolveUrl = Uri.resolve = function (name) {
            var uri, replaced = false;
            if (uri = resolvedPaths[name]) return uri;
            var url = name.replace(/\\/g, "/"), paths = Uri.maps;
            for (var n in paths) {
                n = n.replace(/[/\\]$/g, "");
                if (name.length < n.length) break;

                if (name.substr(0, n.length) === n && name[n.length] === '/') {
                    var k = paths[n].replace(/[\\/]$/g, "");
                    url = k + "/" + name.substring(n.length);
                    replaced = k;
                    break;
                }
            }
            var isAbs = false;
            for (var i = 0, j = protocols.length; i < j; i++) {
                var protocol = protocols[i];
                if (url.substr(0, protocol.length) === protocol) {
                    isAbs = true; break;
                }
            }
            if (!isAbs) url = (Uri.bas || "").replace(/[\\/]$/g, "") + "/" + url;
            uri = resolvedPaths[name] = new Uri(url);
            return uri;
        }
        var imgexts = Uri.imageExts = [".gif", ".png", ".jpg"];
        var isImageExt = Uri.isImageExt = function (ext) {
            for (var i = 0, j = imgexts.length; i < j; i++) if (imgexts[i] === ext) return true;
            return false;
        }
        return Uri;
    })(yi);

    ///*************
    ///* require
    ///*****

    yi.Require = (function (yi, Whenable, Uri, async) {
        var head;
        var loadRes = yi.loadRes = function (url, type) {
            var hd = head;
            if (!hd) {
                hd = document.getElementsByTagName("HEAD");
                if (hd[0]) {
                    hd = head = hd[0];
                } else hd = document.body || document.documentElement;
            }
            var df = new Whenable(), elem;
            if (type === '.js') {
                elem = document.createElement("SCRIPT");
                elem.src = url;
            } else if (type === '.css') {
                elem = document.createElement("LINK");
                elem.href = url; elem.type = "text/css"; elem.rel = "stylesheet";
            } else if (type === '.img') {
                elem = document.createElement("img");
            }
            df.element = elem;
            elem.onerror = function () { df.reject(this); }
            if (elem.onload || elem.onload === null) elem.onload = function () { df.resolve(this); };
            else elem.onstatechange = function () { if (elem.readyState === 4 || elem.readyState === 'complete') df.resolve(this); }
            if (type === '.img') elem.src = url; else hd.appendChild(elem);
            return df.whenable();
        }

        var waitingReqires = [], loadedRequire;
        ///请求管理器。管理请求Require
        var requireManager = {
            //被挂起的Require
            suspends: [],
            defineCount: 0,
            caches: {},
            //刚加载的Require
            loaded: null,
            //请求一个Require
            aquire: function (deps, initial) {
                var req = new Require(initial);
                var suspend = { req: req, deps: deps, isLoading: false };
                this.suspends.push(suspend);
                //如果挂起队列中只有刚请求的那个require，该require直接开始
                if (this.suspends.length === 1) {
                    suspend.isLoading = true;
                    req.loadDeps(deps);
                }
                return req;
            },
            //每一轮请求之后，都要resetLoading,
            resetLoading: function () {
                this.defineCount = 0;
                this.loaded = null;
            },
            //当某个req已经完成所有加载
            //该req里面要求的所有Require会被从suspend状态转为resumes状态。
            loadComplete: function (req) {
                var suspends = this.suspends, resumes = [], removeCount = 0;
                for (var i = 0, j = suspends.length; i < j; i++) {
                    var it = suspends.shift();
                    if (it.req !== req) {
                        suspends.push(it);
                        if (!it.isLoading) resumes.push(it);
                    } else removeCount++;
                }
                if (this.resuming) return removeCount;
                this.resuming = true;
                requireManager.loaded = null; this.defineCount = 0;
                for (var i = 0, j = resumes.length; i < j; i++) {
                    var it = resumes[i];
                    it.isLoading = true;
                    it.req.loadDeps(it.deps);
                }
                this.resuming = false;
                return removeCount;
            }
        };

        var reqid = 0;
        var Require = yi.Require = function (init) {
            this._taskCount = 0;
            this._intial = init;
            this.id = reqid++;
            var mod = this.isModule = init ? true : false;
            if (mod) requireManager.defineCount++;
            requireManager.loaded = this;
            //else loadedRequire.next = this;
        }
        Require.prototype = new yi.When.Whenable();
        Require.prototype.loadDeps = function (dep_names) {

            yi.log(this.id + " invoke loadDeps", dep_names);
            var deps = {}, me = this;
            this._taskCount++;//loadDeps自己就是个waiting
            var c = this._loadingCount = dep_names.length;
            for (var i = 0, j = c; i < j; i++) {
                if (this.isRejected) break;
                //每加载一个依赖项就添加一个任务计数
                this._taskCount++;

                var dep_name = dep_names[i];
                var dep = deps[dep_name] = { name: dep_name };
                var cached = requireManager.caches[dep_name];
                if (cached) {
                    (function (cached, dep, me, deps) {
                        cached.success(function (result) {
                            deps[dep.name] = result;
                            if (--me._taskCount == 0 && !me.isRejected) me._ready();
                        });
                    })(cached, dep, me, deps);

                } else {

                    //var dep_req = cached[dep_name];
                    //if(dep_req)
                    (function (dep_name, dep, me) {
                        var uri = resolveUrl(dep_name);
                        loadRes(uri.url, ".js").done(function () { me._depLoaded(dep); }).fail(function (elem) {
                            me.reject({ src: this, args: elem });
                        });
                    })(dep_name, dep, this);
                }
            }
            this._deps = deps;
            //任务结束
            if (--this._taskCount == 0 && !this.isRejected) this._ready();
            return this;
        }
        Require.prototype._depLoaded = function (dep) {
            yi.log(dep.name + " is into _depLoaded.");
            //每完成一次加载，就减去一次任务计数
            var me = this; this._taskCount--;
            if (requireManager.defineCount > 1) {
                var error = "Module should only define once in a file[" + res.name + "].";
                this.reject({ error: error, res: res });
                throw error;
                return;
            }
            var loaded = requireManager.loaded;
            if (!loaded) {
                //刚加载的没有module ,当作已经调用过了init
                yi.log(me.id + " loaded a non-module js->" + dep.name);
                if (this._taskCount === 0) this._ready();

            } else {
                yi.log(me.id + " is waiting " + loaded.id);
                loaded.name = dep.name;
                requireManager.caches[dep.name] = loaded;
                //里面有require/define，可能发生等待，添加一个任务计数(等待依赖项变成ready状态)
                me._taskCount++;
                loaded.success(function () {
                    //依赖项变成了ready，相关的任务计数减一
                    if (--me._taskCount === 0) me._ready();
                });
            }
            //刚加载的Require已经处理，清空Require,下次callback的时候loadedRequire里面始终是最近一次加载的js里面的Require
            requireManager.resetLoading();
            if (--this._loadingCount == 0) {
                if (requireManager.loadComplete(this) == 0) throw "There are no require in require manager.";
            }
        }
        Require.prototype._ready = function () {
            if (!this._initial) {
                yi.log(this.id + " resolved with not initial.");

                this.resolve(); return;
            }
            var args = [], deps = this._deps;
            for (var n in deps) args.push(deps[n].result);
            this._initial.call(this, args);
            var me = this;
            this.success(function (value) {
                me.module_instance = value;
            });
        }
        var require = yi.require = Global.$require = function (_deps) {
            var deps = typeof _deps === 'string' ? slice.call(arguments) : _deps;
            var req = requireManager.aquire(deps);
            return req;
        }
        yi.define = Global.$define = function (deps, initial) {
            var req = requireManager.aquire(deps, initial);
            return req;
        }
        return Require;
    })(yi, yi.Promise.Whenable, yi.Uri, yi.async);

    yi.Observable = (function (yi) {
        var seed = 1;
        //兼容google 的代码，google的函数这些文字在function中是保留属性名
        var reservedPropertyNames = {
            "name": "name_",
            "arguments": "arguments_",
            "length": "length_",
            "caller": "caller_",
            "prototype": "prototype_",
            "constructor": "constructor_"
        };
        var Observable = function (name, target) {
            target || (target = {});
            name || (name = '@observable.prop-' + (seed == 210000000 ? 1 : seed++));
            this["@observable.object"] = target;
            this["@observable.name"] = name;
            var me = this;
            var accessor = function (value) {
                var self = me;
                if (value === undefined) return self["@observable.object"][self["@observable.name"]];
                self.setValue(value);
                return self["@observable.accessor"];
            }
            Accessor.call(accessor);
            accessor.toString = function () { return me.getValue(); }
            accessor["@object-like"] = true;
            this.accessor = this["@observable.accessor"] = accessor["@observable.accessor"] = accessor;
            this["@observable.observable"] = accessor["@observable.observable"] = this;
        }
        Observable.prototype = {
            $type: "yi.Observable",
            toString : function(){return "[object yi.Observable]";},
            //设置或获取某个Observable的名字，该名字也就是属性名
            name: function (value) {
                if (value === undefined) return this["@observable.name"];
                this["@observable.name"] = value; return this;
            },
            object : function (target, source) {
                //get/set要观察的目标对象
                // target 要观察的对象
                //对象。如果是set操作则返回监听器本身
                //get
                var old = this["@observable.object"];
                if (target === undefined) return old;
                //set
                this["@observable.object"] = target || (target = {});
                //监听目标对象改变后，重新给本监听器赋值，以触发事件
                var name = this["@observable.name"];
                if (old !== target) this.setValue(target[name], "object.change", source,old[name],true);
                return this;
            },
            subject: function (subject) {
                ///get/set该观察器的主体对象(主体观察器)。当一个观察器有主体对象时，表示该观察器是主体对象的一个属性",
                ///        "subject": "要设置的主体对象。必须是另一个Observable。如果该参数设置为‘%root’，返回根"
                ///    returns: "对象。如果是set操作则返回监听器本身。否则返回主体观察器"
                if (subject === undefined) return this["@observable.subject"];
                if (subject === "%root") {
                    var sub = this["@observable.subject"];
                    return sub ? sub.subject("root") : sub;
                }
                var old = this["@observable.subject"];
                //原先的跟新的一样，就什么都不做
                if (old === subject) return this;

                this["@observable.subject"] = subject;
                if (old) {
                    var name = this["@observable.name"];
                    //清除掉原来subject里面的东西
                    delete old["@observable.props"][name];
                    var accor = old.accessor();
                    delete accor[reservedPropertyNames[name] || name];
                }
                var new_targ = subject["@observable.target"] || (subject["@observable.target"] = {});
                this.target(new_targ);
                //数组的item不会当作prop
                if (subject.isArray && typeof name !== 'number') {
                    (subject["@observable.props"] || (subject["@observable.props"] = {}))[name] = this;
                    var accor = subject.accessor();
                    accor[reservedPropertyNames[name] || name] = this.accessor();
                }
                return this;
            },
            
            getValue: function () {
                return this["@observable.object"][this["@observable.name"]];
            },
            setValue: function (new_v, reason, _source,_old,_nocompare) {
                /// <summary>get/set该监听器在目标对象上的值</summary>
                /// <param name="new_v" type="Anything">要设定的新值。如果该参数为undefined，表示获取目标对象上的值</param>
                /// <param name="reason" type="String | not_trigger">变化原因，传递给事件函数用，默认是valuechange。</param>
                /// <param name="_source" type="Object">之名该变化是由哪个个源事件引起</param>
                /// <returns type="Object">监听器本身</returns>
                //get
                var targ = this["@observable.object"], name = this["@observable.name"];
                
                //获取旧值，如果跟新值一样就直接拿返回
                if (!_nocompare) {
                    _old = targ[name];
                    if (_old === new_v) return this;
                    //set value to target
                    targ[name] = new_v;
                }
                
                //表示不需要触发事件，只是设置一下值
                //跳过后面的事件处理
                if (this["@observable.disabledTrigger"]) return this;
                //构建事件参数
                var evtArgs = { type: "valuechange", sender: this, value: new_v,old:_old, reason: (reason || "value.set"), source: _source };

                //获取到该监听上的所有下级监听器
                var props = this["@observable.props"];

                if (props) for (var n in props) props[n].object(new_v, evtArgs);
                var items = this["@observable.items"];
                if (items) {
                    for (var i in items) {
                        var it = items[i];
                        var it_evt = { type: "valuechange", sender: it, value: it.getValue(),index:i, reason: "array.reset", source: evtArgs, index: i };
                        it.trigger(it_evt,false);
                    }
                    //this._initArrayData(this["@observable.itemTemplate"], this.value);
                }
                this.trigger("valuechange", evtArgs, this["observable.bubble"]);
                //this.childchange(evtArgs);
                return this;
            },
            
            bubble: function (value) {
                if (value === undefine) return this["@observable.bubble"];
                this["@observable.bubble"] = value; return this;
            },
            subscribe: function (evtname, callback) {
                if (callback === undefined) { callback = evtname; evtname = "valuechange";}
                if(typeof callback!=='function') throw new Error("invalid argument");
                var obs = this['@observable.subscribers'] || (this['@observable.subscribers'] =[]);
                (obs[evtname]|| (obs[evtname]=[])).push(callback);
                return this;
            },
            unsubscribe: function (evtname, callback) {
                var obs = this['@observable.subscribers'], its,it;
                if (!(its = obs[evtname])) return this;
                for (var i = 0, j = its.length; i < j; i++) if ((it = its.shift()) !== callback) its.push(it);
                return this;
            },
            enableTrigger: function () { 
                //启用事件触发
                this["@observable.triggerDisabled"] = false;
                if (this["@observable.triggler"]) this.trigger = this["@observable.triggler"];
                return this;
            },
            disableTrigger: function () {
                //禁用事件触发
                this["@observable.triggerDisabled"] = true;
                this["@observable.triggler"] = this.trigger;
                this.trigger = function () { return this; };
                return this;
            },
            trigger: function (evtname, args, bubble) {
                /// <summary>触发某个事件/禁用事件/启用事件</summary>
                /// <param name="evtname" type="String">事件名。如果该函数第2个参数没有，evtname='valuechange'.如果该值设置为enabled/disabled对象，表示启用/禁用事件</param>
                /// <param name="evt" type="Object">事件对象</param>
                /// <returns type="Object">监听器本身</returns>
                
                if (this["@observable.triggerDisabled"]) return this;
                var obs = this['@observable.subscribers'], its, it;
                if (!obs) return this;
                if (its = obs[evtname]) for (var i = 0, j = its.length; i < j; i++) {
                    var it = its.shift();
                    var result = it.call(this, args);
                    if (result !== '%discard' && result !== '%discard&interrupt') its.push(it);
                    if (result === '%interrupt' || result === '%discard&interrupt' && result === false) break;
                }
                if (bubble === undefined) bubble = this["@observable.bubble"];
                //如果没有禁用bubble,事件本身也没有取消冒泡
                if (bubble !== false && !args.cancelBubble) {
                    var sup = this.subject();
                    if (sup) {
                        var evtArgs = { type: args.type, sender: this, value: args.value, reason: "bubble", source: (args.source || args) };
                        sup.trigger(name, evtArgs, bubble);
                    }
                }
                return this;
            },

            prop: function (names, value) {
                var props = this["@observable.props"], target;
                if (props) {
                    target = this["@observable.object"][this["@observable.name"]];
                } else {
                    props = this["@observable.props"] = {};
                    target = this["@observable.object"][this["@observable.name"]] = {};
                }
                var isArr = false;
                if (otoStr.call(names) === '[object Array]') {
                    isArr = true;
                } else {
                    names = [names];
                }
                var rs = {};
                for (var i = 0, j = names.length; i < j; i++) {
                    var name = names[i];
                    var prop = props[name];
                    if (!prop) {
                        prop = props[name] = new Observable(name, target);
                        var aname = reservedPropertyNames[name] || name;
                        this["@observable.accessor"][aname] = prop["@observable.accessor"];
                        prop["@observable.subject"] = this;
                        if (otoStr.call(value) === '[object Array]') prop.asArray();
                    }
                    if (isArr) rs[name] = prop;
                }
                
                if (!isArr && value !== undefined) prop.setValue(value);
                return isArr?rs : prop;
            },
            asArray: function (define) {
                var value = this.getValue();
                if (otoStr.call(value) !== '[object Array]') this["@observable.object"] = [];
                ObservableArray.call(this);
                ObservableArray.call(this["@observable.accessor"]);
                this.asArray = function (def) {
                    if (def) this.template().define(def);
                    return this;
                }
                if (define) this.template().define(define);
                return this;
            },

            validate: function (onlyme) {
                var def = this["@observable.define"], rules;
                if (!def) return true;
                if (rules = def.rules) {
                    var val = this["@observable.object"][this["@observable.name"]];
                    if (rules["trim"]) {
                        if (val === undefined || val === null) val = "";
                        else val = val.toString().replace(/(^\s+)|(\s+$)/g, "");
                    }
                    var valids = yi.validations;
                    for (var n in rules) {
                        var check = valids[n];
                        if (!check) continue;
                        if (!check(val)) {
                            this.trigger("validate", { type: "validate", sender: this, value: val });
                            return false;
                        }
                    }
                }
                if (onlyme) return true;
                var props = this["@observable.props"], result = true;
                if (props) for (var n in props) {
                    var prop = props[n];
                    var result = result && prop.validate();
                }
                return result;
            },
            define: function (defination) {
                var df = {
                    $type : "object",
                    conditions: {
                        $type: "object",
                        name: {
                            $type: "text",
                            rules: {
                                "require": true,
                                "trim": true,
                                "length": [5, 20]
                            }
                        },
                        email: {
                            $type: "email",
                            rules: {
                                "require": true,
                                "trim": true,
                                "email": true
                            }
                        },
                        gender: {
                            $type: "enums",
                            enums: {
                                0: "male",
                                1: "female"
                            }
                        },
                        marraged: {
                            $type: "boolean"
                        }
                    },
                    rows: {
                        $type: "array",
                        id: {
                            $type: "number"
                        },
                        name: {
                            $type:"text"
                        }
                    },
                    recordCount: {
                        $type: "number",
                        readonly: true
                    },
                    pagecount: {
                        $type: "number",
                        readonly: true
                    },
                    pageno: {
                        $type: "number",
                        readonly: true
                    }
                };
                if (!defination) return this["@observable.define"];
                if (defination.$defination) {
                    this["@observable.define"] = defination;
                    defination = defination.value;
                }
                if (!defination) return this;
                var props = this["@observable.props"] || (this["@observable.props"] = {});
                for (var pname in defination) {
                    var member = defination[n];
                    var prop = props[name] = new Observable(pname, {}).subject(this);

                    if (typeof member === 'object') {
                        if (member.length !== undefined) {
                            var tmp;
                            if (member.length > 0) tmp = new Observable(0, []).define(member[0]);
                            prop.asArray(tmp);
                        } else {
                            prop.define(member);
                        }
                    }
                }
                return this;
            },

            clone: function (object, evtInc) {
                object || (object = this["@observable.object"]);
                var name = this["@observable.name"];
                var clone = new Observable(name, object);
                var props = this["@observable.props"];
                var target = clone.getValue();
                
                if (props) {
                    var cloneProps = {};
                    if (!target) target = object[name] = {};
                    for (var propname in props) {
                        var prop = props[propname];
                        var cloneProp = cloneProps[propname] = prop.clone(target);
                        clone.accessor[reservedPropertyNames[propname] || propname] = cloneProp["@observable.accessor"];
                        cloneProp["@observable.subject"] = clone;
                    }
                    clone["@observable.props"] = cloneProps;
                }
                if (evtInc) {
                    var subsers = this["@observable.subscribers"];
                    if (subsers) {
                        var newSubs = {};
                        for (var n in subsers) {
                            var lsns = subsers[n];
                            var clonelsns = [];
                            for (var i = 0, j = lsns.length; i < j; i++) clonelsns.push(lsns[i]);
                            newSubs[n] = clonelsns;
                        }
                        clone["@observable.subscribers"] = newSubs;
                    }
                }
                if (this.isArray) clone.asArray(this["@observable.template"]);
                return clone;
            }
        };
        var Accessor = function () {
            //this["@observable.accessor"] = accor["@observable.accessor"] = accor;
            this.subscribe = function (evtname, subscriber) {
                this["@observable.observable"].subscribe(evtname, subscriber);
                return accor;
            }
            this.unsubscribe = function (evtname, subscriber) {
                this["@observable.observable"].unsubscribe(evtname, subscriber);
                return accor;
            }
            this.asArray = function (template) {
                var me = this["@observable.observable"];
                me.asArray(template);
                return accor;
            }
            this.define = function (model) {
                var me = this["@observable.observable"];
                if (!model) return me["@observer.define"];
                me.define(model);
                return actor;
            }
            this.validate = function (onlyme) {
                var me = this["@observable.observable"];
                return me.validate(onlyme);
            }
            return this;
        }
        
        var ObservableArray = function () {

            this.template = function (v) {
                var me = this["@observable.observable"];
                if (v === undefined) {
                    return me["@observable.template"] || (me["@observable.template"] = new Observable(0, []));
                }
                this["@observable.template"] = v;
                return this;
            }
            this.count = function () {
                var me = this["@observable.observable"];
                return me["@observable.object"][me["@observable.name"]].length;
            }
            this.push = function (item) {
                var me = this["@observable.observable"],
                    arr = me["@observable.object"][me["@observable.name"]];//, items = me["@observable.items"], item;
                arr.push(item);
                //var it_evt = { sender: item, value: it, reason: "array.push" };
                var arr_evt = { type: "valuechange", sender: me, value: arr, reason: "array.push" };
                me.trigger("valuechange", arr_evt);
                return this;
            }
            this.pop= function () {
                var me = this["@observable.observable"],
                    arr = me["@observable.object"][me["@observable.name"]],
                    items = me["@observable.props"];
                var it = arr.pop();
                if (items) {
                    var item = items[arr.length], it_evt;
                    if (item) {
                        delete items[arr.length];
                        it_evt = { type: "remove", sender: item, value: it, reason: "array.pop" };
                        item.trigger("remove", it_evt,false);
                    }
                }
                var arr_evt = { type: "valuechange", sender: me, value: arr, reason: "array.pop", item: it };
                me.trigger("valuechange", arr_evt);
                return it;
            }
            this.unshift = function (it) {
                var me = this["@observable.observable"],
                    arr = me["@observable.object"][me["@observable.name"]],
                    items = me["@observable.props"];
                arr.unshift(it);
                if (items) {
                    for (var n in items) {
                        var item = items[n];
                        var it_evt = {
                            type: "valuechange",
                            sender: item,
                            value: arr[n],
                            index:n,
                            reason: "array.unshift"
                        };
                        item.trigger("valuechange", it_evt,false);
                    }
                }
                var arr_evt = { type: "valuechange", sender: me, value: arr, reason: "array.unshift",item:it };
                me.trigger("valuechange", arr_evt);
                return this;
            }

            this.shift = function (it) {
                var me = this["@observable.observable"],
                    arr = me["@observable.object"][me["@observable.name"]],
                    items = me["@observable.props"];
                var it = arr.shift(), cancled = false;
                if (items) {
                    var item = items[0], rmv_evt;
                    if (item) {
                        delete items[0];
                        rmv_evt = { type: "remove", sender: item, value: it, reason: "array.shift", index: 0 };
                        item.trigger("remove", rmv_evt, false);
                        
                    }
                    for (var n in items) {
                        if (n == 0) continue;
                        var item = items[n];
                        var it_evt = {
                            type: "valuechange",
                            sender: item,
                            value: arr[n],
                            index: n,
                            reason: "array.shift",
                            source: rmv_evt
                        };
                        item.trigger("valuechange", it_evt,false);
                    }
                    
                }
                var arr_evt = { type: "valuechange", sender: me, value: arr, reason: "array.shift" };
                me.trigger("valuechange", arr_evt);

                return it;
            }
            this.removeAt= function (at) {
                var me = this["@observable.observable"],
                    arr = me["@observable.object"][me["@observable.name"]],
                    items = me["@observable.props"];
                if (at<0 || arr.length <= at) return this;
                var it;
                for (var i = 0, j = arr.length; i < j; i++) {
                    var itat = arr.shift();
                    if (i === at) { it == itat; } else arr.push(itat);
                }
                
                if (items) {
                    var item = items[at], rm_evt;
                    if (item) {
                        delete items[at];
                        it_evt = { type: "remove", sender: item, value: it, reason: "array.remove", index: at };
                        item.trigger("remove", rm_evt, false);
                    }

                    for (var n in items) {
                        if (n <at) continue;
                        var item = items[n];
                        var it_evt = {
                            type: "valuechange",
                            sender: item,
                            value: arr[n],
                            index: n,
                            reason: "array.shift",
                            source: rm_evt
                        };
                        item.trigger("valuechange", it_evt, false);
                    }
                    
                }
                var arr_evt = { type: "valuechange", sender: me, value: arr, reason: "array.remove",index:at,item:it };
                me.emit("valuechange", arr_evt);
            }

            this.clear =function () {
                var me = this["@observable.observable"],
                    arr = me["@observable.object"][me["@observable.name"]],
                    items = me["@observable.props"];
                for (var i = 0, j = arr.length; i < j; i++) arr.pop();
                if (items) {
                    for (var i = 0, j = items.length; i < j; i++) {
                        var it_evt = { type: "remove", sender: item, value: value, reason: "array.clear", index: i };
                        //不冒泡，处理完成后统一给array发送消息
                        items[i].publish("remove", it_evt, false);
                    }
                    me["@observable.props"] = null;
                }
                
                var arr_evt = { type: "valuechange", sender: me, value: it, reason: "array.clear" };
                me.trigger("valuechange", arr_evt);
                return this;
            }
            
            this.getItem = function (at, cache) {
                var me = this["@observable.observable"],
                    arr = me["@observable.object"][me["@observable.name"]],
                    items = me["@observable.props"];
                if (at<0 || at >= arr.length) throw new Error("InvalidArguments:Out of range");

                if (!items) items = me["@observable.props"] = {};
                var item = items[at];
                if (item) return item;
                item = me.itemTemplate().clone();
                item["@observable.name"] = at;
                item["@observable.subject"] = me;
                item.disableTrigger();
                item.object(arr);
                item.enableTrigger();
                if(cache)items[at] = item;
                return item;
            }
            this.setItem = function (at, value,cache) {
                var me = this["@observable.observable"],
                    arr = me["@observable.object"][me["@observable.name"]],
                    items = me["@observable.props"], item;
                if (at < 0 || at >= arr.length) throw new Error("InvalidArguments:Out of range");
                if (!items) items = me["@observable.props"] = {};
                if (item = items[at]) {
                    item.setValue(value,"array.item");
                    return this;
                } else {

                    arr[at] = value;
                    var arr_evt = { type: "valuechange", sender: me, value: arr, reason: "array.item" };
                    me.trigger("valuechange", arr_evt);
                    if (cache) {
                        item = me.itemTemplate().clone();
                        item["@observable.name"] = at;
                        item["@observable.subject"] = me;
                        item.disableTrigger();
                        item.object(arr,arr_evt);
                        item.enableTrigger();
                        items[at] = item;
                    }
                }
                return this;
            }
        }
        yi.observable = function (value) {
            var ret = new Observer("",{"":value});
            return ret["@observable.accessor"];
        }
        return Observable;
    })(yi);



    

    
    var urlreg = new RegExp('^((https|http|ftp|rtsp|mms)?://)'
		+ '?(([0-9a-z_!~*\'().&=+$%-]+: )?[0-9a-z_!~*\'().&=+$%-]+@)?' //ftp的user@ 
		+ '(([0-9]{1,3}.){3}[0-9]{1,3}' // IP形式的URL- 199.194.52.184 
		+ '|' // 允许IP和DOMAIN（域名） 
		+ '([0-9a-z_!~*\'()-]+.)*' // 域名- www. 
		+ '([0-9a-z][0-9a-z-]{0,61})?[0-9a-z].' // 二级域名 
		+ '[a-z]{2,6})' // first level domain- .com or .museum 
		+ '(:[0-9]{1,4})?' // 端口- :80 
		+ '((/?)|' // a slash isn't required if there is no file name 
		+ '(/[0-9a-z_!~*\'().;?:@&=+$,%#-]+)+/?)$');

    yi.validations = {
        "require": function (value) {
            var t = typeof value;
            if (t === 'function') return true;
            if (t === 'undefined') return false;
            if (t === 'number') return true;
            if (t === 'string') return value !== "";
            if (t === 'object') {
                if (value === null) return false;
                if (value.length) return value.length > 0;
                return true;
            }
            return false;
        },
        "length": function (value, params) {
            var t = typeof params; var len = value.toString().length;
            if (t === 'number') return len <= params;
            var min = params[0] || 0; var max = params[1];
            if (min !== undefined && len < min) return false;
            if (max !== undefined && len > max) return false;
            return true;
        },
        "date": function (value) {
            var reg = /^(?:(?!0000)[0-9]{4}-(?:(?:0[1-9]|1[0-2])-(?:0[1-9]|1[0-9]|2[0-8])|(?:0[13-9]|1[0-2])-(?:29|30)|(?:0[13578]|1[02])-31)|(?:[0-9]{2}(?:0[48]|[2468][048]|[13579][26])|(?:0[48]|[2468][048]|[13579][26])00)-02-29)$/g;
            return reg.test(value);
        },
        "number": function (value) {
            var reg = /^[\+\-]?(?:\d{1,3}){0,1}(?:\d{3})*\d(?:.\d+)$/g;
            return reg.test(value);
        },
        "limit": function (value, params) {
            var min = parseFloat(params[0]) || 0;
            var max = parseFloat(params[1]);
            var val = parseFloat(value);
            if (value === Math.NaN) return false;
            if (min !== Math.NaN && len < min) return false;
            if (max !== Math.NaN && len > max) return false;
            return true;
        },
        "email": function (value) {
            var reg = /^[A-Za-z\d]+([-_.][A-Za-z\d]+)*@([A-Za-z\d]+[-.])+[A-Za-z\d]{2,5}$/g;
            return reg.test(value);
        },
        "url": function (value) {
            return urlreg.test(value);
        },
        "reg": function (value, params) {
            var reg = new RegExp(params);
            return reg.test(value);
        }
    }
})(window, document);