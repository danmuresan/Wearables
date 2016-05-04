using Toybox.Communications as Comm;
using Toybox.Lang as Lang;

class HeartRateBatch {

	hidden var id;
	hidden var heartRateList;
	
	function initialize(_id, _hrList)
	{
		id = _id;
		heartRateList = _hrList;
	}
}


class HttpMbRequestsHelper {

	const POST_ACCEL_URL = "http://localhost:52001/api/AccelerometerData";
	const POST_HR_URL = "http://localhost:52001/api/Ceva";

	var list = new HeartRateBatch(5, new [5]);

	// apparently, this makes a GET request now (same url as the post one, but different content)
	// TODO: check how to make post requests with the data
	hidden function makePostRequest ( rawData, url )
	{
		var x = {
			 "HeartRateValueList" => [ //[],
			 {
			      "Id" => 1,
			      "HeartRateValue" => 1,
			      "TimeStamp" => "2016-03-21T11:30:56.2579502+02:00"
			    },
			    {
			      "Id" => 1,
			      "HeartRateValue" => 1,
			      "TimeStamp" => "2016-03-21T11:30:56.2579502+02:00"
			    }
		  	],
		  "Id" => 1,
		  "TimeStamp" => "2016-03-21T11:30:56.2584506+02:00"
		}; 
	
		var xx = { "Ceva" => { "Id" => 3, "S" => "Ss" } };
		
		var parameter = 
		{
			"Id" => 5,
			"S" => "sssas"
		};

		System.println(toJson(parameter));
		
		var options = 
		{
			:method => Comm.HTTP_REQUEST_METHOD_POST,
			"Content-Type" => Comm.REQUEST_CONTENT_TYPE_JSON
		};
		
		try
		{
			Comm.makeJsonRequest(url, xx, options, method(:onReceivedResponse));
		}
		catch (ex)
		{
			System.println( "Something went wrong when sending the request: " + ex );
		}
	}
	
	function onReceivedResponse(responseCode, data) {
		if (responseCode == 200)
		{
			var response = data["args"];
			System.println("Response data: " + response);
		}
		else
		{
			System.println("Fail occurred => Response code: " + responseCode.toString() + " (" + data + ")");
		}
	}

	function postHrData(hrDataBuffer) {
	
		makePostRequest(hrDataBuffer, POST_HR_URL);
		System.println( "Trying to send post request with HR data..." );
	
	}

	function postAccData(accDataBuffer) {
		
		makePostRequest(accelDataBuffer, POST_ACCEL_URL);
		System.println( "Trying to send post request with acc data..." );
	
	}
	
	static function accelToJson(accel)
	{
		var json = "";
		
		if (accel instanceof Acceleration)
		{
			Toybox.System.println("ACC TYPE");
			
			var x = accel.getXAxisAcceleration();
			var y = accel.getYAxisAcceleration();
			var z = accel.getZAxisAcceleration();
			
			var serializedVal = "{\"x\":\"" + x + "\", " + "\"y\":\"" + y + "\", " + "\"z\":\"" + z + "\"}";
			Toybox.System.println(serializedVal);
			
			json += serializedVal;
		}
		
		return json;
	}
	
	static function bufferToJson(value)
	{
		var json = "{";
		
		if (value instanceof SensorDataBuffer) {
			Toybox.System.println("Value is a sensor type indeed...");
			// TODO: ... (serialize to json)
			
			json += "["; 
			for (var i = 0; i < value.getBufferLength(); i++)
			{
				var elemVal = value.getElementAt(i);
				if (i < value.getBufferLength() - 1)
				{
					if (elemVal instanceof Acceleration)
					{
						json += accelToJson(elemVal) + ",";
					}
					else
					{
						var serializedElem = "\"Val\":\"" + elemVal + "\",";
						json += serializedElem;
					}
				}
				else
				{
					if (elemVal instanceof Acceleration)
					{
						json += accelToJson(elemVal);
					}
					else
					{
						var serializedElem = "\"Val\":\"" + elemVal + "\"";
						json += serializedElem;
					}
				}
			}
			
			json += "]}";
			
			Toybox.System.println("" + json);
		}
	}
	
	static function toJson(dictionary) {
	
		var json = "";
	
		if (dictionary == null || dictionary.size() == 0) {
			return "{}";
		}
		
		var keys = dictionary.keys();
		var value;
		
		for (var i = 0; i < keys.size(); ++i) {
			
			json += ",\"" + keys[i] + "\":";
			
			value = dictionary.get(keys[i]);
		
			if (value instanceof Lang.Dictionary) {
				json += toJson(value);
			}
			else if (value instanceof Lang.String) {
				json += "\"" + value + "\"";
			}
			else {
				json += value;
			}
			
		}
		
		return "{" + json.substring(1, json.length()) + "}";		
	}
	
}