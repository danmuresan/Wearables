using Toybox.Communications as Comm;

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
	
	
		var parameter = 
		{
			"Id" => 1,
			"S" => 1
		};
		
		var options = 
		{
			:method => Comm.HTTP_REQUEST_METHOD_POST
//			"Content-Type" => Comm.REQUEST_CONTENT_TYPE_JSON
		};
		
		try
		{
			Comm.makeJsonRequest(url, parameter, options, method(:onReceivedResponse));
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
	
}