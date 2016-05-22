var global = window;
(function(global,document,undefined){
"strict";
//******************************
/// <summary></summary>
/// <param name="suffix" type="String"></param>
/// <returns type="Boolean"></returns>
//********************************
var yi = global.yi =global.$y = {};

var sp_ = String.prototype;
/// <summary>去掉字符串两头的空格</summary>
/// <returns type="String">去掉空格后的字符串</returns> 
sp_.$trim = function(){return this.replace(/^\s+|\s+$/g,"");}

/// <summary>去掉字符串前面的空格</summary>
/// <returns type="String">去掉空格后的字符串</returns> 
sp_.$trimStart = function(){this.replace(/^\s+/g,"");}

/// <summary>去掉字符串后面的空格</summary>
/// <returns type="String">去掉空格后的字符串</returns> 
sp_.$trimEnd = function(){this.replace(/\s+$/g,"");}

/// <summary>判断字符串是否以s字符串作为开头</summary>
/// <param name="s" type="String">可能 前导字符串</param>
/// <returns type="Boolean">如果是以s开头返回true,否则返回false</returns> 
sp_.$startWith=function(s){return this.substr(0, s.length) === s;}

/// <summary>判断字符串是否以s字符串作为结束</summary>
/// <param name="s" type="String">可能的后置字符串</param>
/// <returns type="Boolean">如果是以s结束返回true,否则返回false</returns>
sp_.$endWith=function(s){return (this.substr(this.length - s.length) === s);}

/// <summary>判断字符串包含s作为子串</summary>
/// <param name="s" type="String">子串</param>
/// <returns type="Boolean">如果包含返回true,否则返回false</returns>
sp_.$contains = function(s){return this.indexOf(s)>=0;}



sp_.$camelize = function camelize() {
  // /\-(\w)/g 正则内的 (\w) 是一个捕获，捕获的内容对应后面 function 的 letter
  // 意思是将 匹配到的 -x 结构的 x 转换为大写的 X (x 这里代表任意字母)
  return this.replace(/\-(\w)/g, function(all, letter) {return letter.toUpperCase();});
}

/// <summary>绑定函数的this 或 参数</summary>
/// <param name="m" type="Object">this指针指向的对象,不可以为空</param>
/// <param name="a" type="Array">指定的调用参数列表</param>
/// <returns type="Function">形成一个新的函数。该函数的this指针总指向m。如果设置了a参数，无论给予新函数怎么样的参数表，其调用参数总是为指定的参数</returns>
Function.prototype.$bind = function(m,a){var s = this;return function(){return self.apply(m,a||arguments);};}

/// <summary>格式化日期</summary>
/// <usage>
///	"yyyy-MM-dd" => 2016-06-18
///	"M/dd/yy hh:ss:mm" => 6/18/16 20:30:01
/// </usage>
/// <param name="fmt" type="String">格式字符串</param>
/// <returns type="String">格式化后的时间字符串</returns>
Date.prototype.$format = function (fmt) { 

	//author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
    if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
yi.parseDate = function(text){
	var reg = /^\s*(?:(?:(\d{2}|\d{4})(\-)((?:[0]?\d)|(?:1[012]))\-((?:[0-2]?\d)|(?:3[0-1])))|(?:((?:[0]?\d)|(?:1[012]))(\/)((?:[0-2]?\d)|(?:3[0-1]))\/(\d{2}|\d{4})))(?:[ T]((?:[01]?\d)|(?:2[0123]))\:([0-5]?\d)(?:\:([0-5]\d))?)?Z?\s*$/g;
	var dm = text?reg.exec(text):null;
	if(!dm) return;
	
	var tp = dm[2],y ,M ,d ,h,m,s;
	
	if(tp=='-'){
		y= parseInt(dm[1]);M = parseInt(dm[3]);d = parseInt(dm[4]);
		
	}else{
		y= parseInt(dm[4]);M = parseInt(dm[1]);d = parseInt(dm[3]);
	}
	h = parseInt(dm[5]) || 0; m = parseInt(dm[6])||0;s = parseInt(dm[7]) || 0;
	if(y<100) y = 2000 + y;
	
	return new Date(y,M-1,d,h,m,s);
}
var arp_ = Array.prototype;
arp_.$contains = function(it){for(var i=0,j=this.length;i<j;i++)if(this[i]===it)return true;return false;}
arp_.$add = function(){ for(var i=0,j=arguments.length;i<j;i++)this.push(arguments[i]);return this;}
arp_.$remove = function(it){var k;for(var i=0,j=this.length;i<j;i++)if((k=this.shift())!==it) this.push(it);return k===it;}
var otstr = Object.prototype;
var isFunc = yi.isFunction = function (it) {return otstr.call(it) === '[object Function]';}
var isArray= yi.isArray =  function (it) {return otstr.call(it) === '[object Array]';}

//CreateInstance
var typeRegex = /^[\$_a-zA-Z][_a-zA-Z\$0-9]*(.[_a-zA-Z\$][_a-zA-Z\$0-9]*)*$/g;
yi.createInstance = function(type,initArgs){
	if(!type.match(typeRegex))return;
	initArgs || (initArgs=[]);
	var code = type + ".apply(type,initArgs);";
	type = {};
	eval(code);
	return type;
};

var uuid_chars = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'.split('');
var uuid = yi.guid = function uuid(len, radix) {
    var uuid = [], i;
    radix = radix || uuid_chars.length;
    if (len) {
      // Compact form
      for (i = 0; i < len; i++) uuid[i] = uuid_chars[0 | Math.random()*radix];
    } else {
      // rfc4122, version 4 form
      var r;
      // rfc4122 requires these characters
      uuid[8] = uuid[13] = uuid[18] = uuid[23] = '-';
      uuid[14] = '4';
      // Fill in random data.  At i==19 set the high bits of clock sequence as
      // per rfc4122, sec. 4.1.5
      for (i = 0; i < 36; i++) {
        if (!uuid[i]) {
          r = 0 | Math.random()*16;
          uuid[i] = uuid_chars[(i == 19) ? (r & 0x3) | 0x8 : r];
        }
      }
    }
    return uuid.join('');
}

var slice = Array.prototype.slice;
/// <summary>写日志函数</summary>
/// <usage>yi.log({a:'1'},'hehe').log("后来");</usage>
/// <param name="arguments" type="arguments">支持不定参数日志</param>
/// <returns type="Object">yi名字空间，以便连写</returns>
var log = yi.log =global.$log = function() {
	
	try {
		// Modern browsers
		if (typeof console != 'undefined' && typeof console.log == 'function') {
			// Opera 11
			if (global.opera) 
				for(var i=0,j=arguments.length;i<j;i++) console.log('Item ' + (i + 1) + ': ' + arguments[i]);
			// All other modern browsers
			else if ((slice.call(arguments)).length == 1 && typeof slice.call(arguments)[0] == 'string') 
				console.log((slice.call(arguments)).toString());
			else 
				console.log.apply(console, slice.call(arguments));
		}
        // IE8
		else if ((!Function.prototype.bind || treatAsIE8) && typeof console != 'undefined' && typeof console.log == 'object') {
			Function.prototype.call.call(console.log, console, slice.call(arguments));
		}
	// IE7 and lower, and other old browsers
	} catch (ignore) { 
		alert(ignore);
	}
	return yi;
}
var logError = yi.elog = function(error, err) {
	var exception = error instanceof Error?error:new Error(error);     
	exception.innerError = err;   
	//Report the error as an error, not as a log
    try {
		// Modern browsers (it's only a single item, no need for argument splitting as in log() above)
		if (typeof console != 'undefined' && typeof console.error == 'function') {console.error(exception);}
        // IE8
		else if ((!Function.prototype.bind) && typeof console != 'undefined' && typeof console.error == 'object') {Function.prototype.call.call(console.error, console, exception);}
        // IE7 and lower, and other old browsers
    } catch (ignore) { 
	}
	return yi;
};
//**function** 对象成员拷贝
var extend  = yi.extend = global.$extend = function(o){
	
	var s;
	for(var i=1,j=arguments.length;i<j;i++){
		if(s=arguments[i])for(var n in s) o[n] = s[n];
	}
	return o;
}
yi.keys = function(obj){
	var ret = [];
	for(var n in obj) ret.push(n);return ret;
}
yi.values = function(obj){
	var ret = [];
	for(var n in obj) ret.push(obj[n]);return ret;
}

var disabled = yi.disabled = function(){throw "disabled";}
yi.enabled = {};


/// <summary>异步延迟对象(机制)</summary>
/// <usage>
/// new Deferred(function(dfd){dfd.resolve('ok');}).done(function(result){yi.log(result);});
/// </usage>
/// <param name="fn" type="Function">有耗费时间的操作的函数，可以为空。如果该参数为空，后面的delay参数不起作用</param>
/// <param name="delay" type="Boolean">该函数不会立即执行，而是随后执行。</param>
/// <returns type="Object">Deferred对象</returns>
var Deferred = yi.Deferred = function(fn,delay){
	this.deferredStatus = function(){return this["@deferred.status"];}
	this.deferredResult = function(){return this._deferredResult;}
	this.isWaiting = function(){return this["@deferred.status"]=="waiting";}
	this.when = function(fn,delay){
		if(!fn)return this;
		var me = this;
		var f = function(){
			var stat = this["@deferred.status"];
			if(stat && stat!='waiting')throw "already done or fail.";
			var  rv = fn.call(me,me);
			if(!me["@deferred.status"]) return me.resolve(rv);
		}
		if(typeof delay==='number')setTimeout(fn,delay);
		else if(delay)setTimeout(fn,0);
		else f();
		return this;
	}
	
	this.follow = function(cb){
		var dfd = new Deferred();
		this.done(function(v,st,d){
			cb.call(this,v,st,d);
			dfd.resolve();
		});
		return dfd.promise();
	}
	
	this.done = function(cb){
		if(!cb)return this;
		var done = this["@deferred.status"]=="done";
		if(done){ if(cb.call(this,this._deferredResult,true,this)===disabled)return this;}
		 else (this["@deferred.done"] || (this["@deferred.done"]=[])).push(cb);
		return this;
	}
	this.fail = function(cb){
		if(!cb)return this;
		var fail = this["@deferred.status"]=="fail";
		if(fail){ if( cb.call(this,this._deferredResult,false,this)==disabled) return this;}
		else (this["@deferred.fail"] || (this["@deferred.fail"]=[])).push(cb);
		return this;
	}
	this.then = function(cb){
		if(!cb)return this;
		var st = this["@deferred.status"],rd = status=='done' || status=='fail';
		if(rd){ if(cb.call(this,this._deferredResult,st=='done',this)===disabled)return this ;}
		else (this["@deferred.fail"] || (this["@deferred.fail"]=[])).push(cb);
		return this;
	}
	this.wait = function(){this["@deferred.status"]="waiting";return this;}
	this.resolve = function(call_v,apply_v){ 
		this["@deferred.status"]='done';
		this._deferredResult = apply_v|| call_v;
		if(apply_v)this["@isApply"] = true;
		var its;
		if(its=this["@deferred.done"])for(var i=0,j=its.length;i<j;i++){
			var it = its.shift();
			var ctne = apply_v?it.apply_v(this,apply_v):it.call(this,call_v,this,true);
			if(ctne!==disabled) its.push(it);
		}
		if(its=this["@deferred.then"]){
			for(var i=0,j=its.length;i<j;i++){
				var it = it=its.shift();
				var ctne = apply_v?it.apply(this,apply_v):it.call(this,call_v,this,true);
				if(ctne!==disabled) its.push(it);	
			}
		}
	}

	this.reject = function(call_v,apply_v){ 
		this["@deferred.status"]='fail';
		this._deferredResult = apply_v|| call_v;
		if(apply_v)this["@isApply"] = true;
		var its;
		if(its=this["@deferred.fail"])for(var i=0,j=its.length;i<j;i++){
			var it = its.shift();
			var ctne = apply_v?it.apply_v(this,apply_v):it.call(this,call_v,this,false);
			if(ctne!==disabled) its.push(it);
		}
		if(its=this["@deferred.then"]){
			for(var i=0,j=its.length;i<j;i++){
				var it = it=its.shift();
				var ctne = apply_v?it.apply(this,apply_v):it.call(this,call_v,this,false);
				if(ctne!==disabled) its.push(it);	
			}
		}
	}
	
	
	this.promise = function(tgt){
		if(!tgt){
			if(this["@deferred.promise"]) return this;
			tgt = {};
		}
		var me = this;
		tgt.deferredResult = function(){return me._referredResult;}
		tgt.deferredStatus = function(){return me["@deferred.status"];}
		tgt.done = function(cb){me.done(cb);return this;}
		tgt.fail = function(cb){me.fail(cb);return this;}
		tgt.promise = function(targ){return (!targ || targ==this) ? this : me.promise(targ);}
		tgt["@deferred.promise"] = me;
		return tgt;
	}
	if(fn) this.when(fn,delay);
	return this;
}
global.$await = yi.await = function(fnc,delay){return new Deferred(fnc,delay);}

//**function** 定时器
var Timer = yi.Timer = function(tick,initFuncs,isTimeout){
	if(initFuncs===true || initFuncs===false){ var a= isTimeout; isTimeout = initFuncs; initFuncs = a;}
	this["@isTimeout"] = isTimeout;
	this.isTimeout = function(){return this["@isTimeout"];}
	this.add = function(func){this["@funcs"].push(func);this.run();return this;}
	this.remove = function(func){
		var funcs = this["@funcs"];
		var fn;for(var i=0,j=funcs.length;i<j;i++)if((fn=funcs.shift())!=func)funcs.push(fn);
		//没有函数了，停掉
		if(funcs.length==0) this.stop();
		return this;
	}
	this.clear = function(){this["@funcs"] = [];return this;}
	this.tick = function(value){
		var tick = this["@tick"];
		if(value===undefined)return tick;
		if(value===tick)return this;
		var timer = this["@timer"];
		if(timer) (this["@isTimeout"]?clearTimeout:clearInterval)(timer);
		this["@tick"] = value||0;
		this["@timer"] = 0;
		if(this["@isRunning"]) this.run();
		return this;
	}
	this.isRunning = function(){return this["@isRunning"];}
	this.run = function(){
		var timer = this["@timer"],tick = this["@tick"],funcs = this["@funcs"],self = this;
		//先停掉以前的定时器
		if(timer)(isTimeout?clearTimeout:clearInterval)(timer);
		this["@isRunning"]=false;
		if(funcs.length===0)return this;
		if(this["@isTimeout"]){
			this["@timer"] = setTimeout(function(){
				for(var i=0,j=funcs.length;i<j;i++)funcs.shift().call(self,self);
				self["@isRunning"] = false;self["@timer"]=0;	
			},tick);
		}else {
			this["@timer"] = setInterval(function(){
				var fn;
				for(var i=0,j=funcs.length;i<j;i++){
					fn=funcs.shift();
					if(fn.call(this,this)!==disabled) funcs.push(fn);
				}
			},tick);
		}
		
		this["@isRunning"] = true;
		return this;
	}
	this.stop = function(){
		var timer = this["@timer"],tick = this["@tick"];
		if(timer) (this["@isTimeout"]?clearTimeout:clearInterval)(timer);
		this["@isRunning"] = false;this["@timer"] = 0;
		return this;
	}
	if(initFuncs){
		this["@funcs"] = initFuncs;
		this.run();
	}else this["@funcs"] = [];
	return this;
}
var immediate = Timer.immediate = new Timer(0);
var fiftyTimer = Timer.fifty = new Timer(50);

})(global,document);

(function(global,document,undefined){
var yi = global.$y,isArray = yi.isArray,isFunction = yi.isFunction,disabled = yi.disabled,enabled = yi.enabled;
var increment_id = 0;
//兼容google 的代码，google的函数这些文字在function中是保留属性名
var  reservedPropertyNames = {
		"name" : "name_",
		"arguments" : "arguments_",
		"length" : "length_",
		"caller" : "caller_",
		"prototype" : "prototype_",
		"constructor" : "constructor_"
};

var Observable =yi.Observable= function(name,target){
	this["@name"] = name===undefined?"prop-" + (increment_id++):name;
	this["@target"]=target || {};
	this.name = function(value){
		/// <summary>get/set观察器名字</summary>
		/// <param name="value" type="String">设定的名字。如果是undefined则返回观察器名字</param>
		/// <returns type="String | Object">读取的名字。如果是set操作则返回监听器本身</returns>
		if(value===undefined)return this["@name"];
		this["@name"] = value;return this;
	}
	this.target = function(target,source){
		/// <summary>get/set要观察的目标对象</summary>
		/// <param name="target" type="Object">要观察的对象</param>
		/// <returns type="String | Object">对象。如果是set操作则返回监听器本身</returns>
		var old = this["@target"];
		if(target===undefined)return old;
		this["@target"] = target || (target ={});
		//监听目标对象改变后，重新给本监听器赋值，以触发
		if(old!==target)this.setValue(target["@name"],"target.change",source);
		return this;
	}
	this.subject = function(subject){
		/// <summary>get/set该观察器的主体对象(主体观察器)。当一个观察器有主体对象时，表示该观察器是主体对象的一个属性</summary>
		/// <param name="subject" type="Object">要设置的主体对象。必须是另一个Observable。如果该参数设置为"root"，返回根</param>
		/// <returns type="Object">对象。如果是set操作则返回监听器本身。否则返回主体观察器</returns>
		if(subject===undefined)return this["@subject"];
		if(subject==="root"){
			var sub = this["@subject"];
			return sub?sub.subject("root"):sub;
		}
		var old = this["@subject"];
		//原先的跟新的一样，就什么都不做
		if(old===subject)return this;

		this["@subject"] = subject;
		if(old){
			var name = this["@name"];
			//清除掉原来subject里面的东西
			delete old["@props"][name];
			var accor = old.accessor();
			delete accor[reservedPropertyNames[name]||name];
		}
		var new_targ = subject["@target"] || (subject["@target"]={});
		this.target(new_targ);
		//数组的item不会当作prop
		if(subject.isArray && typeof name!=='number'){
			(subject["@props"] || (subject["@props"]={}))[name] = this;
			var accor = subject.accessor();
			accor[reservedPropertyNames[name]||name] = this.accessor();
		}
		return this;
	}
	
	this.subscribe = function(subscriber){
		/// <summary>订阅/监听目标值的变化</summary>
		/// <param name="subscriber" type="Function">订阅者，监听者</param>
		/// <returns type="Object">监听器本身</returns>
		
		var its = this['@subscribers'];
		(its || (this['@subscribers']=[])).push(subscriber);return this;
	}
	this.unsubscribe = function(evtname,subscriber){
		/// <summary>退订目标值变化。把监听器函数移除</summary>
		/// <returns type="Object">监听器本身</returns>
		
		var its = this[evtname],it;
		if(!its)return this;
		for(var i=0,j=its.length;i<j;i++) if((it=its.shift())!==subscriber) its.push(it);
		if(its.length==0) this["@subscribers"]=null;
		return this;
	}
	this.trigger = function(evt,bubble){
		/// <summary>触发某个事件/禁用事件/启用事件</summary>
		/// <param name="evtname" type="String">事件名。如果该函数第2个参数没有，evtname='valuechange'.如果该值设置为enabled/disabled对象，表示启用/禁用事件</param>
		/// <param name="evt" type="Object">事件对象</param>
		/// <returns type="Object">监听器本身</returns>
		if(evtname===disabled){
			this["@trigger_disabled"] = true;
			return this;
		}
		if(evtname===enabled){
			this["@trigger_disabled"] = false;
			return this;
		}
		if(this["@trigger_disabled"])return this;
		
		var its = this['@subscribers'],it;
		if(its) for(var i=0,j=its.length;i<j;i++) if((it=its.shift()).call(this,evt)===false) break;
		//如果没有禁用bubble,事件本身也没有取消冒泡
		if(bubble!==false && !evt.cancelBubble){
			var sup = this.subject();
			if(sup) {
				var evtArgs = {type:"change",sender:this,value:this["@target"][this["@name"]],reason:"prop.change",source:(evt.source|| evt)};
				sup.trigger(evtArgs);
			}
		}
		return this;
	}
		
	this.value = function(new_v,reason,_source){
		/// <summary>get/set该监听器在目标对象上的值</summary>
		/// <param name="new_v" type="Anything">要设定的新值。如果该参数为undefined，表示获取目标对象上的值</param>
		/// <param name="reason" type="String | not_trigger">变化原因，传递给事件函数用，默认是valuechange。</param>
		/// <param name="_source" type="Object">之名该变化是由哪个个源事件引起</param>
		/// <returns type="Object">监听器本身</returns>
		
		var targ= this["@target"],name = this["@name"];
		if(new_v===undefined) return targ[name];
		//获取旧值，如果跟新值一样就直接拿返回
		var old_v = targ[name];
		if(old_v===new_v)return this;
		//set value to target
		targ[name] = value;
		//表示不需要触发事件，只是设置一下值
		//跳过后面的事件处理
		if(this["@trigger_disabled"]) return this;
		//构建事件参数
		var evtArgs = {type:"set",sender:this,value:value,reason:(reason||"value.reset"),source:_source};
		
		//获取到该监听上的所有下级监听器
		var props = this["@props"];
		
		if(props) for(var n in props) props[n].target(value,evtArgs);
		var items = this["@items"];
		if(items){
			for(var i=0,j= items.length;i<j;i++){
				var it = items[i];
				var it_evt = {type:"remove",sender:item,value:it,reason:"array.reset",source:evtArgs,index:i};
				it.trigger(it_evt);
			}
			this._initArrayData(this["@itemTemplate"],this.value);
		}
		this.trigger(evtArgs);
		//this.childchange(evtArgs);
		return this;
	}
	this.accessor = function(){
		var me = this;
		var accor = function(value){
			if(value===undefined)return me["@target"][me["@name"]];
			me.value(value);
			return accor;
		}
		accor["@observable"] = this;
		accor.subscribe = function(evtname,subscriber){
			me.subscribe(evtname,subscriber);
			return accor;
		}
		accor.unsubscribe = function(evtname,subscriber){
			me.unsubscribe(evtname,subscriber);
			return accor;
		}
		accor.define = function(model){
			me.define(model);
			return actor;
		}

		this.accessor = function(){return accor;}
		return this["@accessor"]=accor;
	} 
	

	this.prop = function(name,value){
		var props = this["@props"] || (this["@props"]={});
		var prop = props[name];
		if(!prop){
			prop = props[name] = new Observable(name).subject(this);
			if(isArray(value)) prop.asArray();
		}
		if(value===undefined)return prop;
		prop.value(value);
		return this;
	}
	
	this.define = function(model){
		var props = this["@props"] || (this["@props"]={}),target = this["@target"];
		for(var pname in model){
			var member = model[n];
			var prop = props[name] = new Observable(pname,target).subject(this);
			if(typeof member ==='object'){
				if(isArray(member)){
					var tmp;
					if(members.length>0) tmp = new Observable(0,member).define(member[0]);
					prop.asArray(tmp);
				}else{
					prop.define(member);
				}
			}
		}
		return this;
	}
	this.clone = function(name,target,evtInc){
		var clone = new Observable(name,target);
		target = clone.target();
		var props = this["@props"];
		if(props){
			var cloneProps = {};
			for(var propname in props){
				var prop = props[propname];
				var cloneProp = prop.clone(propname,target);
				clone.value[reservedPropertyNames[propname]||propname] = cloneProp.value;
				cloneProp["@subject"] = clone;
			}
			clone["@props"] = cloneProps;
		}
		if(evtInc){
			var subsers = this["@subscribers"];
			if(subsers && subsers.length){
				var cloneSubsers = [];
				for(var i=0,j=subsers.length;i<j;i++)cloneSubsers[i]=subsers[i];
			}
		}
		if(this.isArray) clone.asArray(this["@itemTemplate"]);
		return clone;
	}
	this.asArray = function(itemTemplate){
		this.isArray = true;
		var initArrayData= this._initArrayData = function(itemTemplate,accor){
			var me = this,targ= me["@target"],name = me["@name"];
			if(!targ){me.value(targ=[]);}
			me["@itemTemplate"] = itemTemplate;
			me.isArray = true;
			var items = me["@items"]= itemTemplate?[]:null,len = targ.length;
			var max = accor.length>targ.length?accor.length:targ.length;
			for(var i=0,j=max;i<j;i++){
				//清理数据
				if(i>=len) {delete accor[i];continue;}
				if(items){
					var oIt = itemTemplate.clone(i,targ,true);
					oIt["@subject"] = me;
					items.push(oIt);
				}else accor[i] = targ[i];
			}
			accor.length = targ.length;
			return accor;
		}
		var me  = this,accor = initArrayData.call(this,itemTemplate,this.accessor());
		
		accor.add = accor.push = this.add = this.push = function(it){
			var arr = me["@target"][me["@name"]],items = me["@items"],item;
			arr.push(accor[accor.length++] = it);
			if(items){
				item = me["@itemTemplate"].clone(items.length,arr,true);
				item["@subject"] = me;
				items.push(item);
			}
			var it_evt = {sender:item,value:it,reason:"array.push"};
			var arr_evt = {type:"change",sender:me,value:arr,reason:"array.push",source:it_evt};
			me.trigger(arr_evt);
			return this;
		}
		accor.pop = this.pop = function(retItem){
			var arr = me["@target"][me["@name"]],items = me["@items"],item;
			var it =arr.pop();
			delete accor[arr.length];
			accor.length = arr.length;
			if(items) item = items.pop();
			var it_evt = {type:"add",sender:item,value:it,reason:"array.pop"};
			if(item)item.trigger(it_evt);
			else{
				var arr_evt = {type:"change",sender:me,value:arr,reason:"array.pop",source:it_evt};
				me.trigger(arr_evt);
			}
			return retItem?item:it;
		}
		accor.unshift = this.unshift = function(it){
			var arr = me["@target"][me["@name"]];
			arr.unshift(it);
			for(var i=accor.length;i>0;i--)accor[i]  = accor[i-1];
			accor[0] = it;
			if(items){
				var item = me["@itemTemplate"].clone(0,arr,true);
				item["@subject"] = me;
				items.unshift(item);
			}
			var it_evt = {type : "add",sender:item,value:it,reason:"array.unshift"};
			var arr_evt = {type:"change",sender:me,value:arr,reason:"array.unshift",source:it_evt};
			me.trigger(arr_evt);
			return this;
		}
		accor.shift = this.shift = function(it,retItem){
			var arr = me["@target"][me["@name"]],items = me["@items"],item;
			var it =arr.shift();
			for(var i=0,j=accor.length;i<j;i++)accor[i]  = accor[i+1];
			accor.length = arr.length;
			if(items) item = items.shift();
			var it_evt = {type:"remove",sender:item,value:it,reason:"array.shift"};
			if(item)item.trigger(it_evt);
			else{
				var arr_evt = {type:"change",sender:me,value:arr,reason:"array.shift",source:it_evt};
				me.trigger(arr_evt);
			}
			

			return retItem?item:it;
		}
		accor.remove = this.remove = function(at,retItem){
			var arr = me["@target"][me["@name"]],items = me["@items"],item;
			if(typeof at!=="number"){
				var cp = at;at =undefined;
				for(var i=0,j=arr.length;i<j;i++) if(arr[i]===cp){at = i;break;}
				if(at===undefined)return;
			}
			var ret_v = arr[at],ret_it = items?items[at]:undefined;
			for(var i=at,j=arr.length-1;i<j;i++){
				accor[i] = arr[i] = arr[i+1];
				if(items) items[i] = items[i+1];
			}
			arr.pop();if(items)items.pop();
			var it_evt = {type:"remove",sender:item,value:it,reason:"array.remove",index:at};
			if(ret_it) ret_it.trigger(it_evt);
			else {
				var arr_evt = {type:"change",sender:self,value:it,reason:"array.remove",index:at,source:it_evt};
				me.trigger(arr_evt);
			}
			return retItem ? ret_v:ret_it;
			
		}

		accor.item = this.item = function(index,value){
			var items = me["@items"];
			if(!items)return value===undefined?undefined:this;
			var arr = me["@target"][me["@name"]];
			if(value===undefined) return  items[index];
			accor[index] = arr[index] = value;
			var item = items[index].value(value,"array.item");
			var it_evt = {type:"change",sender:item,value:value,reason:"array.item",index:index};
			item.trigger(it_evt);
			return item;
		}
		accor.clear = this.clear = function(){
			var arr = me["@target"][me["@name"]],items = me["@items"];
			if(items){
				for(var i=0,j=items.length;i<j;i++){
					var it_evt = {type:"remove",sender:item,value:value,reason:"array.clear",index:i};
					//不冒泡，处理完成后统一给array发送消息
					items[i].trigger(it_evt,false);
				}
			}
			var arr_evt = {type:"change",sender:self,value:it,reason:"array.clear"};
			me.trigger(arr_evt);
			return this;
		}

		this.asArray = function(template){
			if(template===undefined)return this;
			this["@itemTemplate"] = template;
			initArrayData.call(this,template,accor);
			return this;
		}
		return this;
	}
	
}//end function Observable
yi.observable = function(value,model){
	var ret = new Observable("",{"":value});
	if(model) ret.define(model);
	return ret;
}
})(global,document);

(function(global,document,undefined){
var yi = global.$y,Deferred = yi.Deferred,extend = yi.extend;
var createHttp = function(){
	if(global.XMLHttpRequest){
		createHttp = function(){
			var http = new XMLHttpRequest();
			if (http.overrideMimeType) {
				http.overrideMimeType('text/xml');
			};
			return http;
		}
	}else {
		createHttp = function(){
			var http=new ActiveXObject("Microsoft.XMLHTTP");
			return http;
		}
	}
	return createHttp();
}
var HttpRequest = function(opts){
	this._opts = opts || {};
	this.header = function(name,value){
		var opts = this._opts;
		if(name===undefined)return this["@headers"] || {};
		var headers = this["@headers"] || (this["@headers"] = {});
		if(typeof name==='object') extend(headers,name);
		else headers[name] = value;
		return this;
	}
	this.url = function(val){
		if(val===undefined){
			var val = this._opts.url;
			return val || "";
		}
		this.opts.url = val;return this;
	}
	this.method = function(val){
		if(val===undefined){
			var val = this._opts.method;
			return (val===undefined) ? "GET":method;
		}
		this._opts.method= val;return this;
	}
	this.async = function(val){
		if(val===undefined)return this._opts.async !==false;
		this._opts.async = val;return this;
	}
	this.dataType = function(val){
		if(val===undefined)return this._opts.dataType;
		this._opts.dataType = val;return this;
	}
	this.accept = function(val){
		if(val===undefined)return this._opts.accept;
		this._opts.accept = val;return this;
	}
	this.opts = function(opts){
		if(opts===undefined)return this._opts;
		extend(this._opts,opts);
		if(opts.headers) this.header(opts.headers);
		return this;
	}
	
	this.send = function(data){
		var http = this.raw = createHttp(),dfd = new Deferred(),me = this;
		try{
			var headers = this.header();
			for(var n in headers) raw.setRequestHeader(n,headers[n]);
			http.onreadystatechange = function(){
				if (http.readyState==4){
					if(http.status==200){
						var acpt = me["accept"];
						var handler = acceptHandlers[acpt];
						var rs;
						if(handler)rs = handler(http);
						else rs = http.responseText;
						dfd.resolve(rs);
					}else{
						dfd.reject({
							status:http.status,
							statusText : http.statusText,
							content : http.responseText
						});
					}
				}
			}
			http.open(this.method(),this.url(),this.async());
			var dataType = this["@dataType"];
			var handler = dataHandlers[dataType] || dataHandler;
			http.send(handler(data));
			
		}catch(e){
			yi.elog(e);
			dfd.reject(e);
		}
		return dfd.promise(this);
	}
	if(opts) this.opts(opts);
	return this;
}
var dataHandler = function(data){
	if(typeof data==="object"){
		var rs = "";
		for(var n in data) {
			if(rs!="")rs+= "&";
			rs+= encodeURIComponent(name);
			rs += "=";
			var val = data[n];
			rs += encodeURIComponent(val===null||val===undefined?"":val);
		}
		return rs;
	}
	return data===null || data===undefined?"":data.toString();
}
var dataHandlers = {
	"json" : function(data){
		return JSON.stringify(data);
	}
}
var accepts = {
	"json": "text/json",
	"xml" : "text/xml"
}
var acceptHandlers = {
	"json" : function(http){
		var text = xmlhttp.responseText;
		return JSON.parse(text);
	},
	"xml" : function(http){
		return xmlhttp.responseXml;
	}
}
yi.ajax = global.$ajax  = function(opts){return new HttpRequest(opts).send(opts.data);}
})(global,document);



(function(global,document,location,undefined){
//module/require
var yi = global.$y,Deferred = yi.Deferred,extend = yi.extend,disabled = yi.disabled,keys = yi.keys,values = yi.values;
var scptPths = {},resolvedPths={},scptBas = "";
var cachedScpts ={};
var resolveUrl = function(pths,bas,name,exts){
	var url = name;
	for(var n in pths){
	if(name.indexOf(n)==0 && name[n.length]==='/'){
		var k = pths[n];
		url =  k + name.substring(n.length);
		break;
		}
	}
	if(exts && !url.$endWith(exts)) url += exts;
	return bas + url;
}
var lastCompleteScript;
var loadScript = yi.loadScript = function(url,initFn){
	var hd = document.getElementsByTagName("HEAD");
	hd = hd[0]|| document.body || document.documentElement;
	loadScript = yi.loadScript = function(url){
		var scpt = new  Deferred();
		scpt.url = url;
		var elm  =scpt.element = document.createElement("script");
		elm.type = "text/javascript";
		elm.src = url;
		var loaded = function(){
			lastCompleteScript = scpt;
			if(!cb)return scpt.resolve(scpt);
			//initFn.
		}
		if(elm.onload==null)elm.onload = function(){
			scpt.resolve(scpt);
		};
		else elm.onreadystatechange = function(){
			if(elm.readyState==4 || elm.readyState=='complete') scpt.resolve(scpt);
		}
		elm.onerror = function(err){scpt.fail(err);}
		hd.appendChild(elm);
		scpt.wait();
		return scpt;
	}
	return loadScript(url);
}

var require  = yi.require = global.$require = function(names,nocache){
	//if(typeof nocache ==='function'){cb = nocache;nocache = false;}
	var dfd = new Deferred(),c = names.length,err;

	var ready = function(scpt){
		if(err)return disabled;
		if(!nocache) {cachedScpts[scpt.name] = scpt;scpt.cached = true;}
		if(--c==0)dfd.resolve();
	};
	for(var i=0,j=c;i<j;i++){
		var n = names[i];
		var xst = cachedScpts[n];
		if(xst){
			if(!nocache){
				dfd.reject(err=xst);
				return dfd.promise();
			}else{
				xst.done(ready);continue;
			}
			
		}
		var url = resolvedPths[n]||(resolvedPths[n]=resolveUrl(scptPths,scptBas,n));
		var scpt = loadScript(url).done(ready).fail(function(scpt){dfd.reject(err = scpt);return disabled;});
		if(!nocache)cachedScpts[n] = scpt;
		scpt.name = n;
	} 
	return dfd.promise();
};
var output_mod,mod_id=1;

var module = global.$module = yi.module = function(_deps,_cb){
	var deps=_deps,cb = _cb;
	if(typeof deps==="string"){
		if( typeof cb==='function'){
			var mod = output_mod = new Deferred(cb);
			mod.name = deps;
			cachedScpts[mod.name] = mod;
			return mod;
		}else {
			deps =arguments;
			cb= null;
		}
		
	}
	var curr_mod =output_mod = new Deferred(),c = deps.length,dnc=0,rslt = {},err;
	if(cb===null) output_mod = null;
	curr_mod.id = (++mod_id>2100000000)?1:mod_id;
	var tryDone = function(){
		if(--c==0){
			var curr_rs ;
			if(cb) {
				curr_rs = cb.apply(curr_mod,values(rslt));
				if(!curr_mod.isWaiting()){
					curr_mod.resolve(curr_rs);
				}
			}else{
				curr_mod.resolve(values(rslt),true);
			}
			//else wait cb invoke resolve
		}
	};	
	var ready = function(scpt){
		if(err)return disabled;
		var dep_mod = output_mod;
		if(dep_mod) {
			dep_mod.name = scpt.name;
			dep_mod.element = scpt.element;
			dep_mod.done(function(val){
				rslt[scpt.name] = val;
				tryDone(); 
			});
		}else {
			tryDone();
		} 
		
		output_mod = null;
		return disabled;
	};
	for(var i=0,j=c;i<j;i++){
		var dep_n = deps[i];
		if(!dep_n)continue;
		rslt[dep_n] = null;
		var existed = cachedScpts[dep_n];
		if(existed)cachedScpts.done(ready);
		else{
			var url = resolvedPths[dep_n]||(resolvedPths[dep_n]=resolveUrl(scptPths,scptBas,dep_n,".js"));
			var scpt = cachedScpts[dep_n] = loadScript(url).done(ready).fail(function(scpt){throw "cannot load " + scpt.name;});
			
			scpt.name = dep_n;
		}
	} 	
	return curr_mod;
}
yi.require = global.$require = module;

module.config = require.config = function(opts){
	if(opts.baseUrl !==undefined){
		scptBas = opts.baseUrl;
		if(scptBas[scptBas.length-1]!='/')scptBas += "/";
	}
	if(opts.paths) extend(scptPths,opts.paths);
	return module;
}
module.load = function(deps){return module.apply(this,arguments)}


})(global,document,location);

(function(window,document,undefined){
var yi = window.$y;
var ctnrs = {
	
	"li": document.createElement("ul"),
	"option": document.createElement("select"),
	"legend":document.createElement("fieldset")
};
var div = ctnrs[""] = document.createElement("div");
ctnrs.th = ctnrs.td = document.createElement("tr");
ctnrs.dt = ctnrs.dd =  document.createElement("dl");
ctnrs.tbody = ctnrs.tfoot = ctnrs.thead = ctnrs.tbody = document.createElement("table");

//--------------元素的事件操作-----------------------
var attach,detech;
if(div.attachEvent){
	yi.attach = function(elem,evt,handler){elem.attachEvent('on' + evt,handler);}
	yi.detech = function(elem,evt,handler){elem.detechEvent('on' + evt, handler);}
}else if(div.addEventListener){
	yi.attach = function(elem,evt,handler){elem.addEventListener(evt,handler,false);}
	yi.detech = function(elem,evt,handler){elem.removeEventListener(evt, handler,false);}
}


yi.hasClass = function(obj,cls){return obj.className.match(new RegExp('(\\s|^)' + cls + '(\\s|$)'));};
yi.addClass = function(obj,cls){if (!this.hasClass(obj, cls)) obj.className += " " + cls;};
var removeClass = yi.removeClass = function(obj, cls) {
	if (hasClass(obj, cls)) {
		var reg = new RegExp('(\\s|^)' + cls + '(\\s|$)');
		obj.className = obj.className.replace(reg, ' ');
	}
}


//-------------获取、设置式样表---------------------
if(window.getComputedStyle){
	yi.getStyle = function(elem, style){return getComputedStyle(elem, null).getPropertyValue(style);}
	yi.getOpacity  = function(elem){ getComputedStyle(elem, null).getPropertyValue('opacity');}
}else {
	yi.getOpacity  = function(elem){
		var filter = null;

		// 早期的 IE 中要设置透明度有两个方法：
		// 1、alpha(opacity=0)
		// 2、filter:progid:DXImageTransform.Microsoft.gradient( GradientType= 0 , startColorstr = ‘#ccccc’, endColorstr = ‘#ddddd’ );
		// 利用正则匹配
		fliter = elem.style.filter.match(/progid:DXImageTransform.Microsoft.Alpha\(.?opacity=(.*).?\)/i) || elem.style.filter.match(/alpha\(opacity=(.*)\)/i);
		if (fliter) {
			var value = parseFloat(fliter);
			if (!NaN(value)) {
				// 转化为标准结果
				return value ? value / 100 : 0;
			}
		}
		// 透明度的值默认返回 1
		return 1;
	};
	yi.getStyle = function(elem,style){
		// IE 下获取透明度
		if (style == "opacity") {
			return getOpacity(elem);
		// IE687 下获取浮动使用 styleFloat
		} else if (style == "float") {
			return elem.currentStyle.getAttribute("styleFloat");
			// 取高宽使用 getBoundingClientRect
		} else if ((style == "width" || style == "height") && (elem.currentStyle[style] == "auto")) {
			var clientRect = elem.getBoundingClientRect();
			return (style == "width" ? clientRect.right - clientRect.left : clientRect.bottom - clientRect.top) + "px";
		}
		// 其他样式，无需特殊处理
		return elem.currentStyle.getAttribute(camelize(style));
	};
}
yi.setOpacity = function(elem,value){
	val = parseFloat(val);
	elem.style.opacity = val;
	elem.style.filter = "filter:alpha(opacity=" + (val * 100) + ")";
	return elem;
}
yi.setStyle = function(elem,style,value){elem.style[camelize(style)] = value;}
yi.isVisible = function(elem){
	while(elem){
		if( getStyle(elem,"display")=='none' || getStyle("visibility")=='hidden')return false;
		elem = elem.parentNode;
	}
	return true;
}

//---获取元素绝对位置-----
if(div.getBoundingClientRect){
	yi.getPosition = function(elem){
		var rect = elem.getBoundingClientRect();
		return {
			x: rect.left + ( document.body.scrollLeft|| document.documentElement.scrollLeft),
			y: rect.top + (document.body.scrollTop || document.documentElement.scrollTop)
		};
	}
}else {
	yi.getPosition = function(elem){
		if(elem==document.body || elem == document.documentElement)return {x:0,y:0};
		var x=0 ,y=0;
		while(elem){
			x += elem.offsetLeft;y += elem.offsetTop;
			elem = elem.offsetParent;
		}
		return {x:x,y:y};
	}
}

//------------添加、移除className----------------
yi.hasClass = function(obj, cls) {return obj.className.match(new RegExp('(\\s|^)' + cls + '(\\s|$)'));}
yi.addClass = function(obj, cls) {if (!hasClass(obj, cls)) {obj.className += " " + cls;return true;}return false;}
yi.removeClass = function(obj, cls) {
	if (hasClass(obj, cls)) {
		var reg = new RegExp('(\\s|^)' + cls + '(\\s|$)');
		obj.className = obj.className.replace(reg, ' ');
		return true;
	}return false;
}

//---------get or set value

yi.getElementValue = function(elem){
	var tag = elem.tagName;
	switch(tag){
		case "INPUT":
			var type = elem.type;
			if(type=='checkbox' || type=='radio'){
				if(elem.checked)return elem.value;
				else return undefined;
			}
			return elem.value;
		case "SELECT":
			return elem.options[elem.selectedIndex].value;
		case "TEXTAREA":return elem.value;
		defautl:return elem.value;
	}
}
})(window,document);

(function(window,document){
var yi = window.$y;
yi.getCookie = function(name){
	var arr,reg=new RegExp("(^| )"+name+"=([^;]*)(;|$)");
	return (arr=document.cookie.match(reg))?unescape(arr[2]):null;
}
yi.setCookie = function(name,value,time){
	if(value===null){
		var exp = new Date();
		exp.setTime(exp.getTime() - 1);
		document.cookie= name + "=;expires="+exp.toGMTString();
		return;
	}
	if(time){
		var exp = yi.parseDate(time);
		if(!exp){
			var strsec = getsec(time||"");
			var exp = new Date();
			exp.setTime(exp.getTime() + strsec*1);
		} 
		
		document.cookie = name + "="+ escape (value) + ";expires=" + exp.toGMTString();
	}else document.cookie = name + "="+ escape (value===undefined?"":value) + ";";
}
function getsec(str){
	var str1=str.substring(1,str.length)*1;
	var str2=str.substring(0,1);
	if (str2=="s")return str1*1000;
	if (str2=="m")return str1*1000*60;
	else if (str2=="h")return str1*60*60*1000;
	else if (str2=="d")return str1*24*60*60*1000;
}
})(window,document);