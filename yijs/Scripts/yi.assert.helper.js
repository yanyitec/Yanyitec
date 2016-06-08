$assert.clearCode = function(code){
	var codes = code.split("\n");
	for(var i =0,j= codes.length;i<j;i++){
		var line = codes.shift();
		var lineCode = line.replace(/(^\s+)|(\s+$)/g,"");

		if(lineCode.indexOf("$assert.")!==0 &&lineCode.indexOf("$assert(")!==0 && lineCode!="$log();" && lineCode.lastIndexOf("//$assert.line")!==(lineCode.length-"//$assert.line".length)) codes.push(line);
	}
	return codes.join("\n");
}
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
	$assert.clear();
	$assert.redirectOutput(id,true);
}
$assert.showCode = function(codeid){
	
	var codeElem = document.getElementById(codeid);
	var code = codeElem.innerHTML;
	var codeView = document.createElement("pre");
	codeView.className = "code";
	var code = $assert.clearCode(code);
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