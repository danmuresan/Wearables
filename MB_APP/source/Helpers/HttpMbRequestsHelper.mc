using Toybox.Communications as Comm;

class HttpMbRequestsHelper {

	const POST_ACCEL_URL = "http://localhost:52001/api/AccelerometerData";
	const POST_HR_URL = "http://localhost:52001/api/HrData";

	hidden function makePostRequest ( rawData, url )
	{
		var parameter = 
		{
			"Monkeys" => "Awesome",
			"ConnectIQ" => "1337"
		};
		
		var options = 
		{
			"Content-Type" => Comm.REQUEST_CONTENT_TYPE_JSON
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
			System.println("Fail occurred => Response code: " + responseCode.toString());
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