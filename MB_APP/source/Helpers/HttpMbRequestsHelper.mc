using Toybox.Communications as Comm;

class HttpMbRequestsHelper {

	const URL = "https://httpbin.org/get";

	hidden function makePostRequest ( rawData )
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
			Comm.makeJsonRequest(URL, parameter, options, method(:onReceivedResponse));
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
	
		makePostRequest(hrDataBuffer);
		System.println( "Trying to send post request with HR data..." );
	
	}

	function postAccData(accDataBuffer) {
		
		System.println( "Trying to send post request with acc data..." );
	
	}
	
}