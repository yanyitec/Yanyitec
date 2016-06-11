(function (Global, document, undefined) {
    var yi = Global.yi || (Global.yi = {});
    var objProto = Object.prototype;
    var arrProto = Array.prototype;
    var aslice = arrProto.slice;
    var otoStr = objProto.toString;
	var log = Global.$log = yi.log = function () {
		var logger = log;
        if (arguments.length == 0 || logger.isLogDisabled) return this;
            var lvfn;
            var lvs = logger["@levels"];
            if ((lvfn = lvs[lv])) {
                if (!lvfn.isLogDisabled) logger["@output"].call(logger, lv, arguments,1);
            } else {
                logger["@output"].call(logger, logger["@defaultLevel"], arguments);
            }
            return this;
    }
    log.outputAggregation = function (type,contents) {
        var ret = function () {
            for (var i = 0, j = fns.length; i < j; i++) fns[i].call(type,contents);
        }
        var fns = ret["@aggregations"]=[];
        ret.addAggregation = function (fn) {
            fns.push(fn); return this;
        }
        ret.removeAggregation = function (fn) {
            for (var i = 0, j = fns.length; i < j; i++) {
                var it = fns.shift();
                if (it !== fn) exist.push(it);
            }
            return this;
        }
        ret.getAggregation = function (i) { return fns[i]; }
        ret.existAggregation = function (it) {
            for (var i = 0, j = fns.length; i < j; i++) {
                if (fns[i] === it) return true;
            }
            return false;
        }
        ret.count = function () { return fns.length;}
        return ret;
    }
    log.defaultOutput = function (lv, params,start) {
        try {
            params || (params = []);
            var t = new Date();
            var s = lv || "#debug";
			if(start){
				if(!params.shift)params = aslice.call(params);
				for(var i=0;i<start;i++) params.shift();
			}
			if(!params.unshift)params = aslice.call(params);
            params.unshift(s);
            console.log.apply(console, params);
        } catch (ignore) { }
    };
    log.Logger = function (_opts) {
		this.toString = function(){return "[object,yi.log.Logger]";}
        var opts = {
            output: log.defaultOutput,
            levels: ["debug","fail","warming","notice","info",'success',"assert"],
            defaultLevel: "debug"
        }
        if (_opts) for (var n in _opts) opts[n] = _opts;
        this.opts = opts;
        this.log = function (lv) {
            if (arguments.length == 0 || this.isLogDisabled) return this;
            var lvfn;
            var lvs = this["@levels"];
            if ((lvfn = lvs[lv])) {
                if (!lvfn.isLogDisabled) this["@output"].call(this, lv, arguments,1);
            } else {
                this["@output"].call(this, this["@defaultLevel"], arguments);
            }
            return this;
        }
        
        this.on = function () {
			if(arguments.length==0){
				this.isLogDisabled = false;
				return this;
			}
            
            var lvs = this["@levels"];
            for (var i = 0, j = arguments.length; i < j; i++) {
                var n = arguments[i]; if (!n) continue;
                var stored = lvs["##" + n];
                if (stored) {
                    this[n] = stored;
                    stored.isLogDisabled = false;
                }
            }
            return this;
        }
        this.off = function () {
            if (arguments.length == 0) {
                this.isLogDisabled = true; return this;
            }
            var lvs = this["@levels"];
            for (var i = 0, j = arguments.length; i < j; i++) {
                var n = arguments[i]; if (!n) continue;
                var stored = lvs["##" + n];
                if (!stored) continue;
                stored.isLogDisabled = true;
                this[n] = function () { return this; };
            }
            return this;
        }
        var config = function (output, lvs) {
            var levels = this["@levels"];
            var elvs = [];
            for (var n in levels) {
                var name = n.substring(2);
                elvs.push(name);
                delete this[name];
            }
            if (lvs) this["@levels"] = levels = {};
            else lvs = elvs;
            if (!output) output = this["@output"];
            else this["@output"] = output;
            for (var i in lvs) {
                var lv = lvs[i];
                (function (logger, name, levels, output, aslice) {
                    var name1 = "##" + name;
                    levels[name1] = logger[name] = function () {
                        if(logger.isLogDisabled)return this;
                        output.call(logger, name1, arguments);
                        return this;
                    }
                })(this, lv, levels, output, aslice);
            }
			this.output = output;
        };
        this.config = function (opts) {
            if (opts.output) this["@output"] = opts.output;
            config.call(this, opts.output, opts.levels);
            if (opts.defaultLevel) this["@defaultLevel"] = "##" + opts.defaultLevel;
            if (opts.enables) this.enable.apply(this, opts.enables);
            if (opts.disables) this.disables.apply(this, opts.disables);
			var dft = this["@levels"][this["@defaultLevel"]];
			if(!dft){
				for(var n in this["@levels"]) {this["@defaultLevel"]=n;break;}
			}
            return this;
        }
        this.addOutput = function (output) {
            var exist = this["@output"];
            if (typeof exist.addArggregation === 'function') {
                if(!exist.existAggregation(output))exist.addArggregation(output);
            } else if (exist) {
                var aggr = log.outputAggregation();
                aggr.addAggregation(output);
                this["@output"] = aggr;
            } else {
                this["@output"] = output;
            }
            config(this["@output"], this["@levels"]);
        }
        this.removeOutput = function (output) {
            var exist = this["@output"];
            if (typeof exist.removeArggregaation === 'function') {
                exist.removeArggregaation(output);
                if (exist.count() == 1) {
                    this["@output"] = exist.getAggregatioin(0);
                }
            } if (exist) {
                if (exist === output) this["@output"] = log.defaultOutput;
            }
            config(this["@output"], this["@levels"]);
        }
        this.config(opts);
    };
    log.Logger.call(log);
	log.toString = function(){return "[function yi.log.Logger]";}

    log.HtmlLogger = function (elem) {
		this.toString = function(){return "[object,yi.log.HtmlLogger]";}
        elem = this.element = elem || document.createElement("div");
		elem.className = "console";
		this["@levels"] =null;
        this["@colors"] = {
            "##assert": "yellow",
            "##debug": "blue",
            "##info": "gray",
            "##notice": "white",
            "##warming": "orange",
            "##success": "green",
            "##fail": "red"
        };
		this.traceStack = function(v){
			if(v===undefined)return this["@traceStack"];
			this["@traceStack"] = v;return this;
		}
        var output = (function (me, outputView, colors) {
            var output = function (type, contents,start) {
                var ctn = document.createElement("div");
                ctn.style.clear = "both";
                ctn.style.color = colors[type] || colors["##debug"] || "blue";
				ctn.style.borderBottom = "1px dashed #666666";
                for (var i = (start||0), j = contents.length; i < j; i++) {
                    var o = contents[i];
                    var elem = toDom(o);
                    var div = document.createElement("div");
                    div.appendChild(elem);
                    div.style.cssText = "float:left;";
                    ctn.appendChild(div);

                }
				var stack = document.createElement("div");
				
                if (me["@traceStack"]) {
					var t = new Date();
					var ts = t.getMonth() + "/" + t.getDate() + "/" + t.getFullYear() + "T" + t.getHours() + ":" + t.getMinutes() + ":" + t.getSeconds() + "." + t.getMilliseconds();
					var tz = t.getTimezoneOffset()/60;
					ts += tz>=0?"+" +tz:tz.toString();
                    stack.innerHTML = "<span style='color:#666;text-decoration:underline;cursor:pointer;'>&"+ts+"</span>";
                    stack.style.cssText = "float:right;";
                    var stacks;
					try{
						throw new Error();
					}catch(ex){
						var stacks = ex.stack.split("\n");
						var hasOutput=false;
						while(true){
							var txt = stacks.shift();
							if(!txt)break;
							if(txt==='Error')continue;
							var at = txt.indexOf("(");
							if(at<=0)break;
							txt = txt.substring(0,at).replace(/( +$)/g,"");
							var isOutput = txt.substring(txt.length-7);
							if(isOutput===' output' || isOutput==='.output'){hasOutput=true; continue;}
							if(hasOutput)break;
							//if(txt.substring(at-,at))
						}
						
					}
                    
                    d = this["@console.tracesElement"] = document.createElement("div");
                    
                    d.style.cssText = "border:1x dashed #999;color:#888;clear:both;";
                    var b = "<ol style=''><li>" + stacks.join("</li><li>") + "</li></ol>";
                    d.innerHTML = b;

                    stack.onclick = function (evt) {
                        var d = me["@console.tracesElement"];
                        if (!d.parentNode) {
                            ctn.appendChild(d);
                        } else d.parentNode.removeChild(d);
                        evt = evt || event;
                        evt.cancelBubble = true; evt.returnValue = true;
                        if (evt.preventDefault) evt.preventDefault();
                        return false;
                    }
                }else{
					//stack.innerHTML = "&nbsp;&nbsp;&nbsp;";
					
				}
				ctn.insertBefore(stack, ctn.firstChild);
				var clear = document.createElement("div");
				clear.style.cssText = "clear:both";
				ctn.appendChild(clear);
                outputView.appendChild(ctn);
                outputView.scrollTop = outputView.scrollHeight;
                outputView.scollLeft = 0;
            }
            return output;
        })(this, this.element, this["@colors"]);
		var opts = {
            output: output,
            levels: ["debug","fail","warming","notice","info",'success',"assert"],
            defaultLevel: "debug"
        }
		this.config(opts);

        this.config = function (opts) {
            if (opts.output) throw new Error("HtmlLogger's output is fixed. You should NOT replace this output.");
            if (opts.colors) {
                var colors = this["@colors"];var ns = [];
                for (var n in colors) ns.push(n);
                for(var i in ns) delete colors[ns[i]];
                for(var n in opts.colors) colors["##" + n] = opts.colors[n];
            }
            log.HtmlLogger.prototype.config.call(this,opts);
        }
       
        
    }
	//不要让Logger的默认levels出现在原型中
    log.HtmlLogger.prototype = new log.Logger({levels:[]});

    var toDom = function (o, exists) {
        var t = typeof o;
        exists || (exists = []);
        if (t === "object" || t === 'function') {
            if (o === null) {
                var elem = document.createElement("span");
                elem.innerHTML = "null"; elem.className = "object null";
                return elem;
            }
            //去重
            for (var i = 0, j = exists.length; i < j; i++) {
                if (exists[i] === o) {
                    var e = document.createElement("span");
                    e.className = "cuit";
                    e.innerHTML = "[cuit ref]";
                    return e;
                }
            }
            exists.push(o);
            var elem = document.createElement("table");
            if (Object.prototype.toString.call(o) === '[object Array]') elem.className = "object array";
            else if (t === 'function') elem.className = "function";
            else elem.className = "object";
            var hd = document.createElement("thead");
            var hdTr = document.createElement("tr"); hd.appendChild(hdTr);
            var preElem = document.createElement("td");
            preElem.innerHTML = "<span style='color:#666;cursor:pointer;'>+</span>";
			preElem.__collapsed = true;
            preElem.onclick = function () {
                var hd = this.parentNode.parentNode;
                var valueTbody = hd.nextSibling;
                if (!this.__collapsed) {
                    valueTbody.style.display = "none";
                    this.innerHTML = "<span style='color:#666;cursor:pointer;'>+</span>";
					this.__collapsed = true;
                } else {
                    valueTbody.style.display = "table-row-group";
                    this.innerHTML = "<span style='color:#666;cursor:pointer;'>-</span>";
					this.__collapsed= false;
                }
            }
            hdTr.appendChild(preElem);
            var nmTd = document.createElement("td");
            nmTd.innerHTML = "<div class='value'>" + htmlEncode(o.toString()) + "</div>"; nmTd.setAttribute("colspan", 2); hdTr.appendChild(nmTd);

            var bd = document.createElement("tbody");
            bd.style.display = "none";
            bd.className = "members";
            for (var n in o) {
                var val = o[n];
                var valElem = toDom(val, exists);

                var tr = document.createElement("tr");
                var preTd = document.createElement("td");
                preTd.innerHTML = "&nbsp;"; tr.appendChild(preTd);
                var nameTd = document.createElement("th");
                nameTd.setAttribute("valign", "top");
                nameTd.innerHTML = "<div class='member-name'>" + htmlEncode(n) + "</div>"; tr.appendChild(nameTd);
                var valueTd = document.createElement("td");
                valueTd.style.width = "100%";
                var valueDiv = document.createElement("div");
                valueDiv.className = "value";
                valueDiv.appendChild(valElem);
                valueTd.appendChild(valueDiv);
                tr.appendChild(valueTd);
                bd.appendChild(tr);
            }

            elem.appendChild(hd);
            elem.appendChild(bd);
            return elem;
        } else {
            var elem = document.createElement("span");
            if (t === 'undefined') {
                elem.innerHTML = "undefined";
                elem.className = "undefined";
            } else if (t === 'number') {
                elem.innerHTML = o.toString();
                elem.className = "number";
            } else if (t === 'string') {
                elem.innerHTML = htmlEncode(o);
                elem.className = "string";
            }
            return elem;
        }
    }
    var htmlEncode = function (txt) {
        return txt.replace(/&/g, "&amp").replace(/ /g, "&nbsp;").replace(/\t/g, "&nbsp;&nbsp;&nbsp;&nbsp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/\n/g, "<br />");
    }

    var Console = function () {
        var me = this;
        var elem = this.element = document.createElement("div");
        elem.style.cssText = "width:500px;height:400px;z-index:999999;position:fixed;right:2px;bottom:2px;font-size:12px;";
        elem.innerHTML = "<textarea style='position:absolute;height:14px;background-color:#efefef;color:#333;border:1px solid black;padding:2px;' title='command window' palcehold='command window'></textarea>";
        var cmdElem = elem.firstChild;

        var outputView = this.outputView = document.createElement("div");
        
        elem.appendChild(outputView);
        outputView.style.cssText = "margin-top:16px;border:1px double #000;background-color:#333;color:#ddd;padding:5px;height:380px;overflow:auto;";
        outputView.ondblclick = function (evt) {
            evt = evt || event; var c = false;
            if (evt.altKey) { me.hide(); c = true; }
            if (evt.ctrlKey) { me.clear(); c = true; }
            if (c) {
                evt.cancelBubble = true; evt.returnValue = true;
                if (evt.preventDefault) evt.preventDefault();
                return false;
            }
        }
        this.isDisabled = true;
        this.enable = function (invade) {
            outputView.innerHTML = "<hr /><h3><em>Virtual console</em></h3>You can type '::help' ,then press CTRL + ENTER to get help info about this console<hr />";
            if (window.addEventListener) window.addEventListener("keyup", onkey, false);
            else if (window.attachEvent) window.attachEvent("keyup", onkey);
            this.isDisabled = false;
            return this.show();
        }
        this.disable = function () {
            if (window.removeEventListener) window.removeEventListener("keyup", onkey, false);
            else if (window.detechEvent) window.detechEvent("onkeyup", onkey);
            return this.hide();
        }
        this.toggle = function () {
            if (!elem.parentNode) this.show();
            else this.hide();
            return this;
        }
        this.show = function () {
            elem.style.display = "block";

            document.body.appendChild(elem);
            cmdElem.style.width = elem.offsetWidth - 6 + "px";
            return this;
        }
        this.hide = function () {
            if (elem.parentNode) elem.parentNode.removeChild(elem);
            return this;
        }
		this.clear = function () { outputView.innerHTML = ""; return this; }
        var onkey = function (evt) {
            evt = evt || event;
            if (evt.keyCode == 67) {
                var c = false;
                if (evt.altKey) { c = true; me.toggle(); }
                if (evt.ctrlKey) { c = true; me.clear(); }
                if (c) {
                    evt.cancelBubble = true; evt.returnValue = true;
                    if (evt.preventDefault) evt.preventDefault();
                    return false;
                }
            }
        }

        this.log = function () { this.log.log(arguments); return this; }
        log.HtmlLogger.call(this.log , outputView);
    }

	  var assert =yi.assert = Global.$assert = function (expect, actual, text) {
        if (arguments.length === 1) return isTrue.call($assert, expect);
        return equal.call($assert, expect, actual, text);
    }
	
    assert.Assert = function (logger) {
		this.toString = function(){return "[object,yi.log.Assert]";}
        this.__data = {};
        this.logger = logger|| yi.log;
        this.clear = function () { this.__data = {}; return this; }
        this.log = function(){
			if(arguments.length==0)return this;
			var params = aslice.call(arguments);
			var lv = params[0];
			if(typeof lv ==='string' && lv.indexOf("##")!==0) params.unshift("##assert");
			this.logger.log.apply(this.logger,params);
			return this;
		}
        this.Array = function (value, text) {
            var t = Object.prototype.toString.call(value);
            if (t !== '[object Array]') {
                this.logger.output("##fail","Array::Expect array,but actual is ",value);
                if(!this.notThrows)throw new Error(text || "Assert.isArray");
            }
            return this;
        }
        this.Equal = function (expect, actual, text) {
            if (expect !== actual) {
                this.logger.output("##fail","Equal::Expect ", expect,", but actual is ",actual);
                throw new Error(text || "Assert.Eual");
            }
            return this;
        }
        this.NotExists = function (value, text) {
            if (value) {
                this.logger.output("##fail","Not Exists::Expect 0 or null or undefined or '', but actual is ",value);
                //alert(arguments.callee.callee);
                //this.log("invoker:",arguments.callee.caller);
                if(!this.notThrows)throw new Error(text || "Assert.NotExists");
            }
            return this;
        }
        this.Exists = function (value, text) {
            if (!value) {
                this.logger.output("##fail","Exists::Expect value should not be 0 or null or undefined or '',but actual is ",value);
                if(!this.notThrows) throw new Error(text || "Assert.Exists");
            }
            return this;
        }
        this.Undefined = function (value, text) {
            if (value !== undefined) {
                this.logger.output("##fail","Undefined::Expect undefined, but actual is ",value);
                if(!this.notThrows) throw new Error(text || "Assert.Undefined");
            }
            return this;
        }
        this.Null = function (value, text) {
            if (value !== null) { 
                this.logger.output("##fail","Null::Expect null, but actual is ", value);
				if(!this.notThrows) throw new Error(text || "Assert.Null");
            }
            return this;
        }

        this.True = function (value, text) {
            if (value !== true) { 
                this.logger.output("##fail","True::Expect true, but actual is ", value);
				if(!this.notThrows) throw new Error(text || "Assert.True");
            }
            return this;
        }
        this.False = function (value, text) {
            if (value !== false) { 
                this.logger.output("##fail","False::Expect true, but actual is ", value);
				if(!this.notThrows) throw new Error(text || "Assert.False");
            }
            return this;
        }
    }

    assert.Assert.call(assert);
	assert.toString = function(){return "[function,yi.assert.Assert]"}
	assert.test = function(tester){
		var ass = new assert.Assert();
		tester.call(this,ass);
		return this;
	}
	assert.clearCode = function(code){
		var codes = code.split("\n");
		for(var i =0,j= codes.length;i<j;i++){
			var line = codes.shift();
			var lineCode = line.replace(/(^\s+)|(\s+$)/g,"");
			if(lineCode.indexOf("$assert.")!==0 &&lineCode.indexOf("$assert(")!==0 && lineCode!="$log();" && lineCode.lastIndexOf("//$assert")!==(lineCode.length-"//$assert".length)) codes.push(line);
		}
		return codes.join("\n");
	}
    var equal = $assert.Equal;
    var isTrue = $assert.True;

    var sysLog;
    /*
    var Console = function () {
        log.Logger.call(this);
        var me = this;
        var elem = this.element = document.createElement("div");
        elem.style.cssText = "width:500px;height:400px;z-index:999999;position:fixed;right:2px;bottom:2px;font-size:12px;";

        
        cmdElem.onkeydown = function (evt) {
            evt = evt || event;
            //yi.console.log(evt.keyCode);
            if (evt.keyCode === 13) {
                if ((cmdElem.value[0] == ":" && cmdElem.value[1] == ':') || evt.ctrlKey) {
                    me.exec(cmdElem.value);
                    return;
                }
                var h = cmdElem.scrollHeight + 9;
                if (h > elem.offsetHeight - 40) h = elem.offsetHeight - 40;
                cmdElem.style.height = h + "px";
            }
        }
        this.exec = function (code) {
            var pre = document.createElement("pre");
            pre.innerHTML = code;
            pre.className = "code";

            //outputView.appendChild(pre);

            var rsc = document.createElement("fieldset");
            rsc.style.cssText = "clear:both;border:1px dashed #999;background-color:#000;color:#eee;";
            rsc.innerHTML = "<legend style='padding:2px 5px;background-color:#ccc;color:#333;font-weight:bold;'></legend>";
            var hd = rsc.firstChild;
            var rs;
            rsc.appendChild(pre);
            if (code.indexOf("::") == 0) {
                code = code.substring(2);
                if (code[code.length - 1] != ")") code += "()";
                try {
                    outputView.appendChild(rsc);
                    var fnc = "return $$me$$." + code;
                    var fn = new Function("$$me$$", fnc);
                    fn(this);
                    rsc.className = "result success";
                    hd.innerHTML = "Exec(success):";
                    hd.style.color = "green";
                } catch (ex) {
                    hd.innerHTML = "Exec(fail):";
                    rs = toDom(ex);
                    rsc.className = "result fail";
                    hd.style.color = "red";
                    rsc.appendChild(document.createElement("hr"));
                    rsc.appendChild(toDom(ex));
                }


            } else {
                try {

                    eval(code);
                    rsc.className = "result success";
                    hd.style.color = "green";
                    hd.innerHTML = "Exec(success):";
                } catch (ex) {
                    hd.innerHTML = "Exec(fail):";
                    rsc.className = "result fail";
                    hd.style.color = "red";
                    rsc.appendChild(document.createElement("hr"));
                    rsc.appendChild(toDom(ex));
                }

                outputView.appendChild(rsc);
            }

            cmdElem.style.height = "15px";
            cmdElem.value = "";
        }
        
        this.fontSize = function (v) {
            if (v === undefined) return this.__fontSize || 14;
            this.__fontSize = parseInt(v) || 14;
            elem.style.fontSize = this.__fontSize + "px";
            if (cmdElem.offsetHeight < this.__fontSize) cmdElem.style.height = this.__fontSize + 4 + "px";
            return this;
        }
        this.height = function (h) {
            var h = parseInt(h) || 400;
            if (h < 100) h = 100;
            elem.style.height = h + "px";
            if (cmdElem.offsetHeight >= (h - 40)) cmdElem.style.height = h - 40 + "px";
            var oH = h - cmdElem.offsetHeight;
            if (oH < 0) oH = 0;
            outputView.style.height = oH + "px";
        }
        this.css = function (name, value) {
            if (typeof name === 'object') for (var n in name) this.css(n, name[n]);
            else elem.style[name] = value;
            return this;
        }
        this.suspend = function () {
            this._suspended = true;
            return this;
        }
        this.resume = function () {
            this._suspended = false;
            return this;
        }
        this.help = function () {
            var help = "<strong>You can enter any js code in command window, and press ctrl + ENTER to execute then.<br />:: means you should type this command in command window which shown at the top of the console.<br />double click the log line , you can see the stack about this log.</strong><ul><li><em>::help = get help</em></li><li><em>ALT + C = toggle show/hide</em></li><li><em>CTRL + C = clear</em></li><li><em>ALT + double click = hide</em></li><li><em>CTRL + double click = clear</em></li><li><em>::clear = clear console</em></li><li><em>::suspend = suspend log</em></li><li><em>::resume = resume log</em></li><li><em>::hide = hide console</em></li><li><em>::show = show console</em></li></ul>";
            var elem = document.createElement("div");
            elem.className = "help";
            elem.innerHTML = help;
            outputView.appendChild(elem);
            return this;
        }
        this["@output"] = (function (me, outputView, colors) {
            var output = function (type, contents) {
                var ctn = document.createElement("div");
                ctn.style.clear = "both";
                ctn.style.color = colors[type] || colors["##debug"];
                for (var i = 0, j = arguments.length; i < j; i++) {
                    var o = arguments[i];
                    var elem = toDom(o);
                    var div = document.createElement("div");
                    div.appendChild(elem);
                    div.style.cssText = "float:left;";
                    ctn.appendChild(div);

                }
                if (me["@traceStack"]) {
                    var stack = document.createElement("div");
                    stack.style.cssText = "text-decoration:underline;font-weight:bold;";
                    stack.innerHTML = "&";
                    ctn.insertBefore(stack, ctn.firstChild);
                    var traces = [];
                    var caller = arguments.callee.caller;
                    while (caller) {
                        traces.push(caller.toString());
                        caller = caller.caller;

                    }
                    d = this["@console.tracesElement"] = document.createElement("div");
                    d.style.cssFloat = "text-decoration:underline;";
                    d.style.cssText = "border:1x dashed #999;clear:both;background-color:#eee;color:#666;";
                    var b = "<ol><li><i>" + traces.join("</i></li><li><i>") + "</i></li></ol>";
                    d.innerHTML = b;

                    stack.onclick = function (evt) {
                        var d = this["@console.tracesElement"];
                        if (!d.parentNode) {
                            stack.parentNode.insertBefore(d, stack);
                        } else d.parentNode.removeChild(d);
                        evt = evt || event;
                        evt.cancelBubble = true; evt.returnValue = true;
                        if (evt.preventDefault) evt.preventDefault();
                        return false;
                    }
                }
                outputView.appendChild(ctn);
                outputView.scrollTop = outputView.scrollHeight;
                outputView.scollLeft = 0;
            }
            return output;
        })(this, outputView, this.colors);
        this.reset({
            output : this["@output"]
        });
        
        var onkey = function (evt) {
            evt = evt || event;
            if (evt.keyCode == 67) {
                var c = false;
                if (evt.altKey) { c = true; me.toggle(); }
                if (evt.ctrlKey) { c = true; me.clear(); }
                if (c) {
                    evt.cancelBubble = true; evt.returnValue = true;
                    if (evt.preventDefault) evt.preventDefault();
                    return false;
                }
            }
        }
        this.isDisabled = true;
        
        
        
    }
    
    yi.console = new Console();
    yi.console.Console = console;
    */
})(window, document);