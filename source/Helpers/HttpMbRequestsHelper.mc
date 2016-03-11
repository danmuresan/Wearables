using Toybox.Communications as Comm;

class HttpMbRequestsHelper {

	const URL = "https://httpbin.org/get";

	hidden function makePostRequest ( rawData )
	{
		var parameter = 
		{
			"Accel" => "Some accel",
			"Hr" => "adsa"
		};
		
		var options = 
		{
			"Content-Type" => Comm.REQUEST_CONTENT_TYPE_JSON
		};
		
		try
		{
			Comm.makeJsonRequest(URL, parameter, options, method(:onReceivedResponse));
		}
		catch (ex)
		{
			System.println( "Something went wrong when sedning the request: " + ex );
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
			System.println("Failed to load\nError: " + responseCode.toString());
		}
	}

	function postHrData(hrDataBuffer) {
	
		makePostRequest(hrDataBuffer);
		System.println( "Trying to send post request with HR data..." );
	
	}

	function postAccData(accDataBuffer) {
		
		System.println( "Trying to send post request with acc data..." );
	
	}
	
}