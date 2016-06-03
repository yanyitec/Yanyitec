(function(Global){
var data = {};
Global.$assert = {
	clear : function(){data = {};return this;},
	set:function(key,value){data[key]=value;return this},
	get:function(key){return data[key];},
	log: function(){try{console.log.apply(console,arguments);}catch(ex){};},
	check : function(value ,then){if(value) then();},
	isArray : function(value,text){
		var t = Object.prototype.toString.call(value);
		if(t!=='[object Array]'){
			this.log(text || "Not []");
			this.log("expect:array");
			this.log("actual:",value);
			//alert(arguments.callee.callee);
			//this.log("invoker:",arguments.callee.caller);
			throw text || "Not []";
		}
		return this;
	},
	Equal : function(expect,actual,text){
		if(expect!==actual){
			this.log(text || "Not equal");
			this.log("expect:",expect);
			this.log("actual:",actual);
			//this.log("invoker:",argument.callee.caller);
			throw text || "Not equal";
		}
		return this;
	},
	None : function(value,text){
		if(value){
			this.log(text || "Not none");
			this.log("expect:0,null,undefined,''");
			this.log("actual:",value);
			//alert(arguments.callee.callee);
			//this.log("invoker:",arguments.callee.caller);
			throw text || "Not none";
		}
		return this;
	},
	Undefined : function(value,text){
		if(value!==undefined){
			this.log(text || "Not undefined");
			this.log("expect:undefined");
			this.log("actual:",value);
			//this.log("invoker:",argument.callee.caller);
			throw text || "Not undefined";
		}
		return this;
	},
	Null : function(value,text){
		if(value!==null){
			this.log(text || "Not null");
			this.log("expect:null");
			this.log("actual:",value);
			//this.log("invoker:",argument.callee.caller);
			throw text || "Not null";
		}
		return this;
	},
	True : function(value,text){
		if(value!==true){
			this.log(text || "Not true");
			this.log("expect:true");
			this.log("actual:",value);
			//this.log("invoker:",argument.callee.caller);
			throw text || "Not true";
		}
		return this;
	},
	False : function(value,text){
		if(value!==false){
			this.log(text || "Not false");
			this.log("expect:false");
			this.log("actual:",value);
			//this.log("invoker:",argument.callee.caller);
			throw text || "Not flase";
		}
		return this;
	}
}//end $assert
})(window);