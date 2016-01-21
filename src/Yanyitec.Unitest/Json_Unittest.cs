using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yanyitec.Json;
using Yanyitec.Testing;

namespace Yanyitec.Unitest
{
    [Test]
    public class Json_Unittest
    {
        [Test]
        public void JsonObject() {
            var jObj = new Json.JObject();
            jObj["Id"] = Guid.Empty;
            jObj["Name"] = "yanyitec";
            jObj["Age"] = 1;
            jObj["Height"] = 11.5;
            jObj["Birthday"] = new DateTime(2016,1,19);
            jObj["Gender"] = new JNull();
            jObj["Favor"] = JUndefined.Default;
            var expect = "{\"Id\":\"00000000-0000-0000-0000-000000000000\",\"Name\":\"yanyitec\",\"Age\":1,\"Height\":11.5,\"Birthday\":\"2016-01-19T00:00:00\",\"Gender\":null,\"Favor\":undefined}";
            var json = jObj.ToJson();
            Assert.Equal(expect, json);
        }

        [Test]
        public void JsonArray()
        {
            var jObj = new Json.JArray();
            jObj.Push(Guid.Empty);
            jObj.Push("yanyitec");
            jObj.Push(1);
            jObj.Push(11.5);
            jObj.Push(new DateTime(2016, 1, 19));
            jObj.Push(new JNull());
            jObj.Push(JUndefined.Default);
            var expect = "[\"00000000-0000-0000-0000-000000000000\",\"yanyitec\",1,11.5,\"2016-01-19T00:00:00\",null,undefined]";
            var json = jObj.ToJson();
            Assert.Equal(expect, json);
        }
    

        [Test]
        public void ParseToJToken()
        {
            var obj = new JObject();
            obj["Id"] = Guid.Empty;
            obj["Username"] = "yiy";

            var profile = new JObject();
            profile["Birthday"] = new DateTime(2015, 1, 1);
            profile["Age"] = 12;

            var favors = new JArray();
            favors.Push(12);
            favors.Push("Sweet Milk");
            obj["Profile"] = profile;
            profile["Favors"] = favors;

            obj["Actived"] = true;

            var jsonString = obj.ToJson();

            var convert = new Json.Parser();
            var json = convert.Parse(jsonString) as JObject;
            System.Text.RegularExpressions.Regex JsonDateRegex = new System.Text.RegularExpressions.Regex("^(\\d{4})\\-(10|11|12|0?\\d)\\-([012]\\d|30|31)T([01]\\d|2[0-3]):([0-5]?\\d):([0-5]?\\d)$", System.Text.RegularExpressions.RegexOptions.Compiled);
            var datestr = "2015-01-01T10:3:3";
            var match = JsonDateRegex.Match(datestr);
            Assert.Equal(Guid.Empty.ToString(), json["Id"].ToString());
            Assert.Equal("yiy", (string)json["Username"]);
            Assert.Equal(true, (bool)json["Actived"]);
            profile = json["Profile"] as JObject;
            Assert.Equal(new DateTime(2015, 1, 1), (DateTime)profile["Birthday"]);
            Assert.Equal(12, (int)profile["Age"]);
            favors = profile["Favors"] as JArray;
            Assert.Equal(12, (int)favors[0]);
            Assert.Equal("Sweet Milk", (string)favors[1]);

        }

        [Test]
        public void ParseToTokenWithFunction()
        {
            var jsonstring = @"{no:1,onstart : ";
            var func = @"function(evt,bk){
    var s = 'hello';
    var err={bk:s};
    for(var n in err){
        s += " + "\"\\\"\" + n + \"\\\"\" ':' + err[n];"
    + @"}
    var fn = function(s){alert(s);}
    fn(s);
}";
            jsonstring += func + "}";

            var convert = new Json.Parser();
            var obj = convert.Parse(jsonstring) as JObject;
            Assert.Equal(1, (int)obj["no"]);
            Assert.InstanceOf(obj["onstart"], typeof(JFunction));
            Assert.Equal(func, obj["onstart"].ToString().Trim());

        }
        [Test]
        public void QuickCrypt() {
            var pswd = "0123";
            var input = new byte[] {65,66,67,68,69,70 };
            var cryptedString = Utils.CryptUtil.QuickCryptToText(input, pswd);
        }
    }
}
