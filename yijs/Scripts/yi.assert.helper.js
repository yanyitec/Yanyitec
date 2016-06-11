(function($assert){
var test = $assert.test;
var findTestScript = function(elem){
	if(!elem) elem = document.body;
	for(var j=elem.childNodes.length-1;j>=0;j--){
		var sub = elem.childNodes[j];
		if(!sub.tagName) continue;
		if(sub.tagName==='SCRIPT' && sub.getAttribute("subtype")==='test')return sub;
		var testScript = findTestScript(sub);
		if(testScript)return testScript;
	}
}
$assert.scope = function(tester){
	var script = findTestScript(document.body);
	var rawCode = script.innerHTML;
	var li = script.parentNode;
	var code = $assert.clearCode(rawCode);
	li.innerHTML = "<h3>示例代码(Sample code)</h3><pre class='code'>" + code + "</pre>";
	var logger = new yi.log.HtmlLogger();
	logger.traceStack(true);
	var assert = new yi.assert.Assert(logger);
	var p = li.parentNode;
	var li = document.createElement("li");
	li.innerHTML = "<h3>运行结果(Execute result)</h3>";
	li.appendChild(logger.element);
	//logger.element.style.height="100px";
	//logger.element.style.overflow = "auto";
	p.appendChild(li);
	tester.call(assert,assert);
	//return $assert.caches[codeid] = assert;
}
})(yi.assert);


$assert.redirectOutput=function(v,enable){
	if(enable!==false){
		var id = (v || new Date().valueOf()) + "-console";
		var consoleView =document.getElementById(id); 
		if(!consoleView){
			consoleView = $assert.consoleView = document.createElement("div");
			consoleView.id = id;
			consoleView.className = "vconsole";
			consoleView.innerHTML = "<h3>运行结果(result):</h3>";
		}else $assert.consoleView =consoleView;
		var existed = window.$log;
		var log = window.$log = function(){
			var html = "";
			for(var i=0,j=arguments.length;i<j;i++){
				html += "<div>" + arguments[i] + "</div>";
			}
			if(arguments.length===0) html = "<br />";
			consoleView.innerHTML += html;
		}
		log.prev = existed;
		return this;
	}else if(enable===false){
		var log = window.$log;
		if(log.prev) window.$log = log.prev;
	}
	//return $assert._log!=null;
}
$assert.begin = function(id){
	var assert = new yi.Assert();
	
	$assert.clear();
	$assert.redirectOutput(id,true);
}
$assert.showCode = function(codeid){
	
	var codeElem = document.getElementById(codeid);
	var code = codeElem.innerHTML;
	var codeView = document.createElement("pre");
	codeView.className = "code";
	
	codeView.innerHTML = "<h3>//代码</h3>" + code;

	var token = codeElem.nextSibling;
	if(token) codeElem.parentNode.insertBefore(codeView,token);
	else codeElem.parentNode.appendChild(codeView);
	return codeView;
}
$assert.end = function(codeid){
	var codeView = $assert.showCode(codeid);
	var token = codeView.nextSibling;
	if(token) codeView.parentNode.insertBefore($assert.consoleView,token);
	else codeView.parentNode.appendChild($assert.consoleView);
	$assert.redirectOutput(codeid,false);
	return $assert.codeView;
}
